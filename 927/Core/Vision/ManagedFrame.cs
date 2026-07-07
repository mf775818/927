using System;
using System.Runtime.InteropServices;

namespace ShoeMoldControl.Core.Vision
{
    /// <summary>
    /// 工業級受控記憶體描述符 - 作為相機 SDK 與 AVL 演算法之間的虛實抽象紐帶
    /// 此結構攜帶硬體上下文元數據，但內部使用完全受控的記憶體區塊
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ManagedFrame : IDisposable
    {
        /// <summary>
        /// 影像寬度（像素）
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 影像高度（像素）
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Stride（跨距）- 每列位元組數，可為負值表示上下顛倒
        /// </summary>
        public int Stride { get; set; }

        /// <summary>
        /// 標準化工業像素格式
        /// </summary>
        public PixelFormat Format { get; set; }

        /// <summary>
        /// 受控記憶體承載區塊 - 指向實際影像資料的 byte array
        /// </summary>
        public byte[] Payload { get; set; }

        /// <summary>
        /// 緩衝區總大小（位元組）
        /// </summary>
        public int BufferSize => Math.Abs(Stride) * Height;

        /// <summary>
        /// 標記此幀是否已被歸還至池中
        /// </summary>
        public bool IsReturned { get; private set; }

        /// <summary>
        /// 初始化一個新的 ManagedFrame 實例
        /// </summary>
        public ManagedFrame(int width, int height, int stride, PixelFormat format, byte[] payload)
        {
            Width = width;
            Height = height;
            Stride = stride;
            Format = format;
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
            IsReturned = false;
        }

        /// <summary>
        /// 重設元數據（用於從池中取回時重複使用）
        /// </summary>
        public void ResetMetadata(int width, int height, int stride, PixelFormat format)
        {
            Width = width;
            Height = height;
            Stride = stride;
            Format = format;
            IsReturned = false;
        }

        /// <summary>
        /// 標記此幀已歸還至池中，防止重複釋放
        /// </summary>
        public void MarkAsReturned()
        {
            IsReturned = true;
        }

        /// <summary>
        /// 驗證此幀是否可用
        /// </summary>
        public void Validate()
        {
            if (IsReturned)
                throw new ObjectDisposedException(nameof(ManagedFrame), "This frame has been returned to the pool and cannot be reused.");
            
            if (Payload == null || Payload.Length < BufferSize)
                throw new InvalidOperationException($"Invalid frame payload: expected at least {BufferSize} bytes, got {(Payload?.Length ?? 0)} bytes.");
        }

        /// <summary>
        /// 釋放資源（注意：不釋放 Payload，由 Pool 管理）
        /// </summary>
        public void Dispose()
        {
            // Payload 由 FrameBufferPool 管理，此處不釋放
            // 僅標記為已歸還，防止誤用
            if (!IsReturned)
            {
                MarkAsReturned();
            }
        }
    }

    /// <summary>
    /// 標準化工業像素格式列舉
    /// </summary>
    public enum PixelFormat
    {
        /// <summary>
        /// 8-bit 單色灰階
        /// </summary>
        Mono8,

        /// <summary>
        /// 24-bit BGR 彩色
        /// </summary>
        Bgr24,

        /// <summary>
        /// 8-bit Bayer RGGB 馬賽克
        /// </summary>
        BayerRG8,

        /// <summary>
        /// 8-bit Bayer GBGR 馬賽克
        /// </summary>
        BayerGB8,

        /// <summary>
        /// 8-bit Bayer GRBG 馬賽克
        /// </summary>
        BayerGR8,

        /// <summary>
        /// 8-bit Bayer BGGR 馬賽克
        /// </summary>
        BayerBG8,

        /// <summary>
        /// 16-bit 單色灰階（未使用）
        /// </summary>
        Mono16,

        /// <summary>
        /// 未知或未定義格式
        /// </summary>
        Unknown
    }
}
