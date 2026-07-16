#if NET48 || NET8_0_OR_GREATER
using Avl;
using AvlNet; // 修正點：統一使用官方標準 AvlNet 命名空間
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
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _bufferPool = bufferPool ?? throw new ArgumentNullException(nameof(bufferPool));

            if (string.IsNullOrWhiteSpace(_config.VisionIpAddress))
            {
                throw new InvalidOperationException("VisionIpAddress configuration is missing or empty. Cannot initialize AVL camera driver.");
            }

            _imageWidth = imageWidth;
            _imageHeight = imageHeight;
            _imageStride = imageWidth;

            _logger = logger ?? Log.ForContext<AvlCameraDriver>();
        }

        public async Task ConnectAsync(CancellationToken token)
        {
            if (IsConnected)
                return;

            _logger.Information("Opening GigE Vision Device via AVL Core API at Address: {IP}", _config.VisionIpAddress);

            await Task.Run(() => {
                // 修正點：改用標準 AVL 靜態類別進行硬體操作
                _hDevice = AVL.GigEVision_OpenDevice(_config.VisionIpAddress);

                if (_hDevice == IntPtr.Zero)
                {
                    throw new InvalidOperationException(
                        $"Failed to open AVL device at {_config.VisionIpAddress}. " +
                        "Please verify: (1) Camera power and network connection, (2) IP address configuration, (3) GigE Vision controller status.");
                }

                try
                {
                    SetSoftwareTriggerMode(_hDevice);

                    // 修正點：改用標準 AVL 靜態類別
                    bool acquisitionStarted = AVL.GigEVision_StartAcquisition(_hDevice, "Mono8");

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
                    // 修正點：改用標準 AVL 靜態類別
                    AVL.GigEVision_CloseDevice(_hDevice);
                    _hDevice = IntPtr.Zero;
                    throw;
                }
            }, token);
        }

        public async Task<ManagedFrame> CaptureFrameAsync(CancellationToken token)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException(
                    "Hardware channel offline. Device handle is invalid or acquisition not started.");
            }

            var tcs = new TaskCompletionSource<ManagedFrame>();
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            cts.Token.Register(() => tcs.TrySetCanceled(cts.Token));

            await Task.Factory.StartNew(() => {
                try
                {
                    token.ThrowIfCancellationRequested();

                    SendSoftwareTrigger();

                    // 修正點：型別對齊 Avl.Image
                    Avl.Image nativeFrame = new Avl.Image();

                    // 修正點：使用 AVL 靜態類別，out 傳參對齊官方簽名
                    bool success = AVL.GigEVision_ReceiveImage(_hDevice, out nativeFrame);

                    if (!success || nativeFrame.Data == IntPtr.Zero)
                    {
                        throw new TimeoutException($"AVL hardware frame reception timeout after {_config.VisionTimeoutMs}ms.");
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
            // 修正點：全面改用大寫 AVL 核心靜態操作方法
            System.Nullable<int> lineIn = 2;
            AVL.GigEVision_SetLineSelector(_hDevice, lineIn);
            AVL.GigEVision_SetLineMode(_hDevice, lineIn, "Trigger");
            AVL.GigEVision_PulseLine(_hDevice, lineIn);
        }

        private void SetSoftwareTriggerMode(IntPtr hDevice)
        {
            // 修正點：全面改用大寫 AVL
            AVL.GigEVision_SetTriggerMode(hDevice, "On");
            AVL.GigEVision_SetTriggerSource(hDevice, "Software");
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
                    // 修正點：改用大寫 AVL
                    AVL.GigEVision_StopAcquisition(_hDevice);
                    _isStreaming = false;
                }

                // 修正點：改用大寫 AVL
                AVL.GigEVision_CloseDevice(_hDevice);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Exception during AVL hardware shutdown sequence");
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
    /// AVL 條碼解碼專屬實作 - 工業級零拷貝版本
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
                throw new ArgumentNullException(nameof(frame), "ManagedFrame payload cannot be null.");
            }

            frame.Validate();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            GCHandle handle = GCHandle.Alloc(frame.Payload, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();

                using (var avlImage = CreateAvlImageFromPtr(ptr, frame.Width, frame.Height, frame.Stride, frame.Format))
                {
                    System.Collections.Generic.List<string> barCodes = new System.Collections.Generic.List<string>();
                    BarcodeParams barcodeParams = new BarcodeParams();

                    // 修正點：呼叫大寫 AVL 的 RecognizeBarcode 或者是標準 ReadBarcodes API
                    // 根據底層中 RecognizeBarcode 的多載定義進行調用
                    AvlNet.RecognizeBarcode(avlImage, new Rectangle2D(0, 0, frame.Width, frame.Height), barcodeParams, 10, 5, 5.0f, out barCodes);

                    stopwatch.Stop();

                    if (barCodes == null || barCodes.Count == 0)
                    {
                        _logger.Debug("No barcode detected in the captured frame.");
                        return new DecodeResult { IsSuccess = false, ErrorMessage = "No barcode pattern detected in frame." };
                    }

                    _logger.Information("Successfully decoded barcode: {Barcode} (Elapsed: {ElapsedMs}ms)", barCodes[0], stopwatch.ElapsedMilliseconds);
                    return new DecodeResult { IsSuccess = true, DecodedText = barCodes[0] };
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Critical exception in AVL Barcode processing unit.");
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