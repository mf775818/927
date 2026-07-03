using System;
using System.Threading;
using System.Threading.Tasks;
using ShoeMoldControl.Core;
using Serilog;

namespace ShoeMoldControl.Infrastructure
{
    /// <summary>
    /// 虛擬機器人控制器 - 用於模擬環境或無連線狀態
    /// 提供模擬的機器人命令執行，確保系統可在無實際硬體下測試
    /// </summary>
    public class MockRobotController : IRobotController
    {
        private readonly ILogger _logger;
        private readonly MockRobotOptions _options;
        private bool _disposed;
        private bool _isConnected;
        private int _currentCommandId;
        private RobotMode _robotMode;

        public MockRobotController(MockRobotOptions options = null, ILogger logger = null)
        {
            _options = options ?? new MockRobotOptions();
            _logger = logger ?? Log.ForContext<MockRobotController>();
            _currentCommandId = 0;
            _robotMode = RobotMode.INIT;
            _isConnected = false;
            _logger.Information("Mock Robot Controller initialized with options: {Options}", _options);
        }

        public async Task<bool> ConnectAsync()
        {
            if (_disposed)
            {
                _logger.Warning("Mock Robot Controller is disposed");
                return false;
            }

            try
            {
                _logger.Debug("Mock Robot: Simulating connection...");
                await Task.Delay(_options.SimulatedConnectionDelayMs, CancellationToken.None);

                _isConnected = true;
                _robotMode = RobotMode.ENABLE;
                _currentCommandId = 0;

                _logger.Information("Mock Robot: Connected successfully, mode: {Mode}", _robotMode);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Mock Robot: Connection failed");
                _isConnected = false;
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            if (_disposed) return;

            _logger.Information("Mock Robot: Disconnecting...");
            await Task.Delay(100, CancellationToken.None);

            _isConnected = false;
            _robotMode = RobotMode.POWEROFF;
            
            _logger.Information("Mock Robot: Disconnected");
        }

        public async Task<int> ExecuteCommandAsync(string command, CancellationToken token)
        {
            if (!_isConnected)
            {
                _logger.Warning("Mock Robot: Cannot execute command - not connected");
                return -1;
            }

            if (_disposed)
            {
                _logger.Warning("Mock Robot: Controller is disposed");
                return -1;
            }

            try
            {
                _logger.Debug("Mock Robot: Executing command: {Command}", command);

                // 模擬處理延遲
                await Task.Delay(_options.SimulatedCommandDelayMs, token);

                // 模擬成功率
                if (_options.SuccessRate < 1.0)
                {
                    var random = new Random();
                    if (random.NextDouble() > _options.SuccessRate)
                    {
                        _logger.Warning("Mock Robot: Simulated command failure");
                        return -1;
                    }
                }

                // 檢查命令格式
                if (string.IsNullOrWhiteSpace(command))
                {
                    _logger.Warning("Mock Robot: Empty command rejected");
                    return -1;
                }

                // 生成新的命令 ID
                _currentCommandId++;
                
                _logger.Information("Mock Robot: Command '{Command}' accepted with ID {CommandId}", 
                    command, _currentCommandId);

                return _currentCommandId;
            }
            catch (OperationCanceledException)
            {
                _logger.Debug("Mock Robot: Command cancelled");
                return -1;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Mock Robot: Unexpected error executing command");
                return -1;
            }
        }

        public async Task<RobotMode> GetRobotModeAsync(CancellationToken token)
        {
            if (!_isConnected)
            {
                return RobotMode.ERROR;
            }

            // 模擬輕微延遲
            await Task.Delay(10, token);

            // 模擬模式轉換
            if (_currentCommandId > 0 && _robotMode == RobotMode.ENABLE)
            {
                // 有命令執行中時，偶爾模擬 RUNNING 模式
                var random = new Random();
                if (random.NextDouble() < 0.3)
                {
                    _robotMode = RobotMode.RUNNING;
                }
                else
                {
                    _robotMode = RobotMode.ENABLE;
                }
            }

            return _robotMode;
        }

        public async Task<int> GetCurrentCommandIdAsync(CancellationToken token)
        {
            if (!_isConnected)
            {
                return -1;
            }

            await Task.Delay(10, token);
            return _currentCommandId;
        }

        public void Dispose()
        {
            if (_disposed) return;
            
            _logger.Information("Disposing Mock Robot Controller");
            _ = DisconnectAsync();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// 虛擬機器人控制器選項
    /// </summary>
    public class MockRobotOptions
    {
        /// <summary>
        /// 模擬連接延遲（毫秒）
        /// </summary>
        public int SimulatedConnectionDelayMs { get; set; } = 200;

        /// <summary>
        /// 模擬命令執行延遲（毫秒）
        /// </summary>
        public int SimulatedCommandDelayMs { get; set; } = 300;

        /// <summary>
        /// 模擬成功率 (0.0 - 1.0)
        /// </summary>
        public double SuccessRate { get; set; } = 1.0;

        /// <summary>
        /// 是否啟用隨機失敗
        /// </summary>
        public bool EnableRandomFailure => SuccessRate < 1.0;

        public override string ToString()
        {
            return $"ConnectDelay:{SimulatedConnectionDelayMs}ms, CommandDelay:{SimulatedCommandDelayMs}ms, SuccessRate:{SuccessRate:P1}";
        }
    }
}
