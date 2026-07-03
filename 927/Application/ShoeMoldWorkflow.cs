using System;
using System.Threading;
using System.Threading.Tasks;
using ShoeMoldControl.Core;
using Serilog;

namespace ShoeMoldControl.Application
{
    public class ShoeMoldWorkflow : IShoeMoldWorkflow
    {
        private readonly IVisionService _visionService;
        private readonly IRobotController _robotController;
        private readonly IBarcodeParser _barcodeParser;
        private readonly ILogger _logger;
        private readonly IConnectionStateManager _connectionStateManager;

        public ShoeMoldWorkflow(IVisionService visionService, IRobotController robotController, IBarcodeParser barcodeParser, IConnectionStateManager connectionStateManager = null, ILogger logger = null)
        {
            _visionService = visionService ?? throw new ArgumentNullException(nameof(visionService));
            _robotController = robotController ?? throw new ArgumentNullException(nameof(robotController));
            _barcodeParser = barcodeParser ?? throw new ArgumentNullException(nameof(barcodeParser));
            _connectionStateManager = connectionStateManager;
            _logger = logger ?? Log.ForContext<ShoeMoldWorkflow>();
        }

        public async Task RunProductionCycleAsync(CancellationToken token)
        {
            _logger.Information("Starting production cycle");
            int cycleCount = 0;
            int successCount = 0;
            int failureCount = 0;

            // 嘗試連接設備（如果尚未連接）
            await InitializeConnectionsAsync(token);

            while (!token.IsCancellationRequested)
            {
                try
                {
                    cycleCount++;
                    _logger.Debug("Production cycle #{CycleNumber} started", cycleCount);

                    DecodeResult visionResult = await _visionService.GrabAndDecodeAsync(token);
                    
                    if (!visionResult.IsSuccess)
                    {
                        failureCount++;
                        _logger.Warning("Vision decode failed in cycle {CycleNumber}: {Error}", 
                            cycleCount, visionResult.ErrorMessage);
                        
                        // Industrial level handling: wait before next trigger attempt on failure
                        await Task.Delay(1000, token);
                        continue;
                    }

                    string command = _barcodeParser.GenerateScriptCommand(visionResult.DecodedText);
                    if (string.IsNullOrEmpty(command))
                    {
                        failureCount++;
                        _logger.Warning("Invalid barcode format in cycle {CycleNumber}: {DecodedText}", 
                            cycleCount, visionResult.DecodedText);
                        await Task.Delay(1000, token);
                        continue;
                    }

                    _logger.Information("Executing robot command for cycle {CycleNumber}: {Command}", 
                        cycleCount, command);
                    
                    int commandId = await _robotController.ExecuteCommandAsync(command, token);

                    if (commandId > 0)
                    {
                        _logger.Information("Robot command accepted with ID {CommandId}, synchronizing...", commandId);
                        await SyncRobotOperationAsync(commandId, token);
                        successCount++;
                        _logger.Information("Production cycle #{CycleNumber} completed successfully. Success rate: {SuccessRate:F1}%", 
                            cycleCount, (double)successCount / cycleCount * 100);
                    }
                    else
                    {
                        failureCount++;
                        _logger.Warning("Robot command rejected in cycle {CycleNumber}", cycleCount);
                        await Task.Delay(1000, token);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.Information("Production cycle cancelled by user");
                    break;
                }
                catch (Exception ex)
                {
                    failureCount++;
                    _logger.Error(ex, "Unhandled error in production cycle {CycleNumber}", cycleCount);
                    // Network or unhandled errors -> backoff and retry
                    await Task.Delay(2000, token);
                }
            }

            _logger.Information("Production cycle stopped. Total: {Total}, Success: {Success}, Failures: {Failures}", 
                cycleCount, successCount, failureCount);
        }

        /// <summary>
        /// 初始化設備連接
        /// </summary>
        private async Task InitializeConnectionsAsync(CancellationToken token)
        {
            try
            {
                // 檢查是否為模擬模式
                if (_connectionStateManager != null && _connectionStateManager.IsSimulationMode)
                {
                    _logger.Information("Running in SIMULATION mode - using mock services");
                    
                    // 模擬模式下，設置虛擬連接狀態
                    if (_robotController is MockRobotController mockRobot)
                    {
                        await mockRobot.ConnectAsync();
                        _connectionStateManager.UpdateRobotConnectionStatus(true);
                    }
                    
                    _connectionStateManager.UpdateVisionConnectionStatus(true);
                    return;
                }

                // 生產模式：嘗試連接實體設備
                _logger.Information("Running in PRODUCTION mode - connecting to physical devices");
                
                if (_robotController is DobotCraController realRobot)
                {
                    bool robotConnected = await realRobot.ConnectAsync();
                    _connectionStateManager?.UpdateRobotConnectionStatus(robotConnected);
                    
                    if (!robotConnected)
                    {
                        _logger.Warning("Failed to connect to robot - production cycle may fail");
                    }
                }
                
                // 視覺服務在每次 GrabAndDecode 時自動處理連接
                _connectionStateManager?.UpdateVisionConnectionStatus(true);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error initializing device connections");
            }
        }

        private async Task SyncRobotOperationAsync(int targetCommandId, CancellationToken token)
        {
            bool isSync = false;
            int pollCount = 0;
            const int maxPollCount = 200; // Prevent infinite loop (10 seconds max at 50ms intervals)

            while (!isSync && !token.IsCancellationRequested && pollCount < maxPollCount)
            {
                pollCount++;
                int currentId = await _robotController.GetCurrentCommandIdAsync(token);
                RobotMode currentMode = await _robotController.GetRobotModeAsync(token);

                if (currentId >= targetCommandId && currentMode == RobotMode.ENABLE)
                {
                    isSync = true;
                    _logger.Debug("Robot synchronization completed after {PollCount} polls", pollCount);
                }
                else
                {
                    if (pollCount % 20 == 0) // Log every second
                    {
                        _logger.Debug("Synchronizing... CurrentId: {CurrentId}, TargetId: {TargetId}, Mode: {Mode}", 
                            currentId, targetCommandId, currentMode);
                    }
                    await Task.Delay(50, token); 
                }
            }

            if (!isSync)
            {
                _logger.Warning("Robot synchronization timeout after {PollCount} polls", pollCount);
            }
        }
    }
}
