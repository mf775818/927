using System;
using ShoeMoldControl.Core.Domain;
using ShoeMoldControl.Core.Hardware;

namespace ShoeMoldControl.Infrastructure.Hardware.Adapters
{
    /// <summary>
    /// 視覺分析器配接器 - 將廠商 Inspection 巨集轉換為標準化介面
    /// </summary>
    public class AvrImageAnalyzerAdapter : IAvrImageAnalyzer
    {
        private readonly AvrHardwareGateway _gateway;

        public AvrImageAnalyzerAdapter(AvrHardwareGateway gateway)
        {
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        }

        /// <summary>
        /// 分析影像幀，識別模具編號與邊界框
        /// </summary>
        public VisionInspectionResult AnalyzeFrame(Avl.Image image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            int moldNum = -1;
            Atl.Box outObject;

            // 呼叫廠商巨集演算法核心
            _gateway.RawMacros.Inspection(image, out moldNum, out outObject);

            if (moldNum <= 0)
            {
                return new VisionInspectionResult
                {
                    IsSuccess = false,
                    ErrorMessage = "未識別到模具 QR code 條碼樣式。"
                };
            }

            return new VisionInspectionResult
            {
                IsSuccess = true,
                MoldNumber = moldNum,
                BoundBox = new System.Drawing.RectangleF(outObject.X, outObject.Y, outObject.Width, outObject.Height)
            };
        }
    }
}
