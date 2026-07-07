using System;
using System.Collections.Concurrent;
using ShoeMoldControl.Core.Vision;

namespace ShoeMoldControl.Infrastructure.Vision
{
    /// <summary>
    /// 工業級幀緩衝區物件池 - 消除 GC 織顫 (Jitter) 的關鍵組件
    /// 系統啟動時預先分配固定數量的 byte[] 記憶體，運行期間零配置 (Zero-Allocation)
    /// </summary>
    public class FrameMemoryPool : IDisposable
    {
        private readonly ConcurrentBag<ManagedFrame> _pool = new();
        private readonly int _width;
        private readonly int _height;
        private readonly int _stride;
        private readonly PixelFormat _format;
        private readonly int _bufferSize;
        private readonly int _initialCapacity;
        private bool _disposed;
        private int _totalRented;
        private int _totalReturned;

        /// <summary>
        /// 池中當前可用的幀數量
        /// </summary>
        public int AvailableCount => _pool.Count;

        /// <summary>
        /// 總共借出的次數（用於監控）
        /// </summary>
        public int TotalRented => _totalRented;

        /// <summary>
        /// 總共歸還的次數（用於監控）
        /// </summary>
        public int TotalReturned => _totalReturned;

        /// <summary>
        /// 初始化一個新的 FrameMemoryPool 實例
        /// </summary>
        /// <param name="width">影像寬度</param>
        /// <param name="height">影像高度</param>
        /// <param name="stride">Stride（跨距）</param>
        /// <param name="format">像素格式</param>
        /// <param name="initialCapacity">初始緩衝區數量（建議 3-5 個）</param>
        public FrameMemoryPool(int width, int height, int stride, PixelFormat format, int initialCapacity = 5)
        {
            _width = width;
            _height = height;
            _stride = stride;
            _format = format;
            _bufferSize = Math.Abs(stride) * height;
            _initialCapacity = initialCapacity;
            _disposed = false;
            _totalRented = 0;
            _totalReturned = 0;

            // 預先分配所有緩衝區
            for (int i = 0; i < initialCapacity; i++)
            {
                _pool.Add(new ManagedFrame(width, height, stride, format, new byte[_bufferSize]));
            }
        }

        /// <summary>
        /// 從池中借用一個 ManagedFrame
        /// 若池為空，則動態創建一個新的（應避免此情況）
        /// </summary>
        /// <returns>可用的 ManagedFrame</returns>
        public ManagedFrame Rent()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(FrameMemoryPool), "Cannot rent from a disposed pool.");

            if (_pool.TryTake(out var frame))
            {
                System.Threading.Interlocked.Increment(ref _totalRented);
                return frame;
            }

            // 池為空時的降級處理（應記錄警告）
            System.Threading.Interlocked.Increment(ref _totalRented);
            return new ManagedFrame(_width, _height, _stride, _format, new byte[_bufferSize]);
        }

        /// <summary>
        /// 將 ManagedFrame 歸還至池中
        /// </summary>
        /// <param name="frame">要歸還的幀</param>
        public void Return(ManagedFrame frame)
        {
            if (_disposed)
                return;

            if (frame.IsReturned)
                return; // 防止重複歸還

            // 驗證幀的完整性
            try
            {
                frame.Validate();
            }
            catch
            {
                // 若幀已損壞，不歸還至池中
                return;
            }

            // 重置狀態並歸還
            frame.MarkAsReturned();
            System.Threading.Interlocked.Increment(ref _totalReturned);
            _pool.Add(frame);
        }

        /// <summary>
        /// 清空池中所有緩衝區
        /// </summary>
        public void Clear()
        {
            while (_pool.TryTake(out _)) { }
        }

        /// <summary>
        /// 釋放資源
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            
            Clear();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// 向後相容別名：FrameBufferPool 指向 FrameMemoryPool
    /// </summary>
    [Obsolete("Use FrameMemoryPool instead. This alias is for backward compatibility only.")]
    public class FrameBufferPool : FrameMemoryPool
    {
        public FrameBufferPool(int width, int height, int stride, PixelFormat format, int initialCapacity = 5)
            : base(width, height, stride, format, initialCapacity)
        {
        }
    }
}
