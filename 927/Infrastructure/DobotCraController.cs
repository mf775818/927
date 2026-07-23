using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NModbus;
using ShoeMoldControl.Core;
using ShoeMoldControl.Core.Domain;
using ShoeMoldControl.Core.Models;
using Serilog;
using Polly;
using ShoeMoldControl.Infrastructure.Polly;
using Polly.CircuitBreaker;

namespace ShoeMoldControl.Infrastructure
{
    public class DobotCraController : IRobotController
    {
        private readonly IRobotConfig _config;
        private readonly IResiliencePolicyProvider _policyProvider;
        private readonly ILogger _logger;
        private TcpClient? _dashboardClient;
        private NetworkStream? _dashboardStream;
        private TcpClient? _modbusClient;
        private IModbusMaster? _modbusMaster;

        private readonly SemaphoreSlim _dashboardSemaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _modbusSemaphore = new SemaphoreSlim(1, 1);
        private bool _disposed;

        public DobotCraController(IRobotConfig config, IResiliencePolicyProvider policyProvider, ILogger logger = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _policyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider));
            _logger = logger ?? Log.ForContext<DobotCraController>();
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                _logger.Information("Connecting to robot at {IP}:{DashboardPort}", 
                    _config.RobotIpAddress, _config.RobotDashboardPort);

                _dashboardClient = new TcpClient
                {
                    ReceiveTimeout = _config.RobotCommandTimeoutMs,
                    SendTimeout = _config.RobotCommandTimeoutMs
                };
                
                await _dashboardClient.ConnectAsync(_config.RobotIpAddress, _config.RobotDashboardPort);
                _dashboardStream = _dashboardClient.GetStream();
                _logger.Information("Robot dashboard connection established");

