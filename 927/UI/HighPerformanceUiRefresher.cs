using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Industrial.UI.Framework
{
    /// <summary>
    /// HFC/HPC 高頻異步事件刷新管理器 (核心架構)
    /// 負責將 HPC 層的高頻數據變更，經過節流後安全地同步至 UI 執行緒。
    /// 解決傳統 Polling 導致的 UI 卡死與 CPU 飆升問題。
    /// </summary>
    /// <typeparam name="T">數據模型型別 (需實作 INotifyPropertyChanged)</typeparam>
    public class HighPerformanceUiRefresher<T> : IDisposable where T : INotifyPropertyChanged
    {
        private readonly T _dataSource;
        private readonly Control _uiControl;
        private readonly Action<T> _updateAction;
        private readonly System.Windows.Forms.Timer _hfcThrottleTimer;
        private bool _isUpdatePending = false;
        private bool _isDisposed = false;
        private readonly int _throttleIntervalMs;

        /// <summary>
        /// 初始化 HFC/HPC 刷新器
        /// </summary>
        /// <param name="dataSource">綁定的數據源 (HPC 層，需實作 INotifyPropertyChanged)</param>
        /// <param name="uiControl">需要刷新的 WinForm 控制項 (用於 Invoke 上下文)</param>
        /// <param name="updateAction">UI 更新的具體渲染邏輯 (僅在 UI 執行緒執行)</param>
        /// <param name="throttleIntervalMs">HFC 緩衝時間視窗 (毫秒)，建議 16ms(60Hz) ~ 33ms(30Hz)</param>
        public HighPerformanceUiRefresher(T dataSource, Control uiControl, Action<T> updateAction, int throttleIntervalMs = 33)
        {
            _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
            _uiControl = uiControl ?? throw new ArgumentNullException(nameof(uiControl));
            _updateAction = updateAction ?? throw new ArgumentNullException(nameof(updateAction));
            _throttleIntervalMs = throttleIntervalMs;

            // 1. 架構分層與事件綁定：只訂閱事件，不直    接操作 UI
            _dataSource.PropertyChanged += OnDataSourcePropertyChanged;

            // 2. HFC 刷新機制：初始化節流計時器
            _hfcThrottleTimer = new System.Windows.Forms.Timer();
            _hfcThrottleTimer.Interval = throttleIntervalMs;
            _hfcThrottleTimer.Tick += HfcThrottleTimer_Tick;
            _hfcThrottleTimer.Start();
        }

        /// <summary>
        /// HPC 層屬性變更事件處理器
        /// 嚴禁在此處操作任何 UI 控制項 (避免 CrossThread 異常)
        /// </summary>
        private void OnDataSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 僅標記旗標，實際更新由 Timer 觸發
            _isUpdatePending = true;
        }

        /// <summary>
        /// HFC 節流計時器觸發 (固定頻率檢查是否需要更新)
        /// </summary>
        private void HfcThrottleTimer_Tick(object sender, EventArgs e)
        {
            if (_isDisposed) return;

            // 檢查是否有待處理的更新，且確認 UI 控制項生命週期正常
            if (_isUpdatePending && !_uiControl.IsDisposed && _uiControl.IsHandleCreated)
            {
                _isUpdatePending = false; // 重置旗標

                // 3. 異步非阻塞切換至 UI 執行緒 (BeginInvoke 避免 Deadlock)
                _uiControl.BeginInvoke(new Action(() =>
                {
                    if (_isDisposed) return;
                    try
                    {
                        // 執行具體渲染邏輯 (此處可進行局部 Invalidate)
                        _updateAction(_dataSource);
                    }
                    catch (Exception ex)
                    {
                        // 異常隔離：防止個別控制項崩潰導致整個工控系統當機
                        System.Diagnostics.Debug.WriteLine($"[HFC UI Refresh Error]: {ex.Message}");
                        // 此處可接入工業日誌系統 (如 NLog/Serilog)
                    }
                }));
            }
        }

        /// <summary>
        /// 手動強制刷新 (用於特殊場景)
        /// </summary>
        public void ForceRefresh()
        {
            if (!_isDisposed && !_uiControl.IsDisposed && _uiControl.IsHandleCreated)
            {
                _uiControl.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        _updateAction(_dataSource);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[HFC ForceRefresh Error]: {ex.Message}");
                    }
                }));
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
            if (_dataSource != null) _dataSource.PropertyChanged -= OnDataSourcePropertyChanged;
            _hfcThrottleTimer?.Stop();
            _hfcThrottleTimer?.Dispose();
        }
    }

    /// <summary>
    /// 警報風暴抑制器 - 事件漏斗演算法實現
    /// 同節點同類型警報在指定時間視窗內僅顯示首次與計數
    /// </summary>
    public class AlarmStormSuppressor
    {
        private readonly ConcurrentDictionary<string, AlarmRecord> _alarmRecords;
        private readonly int _suppressionWindowMs;
        private readonly System.Windows.Forms.Timer _cleanupTimer;

        private class AlarmRecord
        {
            public DateTime FirstOccurrence { get; set; }
            public int Count { get; set; }
            public DateTime LastUpdate { get; set; }
        }

        /// <summary>
        /// 警報事件 (經過抑制後的真實警報)
        /// </summary>
        public event EventHandler<AlarmEventArgs> AlarmTriggered;

        public AlarmStormSuppressor(int suppressionWindowMs = 500)
        {
            _alarmRecords = new ConcurrentDictionary<string, AlarmRecord>();
            _suppressionWindowMs = suppressionWindowMs;

            // 定期清理過期的警報記錄
            _cleanupTimer = new System.Windows.Forms.Timer();
            _cleanupTimer.Interval = 1000; // 每秒清理一次
            _cleanupTimer.Tick += CleanupTimer_Tick;
            _cleanupTimer.Start();
        }

        /// <summary>
        /// 提交警報 (自動應用抑制邏輯)
        /// </summary>
        public void SubmitAlarm(string alarmKey, string message, AlarmLevel level)
        {
            var now = DateTime.Now;
            var record = _alarmRecords.GetOrAdd(alarmKey, k => new AlarmRecord
            {
                FirstOccurrence = now,
                Count = 0,
                LastUpdate = now
            });

            lock (record)
            {
                var timeSinceFirst = (now - record.FirstOccurrence).TotalMilliseconds;

                if (timeSinceFirst <= _suppressionWindowMs)
                {
                    // 在抑制視窗內，僅增加計數
                    record.Count++;
                    record.LastUpdate = now;
                }
                else
                {
                    // 超出視窗，觸發新警報並重置記錄
                    var finalCount = record.Count > 0 ? record.Count + 1 : 1;
                    AlarmTriggered?.Invoke(this, new AlarmEventArgs
                    {
                        AlarmKey = alarmKey,
                        Message = message,
                        Level = level,
                        OccurrenceCount = finalCount,
                        FirstOccurrence = record.FirstOccurrence
                    });

                    // 重置記錄
                    record.FirstOccurrence = now;
                    record.Count = 0;
                    record.LastUpdate = now;
                }
            }
        }

        private void CleanupTimer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var keysToRemove = new System.Collections.Generic.List<string>();

            foreach (var kvp in _alarmRecords)
            {
                if ((now - kvp.Value.LastUpdate).TotalMilliseconds > _suppressionWindowMs * 2)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _alarmRecords.TryRemove(key, out _);
            }
        }

        public void Dispose()
        {
            _cleanupTimer?.Stop();
            _cleanupTimer?.Dispose();
            _alarmRecords.Clear();
        }
    }

    /// <summary>
    /// 警報事件參數
    /// </summary>
    public class AlarmEventArgs : EventArgs
    {
        public string AlarmKey { get; set; }
        public string Message { get; set; }
        public AlarmLevel Level { get; set; }
        public int OccurrenceCount { get; set; }
        public DateTime FirstOccurrence { get; set; }
    }

    /// <summary>
    /// 警報等級定義 (與 Core.Models 保持一致)
    /// </summary>
    public enum AlarmLevel
    {
        Info = 0,      // 藍色 - 一般資訊
        Warning = 1,   // 黃色 - 警告
        Critical = 2,  // 橙色 - 嚴重警告
        Error = 3      // 紅色 - 錯誤/緊急停止
    }
}
