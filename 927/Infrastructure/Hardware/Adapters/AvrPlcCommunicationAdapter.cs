using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShoeMoldControl.Core.Domain;
using ShoeMoldControl.Core.Hardware;

namespace ShoeMoldControl.Infrastructure.Hardware.Adapters
{
    /// <summary>
    /// PLC 通訊配接器 - 將廠商 PLC 讀寫巨集轉換為標準化非同步介面
    /// </summary>
    public class AvrPlcCommunicationAdapter : IAvrPlcCommunicator
    {
        private readonly AvrHardwareGateway _gateway;

        public AvrPlcCommunicationAdapter(AvrHardwareGateway gateway)
        {
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        }

        /// <summary>
        /// 讀取 PLC 暫存器
        /// </summary>
        public async Task<int> ReadRegisterAsync(int address)
        {
            return await _gateway.ExecuteSafeFuncAsync(macros =>
            {
                List<int> outIntegerValue = new List<int>();
                macros.ReadPLC(_gateway.PlcSocketId, address, outIntegerValue);

                if (outIntegerValue == null || outIntegerValue.Count == 0)
                {
                    throw new InvalidOperationException($"讀取 PLC 暫存器位址失敗：{address}");
                }

                return outIntegerValue[0];
            }, CancellationToken.None);
        }

        /// <summary>
        /// 寫入 PLC 暫存器
        /// </summary>
        public async Task WriteRegisterAsync(int address, int value)
        {
            await _gateway.ExecuteSafeActionAsync(macros =>
            {
                macros.WritePLC(_gateway.PlcSocketId, address, value);
            }, CancellationToken.None);
        }
    }
}
