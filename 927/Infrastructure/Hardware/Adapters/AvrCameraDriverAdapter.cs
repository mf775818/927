#if NET48
using System;
using System.Threading;
using System.Threading.Tasks;
using ShoeMoldControl.Core.Hardware;

namespace ShoeMoldControl.Infrastructure.Hardware.Adapters
{
    /// <summary>
    /// 相機驅動配接器 - 將廠商 Grab 巨集轉換為標準化非同步介面
    /// </summary>
    public class AvrCameraDriverAdapter : IAvrCameraDriver, IDisposable
    {
        private readonly AvrHardwareGateway _gateway;
        private bool _disposed;

        public AvrCameraDriverAdapter(AvrHardwareGateway gateway)
        {
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        }

        /// <summary>
        /// 連接相機 (由閘道器統一管理連線，此處僅驗證)
        /// </summary>
        public Task ConnectAsync(CancellationToken token)
        {
            if (!_gateway.IsInitialized)
                throw new InvalidOperationException("硬體閘道器尚未初始化，無法連接相機。");
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// 捕獲原生影像幀
        /// </summary>
        public async Task<Avl.Image> CaptureNativeFrameAsync(CancellationToken token)
        {
            return await _gateway.ExecuteSafeFuncAsync(macros =>
            {
                Avl.Image grabImage = new Avl.Image();
                macros.Grab(grabImage);
                return grabImage;
            }, token);
        }

        /// <summary>
        /// 斷開連接 (由閘道器統一管理資源釋放)
        /// </summary>
        public void Disconnect()
        {
            // 由外層精確控制的 Gateway 統一關閉資源，此處不越權處理
        }

        /// <summary>
        /// 是否已連接
        /// </summary>
        public bool IsConnected => _gateway.IsInitialized;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            // 資源由 Gateway 统一管理，此處不需額外釋放
        }
    }
}

#endif