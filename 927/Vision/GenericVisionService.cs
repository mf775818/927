using System;
using System.Threading;
using System.Threading.Tasks;
using ShoeMoldControl.Core;
using ShoeMoldControl.Infrastructure.Polly;
using Polly.CircuitBreaker;
using Serilog;

namespace ShoeMoldControl.Vision
{
    /// <summary>
    /// 泛型視覺服務協調器 - 整合相機驅動與圖像分析器
    /// 透過泛型隔離底層硬體差異，提供統一的 IVisionService 介面
    /// </summary>
    public class GenericVisionService<TFrame> : IVisionService
    {
        private readonly ICameraDriver<TFrame> _cameraDriver;
        private readonly IImageAnalyzer<TFrame> _imageAnalyzer;
        private readonly IResiliencePolicyProvider _policyProvider;
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private bool _disposed;

        public GenericVisionService(
            ICameraDriver<TFrame> cameraDriver,
            IImageAnalyzer<TFrame> imageAnalyzer,
            IResiliencePolicyProvider policyProvider,
            ILogger logger = null)
        {
            _cameraDriver = cameraDriver ?? throw new ArgumentNullException(nameof(cameraDriver));
            _imageAnalyzer = imageAnalyzer ?? throw new ArgumentNullException(nameof(imageAnalyzer));
            _policyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider));
            _logger = logger ?? Log.ForContext<GenericVisionService<TFrame>>();
        }

        public async Task<DecodeResult> GrabAndDecodeAsync(CancellationToken token)
        {
            try
            {
                return await _policyProvider.VisionCircuitBreaker.ExecuteAsync(
                    async (ct) => await _policyProvider.VisionRetryPolicy.ExecuteAsync(
                        async (t) => await ExecutePipelineInternalAsync(t),
                        ct),
                    token);
            }
            catch (BrokenCircuitException)
            {
                _logger.Warning("Vision pipeline circuit breaker is OPEN - rejecting request");
                return new DecodeResult { IsSuccess = false, ErrorMessage = "Vision hardware subsystem temporarily unavailable (circuit open)" };
            }
            catch (OperationCanceledException)
            {
                _logger.Debug("Vision pipeline operation cancelled by host application");
                return new DecodeResult { IsSuccess = false, ErrorMessage = "Operation cancelled" };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected critical error in vision generic pipeline");
                return new DecodeResult { IsSuccess = false, ErrorMessage = $"Pipeline error: {ex.Message}" };
            }
        }

        private async Task<DecodeResult> ExecutePipelineInternalAsync(CancellationToken token)
        {
            await _semaphore.WaitAsync(token);
            try
            {
                if (!_cameraDriver.IsConnected)
                {
                    _logger.Debug("Camera driver offline. Directing initialization sequence...");
                    await _cameraDriver.ConnectAsync(token);
                }

                // 透過泛型隔離底層取像行為
                TFrame rawFrame = await _cameraDriver.CaptureFrameAsync(token);
                
                // 傳遞至泛型演算法解析模組
                return _imageAnalyzer.Analyze(rawFrame);
            }
            catch (Exception ex)
            {
                _logger.Warning("Pipeline execution pass failed: {Message}. Enforcing driver reset...", ex.Message);
                _cameraDriver.Disconnect();
                throw; 
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _logger.Information("Shutting down generic vision service pipeline...");
            _cameraDriver.Dispose();
            _semaphore?.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
