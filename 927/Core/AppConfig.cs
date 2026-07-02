namespace ShoeMoldControl.Core
{
    public class AppConfig : IVisionConfig, IRobotConfig
    {
        public string RobotIpAddress { get; set; } = "192.168.1.10";
        public int RobotDashboardPort { get; set; } = 29999;
        public int RobotModbusPort { get; set; } = 502;
        public ushort RobotModbusStatusRegister { get; set; } = 1012;
        public int RobotCommandTimeoutMs { get; set; } = 5000;

        public string VisionIpAddress { get; set; } = "192.168.1.20";
        public int VisionPort { get; set; } = 5000;
        public int VisionTimeoutMs { get; set; } = 3000;
        public string VisionTriggerCommand { get; set; } = "T1\r\n";
    }
}
