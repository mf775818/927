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

    /// <summary>
    /// 上位機主流程專用之共通視覺服務介面，隔絕硬體型別差異
    /// </summary>
    public interface IVisionService : IDisposable
    {
        Task<DecodeResult> GrabAndDecodeAsync(CancellationToken token);
    }

    /// <summary>
    /// 泛型相機硬體驅動介面
    /// </summary>
    /// <typeparam name="TFrame">底層硬體所屬之原生影像型別</typeparam>
    public interface ICameraDriver<TFrame> : IDisposable
    {
        Task ConnectAsync(CancellationToken token);
        Task<TFrame> CaptureFrameAsync(CancellationToken token);
        void Disconnect();
        bool IsConnected { get; }
    }

    /// <summary>
    /// 泛型圖像分析演算法介面
    /// </summary>
    /// <typeparam name="TFrame">指定分析之影像型別</typeparam>
    public interface IImageAnalyzer<TFrame>
    {
        DecodeResult Analyze(TFrame frame);
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
