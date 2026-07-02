using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShoeMoldControl.Core;

namespace ShoeMoldControl.Vision
{
    public class TcpVisionService : IVisionService
    {
        private readonly IVisionConfig _config;
        private TcpClient? _tcpClient;
        private NetworkStream? _networkStream;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private bool _disposed;

        public TcpVisionService(IVisionConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<DecodeResult> GrabAndDecodeAsync(CancellationToken token)
        {
            await _semaphore.WaitAsync(token);
            try
            {
                await EnsureConnectionAsync(token);

                if (_networkStream == null)
                {
                    return new DecodeResult { IsSuccess = false, ErrorMessage = "Vision NetworkStream is null." };
                }

                byte[] triggerCmd = Encoding.ASCII.GetBytes(_config.VisionTriggerCommand);
                await _networkStream.WriteAsync(triggerCmd, 0, triggerCmd.Length, token);

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
                cts.CancelAfter(_config.VisionTimeoutMs);

                byte[] buffer = new byte[1024];
                int bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                
                if (bytesRead == 0)
                {
                    Disconnect();
                    return new DecodeResult { IsSuccess = false, ErrorMessage = "Vision system disconnected." };
                }

                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                if (response.StartsWith("OK,", StringComparison.OrdinalIgnoreCase))
                {
                    return new DecodeResult { IsSuccess = true, DecodedText = response.Substring(3).Trim() };
                }
                
                return new DecodeResult { IsSuccess = false, ErrorMessage = $"Vision rejected: {response}" };
            }
            catch (OperationCanceledException)
            {
                Disconnect();
                return new DecodeResult { IsSuccess = false, ErrorMessage = "Vision system timeout." };
            }
            catch (Exception ex)
            {
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
            
            _tcpClient = new TcpClient
            {
                ReceiveTimeout = _config.VisionTimeoutMs,
                SendTimeout = _config.VisionTimeoutMs
            };
            
            await _tcpClient.ConnectAsync(_config.VisionIpAddress, _config.VisionPort, token);
            _networkStream = _tcpClient.GetStream();
        }

        private void Disconnect()
        {
            _networkStream?.Dispose();
            _tcpClient?.Dispose();
            _networkStream = null;
            _tcpClient = null;
        }

        public void Dispose()
        {
            if (_disposed) return;
            Disconnect();
            _semaphore?.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
