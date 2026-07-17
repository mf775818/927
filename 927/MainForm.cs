using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using _927.ViewModels;
using ShoeMoldControl.Core;
using ShoeMoldControl.Core.Models;
using ShoeMoldControl.Core.Hardware;
using Industrial.UI.Framework;

namespace _927
{
    /// <summary>
    /// 工業級 WinForm 主窗口 - 三分頁式專業工控介面
    /// 
    /// 【分頁架構】
    /// 1. Robot+視覺監控分頁 (面向使用者) - 狀態監視、警報摘要、生產統計
    /// 2. Robot 工程師操作分頁 (危險指令) - Jog 手動控制、座標顯示、命令執行
    /// 3. 視覺虛實整合分頁 (模擬/實際) - 影像擷取、檢測分析、參數設定
    /// 
    /// 【核心特性】
    /// - HFC/HPC 高頻數據更新架構 (33ms 節流)
    /// - 警報風暴抑制系統 (500ms 視窗)
    /// - 虛實整合（模擬模式生成虛擬數據，實際模式串接硬體）
    /// - IndustrialSafeButton 防呆設計（長按確認）
    /// - ISO 9241 compliant 深色主題配色
    /// - 完整的異步操作與取消令牌管理
    /// 
    /// 【符合規範】
    /// - ISO 9241 人因工程標準
    /// - SEMI 設備自動化規範
    /// - IEC 61131-3 工控軟體設計準則
    /// </summary>
    public partial class MainForm : Form, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private readonly IConnectionStateManager _connectionStateManager;
        private readonly IRobotController _robotController;
        private readonly IVisionService _visionService;
        private MainWindowViewModel? _viewModel;
        
        // HFC/HPC 核心組件
        private HighPerformanceUiRefresher<MachineDataModel>? _dataRefresher;
        private AlarmStormSuppressor? _alarmSuppressor;
        private MachineDataModel? _machineDataModel;
        
        // Robot 狀態快取
        private RobotMode _cachedRobotMode = RobotMode.INIT;
        private int _cachedCommandId = -1;
        private RobotCoordinatePose? _cachedRobotPose;
        
        // 視覺系統狀態
        private VisionSystemMode _visionSystemMode = VisionSystemMode.Simulation;
        private Avl.Image? _lastCapturedImage;
        private VisionInspectionResult? _lastInspectionResult;
        private bool _isContinuousCaptureRunning = false;
        private CancellationTokenSource? _continuousCaptureCts;
        
        // Jog 操作狀態
        private bool _isJogActive = false;
        private JogType _currentJogType = JogType.JOG_PLUS_X;
        private Dictionary<JogType, Button> _jogButtons = new();
        
        // 深色主題配色定義 (符合 ISO 9241 抗疲勞標準)
        private static readonly Color DarkBackground = Color.FromArgb(30, 30, 30);
        private static readonly Color PanelBackground = Color.FromArgb(45, 45, 45);
        private static readonly Color CardBackground = Color.FromArgb(55, 55, 55);
        private static readonly Color TextPrimary = Color.FromArgb(220, 220, 220);
        private static readonly Color TextSecondary = Color.FromArgb(150, 150, 150);
        private static readonly Color AccentColor = Color.FromArgb(64, 169, 255);
        private static readonly Color AccentGreen = Color.FromArgb(39, 174, 96);
        private static readonly Color AlarmWarning = Color.FromArgb(243, 156, 18);
        private static readonly Color AlarmError = Color.FromArgb(231, 76, 60);
        private static readonly Color StatusRunning = Color.FromArgb(39, 174, 96);
        private static readonly Color StatusIdle = Color.FromArgb(150, 150, 150);
        private static readonly Color StatusError = Color.FromArgb(231, 76, 60);
        private static readonly Color JogButtonColor = Color.FromArgb(52, 152, 219);
        private static readonly Color DangerZoneColor = Color.FromArgb(192, 57, 43);
        
        // 夜班模式標誌
        private bool _isNightMode = false;

        public MainForm(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _cancellationToken = cancellationToken;
            _logger = Log.ForContext<MainForm>();
            
            _connectionStateManager = serviceProvider.GetService<IConnectionStateManager>() 
                ?? new ConnectionStateManager(null);
            _robotController = serviceProvider.GetService<IRobotController>() ?? new MockRobotController();
            _visionService = serviceProvider.GetService<IVisionService>() ?? new MockVisionService();

            InitializeComponent();
            ApplyIndustrialTheme();
            InitializeDataModels();
            InitializeViewModel();
            InitializeHfcComponents();
            InitializeUiBindings();
            InitializeAlarmSystem();
            InitializeDashboardPage();      // 分頁 1: Robot+視覺監控
            InitializeRobotEngineerPage();  // 分頁 2: Robot 工程師
            InitializeVisionIntegrationPage(); // 分頁 3: 視覺虛實整合
            StartStatusPolling();
            
            _connectionStateManager.ConnectionStatusChanged += UpdateConnectionStatusDisplay;
            UpdateConnectionStatusDisplay();
            
            _logger.Information("Industrial MainForm initialized with 3-tab architecture (HFC/HPC)");
        }

