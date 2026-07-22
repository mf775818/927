using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace _927.UI
{
    /// <summary>
    /// 927-Demo UI 移植版 - 純淨視圖層
    /// 特點：設計時可渲染、運行時可注入、無第三方依賴
    /// </summary>
    public partial class DemoMainForm : Form
    {
        // 依賴注入 (可為 null，進入演示模式)
        private readonly IAvrRobotMotion? _robotService;
        private readonly IAvrVisionService? _visionService;
        
        // 狀態管理
        private enum SystemState { Idle, Initializing, Running, Stopped, Error }
        private SystemState _currentState = SystemState.Idle;
        
        // UI 節流 (防止高頻更新造成卡頓)
        private int _uiRefreshCounter = 0;
        private const int REFRESH_THRESHOLD = 5; // 每 5 次 timer 更新一次 UI (約 20Hz)

        public DemoMainForm(IAvrRobotMotion? robot = null, IAvrVisionService? vision = null)
        {
            InitializeComponent();
            
            _robotService = robot;
            _visionService = vision;
            
            // 雙緩衝設定 (解決閃爍)
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                         ControlStyles.OptimizedDoubleBuffer | 
                         ControlStyles.UserPaint, true);
            
            // 事件綁定
            BindEvents();
            UpdateUIState();
        }

        private void BindEvents()
        {
            btnInit.Click += BtnInit_Click;
            btnRun.Click += BtnRun_Click;
            btnStop.Click += BtnStop_Click;
            tmrStatus.Tick += TmrStatus_Tick;
            tmrUIRefresh.Tick += TmrUIRefresh_Tick;
            
            // 啟動計時器
            tmrStatus.Start();
            tmrUIRefresh.Start();
        }

        #region 按鈕事件處理 (移植自 927-Demo)

        private async void BtnInit_Click(object? sender, EventArgs e)
        {
            if (_currentState == SystemState.Initializing || _currentState == SystemState.Running)
                return;

            SetState(SystemState.Initializing);
            LogMessage("開始系統初始化...");

            try
            {
                if (_robotService != null)
                    await _robotService.InitializeAsync();
                else
                    await Task.Delay(1000); // 演示模式延遲

                if (_visionService != null)
                    await _visionService.ConnectAsync();
                else
                    await Task.Delay(500);

                SetState(SystemState.Idle);
                LogMessage("初始化完成", Color.LightGreen);
                dgvScanLog.Rows.Add(DateTime.Now.ToString("HH:mm:ss"), "OK", "0");
            }
            catch (Exception ex)
            {
                SetState(SystemState.Error);
                LogMessage($"初始化失敗：{ex.Message}", Color.Red);
                ShowAlarm("初始化錯誤", ex.Message, "Error");
            }
        }

        private async void BtnRun_Click(object? sender, EventArgs e)
        {
            if (_currentState != SystemState.Idle)
                return;

            SetState(SystemState.Running);
            LogMessage("開始自動運行...");

            // 模擬運行循環 (實際應由背景服務處理)
            await Task.Run(async () =>
            {
                while (_currentState == SystemState.Running)
                {
                    // 模擬掃描週期
                    var cycleTime = new Random().Next(800, 1200);
                    await Task.Delay(cycleTime);
                    
                    // 更新 UI (透過 Invoke)
                    this.Invoke(() =>
                    {
                        dgvScanLog.Rows.Insert(0, 
                            DateTime.Now.ToString("HH:mm:ss"), 
                            "PASS", 
                            cycleTime.ToString());
                        
                        // 維持只顯示最近 50 筆
                        if (dgvScanLog.Rows.Count > 50)
                            dgvScanLog.Rows.RemoveAt(49);
                            
                        lblCycleTime.Text = $"CT: {cycleTime}ms";
                    });
                }
            });
        }

        private void BtnStop_Click(object? sender, EventArgs e)
        {
            if (_currentState == SystemState.Stopped || _currentState == SystemState.Idle)
                return;

            SetState(SystemState.Stopped);
            LogMessage("緊急停止觸發", Color.Orange);
            ShowAlarm("緊急停止", "操作員手動停止", "Warning");
        }

        #endregion

        #region 計時器事件 (UI 更新)

        private void TmrStatus_Tick(object? sender, EventArgs e)
        {
            // 每 500ms 檢查系統狀態 (可對接真實硬體狀態)
            if (_robotService != null)
            {
                // TODO: 讀取真實狀態
            }
        }

        private void TmrUIRefresh_Tick(object? sender, EventArgs e)
        {
            _uiRefreshCounter++;
            if (_uiRefreshCounter % REFRESH_THRESHOLD != 0)
                return;

            // 模擬狀態指示器動畫
            if (_currentState == SystemState.Running)
            {
                pnlStatusIndicator.BackColor = 
                    pnlStatusIndicator.BackColor == Color.Lime ? Color.Green : Color.Lime;
            }
            else if (_currentState == SystemState.Error)
            {
                pnlStatusIndicator.BackColor = Color.Red;
            }
            else
            {
                pnlStatusIndicator.BackColor = Color.Gray;
            }
        }

        #endregion

        #region 輔助方法

        private void SetState(SystemState newState)
        {
            _currentState = newState;
            UpdateUIState();
        }

        private void UpdateUIState()
        {
            lblStatusTitle.Text = $"系統狀態：{GetStateName(_currentState)}";
            
            // 按鈕狀態機
            btnInit.Enabled = _currentState == SystemState.Idle || _currentState == SystemState.Stopped;
            btnRun.Enabled = _currentState == SystemState.Idle;
            btnStop.Enabled = _currentState == SystemState.Running || _currentState == SystemState.Initializing;

            // 色彩反饋
            btnInit.BackColor = _currentState == SystemState.Idle ? Color.LightSkyBlue : Color.Gray;
            btnRun.BackColor = _currentState == SystemState.Idle ? Color.LightSeaGreen : Color.Gray;
            btnStop.BackColor = _currentState == SystemState.Running ? Color.Orange : Color.Gray;
        }

        private string GetStateName(SystemState state) => state switch
        {
            SystemState.Idle => "閒置",
            SystemState.Initializing => "初始化中",
            SystemState.Running => "運行中",
            SystemState.Stopped => "已停止",
            SystemState.Error => "錯誤",
            _ => "未知"
        };

        private void LogMessage(string message, Color? color = null)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var logLine = $"[{timestamp}] {message}\r\n";
            
            txtLogOutput.AppendText(logLine);
            txtLogOutput.ScrollToCaret();
        }

        private void ShowAlarm(string title, string message, string level)
        {
            dgvAlarm.Rows.Insert(0, DateTime.Now.ToString("HH:mm:ss"), level, $"{title}: {message}");
            
            // 切換到警報頁籤 (如果是嚴重錯誤)
            if (level == "Error" && tabControlMain.SelectedTab != tabPageAlarm)
            {
                tabControlMain.SelectedTab = tabPageAlarm;
                tabPageAlarm.BackColor = Color.LightCoral; // 高亮提示
                Task.Delay(2000).ContinueWith(_ => 
                    this.Invoke(() => tabPageAlarm.BackColor = SystemColors.Control));
            }
        }

        #endregion
    }
    
    // ============================================================
    // 介面定義 (若 927 核心尚未定義這些介面，使用此處的版本)
    // ============================================================
    
    /// <summary>
    /// 機器人運動控制介面
    /// </summary>
    public interface IAvrRobotMotion
    {
        Task InitializeAsync();
        Task MoveToAsync(double x, double y, double z);
        Task StopAsync();
        bool IsConnected { get; }
    }

    /// <summary>
    /// 視覺服務介面
    /// </summary>
    public interface IAvrVisionService
    {
        Task ConnectAsync();
        Task DisconnectAsync();
        Task<byte[]?> CaptureImageAsync();
        bool IsConnected { get; }
    }
}
