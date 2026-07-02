using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShoeMoldControl.Core;
using Serilog;
using Polly;

namespace ShoeMoldControl.Vision
{
    public class TcpVisionService : IVisionService
    {
        private readonly IVisionConfig _config;
        private readonly IResiliencePolicyProvider _policyProvider;
        private readonly ILogger _logger;
        private TcpClient? _tcpClient;
        private NetworkStream? _networkStream;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private bool _disposed;

        public TcpVisionService(IVisionConfig config, IResiliencePolicyProvider policyProvider, ILogger logger = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _policyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider));
            _logger = logger ?? Log.ForContext<TcpVisionService>();
        }

        public async Task<DecodeResult> GrabAndDecodeAsync(CancellationToken token)
        {
            try
            {
                return await _policyProvider.VisionCircuitBreaker.ExecuteAsync(
                    async (ctx) => await _policyProvider.VisionRetryPolicy.ExecuteAsync(
                        async () => await ExecuteGrabAndDecodeAsync(token),
                        ctx),
                    new Context("VisionOperation")
                    {
                        ["Logger"] = _logger
                    });
            }
            catch (BrokenCircuitException)
            {
                _logger.Warning("Vision circuit breaker is OPEN - rejecting request");
                return new DecodeResult { IsSuccess = false, ErrorMessage = "Vision service temporarily unavailable (circuit open)" };
            }
            catch (OperationCanceledException)
            {
                _logger.Debug("Vision operation cancelled");
                return new DecodeResult { IsSuccess = false, ErrorMessage = "Vision operation cancelled" };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error in vision operation");
                return new DecodeResult { IsSuccess = false, ErrorMessage = $"Unexpected error: {ex.Message}" };
            }
        }

        private async Task<DecodeResult> ExecuteGrabAndDecodeAsync(CancellationToken token)
        {
            await _semaphore.WaitAsync(token);
            try
            {
                await EnsureConnectionAsync(token);

                if (_networkStream == null)
                {
                    _logger.Error("Vision NetworkStream is null after connection attempt");
                    return new DecodeResult { IsSuccess = false, ErrorMessage = "Vision NetworkStream is null." };
                }

                _logger.Debug("Sending trigger command: {Command}", _config.VisionTriggerCommand.Trim());
                byte[] triggerCmd = Encoding.ASCII.GetBytes(_config.VisionTriggerCommand);
                await _networkStream.WriteAsync(triggerCmd, 0, triggerCmd.Length, token);

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
                cts.CancelAfter(_config.VisionTimeoutMs);

                byte[] buffer = new byte[1024];
                int bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                
                if (bytesRead == 0)
                {
                    _logger.Warning("Vision system disconnected during read");
                    Disconnect();
                    return new DecodeResult { IsSuccess = false, ErrorMessage = "Vision system disconnected." };
                }

                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
                _logger.Debug("Vision response: {Response}", response);

                if (response.StartsWith("OK,", StringComparison.OrdinalIgnoreCase))
                {
                    string decodedText = response.Substring(3).Trim();
                    _logger.Information("Vision decode successful: {DecodedText}", decodedText);
                    return new DecodeResult { IsSuccess = true, DecodedText = decodedText };
                }
                
                _logger.Warning("Vision rejected response: {Response}", response);
                return new DecodeResult { IsSuccess = false, ErrorMessage = $"Vision rejected: {response}" };
            }
            catch (OperationCanceledException)
            {
                _logger.Warning("Vision operation timeout");
                Disconnect();
                return new DecodeResult { IsSuccess = false, ErrorMessage = "Vision system timeout." };
            }
            catch (SocketException ex)
            {
                _logger.Error(ex, "Vision socket error");
                Disconnect();
                return new DecodeResult { IsSuccess = false, ErrorMessage = $"Network error: {ex.Message}" };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Vision unexpected error");
                Disconnect();
                return new DecodeResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        private async Task EnsureConnectionAsync(CancellationToken token)
        {
            if (_tcpClient != null && _tcpClient.Connected) return;

            Disconnect();
            
            _logger.Debug("Connecting to vision system at {IP}:{Port}", _config.VisionIpAddress, _config.VisionPort);
            _tcpClient = new TcpClient
            {
                ReceiveTimeout = _config.VisionTimeoutMs,
                SendTimeout = _config.VisionTimeoutMs
            };
            
            await _tcpClient.ConnectAsync(_config.VisionIpAddress, _config.VisionPort, token);
            _networkStream = _tcpClient.GetStream();
            _logger.Information("Vision system connected successfully");
        }

        private void Disconnect()
        {
            _logger.Debug("Disconnecting from vision system");
            _networkStream?.Dispose();
            _tcpClient?.Dispose();
            _networkStream = null;
            _tcpClient = null;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _logger.Information("Disposing vision service");
            Disconnect();
            _semaphore?.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
