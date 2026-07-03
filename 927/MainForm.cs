using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using _927.ViewModels;
using ShoeMoldControl.Core;

namespace _927
{
    /// <summary>
    /// WinForm 主窗口 - 從 WPF MainWindow 重構而來
    /// 保留所有原始 UI 功能，包含 Status Label、Start/Stop Buttons、TextBox
    /// 整合連接狀態顯示及模擬模式指示器
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private readonly IConnectionStateManager _connectionStateManager;
        private MainWindowViewModel? _viewModel;
        // 改為事件驅動，不再使用定時器

        public MainForm(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _cancellationToken = cancellationToken;
            _logger = Log.ForContext<MainForm>();
            
            // 獲取連接狀態管理器
            _connectionStateManager = serviceProvider.GetService<IConnectionStateManager>() 
                ?? new ConnectionStateManager(null);

            InitializeComponent();
            InitializeViewModel();
            InitializeUiBindings();
            // 訂閱連線狀態管理器的狀態變更事件以驅動 UI 更新
            _connectionStateManager.ConnectionStatusChanged += UpdateConnectionStatusDisplay;
            // 立即執行一次以初始化 UI
            UpdateConnectionStatusDisplay();

            _logger.Information("MainForm initialized");
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
    }
}
