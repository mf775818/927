using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShoeMoldControl.Core.Models
{
    /// <summary>
    /// 工業級數據模型基類 - 實現 INotifyPropertyChanged 事件驅動架構
    /// 用於 HPC 層與 UI 層的解耦，確保高頻數據更新不阻塞 UI 執行緒
    /// </summary>
    public abstract class IndustrialDataModelBase : INotifyPropertyChanged
    {
        private bool _isDisposed = false;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 觸發屬性變更事件 (HPC 層通知 UI 層的核心機制)
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (!_isDisposed)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// 安全設定屬性值並觸發事件
        /// </summary>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public void Dispose()
        {
            _isDisposed = true;
            PropertyChanged = null;
        }
    }

    /// <summary>
    /// 機台生產數據模型 - 高頻刷新數據源 (HPC Layer)
    /// 模擬 PLC/傳感器每秒數百次的高頻數據拋出
    /// </summary>
    public class MachineDataModel : IndustrialDataModelBase
    {
        private int _productionCount;
        private double _temperature;
        private double _pressure;
        private string _currentStatus;
        private DateTime _lastUpdateTime;
        private int _alarmCode;
        private bool _isRunning;

        /// <summary>
        /// 生產計數 (高頻更新字段)
        /// </summary>
        public int ProductionCount
        {
            get => _productionCount;
            set => SetProperty(ref _productionCount, value);
        }

        /// <summary>
        /// 爐溫數據 (模擬高頻傳感器讀數)
        /// </summary>
        public double Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, Math.Round(value, 2));
        }

        /// <summary>
        /// 氣壓數據 (模擬高頻傳感器讀數)
        /// </summary>
        public double Pressure
        {
            get => _pressure;
            set => SetProperty(ref _pressure, Math.Round(value, 3));
        }

        /// <summary>
        /// 當前運行狀態
        /// </summary>
        public string CurrentStatus
        {
            get => _currentStatus;
            set => SetProperty(ref _currentStatus, value);
        }

        /// <summary>
        /// 最後更新時間
        /// </summary>
        public DateTime LastUpdateTime
        {
            get => _lastUpdateTime;
            set => SetProperty(ref _lastUpdateTime, value);
        }

        /// <summary>
        /// 警報代碼 (0=正常)
        /// </summary>
        public int AlarmCode
        {
            get => _alarmCode;
            set => SetProperty(ref _alarmCode, value);
        }

        /// <summary>
        /// 運行標誌
        /// </summary>
        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        /// <summary>
        /// 模擬高頻數據更新 (HPC 計算核心)
        /// 實際應用中由 PLC/Modbus 通訊驅動
        /// </summary>
        public void SimulateHighFrequencyDataUpdate(Random random)
        {
            // 模擬真實產線高頻數據波動
            Temperature = 45.0 + (random.NextDouble() * 10); // 45-55°C
            Pressure = 0.650 + (random.NextDouble() * 0.1);  // 0.65-0.75 MPa
            LastUpdateTime = DateTime.Now;
            
            // 注意：此方法會頻繁觸發 PropertyChanged 事件
            // 需透過 HFC 節流器控制 UI 更新頻率
        }
    }

    /// <summary>
    /// 警報數據模型 - 用於警報風暴抑制
    /// </summary>
    public class AlarmDataModel : IndustrialDataModelBase
    {
        private int _alarmId;
        private string _alarmMessage;
        private DateTime _alarmTime;
        private AlarmLevel _alarmLevel;
        private int _occurrenceCount;

        public int AlarmId
        {
            get => _alarmId;
            set => SetProperty(ref _alarmId, value);
        }

        public string AlarmMessage
        {
            get => _alarmMessage;
            set => SetProperty(ref _alarmMessage, value);
        }

        public DateTime AlarmTime
        {
            get => _alarmTime;
            set => SetProperty(ref _alarmTime, value);
        }

        public AlarmLevel AlarmLevel
        {
            get => _alarmLevel;
            set => SetProperty(ref _alarmLevel, value);
        }

        /// <summary>
        /// 發生次數 (用於警報風暴抑制時的計數顯示)
        /// </summary>
        public int OccurrenceCount
        {
            get => _occurrenceCount;
            set => SetProperty(ref _occurrenceCount, value);
        }
    }

    /// <summary>
    /// 警報等級定義 (符合 ISO 標準色彩編碼)
    /// </summary>
    public enum AlarmLevel
    {
        Info = 0,      // 藍色 - 一般資訊
        Warning = 1,   // 黃色 - 警告
        Critical = 2,  // 橙色 - 嚴重警告
        Error = 3      // 紅色 - 錯誤/緊急停止
    }
    
    /// <summary>
    /// 視覺系統操作模式
    /// </summary>
    public enum VisionSystemMode
    {
        Simulation,  // 虛擬模擬模式
        Real         // 實際硬體模式
    }
    
    /// <summary>
    /// Jog 操作類型定義
    /// </summary>
    public enum JogType
    {
        JOG_PLUS_X,
        JOG_MINUS_X,
        JOG_PLUS_Y,
        JOG_MINUS_Y,
        JOG_PLUS_Z,
        JOG_MINUS_Z,
        JOG_PLUS_R,
        JOG_MINUS_R
    }
    
    /// <summary>
    /// Robot 座標姿態模型
    /// </summary>
    public class RobotCoordinatePose
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double R { get; set; }
        
        public override string ToString() => $"X:{X:F2} Y:{Y:F2} Z:{Z:F2} R:{R:F2}";
    }
    
    /// <summary>
    /// 視覺檢測結果模型
    /// </summary>
    public class VisionInspectionResult
    {
        public bool IsSuccess { get; set; }
        public string BarcodeText { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public List<(double X, double Y)> MarkPositions { get; set; } = new();
    }
}
