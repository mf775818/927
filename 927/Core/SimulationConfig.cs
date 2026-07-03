using System;
using Microsoft.Extensions.Configuration;

namespace ShoeMoldControl.Core
{
    /// <summary>
    /// 模擬模式配置接口
    /// 用於控制是否啟用虛擬服務及相關選項
    /// </summary>
    public interface ISimulationConfig
    {
        /// <summary>
        /// 是否啟用模擬模式
        /// </summary>
        bool EnableSimulation { get; }

        /// <summary>
        /// 是否使用虛擬視覺服務
        /// </summary>
        bool UseMockVision { get; }

        /// <summary>
        /// 是否使用虛擬機器人控制器
        /// </summary>
        bool UseMockRobot { get; }

        /// <summary>
        /// 虛擬視覺延遲（毫秒）
        /// </summary>
        int MockVisionDelayMs { get; }

        /// <summary>
        /// 虛擬機器人命令延遲（毫秒）
        /// </summary>
        int MockRobotCommandDelayMs { get; }

        /// <summary>
        /// 虛擬服務成功率 (0.0 - 1.0)
        /// </summary>
        double MockServiceSuccessRate { get; }

        /// <summary>
        /// 固定模擬條碼（可選）
        /// </summary>
        string FixedMockBarcode { get; }
    }

    /// <summary>
    /// 模擬模式配置實現
    /// 擴展 AppConfig 以支持模擬功能
    /// </summary>
    public class SimulationConfig : ISimulationConfig
    {
        private readonly IConfiguration _configuration;

        public SimulationConfig(IConfiguration configuration = null)
        {
            _configuration = configuration;
            
            // 設置默認值
            EnableSimulation = false;
            UseMockVision = false;
            UseMockRobot = false;
            MockVisionDelayMs = 500;
            MockRobotCommandDelayMs = 300;
            MockServiceSuccessRate = 1.0;
            FixedMockBarcode = string.Empty;

            if (_configuration != null)
            {
                LoadFromConfiguration();
            }
        }

        private void LoadFromConfiguration()
        {
            EnableSimulation = GetBooleanValue("Simulation:EnableSimulation", false);
            UseMockVision = GetBooleanValue("Simulation:UseMockVision", false);
            UseMockRobot = GetBooleanValue("Simulation:UseMockRobot", false);
            MockVisionDelayMs = GetIntValue("Simulation:MockVisionDelayMs", 500);
            MockRobotCommandDelayMs = GetIntValue("Simulation:MockRobotCommandDelayMs", 300);
            MockServiceSuccessRate = GetDoubleValue("Simulation:MockServiceSuccessRate", 1.0);
            FixedMockBarcode = _configuration.GetValue<string>("Simulation:FixedMockBarcode") ?? string.Empty;
        }

        private bool GetBooleanValue(string key, bool defaultValue)
        {
            if (_configuration == null) return defaultValue;
            
            var value = _configuration.GetValue<string>(key);
            if (string.IsNullOrEmpty(value)) return defaultValue;
            
            if (bool.TryParse(value, out bool result))
            {
                return result;
            }
            
            // 支持 "true"/"false", "1"/"0", "yes"/"no"
            return value.ToLowerInvariant() switch
            {
                "1" or "yes" or "on" => true,
                "0" or "no" or "off" => false,
                _ => defaultValue
            };
        }

        private int GetIntValue(string key, int defaultValue)
        {
            if (_configuration == null) return defaultValue;
            return _configuration.GetValue<int>(key, defaultValue);
        }

        private double GetDoubleValue(string key, double defaultValue)
        {
            if (_configuration == null) return defaultValue;
            return _configuration.GetValue<double>(key, defaultValue);
        }

        public bool EnableSimulation { get; private set; }
        public bool UseMockVision { get; private set; }
        public bool UseMockRobot { get; private set; }
        public int MockVisionDelayMs { get; private set; }
        public int MockRobotCommandDelayMs { get; private set; }
        public double MockServiceSuccessRate { get; private set; }
        public string FixedMockBarcode { get; private set; }
    }

    /// <summary>
    /// 連接狀態管理器
    /// 用於追蹤和管理視覺及機器人的連線狀態
    /// </summary>
    public interface IConnectionStateManager
    {
        /// <summary>
        /// 視覺系統是否已連線
        /// </summary>
        bool IsVisionConnected { get; }

        /// <summary>
        /// 機器人是否已連線
        /// </summary>
        bool IsRobotConnected { get; }

        /// <summary>
        /// 是否處於模擬模式
        /// </summary>
        bool IsSimulationMode { get; }

        /// <summary>
        /// 更新視覺連線狀態
        /// </summary>
        void UpdateVisionConnectionStatus(bool isConnected);

        /// <summary>
        /// 更新機器人連線狀態
        /// </summary>
        void UpdateRobotConnectionStatus(bool isConnected);

        /// <summary>
        /// 獲取連線狀態摘要
        /// </summary>
        string GetConnectionStatusSummary();
    }

    /// <summary>
    /// 連接狀態管理器實現
    /// </summary>
    public class ConnectionStateManager : IConnectionStateManager
    {
        private bool _isVisionConnected;
        private bool _isRobotConnected;
        private readonly bool _isSimulationMode;

        public ConnectionStateManager(ISimulationConfig simulationConfig)
        {
            _isSimulationMode = simulationConfig?.EnableSimulation ?? false;
            _isVisionConnected = false;
            _isRobotConnected = false;
        }

        public bool IsVisionConnected => _isVisionConnected;
        public bool IsRobotConnected => _isRobotConnected;
        public bool IsSimulationMode => _isSimulationMode;

        public void UpdateVisionConnectionStatus(bool isConnected)
        {
            _isVisionConnected = isConnected;
        }

        public void UpdateRobotConnectionStatus(bool isConnected)
        {
            _isRobotConnected = isConnected;
        }

        public string GetConnectionStatusSummary()
        {
            if (_isSimulationMode)
            {
                return $"[SIMULATION] Vision: {(_isVisionConnected ? "Connected (Mock)" : "Disconnected")}, " +
                       $"Robot: {(_isRobotConnected ? "Connected (Mock)" : "Disconnected")}";
            }
            else
            {
                return $"[PRODUCTION] Vision: {(_isVisionConnected ? "Connected" : "Disconnected")}, " +
                       $"Robot: {(_isRobotConnected ? "Connected" : "Disconnected")}";
            }
        }
    }
}
