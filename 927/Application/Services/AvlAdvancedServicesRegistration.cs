using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ShoeMoldControl.Vision.Avl;

namespace ShoeMoldControl.Application.Services
{
    /// <summary>
    /// AVL 進階視覺服務 DI 註冊擴展方法
    /// 提供工業級影像前處理、模板匹配、幾何量測、輪廓分析、OCR 辨識等服務的依賴注入配置
    /// </summary>
    public static class AvlAdvancedServicesRegistration
    {
        /// <summary>
        /// 註冊所有 AVL 進階視覺服務
        /// 對應說明書中的六大技術模組：
        /// 1. Image Processing (影像前處理與增強)
        /// 2. Pattern Matching (模板匹配與特徵定位)
        /// 3. Computational Geometry (幾何路徑與量測)
        /// 4. 1D Signal & Profile (一維訊號與輪廓分析)
        /// 5. OCR & SVM (AI 字元與條碼辨識)
        /// 6. System I/O & Networking (硬體通訊與記憶體 I/O) - 已於 AvlImplementations.cs 實作
        /// </summary>
        public static IServiceCollection AddAvlAdvancedVisionServices(this IServiceCollection services)
        {
            // 1. 影像前處理與增強服務
            // 應用情境：消除產線光源不均、去噪、對目標區域 (ROI) 進行自適應二值化
            services.AddSingleton<IImagePreprocessor, AvlImagePreprocessor>();
            
            // 2. 模板匹配與特徵定位服務
            // 應用情境：在鞋模輸送帶或 PCB 板上尋找特定物件的位置、角度與基準座標
            services.AddSingleton<IPatternMatcher, AvlPatternMatcher>();
            
            // 3. 幾何路徑與量測服務
            // 應用情境：測量鞋模邊緣距離、計算手臂軌跡修補、路徑貼合
            services.AddSingleton<IGeometryMeasurer, AvlGeometryMeasurer>();
            
            // 4. 一維訊號與輪廓分析服務
            // 應用情境：線雷射掃描鞋模表面高度、段差量測、膠路連續波峰檢查
            services.AddSingleton<IProfileAnalyzer, AvlProfileAnalyzer>();
            
            // 5. AI 字元與條碼辨識服務
            // 應用情境：讀取鞋模具上的雷雕序號、DataMatrix 條碼（如生產週期中的鞋模具追溯）
            services.AddSingleton<IOcrRecognizer, AvlOcrRecognizer>();

            return services;
        }

        /// <summary>
        /// 註冊指定的 AVL 視覺服務 (選擇性註冊)
        /// </summary>
        /// <param name="services">服務集合</param>
        /// <param name="options">服務註冊選項</param>
        public static IServiceCollection AddAvlAdvancedVisionServices(
            this IServiceCollection services, 
            AvlVisionServiceOptions options)
        {
            if (options.UseImagePreprocessing)
                services.AddSingleton<IImagePreprocessor, AvlImagePreprocessor>();
            
            if (options.UsePatternMatching)
                services.AddSingleton<IPatternMatcher, AvlPatternMatcher>();
            
            if (options.UseGeometryMeasurement)
                services.AddSingleton<IGeometryMeasurer, AvlGeometryMeasurer>();
            
            if (options.UseProfileAnalysis)
                services.AddSingleton<IProfileAnalyzer, AvlProfileAnalyzer>();
            
            if (options.UseOcrRecognition)
                services.AddSingleton<IOcrRecognizer, AvlOcrRecognizer>();

            return services;
        }
    }

    /// <summary>
    /// AVL 視覺服務註冊選項
    /// 允許選擇性啟用特定技術模組，降低記憶體佔用
    /// </summary>
    public class AvlVisionServiceOptions
    {
        /// <summary>
        /// 是否啟用影像前處理服務 (預設：true)
        /// </summary>
        public bool UseImagePreprocessing { get; set; } = true;

        /// <summary>
        /// 是否啟用模板匹配服務 (預設：true)
        /// </summary>
        public bool UsePatternMatching { get; set; } = true;

        /// <summary>
        /// 是否啟用幾何量測服務 (預設：true)
        /// </summary>
        public bool UseGeometryMeasurement { get; set; } = true;

        /// <summary>
        /// 是否啟用輪廓分析服務 (預設：true)
        /// </summary>
        public bool UseProfileAnalysis { get; set; } = true;

        /// <summary>
        /// 是否啟用 OCR 辨識服務 (預設：false)
        /// </summary>
        public bool UseOcrRecognition { get; set; } = false;
    }
}
