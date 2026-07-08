using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShoeMoldControl.Core.Domain;
using ShoeMoldControl.Core.Hardware;

namespace ShoeMoldControl.Infrastructure.Hardware.Adapters
{
    /// <summary>
    /// 機器人運動控制配接器 - 將廠商巨集轉換為標準化非同步介面
    /// </summary>
    public class AvrRobotMotionAdapter : IAvrRobotMotion
    {
        private readonly AvrHardwareGateway _gateway;

        public AvrRobotMotionAdapter(AvrHardwareGateway gateway)
        {
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        }

        /// <summary>
        /// 線性運動控制 (MoveL)
        /// </summary>
        public async Task<HardwareMotionResult> MoveLinearAsync(RobotCoordinatePose pose, int speed, int acceleration, CancellationToken token)
        {
            return await _gateway.ExecuteSafeFuncAsync(macros =>
            {
                var outResult = new Amr.Conditional<bool>();
                var outResponseString = new Amr.Conditional<string>();
                var outResponseStringArray = new Amr.Conditional<List<string>>();

                // 建構廠商原生驅動對應的點位物件
                Atl.Pose6 targetPose = new Atl.Pose6
                {
                    X = pose.X,
                    Y = pose.Y,
                    Z = pose.Z,
                    Rx = pose.Rx,
                    Ry = pose.Ry,
                    Rz = pose.Rz
                };

                macros.MoveL(speed, acceleration, targetPose, outResult, outResponseString, outResponseStringArray);

                return new HardwareMotionResult
                {
                    IsSuccess = outResult.Value,
                    Message = outResponseString.Value ?? string.Empty,
                    DetailedLogs = outResponseStringArray.Value ?? new List<string>()
                };
            }, token);
        }

        /// <summary>
        /// 設定速度倍率
        /// </summary>
        public async Task<HardwareMotionResult> SetSpeedFactorAsync(int speedFactor)
        {
            return await _gateway.ExecuteSafeFuncAsync(macros =>
            {
                var outResult = new Amr.Conditional<bool>();
                var outResponseString = new Amr.Conditional<string>();
                var outResponseStringArray = new Amr.Conditional<List<string>>();

                macros.SpeedFactor(speedFactor, outResult, outResponseString, outResponseStringArray);

                return new HardwareMotionResult
                {
                    IsSuccess = outResult.Value,
                    Message = outResponseString.Value ?? string.Empty,
                    DetailedLogs = outResponseStringArray.Value ?? new List<string>()
                };
            }, CancellationToken.None);
        }

        /// <summary>
        /// 獲取當前機器人姿態
        /// </summary>
        public async Task<RobotCoordinatePose> GetCurrentPoseAsync()
        {
            return await _gateway.ExecuteSafeFuncAsync(macros =>
            {
                var outResult = new Amr.Conditional<bool>();
                var outResponseString = new Amr.Conditional<string>();
                var outResponseStringArray = new Amr.Conditional<List<string>>();
                var outPose = new Amr.Conditional<Atl.Pose6>();

                macros.GetPose(outResult, outResponseString, outResponseStringArray, outPose);

                if (!outResult.Value || outPose.Value == null)
                {
                    throw new InvalidOperationException("無法自廠商內部核心獲取目前座標姿態。");
                }

                return new RobotCoordinatePose
                {
                    X = outPose.Value.X,
                    Y = outPose.Value.Y,
                    Z = outPose.Value.Z,
                    Rx = outPose.Value.Rx,
                    Ry = outPose.Value.Ry,
                    Rz = outPose.Value.Rz
                };
            }, CancellationToken.None);
        }
    }
}
