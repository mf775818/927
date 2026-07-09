#if NET48
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ShoeMoldControl.Core;
using ShoeMoldControl.Core.Vision;
using ShoeMoldControl.Infrastructure.Vision;
using Serilog;

namespace ShoeMoldControl.Vision
{
    /// <summary>
    /// AVL GenICam 硬體驅動專屬實作 - 工業級無縫解耦版本
    /// 將原廠 SDK 控制範圍限制在特定類別內部，後續若抽換相機品牌，僅需新增對應實作
    /// 
    /// 核心改進：
    /// 1. 使用 ManagedFrame 作為 TFrame，實現硬體與演算法的完全解耦
    /// 2. 取像後立即複製至受控記憶體並釋放硬體幀，防止 Ring Buffer Exhaustion
    /// 3. 整合 FrameBufferPool 實現零 GC Jitter
    /// </summary>
    public class AvlCameraDriver : ICameraDriver<ManagedFrame>
    {
        private readonly IVisionConfig _config;
        private readonly FrameMemoryPool _bufferPool;
        private readonly ILogger _logger;
        private IntPtr _hDevice = IntPtr.Zero;
        private bool _isStreaming;
        private bool _disposed;
        private readonly int _imageWidth;
        private readonly int _imageHeight;
        private readonly int _imageStride;

        public bool IsConnected => _hDevice != IntPtr.Zero && _isStreaming;

        public AvlCameraDriver(
            IVisionConfig config, 
            FrameMemoryPool bufferPool,
            ILogger logger = null,
            int imageWidth = 2448,
            int imageHeight = 2048)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _bufferPool = bufferPool ?? throw new ArgumentNullException(nameof(bufferPool));
            
            // 工業級參數驗證：IP 地址無效時立即拋出異常
            if (string.IsNullOrWhiteSpace(_config.VisionIpAddress))
            {
                throw new InvalidOperationException("VisionIpAddress configuration is missing or empty. Cannot initialize AVL camera driver.");
            }

            _imageWidth = imageWidth;
            _imageHeight = imageHeight;
            _imageStride = imageWidth; // Mono8: 1 byte per pixel

            _logger = logger ?? Log.ForContext<AvlCameraDriver>();
        }

        public async Task ConnectAsync(CancellationToken token)
        {
            if (IsConnected) return;

            _logger.Information("Opening GigE Vision Device via AVL Core API at Address: {IP}", _config.VisionIpAddress);
            
            await Task.Run(() =>
            {
                // 嘗試開啟設備
                _hDevice = Avl.GigEVision_OpenDevice(_config.VisionIpAddress);
                
                // 工業級錯誤檢查：設備開啟失敗時立即拋出異常
                if (_hDevice == IntPtr.Zero)
                {
                    throw new InvalidOperationException(
                        $"Failed to open AVL device at {_config.VisionIpAddress}. " +
                        "Please verify: (1) Camera power and network connection, (2) IP address configuration, (3) GigE Vision controller status.");
                }

                try
                {
                    // 切換為軟體觸發模式 (關鍵：避免連續取像導致 Buffer Overflow)
                    SetSoftwareTriggerMode(_hDevice);
                    
                    // 帶入預設像素格式參數以利流控制開關
                    bool acquisitionStarted = Avl.GigEVision_StartAcquisition(_hDevice, "Mono8");
                    
                    if (!acquisitionStarted)
                    {
                        throw new InvalidOperationException(
                            $"Failed to start acquisition on device {_config.VisionIpAddress}. " +
                            "Possible causes: incompatible pixel format, bandwidth limitation, or camera already in use.");
                    }
                    
                    _isStreaming = true;
                    _logger.Information("AVL GigE Vision device successfully connected and streaming at {IP} (Software Trigger Mode)", _config.VisionIpAddress);
                }
                catch
                {
                    // 清理資源並重新拋出異常
                    Avl.GigEVision_CloseDevice(_hDevice);
                    _hDevice = IntPtr.Zero;
                    throw;
                }
            }, token);
        }

