using System;
using System.Collections.Generic;

namespace ShoeMoldControl.Core.Domain
{
    /// <summary>
    /// 硬體運動執行結果封裝
    /// </summary>
    public class HardwareMotionResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> DetailedLogs { get; set; } = new List<string>();
    }

    /// <summary>
    /// 視覺檢測結果封裝
    /// </summary>
    public class VisionInspectionResult
    {
        public bool IsSuccess { get; set; }
        public int MoldNumber { get; set; }
        public System.Drawing.RectangleF BoundBox { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// 機器人座標姿態標準模型
    /// </summary>
    public class RobotCoordinatePose
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Rx { get; set; }
        public double Ry { get; set; }
        public double Rz { get; set; }
    }
}
