using System;
using System.Threading;
using System.Threading.Tasks;
using ShoeMoldControl.Core;

namespace ShoeMoldControl.Application
{
    public class ShoeMoldWorkflow : IShoeMoldWorkflow
    {
        private readonly IVisionService _visionService;
        private readonly IRobotController _robotController;
        private readonly IBarcodeParser _barcodeParser;

        public ShoeMoldWorkflow(IVisionService visionService, IRobotController robotController, IBarcodeParser barcodeParser)
        {
            _visionService = visionService ?? throw new ArgumentNullException(nameof(visionService));
            _robotController = robotController ?? throw new ArgumentNullException(nameof(robotController));
            _barcodeParser = barcodeParser ?? throw new ArgumentNullException(nameof(barcodeParser));
        }

        public async Task RunProductionCycleAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    DecodeResult visionResult = await _visionService.GrabAndDecodeAsync(token);
                    
                    if (!visionResult.IsSuccess)
                    {
                        // Industrial level handling: wait before next trigger attempt on failure
                        await Task.Delay(1000, token);
                        continue;
                    }

                    string command = _barcodeParser.GenerateScriptCommand(visionResult.DecodedText);
                    if (string.IsNullOrEmpty(command))
                    {
                        await Task.Delay(1000, token);
                        continue;
                    }

                    int commandId = await _robotController.ExecuteCommandAsync(command, token);

                    if (commandId > 0)
                    {
                        await SyncRobotOperationAsync(commandId, token);
                    }
                    else
                    {
                        await Task.Delay(1000, token);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                    // Network or unhandled errors -> backoff and retry
                    await Task.Delay(2000, token);
                }
            }
        }

        private async Task SyncRobotOperationAsync(int targetCommandId, CancellationToken token)
        {
            bool isSync = false;
            while (!isSync && !token.IsCancellationRequested)
            {
                int currentId = await _robotController.GetCurrentCommandIdAsync(token);
                RobotMode currentMode = await _robotController.GetRobotModeAsync(token);

                if (currentId >= targetCommandId && currentMode == RobotMode.ENABLE)
                {
                    isSync = true;
                }
                else
                {
                    await Task.Delay(50, token); 
                }
            }
        }
    }
}