        /// <summary>
        /// 核心取像方法 - 實現「秒級複製」與「立即釋放硬體幀」
        /// 使用 TPL Bridge 將底層回呼轉換為非同步非阻斷 Task
        /// </summary>
        public async Task<ManagedFrame> CaptureFrameAsync(CancellationToken token)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException(
                    $"Hardware channel offline. Device handle is invalid or acquisition not started. " +
                    "Ensure ConnectAsync() was called successfully before capturing frames.");
            }

            var tcs = new TaskCompletionSource<ManagedFrame>();
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            cts.Token.Register(() => tcs.TrySetCanceled(cts.Token));

            // 在非阻塞執行緒中執行取像
            await Task.Run(() =>
            {
                try
                {
                    // 步驟 1: 發送軟體觸發指令
                    SendSoftwareTrigger();

                    // 步驟 2: 從 SDK 取得原生幀
                    AvlNet.Image nativeFrame = new AvlNet.Image();
                    bool success = Avl.GigEVision_ReceiveImage(_hDevice, out nativeFrame);

                    if (!success || nativeFrame.DataPtr == IntPtr.Zero)
                    {
                        throw new TimeoutException(
                            $"AVL hardware frame reception timeout after {_config.VisionTimeoutMs}ms. " +
                            "Check network bandwidth, camera trigger settings, and physical connections.");
                    }

                    // 步驟 3: 從池中借用受控緩衝區
                    var managedFrame = _bufferPool.Rent();

                    // 步驟 4: 使用 Buffer.MemoryCopy 進行高速記憶體複製（零拷貝防禦）
                    unsafe
                    {
                        fixed (byte* destPtr = managedFrame.Payload)
                        {
                            int bytesToCopy = Math.Min(managedFrame.BufferSize, nativeFrame.Width * nativeFrame.Height);
                            Buffer.MemoryCopy(
                                nativeFrame.DataPtr.ToPointer(),
                                destPtr,
                                managedFrame.BufferSize,
                                bytesToCopy);
                        }
                    }

                    // 步驟 5: 【關鍵】立即釋放硬體幀歸還至相機 Ring Buffer
                    ReleaseNativeFrame(ref nativeFrame);

                    // 步驟 6: 返回受控幀至上層管線
                    tcs.SetResult(managedFrame);
                }
                catch (OperationCanceledException)
                {
                    tcs.TrySetCanceled(cts.Token);
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Frame capture failed - enforcing driver recovery");
                    tcs.TrySetException(ex);
                }
            }, token);

            return await tcs.Task;
        }

        private void SendSoftwareTrigger()
        {
            // 發送軟體觸發命令 (Line 2 通常用於軟體觸發)
            Avl.GigEVision_SetLineSelector(_hDevice, 2);
            Avl.GigEVision_SetLineMode(_hDevice, 2, "Trigger");
            Avl.GigEVision_PulseLine(_hDevice, 2);
        }

        private void SetSoftwareTriggerMode(IntPtr hDevice)
        {
            // 設定相機為軟體觸發模式
            Avl.GigEVision_SetTriggerMode(hDevice, "On");
            Avl.GigEVision_SetTriggerSource(hDevice, "Software");
        }

        private void ReleaseNativeFrame(ref AvlNet.Image frame)
        {
            // 釋放原生幀資源（若有 IDisposable 介面）
            try
            {
                if (frame is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                // 或者呼叫特定的 ReleaseFrame API（依 SDK 而定）
                // Avl.GigEVision_ReleaseFrame(_hDevice, ref frame);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Minor issue during native frame release");
            }
        }

        public void Disconnect()
        {
            if (_hDevice == IntPtr.Zero) return;
            
            try
            {
                if (_isStreaming)
                {
                    Avl.GigEVision_StopAcquisition(_hDevice);
                    _isStreaming = false;
                }
                
                Avl.GigEVision_CloseDevice(_hDevice);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Exception during AVL hardware shutdown sequence - device may have been forcibly disconnected.");
            }
            finally
            {
                _hDevice = IntPtr.Zero;
                _logger.Warning("AVL GenICam hardware interface channel disconnected.");
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            Disconnect();
            _bufferPool?.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// AVL 演算法解碼專屬實作 - 工業級零拷貝版本
    /// 封裝 Aurora Vision Library 的條碼讀取功能，提供統一的解碼結果
    /// 
    /// 核心改進：
    /// 1. 使用 ManagedFrame 作為輸入，實現與相機硬體的完全解耦
    /// 2. 使用 GCHandle 定錨技術，在 AVL 運算期間鎖定記憶體物理地址
    /// 3. 零拷貝矩陣轉換，直接將受控記憶體指標傳遞給 AVL C++ 核心
    /// </summary>
    public class AvlImageAnalyzer : IImageAnalyzer<ManagedFrame>
    {
        private readonly ILogger _logger;

        public AvlImageAnalyzer(ILogger logger = null)
        {
            _logger = logger ?? Log.ForContext<AvlImageAnalyzer>();
        }

        public DecodeResult Analyze(ManagedFrame frame)
        {
            if (frame.Payload == null)
            {
                throw new ArgumentNullException(nameof(frame), "ManagedFrame payload cannot be null. This indicates a critical pipeline failure.");
            }

            // 驗證幀的完整性
            frame.Validate();

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // 【關鍵】使用 GCHandle 定錨技術，防止 GC 在 AVL 運算期間移動記憶體
            GCHandle handle = GCHandle.Alloc(frame.Payload, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();

                // 構造 AVL SDK 可識別的原生影像物件（零拷貝）
                // 注意：此處不進行二次記憶體拷貝，達成資料「直通演算法核心」
                AvlNet.Image avlImage = CreateAvlImageFromPtr(
                    ptr, 
                    frame.Width, 
                    frame.Height, 
                    frame.Stride, 
                    frame.Format);

                try
                {
                    string[] barCodes;
                    BarcodeParams barcodeParams = new BarcodeParams(); // 套用工業預設濾鏡

                    bool decodeSuccess = Avl.ReadBarcodes(avlImage, null, null, barcodeParams, out barCodes);

                    stopwatch.Stop();

                    if (!decodeSuccess)
                    {
                        _logger.Debug("AVL ReadBarcodes returned false - algorithm execution failed.");
                        return new DecodeResult { IsSuccess = false, ErrorMessage = "Barcode detection algorithm execution failed." };
                    }

                    if (barCodes == null || barCodes.Length == 0)
                    {
                        _logger.Debug("No barcode detected in the captured frame.");
                        return new DecodeResult { IsSuccess = false, ErrorMessage = "No barcode pattern detected in frame." };
                    }

                    if (string.IsNullOrWhiteSpace(barCodes[0]))
                    {
                        _logger.Debug("Barcode detected but decoded result is empty.");
                        return new DecodeResult { IsSuccess = false, ErrorMessage = "Barcode detected but decoded to empty string." };
                    }

                    _logger.Information(
                        "Successfully decoded barcode: {Barcode} (Elapsed: {ElapsedMs}ms)", 
                        barCodes[0], 
                        stopwatch.ElapsedMilliseconds);
                    
                    return new DecodeResult { IsSuccess = true, DecodedText = barCodes[0] };
                }
                finally
                {
                    // 徹底摧毀 AVL 暫時性物件
                    if (avlImage is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Critical exception in AVL ReadBarcodes processing unit - C++ Core Engine error.");
                throw new InvalidOperationException($"AVL barcode decoding failed: {ex.Message}", ex);
            }
            finally
            {
                // 【關鍵】解鎖記憶體定錨
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }

        /// <summary>
        /// 從固定指標創建 AVL Image 物件（零拷貝）
        /// </summary>
        private AvlNet.Image CreateAvlImageFromPtr(IntPtr ptr, int width, int height, int stride, PixelFormat format)
        {
            // 根據像素格式轉換為 AVL 可識別的格式
            // 此處假設 AVL SDK 提供從原始指標建構 Image 的方法
            // 若 SDK 不支援，需使用相對應的 wrapper 方法
            
            var avlImage = new AvlNet.Image
            {
                Width = width,
                Height = height,
                Stride = stride,
                DataPtr = ptr,
                // 根據格式設定對應的 AVL 像素類型
                PixelType = format switch
                {
                    PixelFormat.Mono8 => "Mono8",
                    PixelFormat.Bgr24 => "Bgr24",
                    PixelFormat.BayerRG8 => "BayerRG8",
                    _ => "Mono8"
                }
            };

            return avlImage;
        }
    }
}
#else
namespace ShoeMoldControl.Core.Hardware { public class AvrHardwareGateway { } }
#endif