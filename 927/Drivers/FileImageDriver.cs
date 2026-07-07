using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ShoeMoldControl.Core;
using ShoeMoldControl.Core.Vision;
using Serilog;

namespace ShoeMoldControl.Drivers
{
    /// <summary>
    /// P0 模擬驅動 - 從本機目錄循環讀取測試鞋模圖片並封裝成 ManagedFrame
    /// 用於離線開發與測試階段，無需連接實體相機
    /// 
    /// 核心特色：
    /// 1. 完全實作 ICameraDriver<ManagedFrame> 介面
    /// 2. 使用 FrameMemoryPool 實現記憶體池化
    /// 3. P0→P1 切換時，上層業務邏輯完全無需更動
    /// </summary>
    public class FileImageDriver : ICameraDriver<ManagedFrame>
    {
        private readonly string[] _imagePaths;
        private readonly FrameMemoryPool _bufferPool;
        private readonly ILogger _logger;
        private int _currentIndex;
        private bool _isConnected;
        private bool _disposed;
        private readonly object _lock = new object();

        public bool IsConnected => _isConnected;

        /// <summary>
        /// 初始化一個新的 FileImageDriver 實例
        /// </summary>
        /// <param name="imageDirectory">包含測試圖片的目錄路徑</param>
        /// <param name="bufferPool">幀緩衝區池</param>
        /// <param name="logger">日誌記錄器</param>
        /// <param name="searchPattern">圖片搜尋模式（預設：*.jpg, *.png, *.bmp）</param>
        public FileImageDriver(
            string imageDirectory,
            FrameMemoryPool bufferPool,
            ILogger logger = null,
            string searchPattern = "*.*")
        {
            if (!Directory.Exists(imageDirectory))
            {
                throw new DirectoryNotFoundException($"Image directory not found: {imageDirectory}");
            }

            _bufferPool = bufferPool ?? throw new ArgumentNullException(nameof(bufferPool));
            _logger = logger ?? Log.ForContext<FileImageDriver>();
            _currentIndex = 0;
            _isConnected = false;
            _disposed = false;

            // 載入所有符合條件的圖片路徑
            _imagePaths = Directory.GetFiles(imageDirectory, searchPattern)
                .Where(path => IsSupportedImageFormat(path))
                .ToArray();

            if (_imagePaths.Length == 0)
            {
                throw new InvalidOperationException(
                    $"No supported image files found in {imageDirectory}. " +
                    "Please add .jpg, .jpeg, .png, or .bmp files to the directory.");
            }

            _logger.Information("FileImageDriver initialized with {Count} test images from {Directory}",
                _imagePaths.Length, imageDirectory);
        }

        /// <summary>
        /// 連線至模擬驅動（實際上是驗證圖片路徑）
        /// </summary>
        public Task ConnectAsync(CancellationToken token)
        {
            return Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();
                
                lock (_lock)
                {
                    if (_isConnected) return;
                    
                    _isConnected = true;
                    _logger.Information("FileImageDriver connected (Simulation Mode)");
                }
            }, token);
        }

        /// <summary>
        /// 擷取下一幀影像（從檔案讀取並封裝成 ManagedFrame）
        /// </summary>
        public async Task<ManagedFrame> CaptureFrameAsync(CancellationToken token)
        {
            if (!_isConnected)
            {
                throw new InvalidOperationException("FileImageDriver not connected. Call ConnectAsync() first.");
            }

            return await Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();

                string imagePath;
                lock (_lock)
                {
                    // 循環選擇下一張圖片
                    imagePath = _imagePaths[_currentIndex];
                    _currentIndex = (_currentIndex + 1) % _imagePaths.Length;
                }

                _logger.Debug("Loading test image: {ImagePath}", imagePath);

                // 從池中借用緩衝區
                var managedFrame = _bufferPool.Rent();

                try
                {
                    // 讀取圖片並複製到受控緩衝區
                    using var bitmap = new System.Drawing.Bitmap(imagePath);
                    
                    // 更新元數據
                    managedFrame.ResetMetadata(
                        bitmap.Width,
                        bitmap.Height,
                        bitmap.Width, // Mono8 stride
                        PixelFormat.Mono8);

                    // 將圖片轉換為灰階並複製到 Payload
                    ConvertToMono8(bitmap, managedFrame.Payload);

                    _logger.Debug("Successfully loaded frame from {ImagePath} ({Width}x{Height})",
                        imagePath, bitmap.Width, bitmap.Height);

                    return managedFrame;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to load image from {ImagePath}", imagePath);
                    
                    // 發生錯誤時歸還緩衝區
                    _bufferPool.Return(managedFrame);
                    throw;
                }
            }, token);
        }

        /// <summary>
        /// 斷開連線
        /// </summary>
        public void Disconnect()
        {
            lock (_lock)
            {
                if (!_isConnected) return;
                
                _isConnected = false;
                _logger.Information("FileImageDriver disconnected");
            }
        }

        /// <summary>
        /// 釋放資源
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            
            Disconnect();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 判斷是否為支援的圖片格式
        /// </summary>
        private static bool IsSupportedImageFormat(string path)
        {
            var extension = Path.GetExtension(path).ToLowerInvariant();
            return extension is ".jpg" or ".jpeg" or ".png" or ".bmp";
        }

        /// <summary>
        /// 將 Bitmap 轉換為 Mono8 格式並複製到目標緩衝區
        /// </summary>
        private static void ConvertToMono8(System.Drawing.Bitmap bitmap, byte[] payload)
        {
            // 簡單的灰階轉換公式：Y = 0.299R + 0.587G + 0.114B
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    byte gray = (byte)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                    payload[y * bitmap.Width + x] = gray;
                }
            }
        }
    }
}
