#if NET48 || NET8_0_OR_GREATER
using AuroraVision;
using ShoeMoldControl.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShoeMoldControl.Core.Hardware
{
    /// <summary>
    /// 機器人線性運動控制介面
    /// </summary>
    public interface IAvrRobotMotion
    {
        Task<HardwareMotionResult> MoveLinearAsync(RobotCoordinatePose pose, int speed, int acceleration, CancellationToken token);
        Task<HardwareMotionResult> SetSpeedFactorAsync(int speedFactor);
        Task<RobotCoordinatePose> GetCurrentPoseAsync();
    }

    /// <summary>
    /// 機器人手動教導/拖曳模式控制介面
    /// </summary>
    public interface IAvrRobotInstructionalJog
    {
        Task<HardwareMotionResult> StartJogAsync(JogType jogDirection);
        Task<HardwareMotionResult> StopJogAsync();
        Task<HardwareMotionResult> SetDragModeAsync(bool enable);
    }

    /// <summary>
    /// PLC 通訊服務介面
    /// </summary>
    public interface IAvrPlcCommunicator
    {
        Task<int> ReadRegisterAsync(int address);
        Task WriteRegisterAsync(int address, int value);
    }

    /// <summary>
    /// 相機驅動介面 (AVL 特定)
    /// </summary>
    public interface IAvrCameraDriver
    {
        Task<Avl.Image> CaptureNativeFrameAsync(CancellationToken token);
    }

    /// <summary>
    /// 視覺分析器介面 (AVL 特定)
    /// </summary>
    public interface IAvrImageAnalyzer
    {
        VisionInspectionResult AnalyzeFrame(Avl.Image image);
    }
}

#endif