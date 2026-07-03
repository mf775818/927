using System;
using System.Threading;
using System.Threading.Tasks;
using ShoeMoldControl.Core;
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
