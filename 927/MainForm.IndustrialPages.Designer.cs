using System;
using System.Drawing;
using System.Windows.Forms;
using Industrial.UI.Framework;

namespace _927
{
    partial class MainForm
    {
        /// <summary>
        /// 初始化分頁 1: Robot+視覺監控分頁 (面向使用者)
        /// 包含：Robot 狀態卡、視覺狀態卡、系統總覽、警報摘要
        /// </summary>
        private void InitializeDashboardPage()
        {
            // ========== Robot 狀態卡 ==========
            _pnlRobotStatusCard = new Panel
            {
                Dock = DockStyle.Top,
                Height = 180,
                BackColor = CardBackground,
                Padding = new Padding(20),
                Margin = new Padding(15, 15, 15, 10)
            };

            _lblRobotTitle = new Label
            {
                Text = "🤖 Robot 系統狀態",
                Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            _lblRobotModeLabel = new Label
            {
                Text = "運行模式:",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 55)
            };

            _lblRobotModeValue = new Label
            {
                Text = "INIT",
                Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold),
                ForeColor = AccentColor,
                AutoSize = true,
                Location = new Point(120, 55)
            };

            _lblRobotCommandIdLabel = new Label
            {
                Text = "命令 ID:",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 90)
            };

            _lblRobotCommandIdValue = new Label
            {
                Text = "-1",
                Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(120, 90)
            };

