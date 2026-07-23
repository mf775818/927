using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ShoeMoldControl.Core;
using ShoeMoldControl.Core.Domain;
using Serilog;

namespace ShoeMoldControl.Vision
{
    /// <summary>
    /// 虛擬視覺服務 - 用於模擬環境或無連線狀態
    /// 提供模擬的解碼結果，確保系統可在無實際硬體下測試
    /// </summary>
    public class MockVisionService : IVisionService
    {
        private readonly ILogger _logger;
        private readonly MockVisionOptions _options;
        private bool _disposed;
        private int _cycleCount;

        public MockVisionService(MockVisionOptions options = null, ILogger logger = null)
        {
            _options = options ?? new MockVisionOptions();
            _logger = logger ?? Log.ForContext<MockVisionService>();
            _cycleCount = 0;
            _logger.Information("Mock Vision Service initialized with options: {Options}", _options);
        }

        public async Task<DecodeResult> GrabAndDecodeAsync(CancellationToken token)
        {
            if (_disposed)
            {
                _logger.Warning("Mock Vision Service is disposed");
                return new DecodeResult { IsSuccess = false, ErrorMessage = "Service disposed" };
            }

            try
            {
                _logger.Debug("Mock Vision: Simulating grab and decode operation...");
                
                // 模擬處理延遲
                await Task.Delay(_options.SimulatedDelayMs, token);

                _cycleCount++;

                // 模擬成功率
                if (_options.SuccessRate < 1.0)
                {
                    var random = new Random();
                    if (random.NextDouble() > _options.SuccessRate)
                    {
                        _logger.Warning("Mock Vision: Simulated failure (cycle {CycleCount})", _cycleCount);
                        return new DecodeResult 
                        { 
                            IsSuccess = false, 
                            ErrorMessage = "Simulated decode failure" 
                        };
                    }
                }

                // 生成模擬的條碼數據
                string simulatedBarcode = GenerateSimulatedBarcode(_cycleCount);
                
                _logger.Information("Mock Vision: Successfully decoded '{Barcode}' (cycle {CycleCount})", 
                    simulatedBarcode, _cycleCount);

                return new DecodeResult 
                { 
                    IsSuccess = true, 
                    DecodedText = simulatedBarcode 
                };
            }
            catch (OperationCanceledException)
            {
                _logger.Debug("Mock Vision: Operation cancelled");
                return new DecodeResult { IsSuccess = false, ErrorMessage = "Operation cancelled" };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Mock Vision: Unexpected error");
                return new DecodeResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<byte[]> CaptureAsync(CancellationToken token)
        {
            if (_disposed)
            {
                _logger.Warning("Mock Vision Service is disposed");
                return Array.Empty<byte>();
            }

            try
            {
                _logger.Debug("Mock Vision: Capturing simulated image...");
                await Task.Delay(50, token);

                // 生成模擬影像並轉換為 byte[]
                using var bitmap = GenerateTestImage(new Core.Models.RobotCoordinatePose(), null);
                using var ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
            catch (OperationCanceledException)
            {
                _logger.Debug("Mock Vision: Image capture cancelled");
                return Array.Empty<byte>();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Mock Vision: Error capturing image");
                return Array.Empty<byte>();
            }
        }

        public async Task<VisionInspectionResult> InspectAsync(byte[] imageData, CancellationToken token)
        {
            if (_disposed)
            {
                _logger.Warning("Mock Vision Service is disposed");
                return new VisionInspectionResult { IsSuccess = false, ErrorMessage = "Service disposed" };
            }

            try
            {
                _logger.Debug("Mock Vision: Inspecting image...");
                await Task.Delay(100, token);

                _cycleCount++;
                string barcode = GenerateSimulatedBarcode(_cycleCount);

                _logger.Information("Mock Vision: Inspection successful, barcode: {Barcode}", barcode);

                return new VisionInspectionResult
                {
                    IsSuccess = true,
                    BarcodeText = barcode,
                    Confidence = 0.95,
                    MarkPositions = new System.Collections.Generic.List<(double, double)>
                    {
                        (100.0, 150.0),
                        (200.0, 150.0)
                    }
                };
            }
            catch (OperationCanceledException)
            {
                _logger.Debug("Mock Vision: Inspection cancelled");
                return new VisionInspectionResult { IsSuccess = false, ErrorMessage = "Operation cancelled" };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Mock Vision: Inspection error");
                return new VisionInspectionResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        /// <summary>
        /// 生成模擬測試影像（用於 UI 顯示）
        /// </summary>
        public Bitmap GenerateTestImage(Core.Models.RobotCoordinatePose pose, string barcodeText)
        {
            var bitmap = new Bitmap(640, 480);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.FromArgb(30, 30, 30));

                // 繪製網格線
                using var gridPen = new Pen(Color.FromArgb(50, 50, 50), 1);
                for (int i = 0; i < 640; i += 40)
                {
                    g.DrawLine(gridPen, i, 0, i, 480);
                    g.DrawLine(gridPen, 0, i, 640, i);
                }

                // 繪製模擬標記點
                using var markBrush = new SolidBrush(Color.FromArgb(0, 255, 0));
                g.FillEllipse(markBrush, 280, 200, 80, 80);

                // 繪製座標資訊
                using var font = new Font("Consolas", 12);
                using var textBrush = new SolidBrush(Color.FromArgb(0, 255, 0));
                g.DrawString($"X: {pose.X:F2}", font, textBrush, 10, 10);
                g.DrawString($"Y: {pose.Y:F2}", font, textBrush, 10, 30);
                g.DrawString($"Z: {pose.Z:F2}", font, textBrush, 10, 50);
                g.DrawString($"R: {pose.R:F2}", font, textBrush, 10, 70);

                // 繪製條碼文字
                if (!string.IsNullOrEmpty(barcodeText))
                {
                    g.DrawString($"Barcode: {barcodeText}", font, textBrush, 10, 110);
                }

                // 繪製十字準星
                using var crossPen = new Pen(Color.Red, 2);
                int cx = 320, cy = 240;
                g.DrawLine(crossPen, cx - 20, cy, cx + 20, cy);
                g.DrawLine(crossPen, cx, cy - 20, cx, cy + 20);
            }

            return bitmap;
        }

        /// <summary>
        /// 生成模擬的條碼數據
        /// 格式：TYPE-SIZE-SEQUENCE (例如：A01-42-0001)
        /// </summary>
        private string GenerateSimulatedBarcode(int cycleNumber)
        {
            if (!string.IsNullOrEmpty(_options.FixedBarcode))
            {
                return _options.FixedBarcode;
            }

            string[] types = { "A", "B", "C", "D" };
            string type = types[cycleNumber % types.Length];
            int size = 38 + (cycleNumber % 10); // 38-47
            int sequence = cycleNumber % 10000;

            return $"{type}0{size}-{sequence:D4}";
        }

        public void Dispose()
        {
            if (_disposed) return;
            _logger.Information("Disposing Mock Vision Service");
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// 虛擬視覺服務選項
    /// </summary>
    public class MockVisionOptions
    {
        /// <summary>
        /// 模擬延遲（毫秒）
        /// </summary>
        public int SimulatedDelayMs { get; set; } = 500;

        /// <summary>
        /// 模擬成功率 (0.0 - 1.0)
        /// </summary>
        public double SuccessRate { get; set; } = 1.0;

        /// <summary>
        /// 固定條碼（如果設置，則始終返回此條碼）
        /// </summary>
        public string FixedBarcode { get; set; } = string.Empty;

        /// <summary>
        /// 是否啟用隨機失敗
        /// </summary>
        public bool EnableRandomFailure => SuccessRate < 1.0;

        public override string ToString()
        {
            return $"Delay:{SimulatedDelayMs}ms, SuccessRate:{SuccessRate:P1}, FixedBarcode:{FixedBarcode ?? "None"}";
        }
    }
}
