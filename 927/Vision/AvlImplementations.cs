#if NET48 || NET8_0_OR_GREATER
using Avl;
using Avs;
using Serilog;
using ShoeMoldControl.Core;
using ShoeMoldControl.Core.Vision;
using ShoeMoldControl.Infrastructure.Vision;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ShoeMoldControl.Vision
{
    /// <summary>
    /// AVL GenICam 硬體驅動專屬實作 - 工業級無縫解耦版本
    /// 使用原始 Avl SDK 內置之命名空間方法，謝絕外部捏造之 P/Invoke 宣告
    /// </summary>
    public class AvlCameraDriver : ICameraDriver<ManagedFrame>, IDisposable
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
            //TODO 測試20260715 AvlNet.AVL aVL = new AvlNet.AVL();
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _bufferPool = bufferPool ?? throw new ArgumentNullException(nameof(bufferPool));

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
            if (IsConnected)
                return;

            _logger.Information("Opening GigE Vision Device via AVL Core API at Address: {IP}", _config.VisionIpAddress);

            await Task.Run(() => {
                _hDevice = Avl.GigEVision_OpenDevice(_config.VisionIpAddress);

                if (_hDevice == IntPtr.Zero)
                {
                    throw new InvalidOperationException(
                        $"Failed to open AVL device at {_config.VisionIpAddress}. " +
                        "Please verify: (1) Camera power and network connection, (2) IP address configuration, (3) GigE Vision controller status.");
                }

                try
                {
                    SetSoftwareTriggerMode(_hDevice);

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
                    Avl.GigEVision_CloseDevice(_hDevice);
                    _hDevice = IntPtr.Zero;
                    throw;
                }
            }, token);
        }

        /// <summary>
        /// 核心取像方法 - 實現秒級複製與立即釋放硬體幀
        /// </summary>
        public async Task<ManagedFrame> CaptureFrameAsync(CancellationToken token)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException(
                    "Hardware channel offline. Device handle is invalid or acquisition not started. " +
                    "Ensure ConnectAsync() was called successfully before capturing frames.");
            }

            var tcs = new TaskCompletionSource<ManagedFrame>();
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            cts.Token.Register(() => tcs.TrySetCanceled(cts.Token));

            // 使用 LongRunning 選項防範非託管硬體底層發生不可預期之阻塞，避免耗盡系統 ThreadPool
            await Task.Factory.StartNew(() => {
                try
                {
                    token.ThrowIfCancellationRequested();

                    SendSoftwareTrigger();

                    Avl.Image nativeFrame = new Avl.Image();
                    bool success = Avl.GigEVision_ReceiveImage(_hDevice, out nativeFrame);

                    if (!success || nativeFrame.Data == IntPtr.Zero)
                    {
                        throw new TimeoutException(
                            $"AVL hardware frame reception timeout after {_config.VisionTimeoutMs}ms. " +
                            "Check network bandwidth, camera trigger settings, and physical connections.");
                    }

                    var managedFrame = _bufferPool.Rent();

                    unsafe
                    {
                        fixed (byte* destPtr = managedFrame.Payload)
                        {
                            int bytesToCopy = Math.Min(managedFrame.BufferSize, nativeFrame.Width * nativeFrame.Height);
                            Buffer.MemoryCopy(
                                nativeFrame.Data.ToPointer(),
                                destPtr,
                                managedFrame.BufferSize,
                                bytesToCopy);
                        }
                    }

                    ReleaseNativeFrame(ref nativeFrame);

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
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            return await tcs.Task;
        }

        private void SendSoftwareTrigger()
        {
            Avl.GigEVision_SetLineSelector(_hDevice, 2);
            Avl.GigEVision_SetLineMode(_hDevice, 2, "Trigger");
            Avl.GigEVision_PulseLine(_hDevice, 2);
        }

        private void SetSoftwareTriggerMode(IntPtr hDevice)
        {
            Avl.GigEVision_SetTriggerMode(hDevice, "On");
            Avl.GigEVision_SetTriggerSource(hDevice, "Software");
        }

        private void ReleaseNativeFrame(ref Avl.Image frame)
        {
            try
            {
                if (frame is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Minor issue during native frame release");
            }
        }

        public void Disconnect()
        {
            if (_hDevice == IntPtr.Zero)
                return;

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
            if (_disposed)
                return;
            Disconnect();
            _bufferPool?.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// AVL 演算法解碼專屬實作 - 工業級零拷貝版本
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

            frame.Validate();

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // 使用 GCHandle 固定受管記憶體，防止 GC 在演算法運行期間移動記憶體位址
            GCHandle handle = GCHandle.Alloc(frame.Payload, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();

                // 調用正確的非託管建構函式進行影像封裝，達成零拷貝資料直通演算法核心的目的
                using (var avlImage = CreateAvlImageFromPtr(
                    ptr,
                    frame.Width,
                    frame.Height,
                    frame.Stride,
                    frame.Format))
                {
                    string[] barCodes;
                    BarcodeParams barcodeParams = new BarcodeParams();

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
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Critical exception in AVL ReadBarcodes processing unit - C++ Core Engine error.");
                throw new InvalidOperationException($"AVL barcode decoding failed: {ex.Message}", ex);
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }

        /// <summary>
        /// 依據修正規格：透過對應之多載建構函式封裝非託管影像指標
        /// 引數順序對齊核心定義：width, height, pitch (stride), type, depth, data
        /// </summary>
        private Avl.Image CreateAvlImageFromPtr(IntPtr ptr, int width, int height, int stride, Core.Vision.PixelFormat format)
        {
            (Avl.PlainType plainType, int depth) = format switch
            {
                Core.Vision.PixelFormat.Mono8 => (Avl.PlainType.UInt8, 1),
                Core.Vision.PixelFormat.Bgr24 => (Avl.PlainType.UInt8, 3),
                Core.Vision.PixelFormat.BayerRG8 => (Avl.PlainType.UInt8, 1),
                _ => (Avl.PlainType.UInt8, 1)
            };

            return new Avl.Image(width, height, stride, plainType, depth, ptr);
        }
    }
}
#else
namespace ShoeMoldControl.Core.Hardware { public class AvrHardwareGateway { } }
#endif