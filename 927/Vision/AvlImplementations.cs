using System;
using System.Threading;
using System.Threading.Tasks;
using ShoeMoldControl.Core;
using Serilog;

namespace ShoeMoldControl.Vision
{
    /// <summary>
    /// AVL GenICam 硬體驅動專屬實作
    /// 將原廠 SDK 控制範圍限制在特定類別內部，後續若抽換相機品牌，僅需新增對應實作
    /// </summary>
    public class AvlCameraDriver : ICameraDriver<AvlNet.Image>
    {
        private readonly IVisionConfig _config;
        private readonly ILogger _logger;
        private IntPtr _hDevice = IntPtr.Zero;
        private bool _isStreaming;
        private bool _disposed;

        public bool IsConnected => _hDevice != IntPtr.Zero && _isStreaming;

        public AvlCameraDriver(IVisionConfig config, ILogger logger = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            
            // 工業級參數驗證：IP 地址無效時立即拋出異常
            if (string.IsNullOrWhiteSpace(_config.VisionIpAddress))
            {
                throw new InvalidOperationException("VisionIpAddress configuration is missing or empty. Cannot initialize AVL camera driver.");
            }

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
                    // 帶入預設像素格式參數以利流控制開關
                    bool acquisitionStarted = Avl.GigEVision_StartAcquisition(_hDevice, "Mono8");
                    
                    if (!acquisitionStarted)
                    {
                        throw new InvalidOperationException(
                            $"Failed to start acquisition on device {_config.VisionIpAddress}. " +
                            "Possible causes: incompatible pixel format, bandwidth limitation, or camera already in use.");
                    }
                    
                    _isStreaming = true;
                    _logger.Information("AVL GigE Vision device successfully connected and streaming at {IP}", _config.VisionIpAddress);
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

        public async Task<AvlNet.Image> CaptureFrameAsync(CancellationToken token)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException(
                    $"Hardware channel offline. Device handle is invalid or acquisition not started. " +
                    "Ensure ConnectAsync() was called successfully before capturing frames.");
            }

            AvlNet.Image outImage = new AvlNet.Image();
            
            bool success = await Task.Run(() => 
            {
                return Avl.GigEVision_ReceiveImage(_hDevice, out outImage);
            }, token);

            if (!success)
            {
                _logger.Warning("AVL hardware frame reception timeout. Device may be disconnected or network congested.");
                throw new TimeoutException(
                    $"AVL hardware frame reception timeout after {_config.VisionTimeoutMs}ms. " +
                    "Check network bandwidth, camera trigger settings, and physical connections.");
            }

            return outImage;
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
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// AVL 演算法解碼專屬實作
    /// 封裝 Aurora Vision Library 的條碼讀取功能，提供統一的解碼結果
    /// </summary>
    public class AvlImageAnalyzer : IImageAnalyzer<AvlNet.Image>
    {
        private readonly ILogger _logger;

        public AvlImageAnalyzer(ILogger logger = null)
        {
            _logger = logger ?? Log.ForContext<AvlImageAnalyzer>();
        }

        public DecodeResult Analyze(AvlNet.Image frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame), "AVL image frame cannot be null. This indicates a critical pipeline failure.");
            }

            try
            {
                string[] barCodes;
                BarcodeParams barcodeParams = new BarcodeParams(); // 套用工業預設濾鏡

                bool decodeSuccess = Avl.ReadBarcodes(frame, null, null, barcodeParams, out barCodes);

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

                _logger.Information("Successfully decoded barcode: {Barcode}", barCodes[0]);
                return new DecodeResult { IsSuccess = true, DecodedText = barCodes[0] };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Critical exception in AVL ReadBarcodes processing unit - C++ Core Engine error.");
                throw new InvalidOperationException($"AVL barcode decoding failed: {ex.Message}", ex);
            }
        }
    }
}
