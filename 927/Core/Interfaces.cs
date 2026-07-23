using System;
using System.Threading;
using System.Threading.Tasks;
using ShoeMoldControl.Core.Models;

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
        
        /// <summary>
        /// 擷取原始影像資料（模擬或實際）
        /// </summary>
        Task<byte[]> CaptureAsync(CancellationToken token);
        
        /// <summary>
        /// 對影像進行檢測分析
        /// </summary>
        Task<VisionInspectionResult> InspectAsync(byte[] imageData, CancellationToken token);
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

    /// <summary>
    /// 工業級機器人控制器介面 - 提供完整運動控制功能
    /// 設計原則：避免第三方依賴，使用標準數值類型確保通用性
    /// </summary>
    public interface IRobotController : IDisposable
    {
        Task<bool> ConnectAsync();
        Task DisconnectAsync();
        Task<int> ExecuteCommandAsync(string command, CancellationToken token);
        Task<RobotMode> GetRobotModeAsync(CancellationToken token);
        Task<int> GetCurrentCommandIdAsync(CancellationToken token);
        
        /// <summary>
        /// Jog 點動控制 - 持續移動模式
        /// </summary>
        /// <param name="jogType">Jog 方向類型</param>
        /// <param name="speedPercent">速度百分比 (0-100)</param>
        /// <param name="token">取消權杖</param>
        Task HardwareMotionResult JogAsync(JogType jogType, int speedPercent, CancellationToken token);
        
        /// <summary>
        /// 停止 Jog 點動
        /// </summary>
        Task<HardwareMotionResult> StopJogAsync();
        
        /// <summary>
        /// 移動至指定座標位置
        /// </summary>
        /// <param name="x">X 軸座標</param>
        /// <param name="y">Y 軸座標</param>
        /// <param name="z">Z 軸座標</param>
        /// <param name="r">R 軸角度</param>
        /// <param name="token">取消權杖</param>
        Task<HardwareMotionResult> MoveToAsync(double x, double y, double z, double r, CancellationToken token);
        
        /// <summary>
        /// 回原點
        /// </summary>
        Task<HardwareMotionResult> HomeAsync(CancellationToken token);
        
        /// <summary>
        /// 緊急停止
        /// </summary>
        Task<HardwareMotionResult> StopAsync();
        
        /// <summary>
        /// 獲取當前座標姿態
        /// </summary>
        Task<RobotCoordinatePose> GetPositionAsync(CancellationToken token);
        
        /// <summary>
        /// 獲取當前運行模式（別名方法，相容 GetRobotModeAsync）
        /// </summary>
        Task<RobotMode> GetModeAsync(CancellationToken token);
    }

    public interface IShoeMoldWorkflow
    {
        Task RunProductionCycleAsync(CancellationToken token);
    }
}
