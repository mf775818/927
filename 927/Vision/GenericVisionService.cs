using System;
using System.Threading;
using System.Threading.Tasks;
using ShoeMoldControl.Core;
using ShoeMoldControl.Core.Vision;
using ShoeMoldControl.Infrastructure.Polly;
using ShoeMoldControl.Infrastructure.Vision;
using Polly.CircuitBreaker;
using Serilog;

namespace ShoeMoldControl.Vision
{
    /// <summary>
    /// 泛型視覺服務協調器 - 整合相機驅動與圖像分析器
    /// 透過泛型隔離底層硬體差異，提供統一的 IVisionService 介面
    /// 
    /// 工業級改進：
    /// 1. 支援 ManagedFrame 類型的記憶體池管理
    /// 2. 分析完成後自動歸還幀至緩衝區池
    /// 3. 保持與 Polly 強健性策略的完全相容
    /// </summary>
    public class GenericVisionService<TFrame> : IVisionService
    {
        private readonly ICameraDriver<TFrame> _cameraDriver;
        private readonly IImageAnalyzer<TFrame> _imageAnalyzer;
        private readonly IResiliencePolicyProvider _policyProvider;
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly FrameMemoryPool? _bufferPool; // 若 TFrame 為 ManagedFrame 則使用
        private bool _disposed;

        public GenericVisionService(
            ICameraDriver<TFrame> cameraDriver,
            IImageAnalyzer<TFrame> imageAnalyzer,
            IResiliencePolicyProvider policyProvider,
            ILogger logger = null,
            FrameMemoryPool bufferPool = null)
        {
            _cameraDriver = cameraDriver ?? throw new ArgumentNullException(nameof(cameraDriver));
            _imageAnalyzer = imageAnalyzer ?? throw new ArgumentNullException(nameof(imageAnalyzer));
            _policyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider));
            _bufferPool = bufferPool;
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
            TFrame rawFrame = default(TFrame);
            
            try
            {
                if (!_cameraDriver.IsConnected)
                {
                    _logger.Debug("Camera driver offline. Directing initialization sequence...");
                    await _cameraDriver.ConnectAsync(token);
                }

                // 透過泛型隔離底層取像行為
                rawFrame = await _cameraDriver.CaptureFrameAsync(token);
                
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
                // 【關鍵】若 TFrame 為 ManagedFrame，分析完成後歸還至緩衝區池
                if (_bufferPool != null && rawFrame is ManagedFrame managedFrame)
                {
                    try
                    {
                        _bufferPool.Return(managedFrame);
                        _logger.Debug("ManagedFrame returned to buffer pool successfully");
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning(ex, "Failed to return ManagedFrame to pool - potential memory leak");
                    }
                }

                _semaphore.Release();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _logger.Information("Shutting down generic vision service pipeline...");
            _cameraDriver.Dispose();
            _bufferPool?.Dispose();
            _semaphore?.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
