using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShoeMoldControl.Core.Domain;
using ShoeMoldControl.Core.Hardware;

namespace ShoeMoldControl.Infrastructure.Hardware.Adapters
{
    /// <summary>
    /// 機器人手動教導/拖曳模式控制配接器
    /// 將廠商 MoveJogStart/Stop、StartDrag/StopDrag 巨集轉換為標準化非同步介面
    /// </summary>
    public class AvrRobotInstructionalJogAdapter : IAvrRobotInstructionalJog
    {
        private readonly AvrHardwareGateway _gateway;

        public AvrRobotInstructionalJogAdapter(AvrHardwareGateway gateway)
        {
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        }

        /// <summary>
        /// 啟動手動 Jog 運動
        /// </summary>
        /// <param name="jogDirection">Jog 方向代碼 (由廠商定義)</param>
        public async Task<HardwareMotionResult> StartJogAsync(int jogDirection)
        {
            return await _gateway.ExecuteSafeFuncAsync(macros =>
            {
                var outResult = new Amr.Conditional<bool>();
                var outResponseString = new Amr.Conditional<string>();
                var outResponseStringArray = new Amr.Conditional<List<string>>();

                macros.MoveJogStart(jogDirection, outResult, outResponseString, outResponseStringArray);

                return new HardwareMotionResult
                {
                    IsSuccess = outResult.Value,
                    Message = outResponseString.Value ?? string.Empty,
                    DetailedLogs = outResponseStringArray.Value ?? new List<string>()
                };
            }, CancellationToken.None);
        }

        /// <summary>
        /// 停止手動 Jog 運動
        /// </summary>
        public async Task<HardwareMotionResult> StopJogAsync()
        {
            return await _gateway.ExecuteSafeFuncAsync(macros =>
            {
                var outResult = new Amr.Conditional<bool>();
                var outResponseString = new Amr.Conditional<string>();
                var outResponseStringArray = new Amr.Conditional<List<string>>();

                macros.MoveJogStop(outResult, outResponseString, outResponseStringArray);

                return new HardwareMotionResult
                {
                    IsSuccess = outResult.Value,
                    Message = outResponseString.Value ?? string.Empty,
                    DetailedLogs = outResponseStringArray.Value ?? new List<string>()
                };
            }, CancellationToken.None);
        }

        /// <summary>
        /// 設定拖曳模式 (啟用/停用)
        /// </summary>
        /// <param name="enable">true: 啟用拖曳模式，false: 停用拖曳模式</param>
        public async Task<HardwareMotionResult> SetDragModeAsync(bool enable)
        {
            return await _gateway.ExecuteSafeFuncAsync(macros =>
            {
                var outResult = new Amr.Conditional<bool>();
                var outResponseString = new Amr.Conditional<string>();
                var outResponseStringArray = new Amr.Conditional<List<string>>();

                if (enable)
                {
                    macros.StartDrag(outResult, outResponseString, outResponseStringArray);
                }
                else
                {
                    macros.StopDrag(outResult, outResponseString, outResponseStringArray);
                }

                return new HardwareMotionResult
                {
                    IsSuccess = outResult.Value,
                    Message = outResponseString.Value ?? string.Empty,
                    DetailedLogs = outResponseStringArray.Value ?? new List<string>()
                };
            }, CancellationToken.None);
        }
    }
}