        /// <summary>
        /// 初始化數據模型層 (HPC Layer)
        /// </summary>
        private void InitializeDataModels()
        {
            _machineDataModel = new MachineDataModel
            {
                CurrentStatus = "待機中",
                IsRunning = false,
                ProductionCount = 0,
                Temperature = 25.0,
                Pressure = 0.0,
                AlarmCode = 0,
                LastUpdateTime = DateTime.Now
            };
        }

        /// <summary>
        /// 初始化 HFC/HPC 刷新機制
        /// </summary>
        private void InitializeHfcComponents()
        {
            // 33ms = 約 30 FPS，平衡流暢度與效能
            _dataRefresher = new HighPerformanceUiRefresher<MachineDataModel>(
                _machineDataModel,
                this,
                UpdateDashboardFromModel,
                throttleIntervalMs: 33
            );
        }

        /// <summary>
        /// 初始化警報風暴抑制系統
        /// </summary>
        private void InitializeAlarmSystem()
        {
            _alarmSuppressor = new AlarmStormSuppressor(suppressionWindowMs: 500);
            _alarmSuppressor.AlarmTriggered += OnAlarmTriggered;
        }

        /// <summary>
        /// 從數據模型更新儀表板 UI (僅在 UI 執行緒執行)
        /// </summary>
        private void UpdateDashboardFromModel(MachineDataModel model)
        {
            if (_robotVisionMonitorPage == null || _lblTemperature == null) return;

            try
            {
                // 局部更新，避免全螢幕重繪
                _lblTemperature.Text = $"{model.Temperature:F2} °C";
                _lblPressure.Text = $"{model.Pressure:F3} MPa";
                _lblProductionCountValue.Text = model.ProductionCount.ToString("N0");
                _lblOverallStatusValue.Text = model.CurrentStatus;
                _lblLastUpdateTimeValue.Text = model.LastUpdateTime.ToString("HH:mm:ss.fff");

                // 狀態色彩編碼
                if (model.IsRunning)
                {
                    _lblStatus.ForeColor = Color.FromArgb(39, 174, 96); // 運行綠
                }
                else
                {
                    _lblStatus.ForeColor = TextSecondary;
                }

                // 警報狀態顯示
                if (model.AlarmCode != 0)
                {
                    _lblAlarmCode.Text = $"ALM-{model.AlarmCode:D3}";
                    _lblAlarmCode.ForeColor = AlarmError;
                    _lblAlarmCode.Visible = true;
                }
                else
                {
                    _lblAlarmCode.Visible = false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating dashboard from model");
            }
        }

        /// <summary>
        /// 處理經過抑制的警報事件
        /// </summary>
        private void OnAlarmTriggered(object? sender, AlarmEventArgs e)
        {
            this.InvokeIfRequired(() =>
            {
                var alarmItem = new ListViewItem(new[]
                {
                    e.FirstOccurrence.ToString("HH:mm:ss"),
                    e.Message,
                    e.OccurrenceCount > 1 ? $"x{e.OccurrenceCount}" : "",
                    GetAlarmLevelText(e.Level)
                });

                switch (e.Level)
                {
                    case AlarmLevel.Error:
                        alarmItem.ForeColor = AlarmError;
                        break;
                    case AlarmLevel.Critical:
                        alarmItem.ForeColor = AlarmWarning;
                        break;
                    case AlarmLevel.Warning:
                        alarmItem.ForeColor = Color.Yellow;
                        break;
                    default:
                        alarmItem.ForeColor = TextPrimary;
                }

                _alarmListView.Items.Insert(0, alarmItem);

                // 限制歷史記錄數量，避免記憶體洩漏
                while (_alarmListView.Items.Count > 100)
                {
                    _alarmListView.Items.RemoveAt(_alarmListView.Items.Count - 1);
                }

                // 閃爍提示
                FlashAlarmIndicator(e.Level);
            });
        }

        private string GetAlarmLevelText(AlarmLevel level)
        {
            return level switch
            {
                AlarmLevel.Info => "資訊",
                AlarmLevel.Warning => "警告",
                AlarmLevel.Critical => "嚴重",
                AlarmLevel.Error => "錯誤",
                _ => "未知"
            };
        }

        private void FlashAlarmIndicator(AlarmLevel level)
        {
            var originalColor = _alarmIndicatorLabel.BackColor;
            var flashColor = level == AlarmLevel.Error ? AlarmError : AlarmWarning;
            
            _alarmIndicatorLabel.BackColor = flashColor;
            
            Task.Delay(500).ContinueWith(t =>
            {
                if (!_alarmIndicatorLabel.IsDisposed && _alarmIndicatorLabel.IsHandleCreated)
                {
                    _alarmIndicatorLabel.Invoke(new Action(() =>
                        _alarmIndicatorLabel.BackColor = originalColor
                    ));
                }
            });
        }

        /// <summary>
        /// 初始化 ViewModel（保留 MVVM 模式）
        /// </summary>
        private void InitializeViewModel()
        {
            _viewModel = new MainWindowViewModel(_serviceProvider, _cancellationToken, _logger);
        }

        /// <summary>
        /// 初始化 UI 綁定（將 ViewModel 屬性綁定到 UI 控件）
        /// </summary>
        private void InitializeUiBindings()
        {
            if (_viewModel != null)
            {
                // 綁定 ViewModel.StatusText => Label.Text
                _viewModel.PropertyChanged += (sender, e) => {
                    if (e.PropertyName == nameof(MainWindowViewModel.StatusText))
                    {
                        this.InvokeIfRequired(() => {
                            _statusLabel!.Text = _viewModel.StatusText;
                        });
                    }
                    else if (e.PropertyName == nameof(MainWindowViewModel.StartCommand) ||
                             e.PropertyName == nameof(MainWindowViewModel.StopCommand))
                    {
                        this.InvokeIfRequired(() => {
                            UpdateButtonStates();
                        });
                    }
                };

                // 訂閱 Command 的 CanExecuteChanged 事件
                _viewModel.StartCommand.CanExecuteChanged += (s, e) => UpdateButtonStates();
                _viewModel.StopCommand.CanExecuteChanged += (s, e) => UpdateButtonStates();

                // 初始更新按鈕狀態
                UpdateButtonStates();
            }
        }

        // 已移除定時器初始化；改以 ConnectionStatusChanged 事件驅動

        /// <summary>
        /// 更新連接狀態顯示
        /// </summary>
        private void UpdateConnectionStatusDisplay()
        {
            if (_connectionStateManager == null) return;

            var statusSummary = _connectionStateManager.GetConnectionStatusSummary();
            
            this.InvokeIfRequired(() => {
                // 更新 Title 顯示連接狀態
                this.Text = $"Shoe Mold Control System - {statusSummary}";
                
                // 根據連接狀態改變背景色提示
                if (_connectionStateManager.IsSimulationMode)
                {
                    _simulationIndicatorLabel!.BackColor = Color.LightGreen;
                    _simulationIndicatorLabel!.Text = "SIMULATION MODE";
                    _simulationIndicatorLabel!.Visible = true;
                }
                else
                {
                    bool allConnected = _connectionStateManager.IsVisionConnected && _connectionStateManager.IsRobotConnected;
                    _simulationIndicatorLabel!.BackColor = allConnected ? Color.LightGreen : Color.Orange;
                    _simulationIndicatorLabel!.Text = allConnected ? "ALL CONNECTED" : "CONNECTION LOST";
                    _simulationIndicatorLabel!.Visible = true;
                }
            });
        }

        // Overload to allow subscription to ConnectionStatusChanged
        private void UpdateConnectionStatusDisplay(object? sender, EventArgs e) => UpdateConnectionStatusDisplay();

        /// <summary>
        /// 應用工業級深色主題 (符合 ISO 9241 抗疲勞標準)
        /// </summary>
        private void ApplyIndustrialTheme()
        {
            this.BackColor = DarkBackground;
            this.ForeColor = TextPrimary;
            
            // 夜班模式預設關閉，可透過按鈕切換
            _isNightMode = false;
        }

        /// <summary>
        /// 切換夜班模式 (低應力配色)
        /// </summary>
        public void ToggleNightMode()
        {
            _isNightMode = !_isNightMode;
            
            if (_isNightMode)
            {
                // 夜班模式：降低亮度，提高對比
                this.BackColor = Color.FromArgb(20, 20, 20);
                _robotVisionMonitorPage.BackColor = Color.FromArgb(25, 25, 25);
                _robotEngineerPage.BackColor = Color.FromArgb(25, 25, 25);
                _visionIntegrationPage.BackColor = Color.FromArgb(25, 25, 25);
            }
            else
            {
                // 正常模式
                this.BackColor = DarkBackground;
                _robotVisionMonitorPage.BackColor = PanelBackground;
                _robotEngineerPage.BackColor = PanelBackground;
                _visionIntegrationPage.BackColor = PanelBackground;
            }
            
            // 遞歸更新所有子控制項
            UpdateControlTheme(this.Controls);
        }

        private void UpdateControlTheme(Control.ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl is Panel || ctrl is TabPage)
                {
                    ctrl.BackColor = _isNightMode ? Color.FromArgb(25, 25, 25) : PanelBackground;
                    ctrl.ForeColor = TextPrimary;
                }
                else if (ctrl is Label lbl)
                {
                    lbl.ForeColor = _isNightMode ? Color.FromArgb(180, 180, 180) : TextPrimary;
                }
                else if (ctrl is TextBox tb)
                {
                    tb.BackColor = _isNightMode ? Color.FromArgb(35, 35, 35) : Color.FromArgb(55, 55, 55);
                    tb.ForeColor = TextPrimary;
                }
                
                // 遞歸處理嵌套控制項
                if (ctrl.HasChildren)
                {
                    UpdateControlTheme(ctrl.Controls);
                }
            }
        }

        /// <summary>
        /// 模擬高頻數據更新 (用於測試 HFC 機制)
        /// 實際應用中由 PLC/Modbus 通訊驅動
        /// </summary>
        public void StartDataSimulation()
        {
            Task.Run(async () =>
            {
                var random = new Random();
                while (!_cancellationToken.IsCancellationRequested)
                {
                    if (_machineDataModel != null && _machineDataModel.IsRunning)
                    {
                        _machineDataModel.SimulateHighFrequencyDataUpdate(random);
                        
                        // 模擬偶發警報
                        if (random.NextDouble() < 0.01) // 1% 機率觸發警報
                        {
                            int alarmCode = random.Next(100, 999);
                            _machineDataModel.AlarmCode = alarmCode;
                            
                            _alarmSuppressor?.SubmitAlarm(
                                $"ALM-{alarmCode}",
                                $"模擬警報 {alarmCode}: 參數異常",
                                AlarmLevel.Warning
                            );
                        }
                        else
                        {
                            _machineDataModel.AlarmCode = 0;
                        }
                    }
                    
                    await Task.Delay(10, _cancellationToken); // 100Hz 數據更新頻率
                }
            }, _cancellationToken);
        }

        /// <summary>
        /// 更新按鈕啟用狀態（根據 Command 的 CanExecute）
        /// </summary>
        private void UpdateButtonStates()
        {
            if (_viewModel != null)
            {
                _startButton!.Enabled = _viewModel.StartCommand.CanExecute(null);
                _stopButton!.Enabled = _viewModel.StopCommand.CanExecute(null);
            }
        }

        /// <summary>
        /// Start Button Click Event Handler
        /// </summary>
        private void StartButton_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_viewModel?.StartCommand.CanExecute(null) == true)
                {
                    // 在啟動前更新連接狀態
                    UpdateConnectionStatusDisplay();
                    
                    _viewModel.StartCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in StartButton_Click");
                MessageBox.Show(this, $"啟動失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Stop Button Click Event Handler
        /// </summary>
        private void StopButton_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_viewModel?.StopCommand.CanExecute(null) == true)
                {
                    _viewModel.StopCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in StopButton_Click");
                MessageBox.Show(this, $"停止失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Form Closing Event Handler
        /// </summary>
        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _logger.Information("Form closing, requesting cancellation...");
            
            // 清理 HFC/HPC 組件
            _dataRefresher?.Dispose();
            _alarmSuppressor?.Dispose();
            _machineDataModel?.Dispose();
            
            // 取消訂閱連線狀態事件
            _connectionStateManager.ConnectionStatusChanged -= UpdateConnectionStatusDisplay;
            
            // ViewModel 會透過 CancellationToken 處理取消邏輯
        }

        /// <summary>
        /// Thread-safe UI update helper method
        /// </summary>
        private void InvokeIfRequired(Action action)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// IDisposable 實現
        /// </summary>
        public new void Dispose()
        {
            _dataRefresher?.Dispose();
            _alarmSuppressor?.Dispose();
            _machineDataModel?.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// 夜班模式切換事件處理器
        /// </summary>
        private void BtnNightMode_SafeClick(object? sender, EventArgs e)
        {
            ToggleNightMode();
            _logger.Information($"Night mode toggled: {_isNightMode}");
        }

        /// <summary>
        /// 啟動數據模擬按鈕事件
        /// </summary>
        private void BtnStartSimulation_Click(object? sender, EventArgs e)
        {
            if (_machineDataModel != null)
            {
                _machineDataModel.IsRunning = !_machineDataModel.IsRunning;
                _btnStartSimulation.Text = _machineDataModel.IsRunning ? "⏸ 停止模擬" : "▶ 啟動數據模擬";
                
                if (_machineDataModel.IsRunning)
                {
                    StartDataSimulation();
                    _logger.Information("Data simulation started");
                }
                else
                {
                    _logger.Information("Data simulation stopped");
                }
            }
        }
    }
}
