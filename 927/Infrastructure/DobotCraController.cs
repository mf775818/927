using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NModbus;
using ShoeMoldControl.Core;

namespace ShoeMoldControl.Infrastructure
{
    public class DobotCraController : IRobotController
    {
        private readonly IRobotConfig _config;
        private TcpClient? _dashboardClient;
        private NetworkStream? _dashboardStream;
        private TcpClient? _modbusClient;
        private IModbusMaster? _modbusMaster;

        private readonly SemaphoreSlim _dashboardSemaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _modbusSemaphore = new SemaphoreSlim(1, 1);
        private bool _disposed;

        public DobotCraController(IRobotConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                _dashboardClient = new TcpClient
                {
                    ReceiveTimeout = _config.RobotCommandTimeoutMs,
                    SendTimeout = _config.RobotCommandTimeoutMs
                };
                
                await _dashboardClient.ConnectAsync(_config.RobotIpAddress, _config.RobotDashboardPort);
                _dashboardStream = _dashboardClient.GetStream();

                _modbusClient = new TcpClient();
                await _modbusClient.ConnectAsync(_config.RobotIpAddress, _config.RobotModbusPort);
                var factory = new ModbusFactory();
                _modbusMaster = factory.CreateMaster(_modbusClient);

                return true;
            }
            catch
            {
                await DisconnectAsync();
                return false;
            }
        }

        public async Task<int> ExecuteCommandAsync(string command, CancellationToken token)
        {
            if (_dashboardStream == null || _dashboardClient?.Connected != true) return -1;

            await _dashboardSemaphore.WaitAsync(token);
            try
            {
                string formattedCommand = command.EndsWith(";") ? command : command + ";";
                byte[] data = Encoding.ASCII.GetBytes(formattedCommand);
                await _dashboardStream.WriteAsync(data, 0, data.Length, token);

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
                cts.CancelAfter(_config.RobotCommandTimeoutMs);

                byte[] buffer = new byte[1024];
                int bytesRead = await _dashboardStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                if (bytesRead == 0) return -1;

                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
                
                var parts = response.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && parts[0] == "0" && int.TryParse(parts[1], out int cmdId))
                {
                    return cmdId;
                }
                return -1;
            }
            catch
            {
                return -1;
            }
            finally
            {
                _dashboardSemaphore.Release();
            }
        }

        public async Task<RobotMode> GetRobotModeAsync(CancellationToken token)
        {
            if (_modbusMaster == null || _modbusClient?.Connected != true) return RobotMode.ERROR;

            await _modbusSemaphore.WaitAsync(token);
            try
            {
                ushort[] registers = await _modbusMaster.ReadInputRegistersAsync(1, _config.RobotModbusStatusRegister, 1);
                if (registers.Length > 0)
                {
                    int modeValue = registers[0];
                    if (Enum.IsDefined(typeof(RobotMode), modeValue))
                    {
                        return (RobotMode)modeValue;
                    }
                }
                return RobotMode.ERROR;
            }
            catch
            {
                return RobotMode.ERROR;
            }
            finally
            {
                _modbusSemaphore.Release();
            }
        }

        public async Task<int> GetCurrentCommandIdAsync(CancellationToken token)
        {
            if (_dashboardStream == null || _dashboardClient?.Connected != true) return -1;

            await _dashboardSemaphore.WaitAsync(token);
            try
            {
                byte[] data = Encoding.ASCII.GetBytes("GetCurrentCommandId();");
                await _dashboardStream.WriteAsync(data, 0, data.Length, token);

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
                cts.CancelAfter(_config.RobotCommandTimeoutMs);

                byte[] buffer = new byte[1024];
                int bytesRead = await _dashboardStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                if (bytesRead == 0) return -1;

                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                var parts = response.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && parts[0] == "0" && int.TryParse(parts[1], out int cmdId))
                {
                    return cmdId;
                }
                return -1;
            }
            catch
            {
                return -1;
            }
            finally
            {
                _dashboardSemaphore.Release();
            }
        }

        public async Task DisconnectAsync()
        {
            _dashboardStream?.Dispose();
            _dashboardClient?.Dispose();
            _modbusClient?.Dispose();
            
            _dashboardStream = null;
            _dashboardClient = null;
            _modbusMaster = null;
            _modbusClient = null;

            await Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _ = DisconnectAsync();
            _dashboardSemaphore?.Dispose();
            _modbusSemaphore?.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