                _modbusClient = new TcpClient();
                await _modbusClient.ConnectAsync(_config.RobotIpAddress, _config.RobotModbusPort);
                var factory = new ModbusFactory();
                _modbusMaster = factory.CreateMaster(_modbusClient);
                _logger.Information("Robot Modbus connection established");

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to connect to robot");
                await DisconnectAsync();
                return false;
            }
        }

        public async Task<int> ExecuteCommandAsync(string command, CancellationToken token)
        {
            if (_dashboardStream == null || _dashboardClient?.Connected != true)
            {
                _logger.Warning("Cannot execute command - dashboard not connected");
                return -1;
            }

            try
            {
                return await _policyProvider.RobotCommandCircuitBreaker.ExecuteAsync(
                    async (ctx) => await _policyProvider.RobotCommandRetryPolicy.ExecuteAsync(
                        async (ctx) => await ExecuteCommandInternalAsync(command, token),
                        ctx),
                    new Context("RobotCommand")
                    {
                        ["Logger"] = _logger,
                        ["Command"] = command
                    });
            }
            catch (BrokenCircuitException)
            {
                _logger.Warning("Robot circuit breaker is OPEN - rejecting command {Command}", command);
                return -1;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error executing robot command {Command}", command);
                return -1;
            }
        }

        private async Task<int> ExecuteCommandInternalAsync(string command, CancellationToken token)
        {
            await _dashboardSemaphore.WaitAsync(token);
            try
            {
                string formattedCommand = command.EndsWith(";") ? command : command + ";";
                _logger.Debug("Sending robot command: {Command}", formattedCommand);
                
                byte[] data = Encoding.ASCII.GetBytes(formattedCommand);
                await _dashboardStream.WriteAsync(data, 0, data.Length, token);

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
                cts.CancelAfter(_config.RobotCommandTimeoutMs);

                byte[] buffer = new byte[1024];
                int bytesRead = await _dashboardStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                if (bytesRead == 0)
                {
                    _logger.Warning("Robot dashboard disconnected during command response");
                    return -1;
                }

                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
                _logger.Debug("Robot command response: {Response}", response);
                
                var parts = response.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && parts[0] == "0" && int.TryParse(parts[1], out int cmdId))
                {
                    _logger.Information("Robot command executed successfully, CommandId: {CommandId}", cmdId);
                    return cmdId;
                }
                
                _logger.Warning("Robot command rejected: {Response}", response);
                return -1;
            }
            catch (OperationCanceledException)
            {
                _logger.Warning("Robot command timeout");
                return -1;
            }
            catch (SocketException ex)
            {
                _logger.Error(ex, "Robot socket error during command execution");
                return -1;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error executing robot command");
                return -1;
            }
            finally
            {
                _dashboardSemaphore.Release();
            }
        }

        public async Task<RobotMode> GetRobotModeAsync(CancellationToken token)
        {
            if (_modbusMaster == null || _modbusClient?.Connected != true)
            {
                _logger.Warning("Cannot get robot mode - Modbus not connected");
                return RobotMode.ERROR;
            }

            await _modbusSemaphore.WaitAsync(token);
            try
            {
                _logger.Debug("Reading robot mode from register {Register}", _config.RobotModbusStatusRegister);
                ushort[] registers = await _modbusMaster.ReadInputRegistersAsync(1, _config.RobotModbusStatusRegister, 1);
                if (registers.Length > 0)
                {
                    int modeValue = registers[0];
                    if (Enum.IsDefined(typeof(RobotMode), modeValue))
                    {
                        var mode = (RobotMode)modeValue;
                        _logger.Debug("Robot mode: {Mode}", mode);
                        return mode;
                    }
                    _logger.Warning("Unknown robot mode value: {ModeValue}", modeValue);
                }
                return RobotMode.ERROR;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error reading robot mode");
                return RobotMode.ERROR;
            }
            finally
            {
                _modbusSemaphore.Release();
            }
        }

        public async Task<int> GetCurrentCommandIdAsync(CancellationToken token)
        {
            if (_dashboardStream == null || _dashboardClient?.Connected != true)
            {
                _logger.Warning("Cannot get current command ID - dashboard not connected");
                return -1;
            }

            await _dashboardSemaphore.WaitAsync(token);
            try
            {
                _logger.Debug("Requesting current command ID");
                byte[] data = Encoding.ASCII.GetBytes("GetCurrentCommandId();");
                await _dashboardStream.WriteAsync(data, 0, data.Length, token);

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
                cts.CancelAfter(_config.RobotCommandTimeoutMs);

                byte[] buffer = new byte[1024];
                int bytesRead = await _dashboardStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                if (bytesRead == 0) return -1;

                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
                _logger.Debug("Current command ID response: {Response}", response);

                var parts = response.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && parts[0] == "0" && int.TryParse(parts[1], out int cmdId))
                {
                    _logger.Debug("Current command ID: {CommandId}", cmdId);
                    return cmdId;
                }
                return -1;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting current command ID");
                return -1;
            }
            finally
            {
                _dashboardSemaphore.Release();
            }
        }

        public async Task<HardwareMotionResult> JogAsync(JogType jogType, int speedPercent, CancellationToken token)
        {
            string command = jogType switch
            {
                JogType.JOG_PLUS_X => $"JogPlusX({speedPercent});",
                JogType.JOG_MINUS_X => $"JogMinusX({speedPercent});",
                JogType.JOG_PLUS_Y => $"JogPlusY({speedPercent});",
                JogType.JOG_MINUS_Y => $"JogMinusY({speedPercent});",
                JogType.JOG_PLUS_Z => $"JogPlusZ({speedPercent});",
                JogType.JOG_MINUS_Z => $"JogMinusZ({speedPercent});",
                JogType.JOG_PLUS_R => $"JogPlusR({speedPercent});",
                JogType.JOG_MINUS_R => $"JogMinusR({speedPercent});",
                _ => throw new ArgumentException($"Invalid JogType: {jogType}", nameof(jogType))
            };

            int result = await ExecuteCommandAsync(command, token);
            
            if (result >= 0)
            {
                _logger.Information("Jog command executed: {JogType} at {Speed}%", jogType, speedPercent);
                return HardwareMotionResult.Success($"Jog {jogType} started");
            }
            else
            {
                _logger.Warning("Jog command failed: {JogType}", jogType);
                return HardwareMotionResult.Failure($"Jog {jogType} failed");
            }
        }

        public async Task<HardwareMotionResult> StopJogAsync()
        {
            int result = await ExecuteCommandAsync("StopJog();", CancellationToken.None);
            
            if (result >= 0)
            {
                _logger.Information("Jog stopped successfully");
                return HardwareMotionResult.Success("Jog stopped");
            }
            else
            {
                _logger.Warning("Jog stop command failed");
                return HardwareMotionResult.Failure("Jog stop failed");
            }
        }

        public async Task<HardwareMotionResult> MoveToAsync(double x, double y, double z, double r, CancellationToken token)
        {
            string command = $"MoveL({x},{y},{z},{r},0,0,0,0);";
            int result = await ExecuteCommandAsync(command, token);
            
            if (result >= 0)
            {
                _logger.Information("MoveTo command executed: ({X},{Y},{Z},{R})", x, y, z, r);
                return HardwareMotionResult.Success($"Moved to ({x:F2},{y:F2},{z:F2},{r:F2})");
            }
            else
            {
                _logger.Warning("MoveTo command failed");
                return HardwareMotionResult.Failure("MoveTo failed");
            }
        }

        public async Task<HardwareMotionResult> HomeAsync(CancellationToken token)
        {
            int result = await ExecuteCommandAsync("Home();", token);
            
            if (result >= 0)
            {
                _logger.Information("Home command executed");
                return HardwareMotionResult.Success("Homing completed");
            }
            else
            {
                _logger.Warning("Home command failed");
                return HardwareMotionResult.Failure("Homing failed");
            }
        }

        public async Task<HardwareMotionResult> StopAsync()
        {
            int result = await ExecuteCommandAsync("Stop();", CancellationToken.None);
            
            if (result >= 0)
            {
                _logger.Information("Emergency stop executed");
                return HardwareMotionResult.Success("Stopped");
            }
            else
            {
                _logger.Warning("Emergency stop command failed");
                return HardwareMotionResult.Failure("Stop failed");
            }
        }

        public async Task<RobotCoordinatePose> GetPositionAsync(CancellationToken token)
        {
            if (_dashboardStream == null || _dashboardClient?.Connected != true)
            {
                _logger.Warning("Cannot get position - dashboard not connected");
                return null;
            }

            await _dashboardSemaphore.WaitAsync(token);
            try
            {
                _logger.Debug("Requesting current position");
                byte[] data = Encoding.ASCII.GetBytes("GetActualTCP();");
                await _dashboardStream.WriteAsync(data, 0, data.Length, token);

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
                cts.CancelAfter(_config.RobotCommandTimeoutMs);

                byte[] buffer = new byte[1024];
                int bytesRead = await _dashboardStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                if (bytesRead == 0) return null;

                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
                _logger.Debug("Position response: {Response}", response);

                // Parse response format: 0,x,y,z,r,p,w
                var parts = response.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 5 && parts[0] == "0" &&
                    double.TryParse(parts[1], out double x) &&
                    double.TryParse(parts[2], out double y) &&
                    double.TryParse(parts[3], out double z) &&
                    double.TryParse(parts[4], out double r))
                {
                    return new RobotCoordinatePose { X = x, Y = y, Z = z, R = r };
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting position");
                return null;
            }
            finally
            {
                _dashboardSemaphore.Release();
            }
        }

        public async Task<RobotMode> GetModeAsync(CancellationToken token)
        {
            return await GetRobotModeAsync(token);
        }

        public async Task DisconnectAsync()
        {
            _logger.Information("Disconnecting from robot");
            _dashboardStream?.Dispose();
            _dashboardClient?.Dispose();
            _modbusClient?.Dispose();
            
            _dashboardStream = null;
            _dashboardClient = null;
            _modbusMaster = null;
            _modbusClient = null;

            await Task.CompletedTask;
            _logger.Information("Robot disconnected");
        }

        public void Dispose()
        {
            if (_disposed) return;
            _logger.Information("Disposing robot controller");
            _ = DisconnectAsync();
            _dashboardSemaphore?.Dispose();
            _modbusSemaphore?.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
