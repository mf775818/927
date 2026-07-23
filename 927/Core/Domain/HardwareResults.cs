using System;
using System.Collections.Generic;
using System.Drawing;
using ShoeMoldControl.Core.Models;

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
        
        public static HardwareMotionResult Success(string message = "OK") => 
            new HardwareMotionResult { IsSuccess = true, Message = message };
        
        public static HardwareMotionResult Failure(string message) => 
            new HardwareMotionResult { IsSuccess = false, Message = message };
    }

    /// <summary>
    /// 視覺檢測結果封裝
    /// </summary>
    public class VisionInspectionResult
    {
        public bool IsSuccess { get; set; }
        public string BarcodeText { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public List<(double X, double Y)> MarkPositions { get; set; } = new();
    }

    /// <summary>
    /// 機器人座標姿態標準模型（使用標準 double 類型，避免第三方依賴）
    /// </summary>
    public class RobotCoordinatePose
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double R { get; set; }
        
        public override string ToString() => $"X:{X:F2} Y:{Y:F2} Z:{Z:F2} R:{R:F2}";
    }
}
