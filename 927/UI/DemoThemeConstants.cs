using System.Drawing;

namespace Industrial.UI.Framework
{
    /// <summary>
    /// 從 927-Demo 提取的 UI 主題色彩常數
    /// 用於統一狀態顯示、警報提示與按鈕交互的視覺風格
    /// </summary>
    public static class DemoThemeConstants
    {
        #region 設備狀態色彩 (從 FormMain.SetOperationButtonStyle 提取)
        
        /// <summary>未初始化狀態 - 淺藍色背景</summary>
        public static readonly Color StatusNotInitialed = Color.LightSkyBlue;
        
        /// <summary>初始化中狀態 - 白色前景 (表示處理中)</summary>
        public static readonly Color StatusInitializing = Color.White;
        
        /// <summary>待機狀態 - 透明背景 (深色主題下為深灰色)</summary>
        public static readonly Color StatusIdle = Color.Transparent;
        
        /// <summary>自動運行狀態 - 深綠色背景</summary>
        public static readonly Color StatusAutoRunning = Color.SeaGreen;
        
        /// <summary>準備中狀態 - 禁用交互</summary>
        public static readonly Color StatusPreparing = Color.LightGray;
        
        #endregion

        #region 警報色彩 (從 FormMain.SetAlarmButtonStyle 提取)
        
        /// <summary>錯誤/嚴重警報 - 橘色警示</summary>
        public static readonly Color AlarmError = Color.Orange;
        
        /// <summary>警告等級 - 金黃色提示</summary>
        public static readonly Color AlarmWarning = Color.Gold;
        
        /// <summary>正常狀態 - 無警報時的透明背景</summary>
        public static readonly Color AlarmNormal = Color.Transparent;
        
        #endregion

        #region 按鈕交互色彩
        
        /// <summary>啟用按鈕的前景文字顏色</summary>
        public static readonly Color ButtonEnabled_Foreground = Color.White;
        
        /// <summary>禁用按鈕的背景顏色</summary>
        public static readonly Color ButtonDisabled_Background = Color.Transparent;
        
        /// <summary>禁用按鈕的前景文字顏色</summary>
        public static readonly Color ButtonDisabled_Foreground = Color.Black;
        
        /// <summary>初始按鈕的特殊配色</summary>
        public static readonly Color ButtonInitial_Background = Color.LightSkyBlue;
        
        /// <summary>自動運行按鈕的激活配色</summary>
        public static readonly Color ButtonAutoRun_Active = Color.SeaGreen;
        
        /// <summary>停止按鈕的激活配色</summary>
        public static readonly Color ButtonStop_Active = Color.DeepPink;
        
        /// <summary>停止按鈕的待命配色</summary>
        public static readonly Color ButtonStop_Standby = Color.HotPink;
        
        #endregion

        #region 927 既有深色主題色彩 (保持相容性)
        
        /// <summary>主背景色 - ISO 9241 抗疲勞深灰色</summary>
        public static readonly Color DarkBackground = Color.FromArgb(30, 30, 30);
        
        /// <summary>面板背景色</summary>
        public static readonly Color PanelBackground = Color.FromArgb(45, 45, 45);
        
        /// <summary>卡片背景色</summary>
        public static readonly Color CardBackground = Color.FromArgb(55, 55, 55);
        
        /// <summary>主要文字顏色</summary>
        public static readonly Color TextPrimary = Color.FromArgb(220, 220, 220);
        
        /// <summary>次要文字顏色</summary>
        public static readonly Color TextSecondary = Color.FromArgb(150, 150, 150);
        
        /// <summary>強調色 - 藍色</summary>
        public static readonly Color AccentColor = Color.FromArgb(64, 169, 255);
        
        /// <summary>成功強調色 - 綠色</summary>
        public static readonly Color AccentGreen = Color.FromArgb(39, 174, 96);
        
        /// <summary>Jog 按鈕顏色</summary>
        public static readonly Color JogButtonColor = Color.FromArgb(52, 152, 219);
        
        /// <summary>危險區域顏色</summary>
        public static readonly Color DangerZoneColor = Color.FromArgb(192, 57, 43);
        
        #endregion

        #region 輔助方法
        
        /// <summary>
        /// 根據設備狀態返回對應的背景顏色
        /// </summary>
        public static Color GetStatusBackgroundColor(EquipmentStatus status)
        {
            return status switch
            {
                EquipmentStatus.NotInitialed => StatusNotInitialed,
                EquipmentStatus.Initializing => StatusInitializing,
                EquipmentStatus.Idle => StatusIdle,
                EquipmentStatus.Preparing => StatusPreparing,
                EquipmentStatus.AutoRunning => StatusAutoRunning,
                _ => PanelBackground
            };
        }
        
        /// <summary>
        /// 根據警報等級返回對應的顏色
        /// </summary>
        public static Color GetAlarmColor(AlarmLevel level)
        {
            return level switch
            {
                AlarmLevel.Error => AlarmError,
                AlarmLevel.Warning => AlarmWarning,
                AlarmLevel.Critical => Color.Red,
                AlarmLevel.Info => AccentColor,
                _ => TextPrimary
            };
        }
        
        #endregion
    }

    /// <summary>
    /// 設備狀態列舉 (對標 927-Demo 的 EquipmentStatus)
    /// </summary>
    public enum EquipmentStatus
    {
        NotInitialed = 1,      // Eq01_NotInitialed
        Initializing = 2,      // Eq02_Initializing
        Idle = 3,              // Eq03_Idle
        Preparing = 4,         // Eq04_Preparing
        AutoRunning = 5        // Eq05_AutoRunning
    }

    /// <summary>
    /// 警報等級列舉 (擴充既有 AlarmLevel)
    /// </summary>
    public enum AlarmLevel
    {
        Info = 0,
        Warning = 1,
        Critical = 2,
        Error = 3
    }
}
