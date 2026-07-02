using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShoeMoldControl.Core
{
    public interface IVisionConfig
    {
        string VisionIpAddress { get; }
        int VisionPort { get; }
        int VisionTimeoutMs { get; }
        string VisionTriggerCommand { get; }
    }

    public interface IRobotConfig
    {
        string RobotIpAddress { get; }
        int RobotDashboardPort { get; }
        int RobotModbusPort { get; }
        ushort RobotModbusStatusRegister { get; }
        int RobotCommandTimeoutMs { get; }
    }

    public interface IBarcodeParser
    {
        string GenerateScriptCommand(string decodedText);
    }

    public interface IVisionService : IDisposable
    {
        Task<DecodeResult> GrabAndDecodeAsync(CancellationToken token);
    }

    public interface IRobotController : IDisposable
    {
        Task<bool> ConnectAsync();
        Task DisconnectAsync();
        Task<int> ExecuteCommandAsync(string command, CancellationToken token);
        Task<RobotMode> GetRobotModeAsync(CancellationToken token);
        Task<int> GetCurrentCommandIdAsync(CancellationToken token);
    }

    public interface IShoeMoldWorkflow
    {
        Task RunProductionCycleAsync(CancellationToken token);
    }
}