            _lblRobotConnectionLabel = new Label
            {
                Text = "連接狀態:",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 125)
            };

            _lblRobotConnectionStatus = new Label
            {
                Text = "未連接",
                Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold),
                ForeColor = StatusError,
                AutoSize = true,
                Location = new Point(120, 125)
            };

            _pnlRobotStatusCard.Controls.AddRange(new Control[] {
                _lblRobotTitle, _lblRobotModeLabel, _lblRobotModeValue,
                _lblRobotCommandIdLabel, _lblRobotCommandIdValue,
                _lblRobotConnectionLabel, _lblRobotConnectionStatus
            });

            // ========== 視覺狀態卡 ==========
            _pnlVisionStatusCard = new Panel
            {
                Dock = DockStyle.Top,
                Height = 180,
                BackColor = CardBackground,
                Padding = new Padding(20),
                Margin = new Padding(15, 10, 15, 10)
            };

            _lblVisionTitle = new Label
            {
                Text = "👁️ 視覺系統狀態",
                Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            _lblVisionModeLabel = new Label
            {
                Text = "運行模式:",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 55)
            };

            _lblVisionModeValue = new Label
            {
                Text = "Simulation",
                Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold),
                ForeColor = AccentColor,
                AutoSize = true,
                Location = new Point(120, 55)
            };

            _lblLastBarcodeLabel = new Label
            {
                Text = "最後條碼:",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 90)
            };

            _lblLastBarcodeValue = new Label
            {
                Text = "N/A",
                Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(120, 90)
            };

            _lblVisionConnectionLabel = new Label
            {
                Text = "連接狀態:",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 125)
            };

            _lblVisionConnectionStatus = new Label
            {
                Text = "未連接",
                Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold),
                ForeColor = StatusError,
                AutoSize = true,
                Location = new Point(120, 125)
            };

            _pnlVisionStatusCard.Controls.AddRange(new Control[] {
                _lblVisionTitle, _lblVisionModeLabel, _lblVisionModeValue,
                _lblLastBarcodeLabel, _lblLastBarcodeValue,
                _lblVisionConnectionLabel, _lblVisionConnectionStatus
            });

            // ========== 系統總覽卡 ==========
            _pnlSystemOverviewCard = new Panel
            {
                Dock = DockStyle.Top,
                Height = 220,
                BackColor = CardBackground,
                Padding = new Padding(20),
                Margin = new Padding(15, 10, 15, 10)
            };

            _lblSystemStatusTitle = new Label
            {
                Text = "📊 系統總覽",
                Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            _lblOverallStatusLabel = new Label
            {
                Text = "整體狀態:",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 55)
            };

            _lblOverallStatusValue = new Label
            {
                Text = "待機中",
                Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold),
                ForeColor = StatusIdle,
                AutoSize = true,
                Location = new Point(120, 55)
            };

            _lblProductionCountLabel = new Label
            {
                Text = "生產計數:",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 95)
            };

            _lblProductionCountValue = new Label
            {
                Text = "0",
                Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold),
                ForeColor = AccentGreen,
                AutoSize = true,
                Location = new Point(120, 95)
            };

            _lblLastUpdateTimeLabel = new Label
            {
                Text = "最後更新:",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 135)
            };

            _lblLastUpdateTimeValue = new Label
            {
                Text = "--:--:--",
                Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(120, 135)
            };

            _lblTemperatureLabel = new Label
            {
                Text = "溫度:",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(300, 55)
            };

            _lblTemperature = new Label
            {
                Text = "25.00 °C",
                Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(360, 55)
            };

            _lblPressureLabel = new Label
            {
                Text = "壓力:",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(300, 95)
            };

            _lblPressure = new Label
            {
                Text = "0.000 MPa",
                Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(360, 95)
            };

            _pnlSystemOverviewCard.Controls.AddRange(new Control[] {
                _lblSystemStatusTitle, _lblOverallStatusLabel, _lblOverallStatusValue,
                _lblProductionCountLabel, _lblProductionCountValue,
                _lblLastUpdateTimeLabel, _lblLastUpdateTimeValue,
                _lblTemperatureLabel, _lblTemperature,
                _lblPressureLabel, _lblPressure
            });

            // ========== 警報摘要卡 ==========
            _pnlAlarmSummaryCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = CardBackground,
                Padding = new Padding(20),
                Margin = new Padding(15, 10, 15, 15)
            };

            _lblAlarmTitle = new Label
            {
                Text = "⚠️ 警報摘要 (最近 10 筆)",
                Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            _lblActiveAlarmLabel = new Label
            {
                Text = "當前警報:",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 50)
            };

            _lblActiveAlarmValue = new Label
            {
                Text = "無",
                Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold),
                ForeColor = AccentGreen,
                AutoSize = true,
                Location = new Point(110, 50)
            };

            _alarmListViewMonitor = new ListView
            {
                Location = new Point(20, 80),
                Size = new Size(_pnlAlarmSummaryCard.Width - 40, _pnlAlarmSummaryCard.Height - 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = TextPrimary,
                Font = new Font("Microsoft JhengHei UI", 10F)
            };

            _colAlarmTime = new ColumnHeader { Text = "時間", Width = 120 };
            _colAlarmMessage = new ColumnHeader { Text = "警報訊息", Width = 400 };
            _colAlarmCount = new ColumnHeader { Text = "次數", Width = 80 };
            _colAlarmLevel = new ColumnHeader { Text = "等級", Width = 100 };

            _alarmListViewMonitor.Columns.AddRange(new ColumnHeader[] {
                _colAlarmTime, _colAlarmMessage, _colAlarmCount, _colAlarmLevel
            });

            _pnlAlarmSummaryCard.Controls.AddRange(new Control[] {
                _lblAlarmTitle, _lblActiveAlarmLabel, _lblActiveAlarmValue, _alarmListViewMonitor
            });

            // 組裝分頁
            _robotVisionMonitorPage.Controls.Add(_pnlAlarmSummaryCard);
            _robotVisionMonitorPage.Controls.Add(_pnlSystemOverviewCard);
            _robotVisionMonitorPage.Controls.Add(_pnlVisionStatusCard);
            _robotVisionMonitorPage.Controls.Add(_pnlRobotStatusCard);
        }

        /// <summary>
        /// 初始化分頁 2: Robot 工程師操作分頁 (危險指令區)
        /// 包含：安全警告、連接控制、Jog 手動控制面板、座標顯示、命令執行
        /// </summary>
        private void InitializeRobotEngineerPage()
        {
            var mainPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = PanelBackground,
                Padding = new Padding(15)
            };

            // ========== 安全警告面板 ==========
            _pnlSafetyWarningPanel = new Panel
            {
                Width = mainPanel.Width - 30,
                Height = 80,
                BackColor = DangerZoneColor,
                Padding = new Padding(20),
                Margin = new Padding(0, 0, 0, 15)
            };

            _lblSafetyWarningIcon = new Label
            {
                Text = "⚠️",
                Font = new Font("Segoe UI Emoji", 24F),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            _lblSafetyWarning = new Label
            {
                Text = "危險操作區域！僅限授權工程師操作。請確保急停按鈕可用，並注意人員安全。",
                Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(70, 25)
            };

            _pnlSafetyWarningPanel.Controls.AddRange(new Control[] { _lblSafetyWarningIcon, _lblSafetyWarning });
            mainPanel.Controls.Add(_pnlSafetyWarningPanel);

            // ========== Robot 連接控制面板 ==========
            _pnlRobotConnectionPanel = new Panel
            {
                Width = mainPanel.Width - 30,
                Height = 100,
                BackColor = CardBackground,
                Padding = new Padding(20),
                Margin = new Padding(0, 0, 0, 15)
            };

            _lblRobotConnectTitle = new Label
            {
                Text = "🔌 Robot 連接控制",
                Font = new Font("Microsoft JhengHei UI", 13F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            _btnRobotConnect = new IndustrialSafeButton
            {
                Text = "🔗 連接 Robot",
                RequireLongPress = true,
                TargetHoldTimeMs = 2000,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 2 },
                Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold),
                ForeColor = TextPrimary,
                BackColor = AccentGreen,
                Location = new Point(20, 50),
                Size = new Size(140, 40)
            };
            _btnRobotConnect.SafeClick += BtnRobotConnect_SafeClick;

            _btnRobotDisconnect = new Button
            {
                Text = "❌ 斷開連接",
                Enabled = false,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 2 },
                Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold),
                ForeColor = TextPrimary,
                BackColor = AlarmError,
                Location = new Point(180, 50),
                Size = new Size(140, 40)
            };
            _btnRobotDisconnect.Click += BtnRobotDisconnect_Click;

            _lblRobotConnectionState = new Label
            {
                Text = "狀態：未連接",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(350, 55)
            };

            _pnlRobotConnectionPanel.Controls.AddRange(new Control[] {
                _lblRobotConnectTitle, _btnRobotConnect, _btnRobotDisconnect, _lblRobotConnectionState
            });
            mainPanel.Controls.Add(_pnlRobotConnectionPanel);

            // ========== Jog 手動控制面板 ==========
            _pnlJogControlPanel = new Panel
            {
                Width = mainPanel.Width - 30,
                Height = 320,
                BackColor = CardBackground,
                Padding = new Padding(20),
                Margin = new Padding(0, 0, 0, 15)
            };

            _lblJogTitle = new Label
            {
                Text = "🎮 Jog 手動控制",
                Font = new Font("Microsoft JhengHei UI", 13F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            _lblJogWarning = new Label
            {
                Text = "⚠️ 長按移動，鬆開即停",
                Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Italic),
                ForeColor = AlarmWarning,
                AutoSize = true,
                Location = new Point(20, 45)
            };

            _pnlJogButtons = new FlowLayoutPanel
            {
                Location = new Point(20, 75),
                Size = new Size(400, 200),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent
            };

            // Jog 按鈕佈局 (8 方向 + 停止)
            var jogBtnSize = new Size(80, 45);
            var jogBtnSpacing = 10;

            // 第一排：+X, +Y
            _btnJogPlusX = CreateJogButton(JogType.JOG_PLUS_X, "+X", jogBtnSize);
            _btnJogPlusY = CreateJogButton(JogType.JOG_PLUS_Y, "+Y", jogBtnSize);
            var row1 = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Width = 400, Height = 55 };
            row1.Controls.AddRange(new Control[] { _btnJogPlusX, _btnJogPlusY });
            _pnlJogButtons.Controls.Add(row1);

            // 第二排：-X, -Y
            _btnJogMinusX = CreateJogButton(JogType.JOG_MINUS_X, "-X", jogBtnSize);
            _btnJogMinusY = CreateJogButton(JogType.JOG_MINUS_Y, "-Y", jogBtnSize);
            var row2 = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Width = 400, Height = 55 };
            row2.Controls.AddRange(new Control[] { _btnJogMinusX, _btnJogMinusY });
            _pnlJogButtons.Controls.Add(row2);

            // 第三排：+Z, +R
            _btnJogPlusZ = CreateJogButton(JogType.JOG_PLUS_Z, "+Z", jogBtnSize);
            _btnJogPlusR = CreateJogButton(JogType.JOG_PLUS_R, "+R", jogBtnSize);
            var row3 = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Width = 400, Height = 55 };
            row3.Controls.AddRange(new Control[] { _btnJogPlusZ, _btnJogPlusR });
            _pnlJogButtons.Controls.Add(row3);

            // 第四排：-Z, -R, 急停
            _btnJogMinusZ = CreateJogButton(JogType.JOG_MINUS_Z, "-Z", jogBtnSize);
            _btnJogMinusR = CreateJogButton(JogType.JOG_MINUS_R, "-R", jogBtnSize);
            _btnJogStop = new IndustrialSafeButton
            {
                Text = "🛑 STOP",
                RequireLongPress = false,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 3 },
                Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = AlarmError,
                Size = jogBtnSize,
                Cursor = Cursors.Hand
            };
            _btnJogStop.SafeClick += BtnJogStop_SafeClick;
            var row4 = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Width = 400, Height = 55 };
            row4.Controls.AddRange(new Control[] { _btnJogMinusZ, _btnJogMinusR, _btnJogStop });
            _pnlJogButtons.Controls.Add(row4);

            _chkJogContinuous = new CheckBox
            {
                Text = "連續模式",
                Font = new Font("Microsoft JhengHei UI", 10F),
                ForeColor = TextSecondary,
                Location = new Point(20, 285),
                AutoSize = true
            };

            _lblJogSpeedLabel = new Label
            {
                Text = "速度:",
                Font = new Font("Microsoft JhengHei UI", 10F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(150, 285)
            };

            _numJogSpeed = new NumericUpDown
            {
                Location = new Point(200, 282),
                Size = new Size(80, 25),
                Minimum = 1,
                Maximum = 100,
                Value = 30,
                Increment = 5,
                TextAlign = HorizontalAlignment.Center,
                Font = new Font("Microsoft JhengHei UI", 10F)
            };

            _lblJogSpeedUnit = new Label
            {
                Text = "%",
                Font = new Font("Microsoft JhengHei UI", 10F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(285, 285)
            };

            _pnlJogControlPanel.Controls.AddRange(new Control[] {
                _lblJogTitle, _lblJogWarning, _pnlJogButtons,
                _chkJogContinuous, _lblJogSpeedLabel, _numJogSpeed, _lblJogSpeedUnit
            });
            mainPanel.Controls.Add(_pnlJogControlPanel);

            // ========== 座標顯示面板 ==========
            _pnlPoseDisplayPanel = new Panel
            {
                Width = mainPanel.Width - 30,
                Height = 150,
                BackColor = CardBackground,
                Padding = new Padding(20),
                Margin = new Padding(0, 0, 0, 15)
            };

            _lblPoseTitle = new Label
            {
                Text = "📍 當前座標 (mm/deg)",
                Font = new Font("Microsoft JhengHei UI", 13F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            _lblPoseXLabel = new Label
            {
                Text = "X:",
                Font = new Font("Microsoft JhengHei UI", 12F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 55)
            };

            _lblPoseXValue = new Label
            {
                Text = "0.000",
                Font = new Font("Consolas", 14F, FontStyle.Bold),
                ForeColor = AccentColor,
                AutoSize = true,
                Location = new Point(50, 55)
            };

            _lblPoseYLabel = new Label
            {
                Text = "Y:",
                Font = new Font("Microsoft JhengHei UI", 12F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(150, 55)
            };

            _lblPoseYValue = new Label
            {
                Text = "0.000",
                Font = new Font("Consolas", 14F, FontStyle.Bold),
                ForeColor = AccentColor,
                AutoSize = true,
                Location = new Point(180, 55)
            };

            _lblPoseZLabel = new Label
            {
                Text = "Z:",
                Font = new Font("Microsoft JhengHei UI", 12F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(280, 55)
            };

            _lblPoseZValue = new Label
            {
                Text = "0.000",
                Font = new Font("Consolas", 14F, FontStyle.Bold),
                ForeColor = AccentColor,
                AutoSize = true,
                Location = new Point(310, 55)
            };

            _lblPoseRLabel = new Label
            {
                Text = "R:",
                Font = new Font("Microsoft JhengHei UI", 12F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(410, 55)
            };

            _lblPoseRValue = new Label
            {
                Text = "0.000",
                Font = new Font("Consolas", 14F, FontStyle.Bold),
                ForeColor = AccentColor,
                AutoSize = true,
                Location = new Point(440, 55)
            };

            _btnRefreshPose = new Button
            {
                Text = "🔄 刷新",
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 2 },
                Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Bold),
                ForeColor = TextPrimary,
                BackColor = AccentColor,
                Location = new Point(550, 52),
                Size = new Size(90, 35)
            };
            _btnRefreshPose.Click += BtnRefreshPose_Click;

            _pnlPoseDisplayPanel.Controls.AddRange(new Control[] {
                _lblPoseTitle, _lblPoseXLabel, _lblPoseXValue,
                _lblPoseYLabel, _lblPoseYValue, _lblPoseZLabel, _lblPoseZValue,
                _lblPoseRLabel, _lblPoseRValue, _btnRefreshPose
            });
            mainPanel.Controls.Add(_pnlPoseDisplayPanel);

            // ========== 命令執行面板 ==========
            _pnlCommandPanel = new Panel
            {
                Width = mainPanel.Width - 30,
                Height = 200,
                BackColor = CardBackground,
                Padding = new Padding(20),
                Margin = new Padding(0, 0, 0, 15)
            };

            _lblCommandTitle = new Label
            {
                Text = "💻 命令執行",
                Font = new Font("Microsoft JhengHei UI", 13F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            _txtCommandInput = new TextBox
            {
                Location = new Point(20, 50),
                Size = new Size(400, 30),
                Font = new Font("Consolas", 11F),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = TextPrimary,
                BorderStyle = BorderStyle.FixedSingle
            };

            _btnExecuteCommand = new IndustrialSafeButton
            {
                Text = "▶ 執行",
                RequireLongPress = true,
                TargetHoldTimeMs = 1500,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 2 },
                Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold),
                ForeColor = TextPrimary,
                BackColor = AccentGreen,
                Location = new Point(440, 50),
                Size = new Size(100, 30)
            };
            _btnExecuteCommand.SafeClick += BtnExecuteCommand_SafeClick;

            _lstCommandHistory = new ListBox
            {
                Location = new Point(20, 90),
                Size = new Size(mainPanel.Width - 70, 90),
                Font = new Font("Consolas", 10F),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = TextPrimary,
                BorderStyle = BorderStyle.FixedSingle,
                IntegralHeight = false
            };

            _pnlCommandPanel.Controls.AddRange(new Control[] {
                _lblCommandTitle, _txtCommandInput, _btnExecuteCommand, _lstCommandHistory
            });
            mainPanel.Controls.Add(_pnlCommandPanel);

            // 添加到分頁
            _robotEngineerPage.Controls.Add(mainPanel);
        }

        /// <summary>
        /// 創建 Jog 按鈕 (工業級樣式)
        /// </summary>
        private Button CreateJogButton(AuroraVision.JogType jogType, string text, Size size)
        {
            var btn = new Button
            {
                Text = text,
                Tag = jogType,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 2 },
                Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold),
                ForeColor = TextPrimary,
                BackColor = JogButtonColor,
                Size = size,
                Cursor = Cursors.Hand,
                Tag = jogType
            };
            btn.MouseDown += BtnJog_MouseDown;
            btn.MouseUp += BtnJog_MouseUp;
            btn.Leave += BtnJog_Leave;
            return btn;
        }

        /// <summary>
        /// 初始化分頁 3: 視覺虛實整合分頁 (模擬/實際模式切換)
        /// 包含：模式選擇、影像顯示、檢測結果、控制按鈕、模擬參數
        /// </summary>
        private void InitializeVisionIntegrationPage()
        {
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(15),
                BackColor = PanelBackground
            };

            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 65F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));

            // ========== 左側：影像顯示面板 ==========
            _pnlImageDisplayPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = CardBackground,
                Padding = new Padding(20),
                Margin = new Padding(0, 0, 10, 10)
            };

            _lblImageTitle = new Label
            {
                Text = "📷 即時影像",
                Font = new Font("Microsoft JhengHei UI", 13F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            _pnlImageContainer = new Panel
            {
                Location = new Point(20, 50),
                Size = new Size(_pnlImageDisplayPanel.Width - 40, _pnlImageDisplayPanel.Height - 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = Color.FromArgb(20, 20, 20),
                BorderStyle = BorderStyle.FixedSingle
            };

            _imgCapturedImage = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            _lblNoImagePlaceholder = new Label
            {
                Text = "📭 無影像\n\n點擊「單張擷取」或「開始連續」獲取影像",
                Font = new Font("Microsoft JhengHei UI", 12F),
                ForeColor = TextSecondary,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            _pnlImageContainer.Controls.AddRange(new Control[] { _imgCapturedImage, _lblNoImagePlaceholder });
            _imgCapturedImage.Visible = false;

            _pnlImageDisplayPanel.Controls.AddRange(new Control[] { _lblImageTitle, _pnlImageContainer });
            mainPanel.Controls.Add(_pnlImageDisplayPanel, 0, 0);

            // ========== 右側上：模式選擇與檢測結果 ==========
            var rightTopPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Margin = new Padding(10, 0, 0, 10)
            };

            // 模式選擇面板
            _pnlVisionModePanel = new Panel
            {
                Width = rightTopPanel.Width - 20,
                Height = 120,
                BackColor = CardBackground,
                Padding = new Padding(20),
                Margin = new Padding(0, 0, 0, 10)
            };

            _lblVisionModeTitle = new Label
            {
                Text = "🔀 運行模式",
                Font = new Font("Microsoft JhengHei UI", 13F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            _rbVisionModeSimulation = new RadioButton
            {
                Text = "🎭 模擬模式 (虛)",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextPrimary,
                Location = new Point(20, 50),
                AutoSize = true,
                Checked = true
            };
            _rbVisionModeSimulation.CheckedChanged += RbVisionMode_CheckedChanged;

            _rbVisionModeReal = new RadioButton
            {
                Text = "🔌 實際模式 (實)",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextPrimary,
                Location = new Point(20, 80),
                AutoSize = true
            };
            _rbVisionModeReal.CheckedChanged += RbVisionMode_CheckedChanged;

            _lblVisionModeStatus = new Label
            {
                Text = "當前：模擬模式",
                Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Italic),
                ForeColor = AccentColor,
                AutoSize = true,
                Location = new Point(200, 55)
            };

            _pnlVisionModePanel.Controls.AddRange(new Control[] {
                _lblVisionModeTitle, _rbVisionModeSimulation, _rbVisionModeReal, _lblVisionModeStatus
            });
            rightTopPanel.Controls.Add(_pnlVisionModePanel);

            // 檢測結果面板
            _pnlInspectionResultPanel = new Panel
            {
                Width = rightTopPanel.Width - 20,
                Height = 200,
                BackColor = CardBackground,
                Padding = new Padding(20),
                Margin = new Padding(0, 0, 0, 0)
            };

            _lblInspectionTitle = new Label
            {
                Text = "✅ 檢測結果",
                Font = new Font("Microsoft JhengHei UI", 13F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            _lblInspectionSuccessLabel = new Label
            {
                Text = "判定:",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 55)
            };

            _lblInspectionSuccessValue = new Label
            {
                Text = "--",
                Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(70, 55)
            };

            _lblBarcodeResultLabel = new Label
            {
                Text = "條碼:",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 90)
            };

            _lblBarcodeResultValue = new Label
            {
                Text = "N/A",
                Font = new Font("Consolas", 11F, FontStyle.Bold),
                ForeColor = AccentColor,
                AutoSize = true,
                Location = new Point(70, 90)
            };

            _lblConfidenceLabel = new Label
            {
                Text = "置信度:",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 125)
            };

            _lblConfidenceValue = new Label
            {
                Text = "--%",
                Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(90, 125)
            };

            _lblMarkPositionsLabel = new Label
            {
                Text = "Mark 點:",
                Font = new Font("Microsoft JhengHei UI", 11F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 160)
            };

            _lblMarkPositionsValue = new Label
            {
                Text = "(--, --)",
                Font = new Font("Consolas", 10F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(90, 160)
            };

            _pnlInspectionResultPanel.Controls.AddRange(new Control[] {
                _lblInspectionTitle, _lblInspectionSuccessLabel, _lblInspectionSuccessValue,
                _lblBarcodeResultLabel, _lblBarcodeResultValue, _lblConfidenceLabel, _lblConfidenceValue,
                _lblMarkPositionsLabel, _lblMarkPositionsValue
            });
            rightTopPanel.Controls.Add(_pnlInspectionResultPanel);

            mainPanel.Controls.Add(rightTopPanel, 1, 0);

            // ========== 左側下：控制按鈕面板 ==========
            _pnlVisionControlPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = CardBackground,
                Padding = new Padding(20),
                Margin = new Padding(0, 10, 10, 0)
            };

            _lblVisionControlTitle = new Label
            {
                Text = "🎛️ 影像控制",
                Font = new Font("Microsoft JhengHei UI", 13F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            _btnCaptureSingle = new IndustrialSafeButton
            {
                Text = "📸 單張擷取",
                RequireLongPress = false,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 2 },
                Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold),
                ForeColor = TextPrimary,
                BackColor = AccentBlue,
                Location = new Point(20, 50),
                Size = new Size(130, 45)
            };
            _btnCaptureSingle.SafeClick += BtnCaptureSingle_SafeClick;

            _btnStartContinuous = new Button
            {
                Text = "▶ 開始連續",
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 2 },
                Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold),
                ForeColor = TextPrimary,
                BackColor = AccentGreen,
                Location = new Point(170, 50),
                Size = new Size(130, 45)
            };
            _btnStartContinuous.Click += BtnStartContinuous_Click;

            _btnStopContinuous = new Button
            {
                Text = "⏹ 停止連續",
                Enabled = false,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 2 },
                Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold),
                ForeColor = TextPrimary,
                BackColor = AlarmError,
                Location = new Point(320, 50),
                Size = new Size(130, 45)
            };
            _btnStopContinuous.Click += BtnStopContinuous_Click;

            _btnSaveImage = new Button
            {
                Text = "💾 儲存影像",
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 2 },
                Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold),
                ForeColor = TextPrimary,
                BackColor = Color.FromArgb(142, 68, 173),
                Location = new Point(470, 50),
                Size = new Size(130, 45)
            };
            _btnSaveImage.Click += BtnSaveImage_Click;

            _pnlVisionControlPanel.Controls.AddRange(new Control[] {
                _lblVisionControlTitle, _btnCaptureSingle, _btnStartContinuous,
                _btnStopContinuous, _btnSaveImage
            });
            mainPanel.Controls.Add(_pnlVisionControlPanel, 0, 1);

            // ========== 右側下：模擬控制面板 ==========
            _pnlSimulationControlPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = CardBackground,
                Padding = new Padding(20),
                Margin = new Padding(10, 10, 0, 0)
            };

            _lblSimControlTitle = new Label
            {
                Text = "🎮 模擬參數設定",
                Font = new Font("Microsoft JhengHei UI", 13F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            _lblSimulatedPoseLabel = new Label
            {
                Text = "模擬 Robot 座標:",
                Font = new Font("Microsoft JhengHei UI", 10F),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(20, 50)
            };

            _numSimulatedX = new NumericUpDown
            {
                Location = new Point(20, 75),
                Size = new Size(80, 25),
                Minimum = -500,
                Maximum = 500,
                Value = 100,
                Increment = 10,
                TextAlign = HorizontalAlignment.Center,
                Font = new Font("Consolas", 10F)
            };

            _numSimulatedY = new NumericUpDown
            {
                Location = new Point(110, 75),
                Size = new Size(80, 25),
                Minimum = -500,
                Maximum = 500,
                Value = 0,
                Increment = 10,
                TextAlign = HorizontalAlignment.Center,
                Font = new Font("Consolas", 10F)
            };

            _numSimulatedZ = new NumericUpDown
            {
                Location = new Point(200, 75),
                Size = new Size(80, 25),
                Minimum = -200,
                Maximum = 200,
                Value = 50,
                Increment = 10,
                TextAlign = HorizontalAlignment.Center,
                Font = new Font("Consolas", 10F)
            };

            _numSimulatedR = new NumericUpDown
            {
                Location = new Point(290, 75),
                Size = new Size(80, 25),
                Minimum = -180,
                Maximum = 180,
                Value = 0,
                Increment = 15,
                TextAlign = HorizontalAlignment.Center,
                Font = new Font("Consolas", 10F)
            };

            _btnApplyImage = new Button
            {
                Text = "🔄 套用至影像",
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 2 },
                Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Bold),
                ForeColor = TextPrimary,
                BackColor = AccentColor,
                Location = new Point(20, 110),
                Size = new Size(150, 35)
            };
            _btnApplyImage.Click += BtnApplyImage_Click;

            _chkSimulateBarcode = new CheckBox
            {
                Text = "模擬條碼讀取",
                Font = new Font("Microsoft JhengHei UI", 10F),
                ForeColor = TextPrimary,
                Location = new Point(20, 155),
                AutoSize = true,
                Checked = true
            };

            _txtSimulatedBarcode = new TextBox
            {
                Location = new Point(20, 185),
                Size = new Size(250, 25),
                Font = new Font("Consolas", 10F),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = TextPrimary,
                BorderStyle = BorderStyle.FixedSingle,
                Text = "SN20250101001"
            };

            _pnlSimulationControlPanel.Controls.AddRange(new Control[] {
                _lblSimControlTitle, _lblSimulatedPoseLabel,
                _numSimulatedX, _numSimulatedY, _numSimulatedZ, _numSimulatedR,
                _btnApplyImage, _chkSimulateBarcode, _txtSimulatedBarcode
            });
            mainPanel.Controls.Add(_pnlSimulationControlPanel, 1, 1);

            // 添加到分頁
            _visionIntegrationPage.Controls.Add(mainPanel);
        }

        // ========== 工業級配色定義 (靜態常數) ==========
        // 所有色彩常數已於 MainForm.cs 定義以避免 partial class 重複
        // IndustrialPages.Designer.cs 不應重複定義任何色彩常數
    }
}
