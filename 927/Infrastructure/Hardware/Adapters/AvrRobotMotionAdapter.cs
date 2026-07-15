using Amr; 
using Atl; 
using AuroraVision;
using ShoeMoldControl.Core.Domain;
using ShoeMoldControl.Core.Hardware;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShoeMoldControl.Infrastructure.Hardware.Adapters
{
    public class AvrRobotMotionAdapter: IAvrRobotMotion
    {
        private readonly AvrHardwareGateway _gateway;

        public AvrRobotMotionAdapter(AvrHardwareGateway gateway)
        {
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        }

        public async Task<HardwareMotionResult> MoveLinearAsync(RobotCoordinatePose pose, int speed, int acceleration, CancellationToken token)
        {
            return await _gateway.ExecuteSafeFuncAsync(macros => {
                var outResult = new Conditional<bool>();
                var outResponseString = new Conditional<string>();
                var outResponseStringArray = new Conditional<List<string>>();

                Pose6 targetPose = new Pose6
                {
                    X =  ((float)pose.X),
                    Y =  ((float)pose.Y),    
                    Z =  ((float)pose.Z),
                    Rx = ((float)pose.Rx),
                    Ry = ((float)pose.Ry),
                    Rz = ((float)pose.Rz)
                };

                macros.MoveL(speed, acceleration, targetPose, outResult, outResponseString, outResponseStringArray);

                return new HardwareMotionResult
                {
                    IsSuccess = outResult.Value,
                    Message = outResponseString.Value,
                    DetailedLogs = outResponseStringArray.Value ?? new List<string>()
                };
            }, token);
        }

        public async Task<HardwareMotionResult> SetSpeedFactorAsync(int speedFactor)
        {
            return await _gateway.ExecuteSafeFuncAsync(macros => {
                var outResult = new Conditional<bool>();
                var outResponseString = new Conditional<string>();
                var outResponseStringArray = new Conditional<List<string>>();

                macros.SpeedFactor(speedFactor, outResult, outResponseString, outResponseStringArray);

                return new HardwareMotionResult
                {
                    IsSuccess = outResult.Value,
                    Message = outResponseString.Value,
                    DetailedLogs = outResponseStringArray.Value ?? new List<string>()
                };
            }, CancellationToken.None);
        }

        public async Task<RobotCoordinatePose> GetCurrentPoseAsync()
        {
            return await _gateway.ExecuteSafeFuncAsync(macros => {
                var outResult = new Conditional<bool>();
                var outResponseString = new Conditional<string>();
                var outResponseStringArray = new Conditional<List<string>>();
                var outPose = new Conditional<Pose6>();

                macros.GetPose(outResult, outResponseString, outResponseStringArray, outPose);

                if (!outResult.Value || outPose.Value == null)
                {
                    throw new InvalidOperationException($"無法取得目前姿態: {outResponseString.Value}");
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