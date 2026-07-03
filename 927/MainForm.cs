using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using _927.ViewModels;

namespace _927
{
    /// <summary>
    /// WinForm 主窗口 - 從 WPF MainWindow 重構而來
    /// 保留所有原始 UI 功能，包含 Status Label、Start/Stop Buttons、TextBox
    /// </summary>
    public class MainForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private MainWindowViewModel? _viewModel;

        // UI Controls (對應原 XAML 中的控件)
        private Label? _statusLabel;
        private Button? _startButton;
        private Button? _stopButton;
        private TextBox? _textBox;  // 對應原 XAML 中的 _927tbx

        public MainForm(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _cancellationToken = cancellationToken;
            _logger = Log.ForContext<MainForm>();

            InitializeComponent();
            InitializeViewModel();
            InitializeUiBindings();

            _logger.Information("MainForm initialized");
        }

        /// <summary>
        /// 初始化 UI 組件（對應原 XAML 定義）
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form 基本設定（對應原 XAML: Title="MainWindow" Height="450" Width="800"）
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 600);
            this.Text = "Shoe Mold Control System - Industrial Edition";
            this.MinimumSize = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosing += MainForm_FormClosing;

            // Status Label（對應原 InitializeUi 中的 statusLabel，綁定 ViewModel.StatusText）
            _statusLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold),
                Location = new Point(20, 20),
                Name = "statusLabel",
                TabIndex = 0,
                Text = "Ready"
            };

            // Start Button（對應原 InitializeUi 中的 startButton）
            _startButton = new Button
            {
                Location = new Point(20, 60),
                Name = "startButton",
                Size = new Size(150, 40),
                TabIndex = 1,
                Text = "Start Production",
                UseVisualStyleBackColor = true,
                Font = new Font("Microsoft JhengHei UI", 10F)
            };
            _startButton.Click += StartButton_Click;

            // Stop Button（對應原 InitializeUi 中的 stopButton）
            _stopButton = new Button
            {
                Location = new Point(20, 110),
                Name = "stopButton",
                Size = new Size(150, 40),
                TabIndex = 2,
                Text = "Stop Production",
                UseVisualStyleBackColor = true,
                Font = new Font("Microsoft JhengHei UI", 10F),
                Enabled = false
            };
            _stopButton.Click += StopButton_Click;

            // TextBox（對應原 XAML 中的 _927tbx）
            // 原 XAML: <TextBox x:Name="_927tbx" TextWrapping="Wrap" Text="TextBox" Margin="60,33,481,226" Grid.ColumnSpan="2"/>
            _textBox = new TextBox
            {
                Location = new Point(60, 33),
                Name = "_927tbx",
                Size = new Size(200, 23),
                TabIndex = 3,
                Text = "TextBox",
                Multiline = false
            };

            // Add all controls to Form
            this.Controls.Add(_statusLabel);
            this.Controls.Add(_startButton);
            this.Controls.Add(_stopButton);
            this.Controls.Add(_textBox);

            this.ResumeLayout(false);
            this.PerformLayout();
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
                _viewModel.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(MainWindowViewModel.StatusText))
                    {
                        this.InvokeIfRequired(() =>
                        {
                            _statusLabel!.Text = _viewModel.StatusText;
                        });
                    }
                    else if (e.PropertyName == nameof(MainWindowViewModel.StartCommand) ||
                             e.PropertyName == nameof(MainWindowViewModel.StopCommand))
                    {
                        this.InvokeIfRequired(() =>
                        {
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
