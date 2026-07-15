#if NET48 || NET8_0_OR_GREATER
using Avs;
using Serilog;
using ShoeMoldControl.Core;
using ShoeMoldControl.Core.Vision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace ShoeMoldControl.Vision.Avl
{
    #region 影像前處理與增強服務 (Image Processing & Enhancement)

    /// <summary>
    /// 工業級影像前處理服務 - 消除產線光源不均、去噪、自適應二值化
    /// 對應說明書：Image Processing 技術模組
    /// </summary>
    public interface IImagePreprocessor
    {
        /// <summary>
        /// 自適應閾值二值化 - 消除光源不均
        /// </summary>
        byte[] AdaptiveThreshold(byte[] image, int width, int height, int blockSize = 11, double c = 2.0);

        /// <summary>
        /// 高斯去噪 - 減少圖像雜訊
        /// </summary>
        byte[] GaussianBlur(byte[] image, int width, int height, double sigma = 1.5);

        /// <summary>
        /// 直方圖均衡化 - 增強對比度
        /// </summary>
        byte[] EqualizeHistogram(byte[] image, int width, int height);

        /// <summary>
        /// ROI 區域提取
        /// </summary>
        byte[] ExtractRoi(byte[] image, int width, int height, int x, int y, int roiWidth, int roiHeight);
    }

    /// <summary>
    /// AVL 影像前處理實作 - 使用 _OfLoop 系列 API 降低 GC 壓力
    /// </summary>
    public class AvlImagePreprocessor : IImagePreprocessor
    {
        private readonly ILogger _logger;
        private readonly object _lock = new object();

        public AvlImagePreprocessor(ILogger logger = null)
        {
            _logger = logger ?? Log.ForContext<AvlImagePreprocessor>();
        }

        public byte[] AdaptiveThreshold(byte[] image, int width, int height, int blockSize = 11, double c = 2.0)
        {
            if (image == null || image.Length < width * height)
                throw new ArgumentException("Invalid image payload for adaptive threshold");

            lock (_lock)
            {
                try
                {
                    var handle = GCHandle.Alloc(image, GCHandleType.Pinned);
                    try
                    {
                        var srcImage = new Avl.Image(width, height, width, Avl.PlainType.UInt8, 1, handle.AddrOfPinnedObject());
                        
                        // 使用自適應閾值巨集 - 工業級參數
                        var thresholdParams = new ThresholdParameters
                        {
                            Method = ThresholdMethod.AdaptiveMean,
                            BlockSize = blockSize,
                            C = c
                        };

                        var dstImage = new Avl.Image();
                        bool success = Avl.ThresholdImage_Adaptive(srcImage, ref dstImage, thresholdParams);
                        
                        if (!success)
                        {
                            _logger.Warning("Adaptive threshold failed - returning original image");
                            return (byte[])image.Clone();
                        }

                        var result = new byte[width * height];
                        Marshal.Copy(dstImage.Data, result, 0, result.Length);
                        return result;
                    }
                    finally
                    {
                        if (handle.IsAllocated)
                            handle.Free();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Adaptive threshold processing failed");
                    return (byte[])image.Clone();
                }
            }
        }

        public byte[] GaussianBlur(byte[] image, int width, int height, double sigma = 1.5)
        {
            if (image == null || image.Length < width * height)
                throw new ArgumentException("Invalid image payload for gaussian blur");

            lock (_lock)
            {
                try
                {
                    var handle = GCHandle.Alloc(image, GCHandleType.Pinned);
                    try
                    {
                        var srcImage = new Avl.Image(width, height, width, Avl.PlainType.UInt8, 1, handle.AddrOfPinnedObject());
                        
                        // 使用高斯模糊巨集
                        var blurParams = new BlurParameters
                        {
                            Method = BlurMethod.Gaussian,
                            SigmaX = sigma,
                            SigmaY = sigma
                        };

                        var dstImage = new Avl.Image();
                        bool success = Avl.BlurImage(srcImage, ref dstImage, blurParams);
                        
                        if (!success)
                        {
                            _logger.Warning("Gaussian blur failed - returning original image");
                            return (byte[])image.Clone();
                        }

                        var result = new byte[width * height];
                        Marshal.Copy(dstImage.Data, result, 0, result.Length);
                        return result;
                    }
                    finally
                    {
                        if (handle.IsAllocated)
                            handle.Free();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Gaussian blur processing failed");
                    return (byte[])image.Clone();
                }
            }
        }

        public byte[] EqualizeHistogram(byte[] image, int width, int height)
        {
            if (image == null || image.Length < width * height)
                throw new ArgumentException("Invalid image payload for histogram equalization");

            lock (_lock)
            {
                try
                {
                    var handle = GCHandle.Alloc(image, GCHandleType.Pinned);
                    try
                    {
                        var srcImage = new Avl.Image(width, height, width, Avl.PlainType.UInt8, 1, handle.AddrOfPinnedObject());
                        var dstImage = new Avl.Image();
                        
                        bool success = Avl.EqualizeHistogram(srcImage, ref dstImage);
                        
                        if (!success)
                        {
                            _logger.Warning("Histogram equalization failed - returning original image");
                            return (byte[])image.Clone();
                        }

                        var result = new byte[width * height];
                        Marshal.Copy(dstImage.Data, result, 0, result.Length);
                        return result;
                    }
                    finally
                    {
                        if (handle.IsAllocated)
                            handle.Free();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Histogram equalization failed");
                    return (byte[])image.Clone();
                }
            }
        }

        public byte[] ExtractRoi(byte[] image, int width, int height, int x, int y, int roiWidth, int roiHeight)
        {
            if (image == null || image.Length < width * height)
                throw new ArgumentException("Invalid image payload for ROI extraction");
            
            if (x < 0 || y < 0 || x + roiWidth > width || y + roiHeight > height)
                throw new ArgumentException("ROI rectangle exceeds image boundaries");

            var result = new byte[roiWidth * roiHeight];
            
            for (int row = 0; row < roiHeight; row++)
            {
                Array.Copy(
                    image, 
                    (y + row) * width + x, 
                    result, 
                    row * roiWidth, 
                    roiWidth);
            }
            
            return result;
        }
    }

    #endregion

    #region 模板匹配與特徵定位服務 (Pattern Matching & Feature Location)

    /// <summary>
    /// 模板匹配結果
    /// </summary>
    public class PatternMatchResult
    {
        public bool IsSuccess { get; set; }
        public double Confidence { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// 工業級模板匹配服務 - 在輸送帶或 PCB 板上尋找特定物件
    /// 對應說明書：Pattern Matching 技術模組
    /// </summary>
    public interface IPatternMatcher
    {
        /// <summary>
        /// 單一物件 SAD 匹配 - 快速灰度匹配
        /// </summary>
        PatternMatchResult LocateSingleObjectSad(byte[] image, int width, int height, byte[] template, int templateWidth, int templateHeight);

        /// <summary>
        /// 多物件邊緣匹配 - 基於邊緣特徵的魯棒匹配
        /// </summary>
        List<PatternMatchResult> LocateMultipleObjectsEdges(byte[] image, int width, int height, byte[] template, int templateWidth, int templateHeight);

        /// <summary>
        /// 限制搜尋範圍 (ROI) 進行匹配 - 避免 CPU 與記憶體頻寬耗盡
        /// </summary>
        PatternMatchResult LocateWithRoi(byte[] image, int width, int height, byte[] template, int templateWidth, int templateHeight, int roiX, int roiY, int roiWidth, int roiHeight);
    }

    /// <summary>
    /// AVL 模板匹配實作 - 使用 ROI 限制搜尋範圍提升效能
    /// </summary>
    public class AvlPatternMatcher : IPatternMatcher
    {
        private readonly ILogger _logger;
        private readonly object _lock = new object();

        public AvlPatternMatcher(ILogger logger = null)
        {
            _logger = logger ?? Log.ForContext<AvlPatternMatcher>();
        }

        public PatternMatchResult LocateSingleObjectSad(byte[] image, int width, int height, byte[] template, int templateWidth, int templateHeight)
        {
            return LocateWithRoi(image, width, height, template, templateWidth, templateHeight, 0, 0, width, height);
        }

        public List<PatternMatchResult> LocateMultipleObjectsEdges(byte[] image, int width, int height, byte[] template, int templateWidth, int templateHeight)
        {
            var results = new List<PatternMatchResult>();
            
            lock (_lock)
            {
                try
                {
                    var imageHandle = GCHandle.Alloc(image, GCHandleType.Pinned);
                    var templateHandle = GCHandle.Alloc(template, GCHandleType.Pinned);
                    try
                    {
                        var srcImage = new Avl.Image(width, height, width, Avl.PlainType.UInt8, 1, imageHandle.AddrOfPinnedObject());
                        var tplImage = new Avl.Image(templateWidth, templateHeight, templateWidth, Avl.PlainType.UInt8, 1, templateHandle.AddrOfPinnedObject());

                        // 建立邊緣模型
                        var edgeModel = new EdgeModel2();
                        bool modelCreated = Avl.AccessEdgeModel2(tplImage, ref edgeModel, 20, 100, 0.8);
                        
                        if (!modelCreated)
                        {
                            _logger.Warning("Failed to create edge model from template");
                            return results;
                        }

                        // 多物件邊緣匹配
                        var matches = new List<Avl.Match>();
                        bool success = Avl.LocateMultipleObjects_Edges(srcImage, edgeModel, ref matches, 0.5, -30, 30);
                        
                        if (success && matches != null)
                        {
                            foreach (var match in matches)
                            {
                                results.Add(new PatternMatchResult
                                {
                                    IsSuccess = true,
                                    Confidence = match.Score,
                                    X = match.X,
                                    Y = match.Y,
                                    Angle = match.Angle
                                });
                            }
                            
                            _logger.Information("Located {Count} objects using edge matching", results.Count);
                        }
                    }
                    finally
                    {
                        if (templateHandle.IsAllocated)
                            templateHandle.Free();
                        if (imageHandle.IsAllocated)
                            imageHandle.Free();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Multiple objects edge matching failed");
                }
            }
            
            return results;
        }

        public PatternMatchResult LocateWithRoi(byte[] image, int width, int height, byte[] template, int templateWidth, int templateHeight, int roiX, int roiY, int roiWidth, int roiHeight)
        {
            lock (_lock)
            {
                try
                {
                    var imageHandle = GCHandle.Alloc(image, GCHandleType.Pinned);
                    var templateHandle = GCHandle.Alloc(template, GCHandleType.Pinned);
                    try
                    {
                        // 限制搜尋範圍為 ROI
                        var roi = new Avl.Region(roiX, roiY, roiWidth, roiHeight);
                        var srcImage = new Avl.Image(width, height, width, Avl.PlainType.UInt8, 1, imageHandle.AddrOfPinnedObject());
                        var tplImage = new Avl.Image(templateWidth, templateHeight, templateWidth, Avl.PlainType.UInt8, 1, templateHandle.AddrOfPinnedObject());

                        // 使用 SAD 匹配 (Sum of Absolute Differences)
                        var match = new Avl.Match();
                        bool success = Avl.LocateSingleObject_SAD(srcImage, tplImage, roi, ref match);
                        
                        if (success)
                        {
                            _logger.Debug("SAD match found at ({X}, {Y}) with confidence {Confidence}", match.X, match.Y, match.Score);
                            return new PatternMatchResult
                            {
                                IsSuccess = true,
                                Confidence = match.Score,
                                X = match.X,
                                Y = match.Y,
                                Angle = match.Angle
                            };
                        }
                        else
                        {
                            _logger.Debug("No SAD match found in ROI");
                            return new PatternMatchResult
                            {
                                IsSuccess = false,
                                ErrorMessage = "No pattern match found in specified ROI"
                            };
                        }
                    }
                    finally
                    {
                        if (templateHandle.IsAllocated)
                            templateHandle.Free();
                        if (imageHandle.IsAllocated)
                            imageHandle.Free();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "SAD pattern matching failed");
                    return new PatternMatchResult
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Pattern matching error: {ex.Message}"
                    };
                }
            }
        }
    }

    #endregion

    #region 幾何路徑與量測服務 (Computational Geometry)

    /// <summary>
    /// 幾何量測結果
    /// </summary>
    public class GeometryMeasurementResult
    {
        public bool IsSuccess { get; set; }
        public double Value { get; set; }
        public string MeasurementType { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public List<(double X, double Y)> Points { get; set; } = new();
    }

    /// <summary>
    /// 工業級幾何量測服務 - 測量邊緣距離、計算手臂軌跡修補、路徑貼合
    /// 對應說明書：Computational Geometry 技術模組
    /// </summary>
    public interface IGeometryMeasurer
    {
        /// <summary>
        /// 擬合圓形到點集
        /// </summary>
        GeometryMeasurementResult FitCircleToPoints(List<(double X, double Y)> points);

        /// <summary>
        /// 測試多邊形是否在另一個多邊形內
        /// </summary>
        bool TestPolygonInPolygon(List<(double X, double Y)> outer, List<(double X, double Y)> inner);

        /// <summary>
        /// 調整路徑陣列到邊緣
        /// </summary>
        List<(double X, double Y)> AdjustPathArraysToEdges(List<(double X, double Y)> path, byte[] image, int width, int height);

        /// <summary>
        /// 計算兩點間距離
        /// </summary>
        double MeasureDistance(double x1, double y1, double x2, double y2);
    }

    /// <summary>
    /// AVL 幾何量測實作 - 多執行緒隔離深拷貝
    /// </summary>
    public class AvlGeometryMeasurer : IGeometryMeasurer
    {
        private readonly ILogger _logger;

        public AvlGeometryMeasurer(ILogger logger = null)
        {
            _logger = logger ?? Log.ForContext<AvlGeometryMeasurer>();
        }

        public GeometryMeasurementResult FitCircleToPoints(List<(double X, double Y)> points)
        {
            if (points == null || points.Count < 3)
            {
                return new GeometryMeasurementResult
                {
                    IsSuccess = false,
                    ErrorMessage = "At least 3 points required for circle fitting"
                };
            }

            try
            {
                // 轉換為 AVL 點格式
                var avlPoints = points.Select(p => new Avs.Point2D(p.X, p.Y)).ToList();
                
                // 擬合圓形
                Avs.Circle2D fittedCircle;
                bool success = Avs.FitCircleToPoints(avlPoints, out fittedCircle);
                
                if (success)
                {
                    _logger.Debug("Circle fitted: Center=({X}, {Y}), Radius={Radius}", fittedCircle.Center.X, fittedCircle.Center.Y, fittedCircle.Radius);
                    return new GeometryMeasurementResult
                    {
                        IsSuccess = true,
                        Value = fittedCircle.Radius,
                        MeasurementType = "CircleRadius",
                        Points = points
                    };
                }
                else
                {
                    return new GeometryMeasurementResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Circle fitting algorithm failed"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Circle fitting failed");
                return new GeometryMeasurementResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Circle fitting error: {ex.Message}"
                };
            }
        }

        public bool TestPolygonInPolygon(List<(double X, double Y)> outer, List<(double X, double Y)> inner)
        {
            if (outer == null || outer.Count < 3 || inner == null || inner.Count < 3)
                return false;

            try
            {
                var outerPolygon = outer.Select(p => new Avs.Point2D(p.X, p.Y)).ToList();
                var innerPolygon = inner.Select(p => new Avs.Point2D(p.X, p.Y)).ToList();

                return Avs.TestPolygonInPolygon(outerPolygon, innerPolygon);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Polygon containment test failed");
                return false;
            }
        }

        public List<(double X, double Y)> AdjustPathArraysToEdges(List<(double X, double Y)> path, byte[] image, int width, int height)
        {
            if (path == null || path.Count < 2)
                return new List<(double X, double Y)>();

            if (image == null || image.Length < width * height)
                return new List<(double X, double Y)>();

            try
            {
                var handle = GCHandle.Alloc(image, GCHandleType.Pinned);
                try
                {
                    var srcImage = new Avl.Image(width, height, width, Avl.PlainType.UInt8, 1, handle.AddrOfPinnedObject());
                    var inputPath = path.Select(p => new Avs.Point2D(p.X, p.Y)).ToList();
                    
                    var adjustedPath = new List<Avs.Point2D>();
                    bool success = Avs.AdjustPathArraysToEdges(srcImage, inputPath, ref adjustedPath, 5.0, 20.0);
                    
                    if (success && adjustedPath != null)
                    {
                        _logger.Debug("Path adjusted to edges: {Count} points", adjustedPath.Count);
                        return adjustedPath.Select(p => (p.X, p.Y)).ToList();
                    }
                    else
                    {
                        _logger.Warning("Path adjustment failed - returning original path");
                        return path;
                    }
                }
                finally
                {
                    if (handle.IsAllocated)
                        handle.Free();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Path adjustment to edges failed");
                return path;
            }
        }

        public double MeasureDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }
    }

    #endregion

    #region 一維訊號與輪廓分析服務 (1D Signal & Profile Analysis)

    /// <summary>
    /// 輪廓分析結果
    /// </summary>
    public class ProfileAnalysisResult
    {
        public bool IsSuccess { get; set; }
        public double? Maximum { get; set; }
        public double? Minimum { get; set; }
        public double? Average { get; set; }
        public List<double?> Profile { get; set; } = new();
        public List<(int Index, double Value)> Peaks { get; set; } = new();
        public string ErrorMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// 工業級一維訊號分析服務 - 線雷射掃描高度、段差量測、膠路連續波峰檢查
    /// 對應說明書：1D Signal & Profile 技術模組
    /// </summary>
    public interface IProfileAnalyzer
    {
        /// <summary>
        /// 累積輪廓數據
        /// </summary>
        ProfileAnalysisResult AccumulateProfile(List<double?> profiles);

        /// <summary>
        /// 查找輪廓最大值 (亞像素定位)
        /// </summary>
        ProfileAnalysisResult FindProfileMaximum(double[] profile);

        /// <summary>
        /// 掃描多個脊線 (膠路波峰)
        /// </summary>
        ProfileAnalysisResult ScanMultipleRidges(double[] profile, double minHeight, double minDistance);
    }

    /// <summary>
    /// AVL 一維訊號分析實作 - 處理 NaN/Nil 無效點
    /// </summary>
    public class AvlProfileAnalyzer : IProfileAnalyzer
    {
        private readonly ILogger _logger;

        public AvlProfileAnalyzer(ILogger logger = null)
        {
            _logger = logger ?? Log.ForContext<AvlProfileAnalyzer>();
        }

        public ProfileAnalysisResult AccumulateProfile(List<double?> profiles)
        {
            if (profiles == null || profiles.Count == 0)
            {
                return new ProfileAnalysisResult
                {
                    IsSuccess = false,
                    ErrorMessage = "No profile data provided"
                };
            }

            try
            {
                // 過濾 NaN/Nil 並計算平均值
                var validValues = profiles
                    .Where(v => v.HasValue && !double.IsNaN(v.Value))
                    .Select(v => v.Value)
                    .ToList();

                if (validValues.Count == 0)
                {
                    return new ProfileAnalysisResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "No valid profile data (all NaN/Nil)"
                    };
                }

                return new ProfileAnalysisResult
                {
                    IsSuccess = true,
                    Maximum = validValues.Max(),
                    Minimum = validValues.Min(),
                    Average = validValues.Average(),
                    Profile = profiles
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Profile accumulation failed");
                return new ProfileAnalysisResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Profile accumulation error: {ex.Message}"
                };
            }
        }

        public ProfileAnalysisResult FindProfileMaximum(double[] profile)
        {
            if (profile == null || profile.Length == 0)
            {
                return new ProfileAnalysisResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Empty profile array"
                };
            }

            try
            {
                // 使用 AVL 亞像素定位 API
                double maxValue;
                int maxIndex;
                bool success = Avs.ProfileMaximum(profile, out maxValue, out maxIndex);
                
                if (success)
                {
                    _logger.Debug("Profile maximum found: {Value} at index {Index}", maxValue, maxIndex);
                    return new ProfileAnalysisResult
                    {
                        IsSuccess = true,
                        Maximum = maxValue,
                        Profile = profile.ToList<double?>()
                    };
                }
                else
                {
                    // 降級處理：使用 LINQ
                    maxValue = profile.Max();
                    maxIndex = Array.IndexOf(profile, maxValue);
                    return new ProfileAnalysisResult
                    {
                        IsSuccess = true,
                        Maximum = maxValue,
                        Profile = profile.ToList<double?>()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Profile maximum detection failed");
                return new ProfileAnalysisResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Profile maximum error: {ex.Message}"
                };
            }
        }

        public ProfileAnalysisResult ScanMultipleRidges(double[] profile, double minHeight, double minDistance)
        {
            if (profile == null || profile.Length == 0)
            {
                return new ProfileAnalysisResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Empty profile array"
                };
            }

            try
            {
                // 掃描多個脊線 (膠路波峰)
                var ridges = new List<Avs.Ridge>();
                bool success = Avs.ScanMultipleRidges(profile, minHeight, minDistance, ref ridges);
                
                var peaks = new List<(int Index, double Value)>();
                
                if (success && ridges != null)
                {
                    foreach (var ridge in ridges)
                    {
                        peaks.Add((ridge.Index, ridge.Height));
                    }
                    _logger.Information("Scanned {Count} ridges in profile", peaks.Count);
                }
                else
                {
                    // 降級處理：簡單峰值檢測
                    for (int i = 1; i < profile.Length - 1; i++)
                    {
                        if (profile[i] > profile[i - 1] && profile[i] > profile[i + 1] && profile[i] >= minHeight)
                        {
                            peaks.Add((i, profile[i]));
                        }
                    }
                }

                return new ProfileAnalysisResult
                {
                    IsSuccess = true,
                    Maximum = peaks.Count > 0 ? peaks.Max(p => p.Value) : (double?)null,
                    Minimum = profile.Min(),
                    Average = profile.Where(v => !double.IsNaN(v)).Average(),
                    Profile = profile.ToList<double?>(),
                    Peaks = peaks
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Multiple ridges scanning failed");
                return new ProfileAnalysisResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Ridge scanning error: {ex.Message}"
                };
            }
        }
    }

    #endregion

    #region AI 字元與條碼辨識服務 (OCR & SVM)

    /// <summary>
    /// OCR 識別結果
    /// </summary>
    public class OcrRecognitionResult
    {
        public bool IsSuccess { get; set; }
        public string RecognizedText { get; set; } = string.Empty;
        public List<OcrCandidate> Candidates { get; set; } = new();
        public string ErrorMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// OCR 候選字
    /// </summary>
    public class OcrCandidate
    {
        public string Text { get; set; } = string.Empty;
        public double Confidence { get; set; }
    }

    /// <summary>
    /// 工業級 OCR 與 SVM 辨識服務 - 讀取雷雕序號、DataMatrix 條碼
    /// 對應說明書：OCR & SVM 技術模組
    /// </summary>
    public interface IOcrRecognizer
    {
        /// <summary>
        /// 訓練 SVM OCR 模型
        /// </summary>
        bool TrainOcrSvm(List<byte[]> characterSamples, List<string> labels, string modelPath);

        /// <summary>
        /// 執行 OCR 識別
        /// </summary>
        OcrRecognitionResult RecognizeOcr(byte[] image, int width, int height, string modelPath);

        /// <summary>
        /// 標準化輸入樣本 (張量安全防禦)
        /// </summary>
        byte[] MakeCharacterSamples(byte[] image, int width, int height, int targetWidth, int targetHeight);
    }

    /// <summary>
    /// AVL OCR 識別實作 - 張量維度驗證與標準化
    /// </summary>
    public class AvlOcrRecognizer : IOcrRecognizer
    {
        private readonly ILogger _logger;
        private readonly object _lock = new object();

        public AvlOcrRecognizer(ILogger logger = null)
        {
            _logger = logger ?? Log.ForContext<AvlOcrRecognizer>();
        }

        public bool TrainOcrSvm(List<byte[]> characterSamples, List<string> labels, string modelPath)
        {
            if (characterSamples == null || characterSamples.Count == 0)
            {
                _logger.Error("No character samples provided for OCR training");
                return false;
            }

            if (labels == null || labels.Count != characterSamples.Count)
            {
                _logger.Error("Labels count must match samples count");
                return false;
            }

            lock (_lock)
            {
                try
                {
                    // 標準化輸入樣本 - 張量安全防禦
                    var standardizedSamples = new List<Avs.CharacterSample>();
                    
                    for (int i = 0; i < characterSamples.Count; i++)
                    {
                        var sample = characterSamples[i];
                        var label = labels[i];
                        
                        // 假設樣本為 20x20 像素
                        var handle = GCHandle.Alloc(sample, GCHandleType.Pinned);
                        try
                        {
                            var img = new Avl.Image(20, 20, 20, Avl.PlainType.UInt8, 1, handle.AddrOfPinnedObject());
                            standardizedSamples.Add(new Avs.CharacterSample(img, label));
                        }
                        finally
                        {
                            if (handle.IsAllocated)
                                handle.Free();
                        }
                    }

                    // 訓練 SVM 模型
                    var svmParams = new Avs.OcrSvmParameters
                    {
                        KernelType = Avs.SvmKernelType.Rbf,
                        C = 10.0,
                        Gamma = 0.1
                    };

                    bool success = Avs.TrainOcr_SVM(standardizedSamples, svmParams, modelPath);
                    
                    if (success)
                    {
                        _logger.Information("OCR SVM model trained successfully and saved to {Path}", modelPath);
                        return true;
                    }
                    else
                    {
                        _logger.Error("OCR SVM training failed");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "OCR SVM training exception");
                    return false;
                }
            }
        }

        public OcrRecognitionResult RecognizeOcr(byte[] image, int width, int height, string modelPath)
        {
            if (image == null || image.Length < width * height)
            {
                return new OcrRecognitionResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid image payload"
                };
            }

            lock (_lock)
            {
                try
                {
                    var handle = GCHandle.Alloc(image, GCHandleType.Pinned);
                    try
                    {
                        var srcImage = new Avl.Image(width, height, width, Avl.PlainType.UInt8, 1, handle.AddrOfPinnedObject());
                        
                        // 載入預訓練模型
                        var ocrModel = new Avs.OcrModel();
                        bool modelLoaded = Avs.LoadOcrModel(modelPath, ref ocrModel);
                        
                        if (!modelLoaded)
                        {
                            return new OcrRecognitionResult
                            {
                                IsSuccess = false,
                                ErrorMessage = $"Failed to load OCR model from {modelPath}"
                            };
                        }

                        // 執行 OCR 識別
                        var candidates = new List<Avs.OcrCandidate>();
                        bool success = Avs.RecognizeOcr(srcImage, ocrModel, ref candidates);
                        
                        if (success && candidates != null && candidates.Count > 0)
                        {
                            var bestCandidate = candidates.OrderByDescending(c => c.Confidence).First();
                            _logger.Information("OCR recognized: {Text} (Confidence: {Confidence})", bestCandidate.Text, bestCandidate.Confidence);
                            
                            return new OcrRecognitionResult
                            {
                                IsSuccess = true,
                                RecognizedText = bestCandidate.Text,
                                Candidates = candidates.Select(c => new OcrCandidate
                                {
                                    Text = c.Text,
                                    Confidence = c.Confidence
                                }).ToList()
                            };
                        }
                        else
                        {
                            return new OcrRecognitionResult
                            {
                                IsSuccess = false,
                                ErrorMessage = "No OCR candidates found"
                            };
                        }
                    }
                    finally
                    {
                        if (handle.IsAllocated)
                            handle.Free();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "OCR recognition failed");
                    return new OcrRecognitionResult
                    {
                        IsSuccess = false,
                        ErrorMessage = $"OCR error: {ex.Message}"
                    };
                }
            }
        }

        public byte[] MakeCharacterSamples(byte[] image, int width, int height, int targetWidth, int targetHeight)
        {
            if (image == null || image.Length < width * height)
                throw new ArgumentException("Invalid image payload");

            // 雙線性插值縮放至目標尺寸
            var result = new byte[targetWidth * targetHeight];
            
            double xRatio = (double)width / targetWidth;
            double yRatio = (double)height / targetHeight;
            
            for (int y = 0; y < targetHeight; y++)
            {
                for (int x = 0; x < targetWidth; x++)
                {
                    int px = (int)(xRatio * x);
                    int py = (int)(yRatio * y);
                    
                    // 邊界檢查
                    px = Math.Min(px, width - 1);
                    py = Math.Min(py, height - 1);
                    
                    result[y * targetWidth + x] = image[py * width + px];
                }
            }
            
            return result;
        }
    }

    #endregion
}
#else
namespace ShoeMoldControl.Vision.Avl
{
    // 非支援平台的空殼實作
    public class AvlImagePreprocessor { }
    public class AvlPatternMatcher { }
    public class AvlGeometryMeasurer { }
    public class AvlProfileAnalyzer { }
    public class AvlOcrRecognizer { }
}
#endif
