using System;
using Microsoft.Extensions.Configuration;

namespace ShoeMoldControl.Core
{
    public class AppConfig : IVisionConfig, IRobotConfig
    {
        public AppConfig(IConfiguration configuration)
        {
            if (configuration == null)
            {
                // Fallback to defaults if no configuration provided
                SetDefaults();
                return;
            }

            RobotIpAddress = configuration.GetValue<string>("Robot:IpAddress") ?? "192.168.1.10";
            RobotDashboardPort = configuration.GetValue<int>("Robot:DashboardPort", 29999);
            RobotModbusPort = configuration.GetValue<int>("Robot:ModbusPort", 502);
            RobotModbusStatusRegister = configuration.GetValue<ushort>("Robot:ModbusStatusRegister", 1012);
            RobotCommandTimeoutMs = configuration.GetValue<int>("Robot:CommandTimeoutMs", 5000);

            VisionIpAddress = configuration.GetValue<string>("Vision:IpAddress") ?? "192.168.1.20";
            VisionPort = configuration.GetValue<int>("Vision:Port", 5000);
            VisionTimeoutMs = configuration.GetValue<int>("Vision:TimeoutMs", 3000);
            VisionTriggerCommand = configuration.GetValue<string>("Vision:TriggerCommand") ?? "T1\r\n";

            ValidateConfiguration();
        }

        private void SetDefaults()
        {
            RobotIpAddress = "192.168.1.10";
            RobotDashboardPort = 29999;
            RobotModbusPort = 502;
            RobotModbusStatusRegister = 1012;
            RobotCommandTimeoutMs = 5000;

            VisionIpAddress = "192.168.1.20";
            VisionPort = 5000;
            VisionTimeoutMs = 3000;
            VisionTriggerCommand = "T1\r\n";
        }

        private void ValidateConfiguration()
        {
            if (string.IsNullOrWhiteSpace(RobotIpAddress))
                throw new InvalidOperationException("Robot IP address cannot be empty");
            
            if (string.IsNullOrWhiteSpace(VisionIpAddress))
                throw new InvalidOperationException("Vision IP address cannot be empty");

            if (RobotDashboardPort <= 0 || RobotDashboardPort > 65535)
                throw new InvalidOperationException($"Invalid robot dashboard port: {RobotDashboardPort}");

            if (RobotModbusPort <= 0 || RobotModbusPort > 65535)
                throw new InvalidOperationException($"Invalid robot modbus port: {RobotModbusPort}");

            if (VisionPort <= 0 || VisionPort > 65535)
                throw new InvalidOperationException($"Invalid vision port: {VisionPort}");

            if (RobotCommandTimeoutMs <= 0)
                throw new InvalidOperationException("Robot command timeout must be positive");

            if (VisionTimeoutMs <= 0)
                throw new InvalidOperationException("Vision timeout must be positive");
        }

        public string RobotIpAddress { get; }
        public int RobotDashboardPort { get; }
        public int RobotModbusPort { get; }
        public ushort RobotModbusStatusRegister { get; }
        public int RobotCommandTimeoutMs { get; }

        public string VisionIpAddress { get; }
        public int VisionPort { get; }
        public int VisionTimeoutMs { get; }
        public string VisionTriggerCommand { get; }
    }
}
