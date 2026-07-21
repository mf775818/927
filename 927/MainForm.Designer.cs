namespace _927
{
    using System.Drawing;
    using System.Windows.Forms;

    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            _mainTabControl = new TabControl();
            _robotVisionMonitorPage = new TabPage();
            _pnlAlarmSummaryCard = new Panel();
            lblAlarmSummaryTitle = new Label();
            alarmListBox = new ListBox();
            _pnlSystemOverviewCard = new Panel();
            _lblSystemStatusTitle = new Label();
            _lblOverallStatusLabel = new Label();
            _lblOverallStatusValue = new Label();
            _lblProductionCountLabel = new Label();
            _lblProductionCountValue = new Label();
            _lblLastUpdateTimeLabel = new Label();
            _lblLastUpdateTimeValue = new Label();
            _lblTemperatureLabel = new Label();
            _lblTemperature = new Label();
            _pnlVisionStatusCard = new Panel();
            _lblVisionTitle = new Label();
            _lblVisionModeLabel = new Label();
            _lblVisionModeValue = new Label();
            _lblLastBarcodeLabel = new Label();
            _lblLastBarcodeValue = new Label();
            _lblVisionConnectionLabel = new Label();
            _lblVisionConnectionStatus = new Label();
            _pnlRobotStatusCard = new Panel();
            _lblRobotTitle = new Label();
            _lblRobotModeLabel = new Label();
            _lblRobotModeValue = new Label();
            _statusLabel = new Label();
            _lblRobotCommandIdLabel = new Label();
            _lblRobotCommandIdValue = new Label();
            _lblRobotConnectionLabel = new Label();
            _lblRobotConnectionStatus = new Label();
            _robotEngineerPage = new TabPage();
            pnlEngineerScroll = new Panel();
            _pnlRobotConnectionPanel = new Panel();
            _lblRobotConnectTitle = new Label();
            _btnRobotDisconnect = new Button();
            _lblRobotConnectionState = new Label();
            _pnlSafetyWarningPanel = new Panel();
            _lblSafetyWarningIcon = new Label();
            _lblSafetyWarning = new Label();
            _visionIntegrationPage = new TabPage();
            pnlVisionScroll = new Panel();
            _pnlVisionControlPanel = new Panel();
            _lblVisionControlTitle = new Label();
            _btnStartContinuous = new Button();
            _btnStopContinuous = new Button();
            _btnSaveImage = new Button();
            _pnlInspectionResultPanel = new Panel();
            _lblInspectionTitle = new Label();
            _lblInspectionSuccessLabel = new Label();
            _lblInspectionSuccessValue = new Label();
            _lblBarcodeResultLabel = new Label();
            _lblBarcodeResultValue = new Label();
            _lblConfidenceLabel = new Label();
            _lblConfidenceValue = new Label();
            _pnlImageDisplayPanel = new Panel();
            _lblImageTitle = new Label();
            _pnlImageContainer = new Panel();
            _imgCapturedImage = new PictureBox();
            _lblNoImagePlaceholder = new Label();
            _pnlVisionModePanel = new Panel();
            _lblVisionModeTitle = new Label();
            _rbVisionModeSimulation = new RadioButton();
            _rbVisionModeReal = new RadioButton();
            _lblVisionModeStatus = new Label();
            _simulationIndicatorLabel = new Label();
            _alarmIndicatorLabel = new Label();
            _stopButton = new Button();
            _lblAlarmCode = new Label();
            _mainTabControl.SuspendLayout();
            _robotVisionMonitorPage.SuspendLayout();
            _pnlAlarmSummaryCard.SuspendLayout();
            _pnlSystemOverviewCard.SuspendLayout();
            _pnlVisionStatusCard.SuspendLayout();
            _pnlRobotStatusCard.SuspendLayout();
            _robotEngineerPage.SuspendLayout();
            pnlEngineerScroll.SuspendLayout();
            _pnlRobotConnectionPanel.SuspendLayout();
            _pnlSafetyWarningPanel.SuspendLayout();
            _visionIntegrationPage.SuspendLayout();
            pnlVisionScroll.SuspendLayout();
            _pnlVisionControlPanel.SuspendLayout();
            _pnlInspectionResultPanel.SuspendLayout();
            _pnlImageDisplayPanel.SuspendLayout();
            _pnlImageContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_imgCapturedImage).BeginInit();
            _pnlVisionModePanel.SuspendLayout();
            SuspendLayout();
            // 
            // _mainTabControl
            // 
            _mainTabControl.Appearance = TabAppearance.FlatButtons;
            _mainTabControl.Controls.Add(_robotVisionMonitorPage);
            _mainTabControl.Controls.Add(_robotEngineerPage);
            _mainTabControl.Controls.Add(_visionIntegrationPage);
            _mainTabControl.Dock = DockStyle.Fill;
            _mainTabControl.Font = new Font("Microsoft JhengHei UI", 11F);
            _mainTabControl.ItemSize = new Size(150, 35);
            _mainTabControl.Location = new Point(0, 0);
            _mainTabControl.Name = "_mainTabControl";
            _mainTabControl.SelectedIndex = 0;
            _mainTabControl.Size = new Size(1400, 850);
            _mainTabControl.SizeMode = TabSizeMode.Fixed;
            _mainTabControl.TabIndex = 0;
            // 
            // _robotVisionMonitorPage
            // 
            _robotVisionMonitorPage.BackColor = Color.FromArgb(45, 45, 45);
            _robotVisionMonitorPage.Controls.Add(_pnlAlarmSummaryCard);
            _robotVisionMonitorPage.Controls.Add(_pnlSystemOverviewCard);
            _robotVisionMonitorPage.Controls.Add(_pnlVisionStatusCard);
            _robotVisionMonitorPage.Controls.Add(_pnlRobotStatusCard);
            _robotVisionMonitorPage.Location = new Point(4, 39);
            _robotVisionMonitorPage.Name = "_robotVisionMonitorPage";
            _robotVisionMonitorPage.Padding = new Padding(15);
            _robotVisionMonitorPage.Size = new Size(1392, 807);
            _robotVisionMonitorPage.TabIndex = 0;
            _robotVisionMonitorPage.Text = "🤖 Robot+ 視覺監控";
            // 
            // _pnlAlarmSummaryCard
            // 
            _pnlAlarmSummaryCard.BackColor = Color.FromArgb(55, 55, 55);
            _pnlAlarmSummaryCard.Controls.Add(lblAlarmSummaryTitle);
            _pnlAlarmSummaryCard.Controls.Add(alarmListBox);
            _pnlAlarmSummaryCard.Dock = DockStyle.Top;
            _pnlAlarmSummaryCard.Location = new Point(15, 595);
            _pnlAlarmSummaryCard.Margin = new Padding(15, 10, 15, 10);
            _pnlAlarmSummaryCard.Name = "_pnlAlarmSummaryCard";
            _pnlAlarmSummaryCard.Padding = new Padding(20);
            _pnlAlarmSummaryCard.Size = new Size(1362, 250);
            _pnlAlarmSummaryCard.TabIndex = 0;
            // 
            // lblAlarmSummaryTitle
            // 
            lblAlarmSummaryTitle.AutoSize = true;
            lblAlarmSummaryTitle.Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold);
            lblAlarmSummaryTitle.ForeColor = Color.FromArgb(220, 220, 220);
            lblAlarmSummaryTitle.Location = new Point(20, 15);
            lblAlarmSummaryTitle.Name = "lblAlarmSummaryTitle";
            lblAlarmSummaryTitle.Size = new Size(221, 24);
            lblAlarmSummaryTitle.TabIndex = 0;
            lblAlarmSummaryTitle.Text = "⚠️ 警報摘要 (最近 10 筆)";
            // 
            // alarmListBox
            // 
            alarmListBox.BackColor = Color.FromArgb(35, 35, 35);
            alarmListBox.BorderStyle = BorderStyle.None;
            alarmListBox.Dock = DockStyle.Fill;
            alarmListBox.Font = new Font("Microsoft JhengHei UI", 10F);
            alarmListBox.ForeColor = Color.FromArgb(220, 220, 220);
            alarmListBox.ItemHeight = 17;
            alarmListBox.Items.AddRange(new object[] { "無警報" });
            alarmListBox.Location = new Point(20, 20);
            alarmListBox.Name = "alarmListBox";
            alarmListBox.Size = new Size(1322, 210);
            alarmListBox.TabIndex = 1;
            // 
            // _pnlSystemOverviewCard
            // 
            _pnlSystemOverviewCard.BackColor = Color.FromArgb(55, 55, 55);
            _pnlSystemOverviewCard.Controls.Add(_lblSystemStatusTitle);
            _pnlSystemOverviewCard.Controls.Add(_lblOverallStatusLabel);
            _pnlSystemOverviewCard.Controls.Add(_lblOverallStatusValue);
            _pnlSystemOverviewCard.Controls.Add(_lblProductionCountLabel);
            _pnlSystemOverviewCard.Controls.Add(_lblProductionCountValue);
            _pnlSystemOverviewCard.Controls.Add(_lblLastUpdateTimeLabel);
            _pnlSystemOverviewCard.Controls.Add(_lblLastUpdateTimeValue);
            _pnlSystemOverviewCard.Controls.Add(_lblTemperatureLabel);
            _pnlSystemOverviewCard.Controls.Add(_lblTemperature);
            _pnlSystemOverviewCard.Dock = DockStyle.Top;
            _pnlSystemOverviewCard.Location = new Point(15, 375);
            _pnlSystemOverviewCard.Margin = new Padding(15, 10, 15, 10);
            _pnlSystemOverviewCard.Name = "_pnlSystemOverviewCard";
            _pnlSystemOverviewCard.Padding = new Padding(20);
            _pnlSystemOverviewCard.Size = new Size(1362, 220);
            _pnlSystemOverviewCard.TabIndex = 1;
            // 
            // _lblSystemStatusTitle
            // 
            _lblSystemStatusTitle.AutoSize = true;
            _lblSystemStatusTitle.Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold);
            _lblSystemStatusTitle.ForeColor = Color.FromArgb(220, 220, 220);
            _lblSystemStatusTitle.Location = new Point(20, 15);
            _lblSystemStatusTitle.Name = "_lblSystemStatusTitle";
            _lblSystemStatusTitle.Size = new Size(112, 24);
            _lblSystemStatusTitle.TabIndex = 0;
            _lblSystemStatusTitle.Text = "📊 系統總覽";
            // 
            // _lblOverallStatusLabel
            // 
            _lblOverallStatusLabel.AutoSize = true;
            _lblOverallStatusLabel.Font = new Font("Microsoft JhengHei UI", 11F);
            _lblOverallStatusLabel.ForeColor = Color.FromArgb(150, 150, 150);
            _lblOverallStatusLabel.Location = new Point(20, 55);
            _lblOverallStatusLabel.Name = "_lblOverallStatusLabel";
            _lblOverallStatusLabel.Size = new Size(72, 19);
            _lblOverallStatusLabel.TabIndex = 1;
            _lblOverallStatusLabel.Text = "整體狀態:";
            // 
            // _lblOverallStatusValue
            // 
            _lblOverallStatusValue.AutoSize = true;
            _lblOverallStatusValue.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
            _lblOverallStatusValue.ForeColor = Color.FromArgb(150, 150, 150);
            _lblOverallStatusValue.Location = new Point(120, 55);
            _lblOverallStatusValue.Name = "_lblOverallStatusValue";
            _lblOverallStatusValue.Size = new Size(57, 20);
            _lblOverallStatusValue.TabIndex = 2;
            _lblOverallStatusValue.Text = "待機中";
            // 
            // _lblProductionCountLabel
            // 
            _lblProductionCountLabel.AutoSize = true;
            _lblProductionCountLabel.Font = new Font("Microsoft JhengHei UI", 11F);
            _lblProductionCountLabel.ForeColor = Color.FromArgb(150, 150, 150);
            _lblProductionCountLabel.Location = new Point(20, 90);
            _lblProductionCountLabel.Name = "_lblProductionCountLabel";
            _lblProductionCountLabel.Size = new Size(72, 19);
            _lblProductionCountLabel.TabIndex = 3;
            _lblProductionCountLabel.Text = "生產計數:";
            // 
            // _lblProductionCountValue
            // 
            _lblProductionCountValue.AutoSize = true;
            _lblProductionCountValue.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
            _lblProductionCountValue.ForeColor = Color.FromArgb(220, 220, 220);
            _lblProductionCountValue.Location = new Point(120, 90);
            _lblProductionCountValue.Name = "_lblProductionCountValue";
            _lblProductionCountValue.Size = new Size(18, 19);
            _lblProductionCountValue.TabIndex = 4;
            _lblProductionCountValue.Text = "0";
            // 
            // _lblLastUpdateTimeLabel
            // 
            _lblLastUpdateTimeLabel.AutoSize = true;
            _lblLastUpdateTimeLabel.Font = new Font("Microsoft JhengHei UI", 11F);
            _lblLastUpdateTimeLabel.ForeColor = Color.FromArgb(150, 150, 150);
            _lblLastUpdateTimeLabel.Location = new Point(20, 125);
            _lblLastUpdateTimeLabel.Name = "_lblLastUpdateTimeLabel";
            _lblLastUpdateTimeLabel.Size = new Size(72, 19);
            _lblLastUpdateTimeLabel.TabIndex = 5;
            _lblLastUpdateTimeLabel.Text = "最後更新:";
            // 
            // _lblLastUpdateTimeValue
            // 
            _lblLastUpdateTimeValue.AutoSize = true;
            _lblLastUpdateTimeValue.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
            _lblLastUpdateTimeValue.ForeColor = Color.FromArgb(220, 220, 220);
            _lblLastUpdateTimeValue.Location = new Point(120, 125);
            _lblLastUpdateTimeValue.Name = "_lblLastUpdateTimeValue";
            _lblLastUpdateTimeValue.Size = new Size(39, 19);
            _lblLastUpdateTimeValue.TabIndex = 6;
            _lblLastUpdateTimeValue.Text = "N/A";
            // 
            // _lblTemperatureLabel
            // 
            _lblTemperatureLabel.AutoSize = true;
            _lblTemperatureLabel.Font = new Font("Microsoft JhengHei UI", 11F);
            _lblTemperatureLabel.ForeColor = Color.FromArgb(150, 150, 150);
            _lblTemperatureLabel.Location = new Point(20, 160);
            _lblTemperatureLabel.Name = "_lblTemperatureLabel";
            _lblTemperatureLabel.Size = new Size(42, 19);
            _lblTemperatureLabel.TabIndex = 7;
            _lblTemperatureLabel.Text = "溫度:";
            // 
            // _lblTemperature
            // 
            _lblTemperature.AutoSize = true;
            _lblTemperature.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
            _lblTemperature.ForeColor = Color.FromArgb(220, 220, 220);
            _lblTemperature.Location = new Point(120, 160);
            _lblTemperature.Name = "_lblTemperature";
            _lblTemperature.Size = new Size(56, 19);
            _lblTemperature.TabIndex = 8;
            _lblTemperature.Text = "25.0°C";
            // 
            // _pnlVisionStatusCard
            // 
            _pnlVisionStatusCard.BackColor = Color.FromArgb(55, 55, 55);
            _pnlVisionStatusCard.Controls.Add(_lblVisionTitle);
            _pnlVisionStatusCard.Controls.Add(_lblVisionModeLabel);
            _pnlVisionStatusCard.Controls.Add(_lblVisionModeValue);
            _pnlVisionStatusCard.Controls.Add(_lblLastBarcodeLabel);
            _pnlVisionStatusCard.Controls.Add(_lblLastBarcodeValue);
            _pnlVisionStatusCard.Controls.Add(_lblVisionConnectionLabel);
            _pnlVisionStatusCard.Controls.Add(_lblVisionConnectionStatus);
            _pnlVisionStatusCard.Dock = DockStyle.Top;
            _pnlVisionStatusCard.Location = new Point(15, 195);
            _pnlVisionStatusCard.Margin = new Padding(15, 10, 15, 10);
            _pnlVisionStatusCard.Name = "_pnlVisionStatusCard";
            _pnlVisionStatusCard.Padding = new Padding(20);
            _pnlVisionStatusCard.Size = new Size(1362, 180);
            _pnlVisionStatusCard.TabIndex = 2;
            // 
            // _lblVisionTitle
            // 
            _lblVisionTitle.AutoSize = true;
            _lblVisionTitle.Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold);
            _lblVisionTitle.ForeColor = Color.FromArgb(220, 220, 220);
            _lblVisionTitle.Location = new Point(20, 15);
            _lblVisionTitle.Name = "_lblVisionTitle";
            _lblVisionTitle.Size = new Size(151, 24);
            _lblVisionTitle.TabIndex = 0;
            _lblVisionTitle.Text = "👁️ 視覺系統狀態";
            // 
            // _lblVisionModeLabel
            // 
            _lblVisionModeLabel.AutoSize = true;
            _lblVisionModeLabel.Font = new Font("Microsoft JhengHei UI", 11F);
            _lblVisionModeLabel.ForeColor = Color.FromArgb(150, 150, 150);
            _lblVisionModeLabel.Location = new Point(20, 55);
            _lblVisionModeLabel.Name = "_lblVisionModeLabel";
            _lblVisionModeLabel.Size = new Size(72, 19);
            _lblVisionModeLabel.TabIndex = 1;
            _lblVisionModeLabel.Text = "運行模式:";
            // 
            // _lblVisionModeValue
            // 
            _lblVisionModeValue.AutoSize = true;
            _lblVisionModeValue.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
            _lblVisionModeValue.ForeColor = Color.FromArgb(64, 169, 255);
            _lblVisionModeValue.Location = new Point(120, 55);
            _lblVisionModeValue.Name = "_lblVisionModeValue";
            _lblVisionModeValue.Size = new Size(86, 19);
            _lblVisionModeValue.TabIndex = 2;
            _lblVisionModeValue.Text = "Simulation";
            // 
            // _lblLastBarcodeLabel
            // 
            _lblLastBarcodeLabel.AutoSize = true;
            _lblLastBarcodeLabel.Font = new Font("Microsoft JhengHei UI", 11F);
            _lblLastBarcodeLabel.ForeColor = Color.FromArgb(150, 150, 150);
            _lblLastBarcodeLabel.Location = new Point(20, 90);
            _lblLastBarcodeLabel.Name = "_lblLastBarcodeLabel";
            _lblLastBarcodeLabel.Size = new Size(72, 19);
            _lblLastBarcodeLabel.TabIndex = 3;
            _lblLastBarcodeLabel.Text = "最後條碼:";
            // 
            // _lblLastBarcodeValue
            // 
            _lblLastBarcodeValue.AutoSize = true;
            _lblLastBarcodeValue.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
            _lblLastBarcodeValue.ForeColor = Color.FromArgb(220, 220, 220);
            _lblLastBarcodeValue.Location = new Point(120, 90);
            _lblLastBarcodeValue.Name = "_lblLastBarcodeValue";
            _lblLastBarcodeValue.Size = new Size(39, 19);
            _lblLastBarcodeValue.TabIndex = 4;
            _lblLastBarcodeValue.Text = "N/A";
            // 
            // _lblVisionConnectionLabel
            // 
            _lblVisionConnectionLabel.AutoSize = true;
            _lblVisionConnectionLabel.Font = new Font("Microsoft JhengHei UI", 11F);
            _lblVisionConnectionLabel.ForeColor = Color.FromArgb(150, 150, 150);
            _lblVisionConnectionLabel.Location = new Point(20, 125);
            _lblVisionConnectionLabel.Name = "_lblVisionConnectionLabel";
            _lblVisionConnectionLabel.Size = new Size(72, 19);
            _lblVisionConnectionLabel.TabIndex = 5;
            _lblVisionConnectionLabel.Text = "連接狀態:";
            // 
            // _lblVisionConnectionStatus
            // 
            _lblVisionConnectionStatus.AutoSize = true;
            _lblVisionConnectionStatus.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
            _lblVisionConnectionStatus.ForeColor = Color.FromArgb(231, 76, 60);
            _lblVisionConnectionStatus.Location = new Point(120, 125);
            _lblVisionConnectionStatus.Name = "_lblVisionConnectionStatus";
            _lblVisionConnectionStatus.Size = new Size(54, 19);
            _lblVisionConnectionStatus.TabIndex = 6;
            _lblVisionConnectionStatus.Text = "未連接";
            // 
            // _pnlRobotStatusCard
            // 
            _pnlRobotStatusCard.BackColor = Color.FromArgb(55, 55, 55);
            _pnlRobotStatusCard.Controls.Add(_lblRobotTitle);
            _pnlRobotStatusCard.Controls.Add(_lblRobotModeLabel);
            _pnlRobotStatusCard.Controls.Add(_lblRobotModeValue);
            _pnlRobotStatusCard.Controls.Add(_lblAlarmCode);
            _pnlRobotStatusCard.Controls.Add(_statusLabel);
            _pnlRobotStatusCard.Controls.Add(_lblRobotCommandIdLabel);
            _pnlRobotStatusCard.Controls.Add(_lblRobotCommandIdValue);
            _pnlRobotStatusCard.Controls.Add(_lblRobotConnectionLabel);
            _pnlRobotStatusCard.Controls.Add(_lblRobotConnectionStatus);
            _pnlRobotStatusCard.Dock = DockStyle.Top;
            _pnlRobotStatusCard.Location = new Point(15, 15);
            _pnlRobotStatusCard.Margin = new Padding(15, 15, 15, 10);
            _pnlRobotStatusCard.Name = "_pnlRobotStatusCard";
            _pnlRobotStatusCard.Padding = new Padding(20);
            _pnlRobotStatusCard.Size = new Size(1362, 180);
            _pnlRobotStatusCard.TabIndex = 3;
            // 
            // _lblRobotTitle
            // 
            _lblRobotTitle.AutoSize = true;
            _lblRobotTitle.Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold);
            _lblRobotTitle.ForeColor = Color.FromArgb(220, 220, 220);
            _lblRobotTitle.Location = new Point(20, 15);
            _lblRobotTitle.Name = "_lblRobotTitle";
            _lblRobotTitle.Size = new Size(174, 24);
            _lblRobotTitle.TabIndex = 0;
            _lblRobotTitle.Text = "🤖 Robot 系統狀態";
            // 
            // _lblRobotModeLabel
            // 
            _lblRobotModeLabel.AutoSize = true;
            _lblRobotModeLabel.Font = new Font("Microsoft JhengHei UI", 11F);
            _lblRobotModeLabel.ForeColor = Color.FromArgb(150, 150, 150);
            _lblRobotModeLabel.Location = new Point(20, 55);
            _lblRobotModeLabel.Name = "_lblRobotModeLabel";
            _lblRobotModeLabel.Size = new Size(72, 19);
            _lblRobotModeLabel.TabIndex = 1;
            _lblRobotModeLabel.Text = "運行模式:";
            // 
            // _lblRobotModeValue
            // 
            _lblRobotModeValue.AutoSize = true;
            _lblRobotModeValue.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
            _lblRobotModeValue.ForeColor = Color.FromArgb(64, 169, 255);
            _lblRobotModeValue.Location = new Point(120, 55);
            _lblRobotModeValue.Name = "_lblRobotModeValue";
            _lblRobotModeValue.Size = new Size(40, 19);
            _lblRobotModeValue.TabIndex = 2;
            _lblRobotModeValue.Text = "INIT";
            // 
            // _statusLabel
            // 
            _statusLabel.AutoSize = true;
            _statusLabel.Font = new Font("Microsoft JhengHei UI", 10F);
            _statusLabel.ForeColor = Color.FromArgb(150, 150, 150);
            _statusLabel.Location = new Point(953, 21);
            _statusLabel.Name = "_statusLabel";
            _statusLabel.Size = new Size(49, 18);
            _statusLabel.TabIndex = 8;
            _statusLabel.Text = "Ready";
            // 
            // _lblRobotCommandIdLabel
            // 
            _lblRobotCommandIdLabel.AutoSize = true;
            _lblRobotCommandIdLabel.Font = new Font("Microsoft JhengHei UI", 11F);
            _lblRobotCommandIdLabel.ForeColor = Color.FromArgb(150, 150, 150);
            _lblRobotCommandIdLabel.Location = new Point(20, 90);
            _lblRobotCommandIdLabel.Name = "_lblRobotCommandIdLabel";
            _lblRobotCommandIdLabel.Size = new Size(61, 19);
            _lblRobotCommandIdLabel.TabIndex = 3;
            _lblRobotCommandIdLabel.Text = "命令 ID:";
            // 
            // _lblRobotCommandIdValue
            // 
            _lblRobotCommandIdValue.AutoSize = true;
            _lblRobotCommandIdValue.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
            _lblRobotCommandIdValue.ForeColor = Color.FromArgb(220, 220, 220);
            _lblRobotCommandIdValue.Location = new Point(120, 90);
            _lblRobotCommandIdValue.Name = "_lblRobotCommandIdValue";
            _lblRobotCommandIdValue.Size = new Size(25, 19);
            _lblRobotCommandIdValue.TabIndex = 4;
            _lblRobotCommandIdValue.Text = "-1";
            // 
            // _lblRobotConnectionLabel
            // 
            _lblRobotConnectionLabel.AutoSize = true;
            _lblRobotConnectionLabel.Font = new Font("Microsoft JhengHei UI", 11F);
            _lblRobotConnectionLabel.ForeColor = Color.FromArgb(150, 150, 150);
            _lblRobotConnectionLabel.Location = new Point(20, 125);
            _lblRobotConnectionLabel.Name = "_lblRobotConnectionLabel";
            _lblRobotConnectionLabel.Size = new Size(72, 19);
            _lblRobotConnectionLabel.TabIndex = 5;
            _lblRobotConnectionLabel.Text = "連接狀態:";
            // 
            // _lblRobotConnectionStatus
            // 
            _lblRobotConnectionStatus.AutoSize = true;
            _lblRobotConnectionStatus.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
            _lblRobotConnectionStatus.ForeColor = Color.FromArgb(231, 76, 60);
            _lblRobotConnectionStatus.Location = new Point(120, 125);
            _lblRobotConnectionStatus.Name = "_lblRobotConnectionStatus";
            _lblRobotConnectionStatus.Size = new Size(54, 19);
            _lblRobotConnectionStatus.TabIndex = 6;
            _lblRobotConnectionStatus.Text = "未連接";
            // 
            // _robotEngineerPage
            // 
            _robotEngineerPage.BackColor = Color.FromArgb(45, 45, 45);
            _robotEngineerPage.Controls.Add(pnlEngineerScroll);
            _robotEngineerPage.Location = new Point(4, 39);
            _robotEngineerPage.Name = "_robotEngineerPage";
            _robotEngineerPage.Padding = new Padding(15);
            _robotEngineerPage.Size = new Size(1392, 807);
            _robotEngineerPage.TabIndex = 1;
            _robotEngineerPage.Text = "🔧 Robot 工程師";
            // 
            // pnlEngineerScroll
            // 
            pnlEngineerScroll.AutoScroll = true;
            pnlEngineerScroll.BackColor = Color.FromArgb(45, 45, 45);
            pnlEngineerScroll.Controls.Add(_pnlRobotConnectionPanel);
            pnlEngineerScroll.Controls.Add(_pnlSafetyWarningPanel);
            pnlEngineerScroll.Dock = DockStyle.Fill;
            pnlEngineerScroll.Location = new Point(15, 15);
            pnlEngineerScroll.Name = "pnlEngineerScroll";
            pnlEngineerScroll.Padding = new Padding(15);
            pnlEngineerScroll.Size = new Size(1362, 777);
            pnlEngineerScroll.TabIndex = 0;
            // 
            // _pnlRobotConnectionPanel
            // 
            _pnlRobotConnectionPanel.BackColor = Color.FromArgb(55, 55, 55);
            _pnlRobotConnectionPanel.Controls.Add(_lblRobotConnectTitle);
            _pnlRobotConnectionPanel.Controls.Add(_btnRobotDisconnect);
            _pnlRobotConnectionPanel.Controls.Add(_lblRobotConnectionState);
            _pnlRobotConnectionPanel.Dock = DockStyle.Top;
            _pnlRobotConnectionPanel.Location = new Point(15, 95);
            _pnlRobotConnectionPanel.Margin = new Padding(0, 0, 0, 15);
            _pnlRobotConnectionPanel.Name = "_pnlRobotConnectionPanel";
            _pnlRobotConnectionPanel.Padding = new Padding(20);
            _pnlRobotConnectionPanel.Size = new Size(1332, 100);
            _pnlRobotConnectionPanel.TabIndex = 0;
            // 
            // _lblRobotConnectTitle
            // 
            _lblRobotConnectTitle.AutoSize = true;
            _lblRobotConnectTitle.Font = new Font("Microsoft JhengHei UI", 13F, FontStyle.Bold);
            _lblRobotConnectTitle.ForeColor = Color.FromArgb(220, 220, 220);
            _lblRobotConnectTitle.Location = new Point(20, 15);
            _lblRobotConnectTitle.Name = "_lblRobotConnectTitle";
            _lblRobotConnectTitle.Size = new Size(166, 23);
            _lblRobotConnectTitle.TabIndex = 0;
            _lblRobotConnectTitle.Text = "🔌 Robot 連接控制";
            // 
            // _btnRobotDisconnect
            // 
            _btnRobotDisconnect.BackColor = Color.FromArgb(231, 76, 60);
            _btnRobotDisconnect.Enabled = false;
            _btnRobotDisconnect.FlatAppearance.BorderSize = 2;
            _btnRobotDisconnect.FlatStyle = FlatStyle.Flat;
            _btnRobotDisconnect.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
            _btnRobotDisconnect.ForeColor = Color.FromArgb(220, 220, 220);
            _btnRobotDisconnect.Location = new Point(180, 50);
            _btnRobotDisconnect.Name = "_btnRobotDisconnect";
            _btnRobotDisconnect.Size = new Size(140, 40);
            _btnRobotDisconnect.TabIndex = 1;
            _btnRobotDisconnect.Text = "❌ 斷開連接";
            _btnRobotDisconnect.UseVisualStyleBackColor = false;
            // 
            // _lblRobotConnectionState
            // 
            _lblRobotConnectionState.AutoSize = true;
            _lblRobotConnectionState.Font = new Font("Microsoft JhengHei UI", 11F);
            _lblRobotConnectionState.ForeColor = Color.FromArgb(150, 150, 150);
            _lblRobotConnectionState.Location = new Point(350, 55);
            _lblRobotConnectionState.Name = "_lblRobotConnectionState";
            _lblRobotConnectionState.Size = new Size(99, 19);
            _lblRobotConnectionState.TabIndex = 2;
            _lblRobotConnectionState.Text = "狀態：未連接";
            // 
            // _pnlSafetyWarningPanel
            // 
            _pnlSafetyWarningPanel.BackColor = Color.FromArgb(192, 57, 43);
            _pnlSafetyWarningPanel.Controls.Add(_lblSafetyWarningIcon);
            _pnlSafetyWarningPanel.Controls.Add(_lblSafetyWarning);
            _pnlSafetyWarningPanel.Dock = DockStyle.Top;
            _pnlSafetyWarningPanel.Location = new Point(15, 15);
            _pnlSafetyWarningPanel.Margin = new Padding(0, 0, 0, 15);
            _pnlSafetyWarningPanel.Name = "_pnlSafetyWarningPanel";
            _pnlSafetyWarningPanel.Padding = new Padding(20);
            _pnlSafetyWarningPanel.Size = new Size(1332, 80);
            _pnlSafetyWarningPanel.TabIndex = 1;
            // 
            // _lblSafetyWarningIcon
            // 
            _lblSafetyWarningIcon.AutoSize = true;
            _lblSafetyWarningIcon.Font = new Font("Segoe UI Emoji", 24F);
            _lblSafetyWarningIcon.ForeColor = Color.White;
            _lblSafetyWarningIcon.Location = new Point(20, 15);
            _lblSafetyWarningIcon.Name = "_lblSafetyWarningIcon";
            _lblSafetyWarningIcon.Size = new Size(63, 43);
            _lblSafetyWarningIcon.TabIndex = 0;
            _lblSafetyWarningIcon.Text = "⚠️";
            // 
            // _lblSafetyWarning
            // 
            _lblSafetyWarning.AutoSize = true;
            _lblSafetyWarning.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
            _lblSafetyWarning.ForeColor = Color.White;
            _lblSafetyWarning.Location = new Point(124, 20);
            _lblSafetyWarning.Name = "_lblSafetyWarning";
            _lblSafetyWarning.Size = new Size(569, 20);
            _lblSafetyWarning.TabIndex = 1;
            _lblSafetyWarning.Text = "危險操作區域！僅限授權工程師操作。請確保急停按鍵可用，並注意人員安全。";
            // 
            // _visionIntegrationPage
            // 
            _visionIntegrationPage.BackColor = Color.FromArgb(45, 45, 45);
            _visionIntegrationPage.Controls.Add(pnlVisionScroll);
            _visionIntegrationPage.Location = new Point(4, 39);
            _visionIntegrationPage.Name = "_visionIntegrationPage";
            _visionIntegrationPage.Padding = new Padding(15);
            _visionIntegrationPage.Size = new Size(1392, 807);
            _visionIntegrationPage.TabIndex = 2;
            _visionIntegrationPage.Text = "👁️ 視覺虛實整合";
            // 
            // pnlVisionScroll
            // 
            pnlVisionScroll.AutoScroll = true;
            pnlVisionScroll.BackColor = Color.FromArgb(45, 45, 45);
            pnlVisionScroll.Controls.Add(_pnlVisionControlPanel);
            pnlVisionScroll.Controls.Add(_pnlInspectionResultPanel);
            pnlVisionScroll.Controls.Add(_pnlImageDisplayPanel);
            pnlVisionScroll.Controls.Add(_pnlVisionModePanel);
            pnlVisionScroll.Dock = DockStyle.Fill;
            pnlVisionScroll.Location = new Point(15, 15);
            pnlVisionScroll.Name = "pnlVisionScroll";
            pnlVisionScroll.Padding = new Padding(15);
            pnlVisionScroll.Size = new Size(1362, 777);
            pnlVisionScroll.TabIndex = 0;
            // 
            // _pnlVisionControlPanel
            // 
            _pnlVisionControlPanel.BackColor = Color.FromArgb(55, 55, 55);
            _pnlVisionControlPanel.Controls.Add(_lblVisionControlTitle);
            _pnlVisionControlPanel.Controls.Add(_btnStartContinuous);
            _pnlVisionControlPanel.Controls.Add(_btnStopContinuous);
            _pnlVisionControlPanel.Controls.Add(_btnSaveImage);
            _pnlVisionControlPanel.Dock = DockStyle.Top;
            _pnlVisionControlPanel.Location = new Point(15, 665);
            _pnlVisionControlPanel.Margin = new Padding(0, 0, 0, 15);
            _pnlVisionControlPanel.Name = "_pnlVisionControlPanel";
            _pnlVisionControlPanel.Padding = new Padding(20);
            _pnlVisionControlPanel.Size = new Size(1332, 100);
            _pnlVisionControlPanel.TabIndex = 0;
            // 
            // _lblVisionControlTitle
            // 
            _lblVisionControlTitle.AutoSize = true;
            _lblVisionControlTitle.Font = new Font("Microsoft JhengHei UI", 13F, FontStyle.Bold);
            _lblVisionControlTitle.ForeColor = Color.FromArgb(220, 220, 220);
            _lblVisionControlTitle.Location = new Point(20, 15);
            _lblVisionControlTitle.Name = "_lblVisionControlTitle";
            _lblVisionControlTitle.Size = new Size(107, 23);
            _lblVisionControlTitle.TabIndex = 0;
            _lblVisionControlTitle.Text = "📷 視覺控制";
            // 
            // _btnStartContinuous
            // 
            _btnStartContinuous.BackColor = Color.FromArgb(46, 204, 113);
            _btnStartContinuous.FlatAppearance.BorderSize = 2;
            _btnStartContinuous.FlatStyle = FlatStyle.Flat;
            _btnStartContinuous.Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Bold);
            _btnStartContinuous.ForeColor = Color.FromArgb(220, 220, 220);
            _btnStartContinuous.Location = new Point(150, 50);
            _btnStartContinuous.Name = "_btnStartContinuous";
            _btnStartContinuous.Size = new Size(80, 35);
            _btnStartContinuous.TabIndex = 2;
            _btnStartContinuous.Text = "▶ 持續";
            _btnStartContinuous.UseVisualStyleBackColor = false;
            // 
            // _btnStopContinuous
            // 
            _btnStopContinuous.BackColor = Color.FromArgb(231, 76, 60);
            _btnStopContinuous.Enabled = false;
            _btnStopContinuous.FlatAppearance.BorderSize = 2;
            _btnStopContinuous.FlatStyle = FlatStyle.Flat;
            _btnStopContinuous.Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Bold);
            _btnStopContinuous.ForeColor = Color.FromArgb(220, 220, 220);
            _btnStopContinuous.Location = new Point(240, 50);
            _btnStopContinuous.Name = "_btnStopContinuous";
            _btnStopContinuous.Size = new Size(80, 35);
            _btnStopContinuous.TabIndex = 3;
            _btnStopContinuous.Text = "⏹ 停止";
            _btnStopContinuous.UseVisualStyleBackColor = false;
            // 
            // _btnSaveImage
            // 
            _btnSaveImage.BackColor = Color.FromArgb(64, 169, 255);
            _btnSaveImage.FlatAppearance.BorderSize = 2;
            _btnSaveImage.FlatStyle = FlatStyle.Flat;
            _btnSaveImage.Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Bold);
            _btnSaveImage.ForeColor = Color.FromArgb(220, 220, 220);
            _btnSaveImage.Location = new Point(330, 50);
            _btnSaveImage.Name = "_btnSaveImage";
            _btnSaveImage.Size = new Size(80, 35);
            _btnSaveImage.TabIndex = 4;
            _btnSaveImage.Text = "💾 保存";
            _btnSaveImage.UseVisualStyleBackColor = false;
            // 
            // _pnlInspectionResultPanel
            // 
            _pnlInspectionResultPanel.BackColor = Color.FromArgb(55, 55, 55);
            _pnlInspectionResultPanel.Controls.Add(_lblInspectionTitle);
            _pnlInspectionResultPanel.Controls.Add(_lblInspectionSuccessLabel);
            _pnlInspectionResultPanel.Controls.Add(_lblInspectionSuccessValue);
            _pnlInspectionResultPanel.Controls.Add(_lblBarcodeResultLabel);
            _pnlInspectionResultPanel.Controls.Add(_lblBarcodeResultValue);
            _pnlInspectionResultPanel.Controls.Add(_lblConfidenceLabel);
            _pnlInspectionResultPanel.Controls.Add(_lblConfidenceValue);
            _pnlInspectionResultPanel.Dock = DockStyle.Top;
            _pnlInspectionResultPanel.Location = new Point(15, 515);
            _pnlInspectionResultPanel.Margin = new Padding(0, 0, 0, 15);
            _pnlInspectionResultPanel.Name = "_pnlInspectionResultPanel";
            _pnlInspectionResultPanel.Padding = new Padding(20);
            _pnlInspectionResultPanel.Size = new Size(1332, 150);
            _pnlInspectionResultPanel.TabIndex = 1;
            // 
            // _lblInspectionTitle
            // 
            _lblInspectionTitle.AutoSize = true;
            _lblInspectionTitle.Font = new Font("Microsoft JhengHei UI", 13F, FontStyle.Bold);
            _lblInspectionTitle.ForeColor = Color.FromArgb(220, 220, 220);
            _lblInspectionTitle.Location = new Point(20, 15);
            _lblInspectionTitle.Name = "_lblInspectionTitle";
            _lblInspectionTitle.Size = new Size(98, 23);
            _lblInspectionTitle.TabIndex = 0;
            _lblInspectionTitle.Text = "✓ 檢驗結果";
            // 
            // _lblInspectionSuccessLabel
            // 
            _lblInspectionSuccessLabel.AutoSize = true;
            _lblInspectionSuccessLabel.Font = new Font("Microsoft JhengHei UI", 11F);
            _lblInspectionSuccessLabel.ForeColor = Color.FromArgb(150, 150, 150);
            _lblInspectionSuccessLabel.Location = new Point(20, 50);
            _lblInspectionSuccessLabel.Name = "_lblInspectionSuccessLabel";
            _lblInspectionSuccessLabel.Size = new Size(42, 19);
            _lblInspectionSuccessLabel.TabIndex = 1;
            _lblInspectionSuccessLabel.Text = "合格:";
            // 
            // _lblInspectionSuccessValue
            // 
            _lblInspectionSuccessValue.AutoSize = true;
            _lblInspectionSuccessValue.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
            _lblInspectionSuccessValue.ForeColor = Color.FromArgb(220, 220, 220);
            _lblInspectionSuccessValue.Location = new Point(100, 50);
            _lblInspectionSuccessValue.Name = "_lblInspectionSuccessValue";
            _lblInspectionSuccessValue.Size = new Size(39, 19);
            _lblInspectionSuccessValue.TabIndex = 2;
            _lblInspectionSuccessValue.Text = "N/A";
            // 
            // _lblBarcodeResultLabel
            // 
            _lblBarcodeResultLabel.AutoSize = true;
            _lblBarcodeResultLabel.Font = new Font("Microsoft JhengHei UI", 11F);
            _lblBarcodeResultLabel.ForeColor = Color.FromArgb(150, 150, 150);
            _lblBarcodeResultLabel.Location = new Point(20, 80);
            _lblBarcodeResultLabel.Name = "_lblBarcodeResultLabel";
            _lblBarcodeResultLabel.Size = new Size(42, 19);
            _lblBarcodeResultLabel.TabIndex = 3;
            _lblBarcodeResultLabel.Text = "條碼:";
            // 
            // _lblBarcodeResultValue
            // 
            _lblBarcodeResultValue.AutoSize = true;
            _lblBarcodeResultValue.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
            _lblBarcodeResultValue.ForeColor = Color.FromArgb(220, 220, 220);
            _lblBarcodeResultValue.Location = new Point(100, 80);
            _lblBarcodeResultValue.Name = "_lblBarcodeResultValue";
            _lblBarcodeResultValue.Size = new Size(39, 19);
            _lblBarcodeResultValue.TabIndex = 4;
            _lblBarcodeResultValue.Text = "N/A";
            // 
            // _lblConfidenceLabel
            // 
            _lblConfidenceLabel.AutoSize = true;
            _lblConfidenceLabel.Font = new Font("Microsoft JhengHei UI", 11F);
            _lblConfidenceLabel.ForeColor = Color.FromArgb(150, 150, 150);
            _lblConfidenceLabel.Location = new Point(20, 110);
            _lblConfidenceLabel.Name = "_lblConfidenceLabel";
            _lblConfidenceLabel.Size = new Size(57, 19);
            _lblConfidenceLabel.TabIndex = 5;
            _lblConfidenceLabel.Text = "信心度:";
            // 
            // _lblConfidenceValue
            // 
            _lblConfidenceValue.AutoSize = true;
            _lblConfidenceValue.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
            _lblConfidenceValue.ForeColor = Color.FromArgb(220, 220, 220);
            _lblConfidenceValue.Location = new Point(100, 110);
            _lblConfidenceValue.Name = "_lblConfidenceValue";
            _lblConfidenceValue.Size = new Size(32, 19);
            _lblConfidenceValue.TabIndex = 6;
            _lblConfidenceValue.Text = "0%";
            // 
            // _pnlImageDisplayPanel
            // 
            _pnlImageDisplayPanel.BackColor = Color.FromArgb(55, 55, 55);
            _pnlImageDisplayPanel.Controls.Add(_lblImageTitle);
            _pnlImageDisplayPanel.Controls.Add(_pnlImageContainer);
            _pnlImageDisplayPanel.Dock = DockStyle.Top;
            _pnlImageDisplayPanel.Location = new Point(15, 115);
            _pnlImageDisplayPanel.Margin = new Padding(0, 0, 0, 15);
            _pnlImageDisplayPanel.Name = "_pnlImageDisplayPanel";
            _pnlImageDisplayPanel.Padding = new Padding(20);
            _pnlImageDisplayPanel.Size = new Size(1332, 400);
            _pnlImageDisplayPanel.TabIndex = 2;
            // 
            // _lblImageTitle
            // 
            _lblImageTitle.AutoSize = true;
            _lblImageTitle.Font = new Font("Microsoft JhengHei UI", 13F, FontStyle.Bold);
            _lblImageTitle.ForeColor = Color.FromArgb(220, 220, 220);
            _lblImageTitle.Location = new Point(20, 15);
            _lblImageTitle.Name = "_lblImageTitle";
            _lblImageTitle.Size = new Size(107, 23);
            _lblImageTitle.TabIndex = 0;
            _lblImageTitle.Text = "🖼️ 影像捕取";
            // 
            // _pnlImageContainer
            // 
            _pnlImageContainer.BackColor = Color.FromArgb(35, 35, 35);
            _pnlImageContainer.BorderStyle = BorderStyle.FixedSingle;
            _pnlImageContainer.Controls.Add(_imgCapturedImage);
            _pnlImageContainer.Controls.Add(_lblNoImagePlaceholder);
            _pnlImageContainer.Location = new Point(20, 50);
            _pnlImageContainer.Name = "_pnlImageContainer";
            _pnlImageContainer.Size = new Size(350, 320);
            _pnlImageContainer.TabIndex = 1;
            // 
            // _imgCapturedImage
            // 
            _imgCapturedImage.BackColor = Color.FromArgb(35, 35, 35);
            _imgCapturedImage.Dock = DockStyle.Fill;
            _imgCapturedImage.Location = new Point(0, 0);
            _imgCapturedImage.Name = "_imgCapturedImage";
            _imgCapturedImage.Size = new Size(348, 318);
            _imgCapturedImage.SizeMode = PictureBoxSizeMode.Zoom;
            _imgCapturedImage.TabIndex = 0;
            _imgCapturedImage.TabStop = false;
            // 
            // _lblNoImagePlaceholder
            // 
            _lblNoImagePlaceholder.Dock = DockStyle.Fill;
            _lblNoImagePlaceholder.Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold);
            _lblNoImagePlaceholder.ForeColor = Color.FromArgb(150, 150, 150);
            _lblNoImagePlaceholder.Location = new Point(0, 0);
            _lblNoImagePlaceholder.Name = "_lblNoImagePlaceholder";
            _lblNoImagePlaceholder.Size = new Size(348, 318);
            _lblNoImagePlaceholder.TabIndex = 1;
            _lblNoImagePlaceholder.Text = "[無影像]";
            _lblNoImagePlaceholder.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _pnlVisionModePanel
            // 
            _pnlVisionModePanel.BackColor = Color.FromArgb(55, 55, 55);
            _pnlVisionModePanel.Controls.Add(_lblVisionModeTitle);
            _pnlVisionModePanel.Controls.Add(_rbVisionModeSimulation);
            _pnlVisionModePanel.Controls.Add(_rbVisionModeReal);
            _pnlVisionModePanel.Controls.Add(_lblVisionModeStatus);
            _pnlVisionModePanel.Dock = DockStyle.Top;
            _pnlVisionModePanel.Location = new Point(15, 15);
            _pnlVisionModePanel.Margin = new Padding(0, 0, 0, 15);
            _pnlVisionModePanel.Name = "_pnlVisionModePanel";
            _pnlVisionModePanel.Padding = new Padding(20);
            _pnlVisionModePanel.Size = new Size(1332, 100);
            _pnlVisionModePanel.TabIndex = 3;
            // 
            // _lblVisionModeTitle
            // 
            _lblVisionModeTitle.AutoSize = true;
            _lblVisionModeTitle.Font = new Font("Microsoft JhengHei UI", 13F, FontStyle.Bold);
            _lblVisionModeTitle.ForeColor = Color.FromArgb(220, 220, 220);
            _lblVisionModeTitle.Location = new Point(20, 15);
            _lblVisionModeTitle.Name = "_lblVisionModeTitle";
            _lblVisionModeTitle.Size = new Size(143, 23);
            _lblVisionModeTitle.TabIndex = 0;
            _lblVisionModeTitle.Text = "👀 視覺模式選擇";
            // 
            // _rbVisionModeSimulation
            // 
            _rbVisionModeSimulation.AutoSize = true;
            _rbVisionModeSimulation.Checked = true;
            _rbVisionModeSimulation.Font = new Font("Microsoft JhengHei UI", 11F);
            _rbVisionModeSimulation.ForeColor = Color.FromArgb(220, 220, 220);
            _rbVisionModeSimulation.Location = new Point(20, 50);
            _rbVisionModeSimulation.Name = "_rbVisionModeSimulation";
            _rbVisionModeSimulation.Size = new Size(87, 23);
            _rbVisionModeSimulation.TabIndex = 1;
            _rbVisionModeSimulation.TabStop = true;
            _rbVisionModeSimulation.Text = "模擬模式";
            // 
            // _rbVisionModeReal
            // 
            _rbVisionModeReal.AutoSize = true;
            _rbVisionModeReal.Font = new Font("Microsoft JhengHei UI", 11F);
            _rbVisionModeReal.ForeColor = Color.FromArgb(220, 220, 220);
            _rbVisionModeReal.Location = new Point(200, 50);
            _rbVisionModeReal.Name = "_rbVisionModeReal";
            _rbVisionModeReal.Size = new Size(87, 23);
            _rbVisionModeReal.TabIndex = 2;
            _rbVisionModeReal.Text = "實際硬體";
            // 
            // _lblVisionModeStatus
            // 
            _lblVisionModeStatus.AutoSize = true;
            _lblVisionModeStatus.Font = new Font("Microsoft JhengHei UI", 10F);
            _lblVisionModeStatus.ForeColor = Color.FromArgb(64, 169, 255);
            _lblVisionModeStatus.Location = new Point(20, 75);
            _lblVisionModeStatus.Name = "_lblVisionModeStatus";
            _lblVisionModeStatus.Size = new Size(99, 18);
            _lblVisionModeStatus.TabIndex = 3;
            _lblVisionModeStatus.Text = "當前: 模擬模式";
            // 
            // _simulationIndicatorLabel
            // 
            _simulationIndicatorLabel.AutoSize = true;
            _simulationIndicatorLabel.BackColor = Color.LightGreen;
            _simulationIndicatorLabel.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold);
            _simulationIndicatorLabel.Location = new Point(1100, 10);
            _simulationIndicatorLabel.Name = "_simulationIndicatorLabel";
            _simulationIndicatorLabel.Padding = new Padding(10, 5, 10, 5);
            _simulationIndicatorLabel.Size = new Size(105, 25);
            _simulationIndicatorLabel.TabIndex = 7;
            _simulationIndicatorLabel.Text = "SIMULATION";
            _simulationIndicatorLabel.Visible = false;
            // 
            // _alarmIndicatorLabel
            // 
            _alarmIndicatorLabel.AutoSize = true;
            _alarmIndicatorLabel.BackColor = Color.Transparent;
            _alarmIndicatorLabel.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold);
            _alarmIndicatorLabel.Location = new Point(950, 10);
            _alarmIndicatorLabel.Name = "_alarmIndicatorLabel";
            _alarmIndicatorLabel.Padding = new Padding(10, 5, 10, 5);
            _alarmIndicatorLabel.Size = new Size(71, 25);
            _alarmIndicatorLabel.TabIndex = 6;
            _alarmIndicatorLabel.Text = "ALARM";
            // 
            // _stopButton
            // 
            _stopButton.Enabled = false;
            _stopButton.FlatAppearance.BorderSize = 2;
            _stopButton.FlatStyle = FlatStyle.Flat;
            _stopButton.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
            _stopButton.ForeColor = Color.FromArgb(220, 220, 220);
            _stopButton.Location = new Point(1350, 10);
            _stopButton.Name = "_stopButton";
            _stopButton.Size = new Size(140, 50);
            _stopButton.TabIndex = 5;
            _stopButton.Text = "⏹ 停止";
            _stopButton.UseVisualStyleBackColor = true;
            // 
            // _lblAlarmCode
            // 
            _lblAlarmCode.AutoSize = true;
            _lblAlarmCode.Font = new Font("Microsoft JhengHei UI", 10F);
            _lblAlarmCode.ForeColor = Color.FromArgb(150, 150, 150);
            _lblAlarmCode.Location = new Point(953, 57);
            _lblAlarmCode.Name = "_lblAlarmCode";
            _lblAlarmCode.Size = new Size(49, 18);
            _lblAlarmCode.TabIndex = 8;
            _lblAlarmCode.Text = "Ready";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1400, 850);
            Controls.Add(_stopButton);
            Controls.Add(_alarmIndicatorLabel);
            Controls.Add(_simulationIndicatorLabel);
            Controls.Add(_mainTabControl);
            MinimumSize = new Size(1200, 750);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Shoe Mold Control System - Industrial 3-Tab Edition";
            _mainTabControl.ResumeLayout(false);
            _robotVisionMonitorPage.ResumeLayout(false);
            _pnlAlarmSummaryCard.ResumeLayout(false);
            _pnlAlarmSummaryCard.PerformLayout();
            _pnlSystemOverviewCard.ResumeLayout(false);
            _pnlSystemOverviewCard.PerformLayout();
            _pnlVisionStatusCard.ResumeLayout(false);
            _pnlVisionStatusCard.PerformLayout();
            _pnlRobotStatusCard.ResumeLayout(false);
            _pnlRobotStatusCard.PerformLayout();
            _robotEngineerPage.ResumeLayout(false);
            pnlEngineerScroll.ResumeLayout(false);
            _pnlRobotConnectionPanel.ResumeLayout(false);
            _pnlRobotConnectionPanel.PerformLayout();
            _pnlSafetyWarningPanel.ResumeLayout(false);
            _pnlSafetyWarningPanel.PerformLayout();
            _visionIntegrationPage.ResumeLayout(false);
            pnlVisionScroll.ResumeLayout(false);
            _pnlVisionControlPanel.ResumeLayout(false);
            _pnlVisionControlPanel.PerformLayout();
            _pnlInspectionResultPanel.ResumeLayout(false);
            _pnlInspectionResultPanel.PerformLayout();
            _pnlImageDisplayPanel.ResumeLayout(false);
            _pnlImageDisplayPanel.PerformLayout();
            _pnlImageContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_imgCapturedImage).EndInit();
            _pnlVisionModePanel.ResumeLayout(false);
            _pnlVisionModePanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        // 宣告
        private TabControl _mainTabControl;
        private TabPage _robotVisionMonitorPage;
        private TabPage _robotEngineerPage;
        private TabPage _visionIntegrationPage;
        private Label _statusLabel;
        private Label _simulationIndicatorLabel;
        private Label _alarmIndicatorLabel;
        private Industrial.UI.Framework.IndustrialSafeButton _startButton;
        private Button _stopButton;

        // 分頁 1
        private Panel _pnlRobotStatusCard;
        private Label _lblRobotTitle;
        private Label _lblRobotModeLabel;
        private Label _lblRobotModeValue;
        private Label _lblRobotCommandIdLabel;
        private Label _lblRobotCommandIdValue;
        private Label _lblRobotConnectionLabel;
        private Label _lblRobotConnectionStatus;

        private Panel _pnlVisionStatusCard;
        private Label _lblVisionTitle;
        private Label _lblVisionModeLabel;
        private Label _lblVisionModeValue;
        private Label _lblLastBarcodeLabel;
        private Label _lblLastBarcodeValue;
        private Label _lblVisionConnectionLabel;
        private Label _lblVisionConnectionStatus;

        private Panel _pnlSystemOverviewCard;
        private Label _lblSystemStatusTitle;
        private Label _lblOverallStatusLabel;
        private Label _lblOverallStatusValue;
        private Label _lblProductionCountLabel;
        private Label _lblProductionCountValue;
        private Label _lblLastUpdateTimeLabel;
        private Label _lblLastUpdateTimeValue;
        private Label _lblTemperatureLabel;
        private Label _lblTemperature;
        private Label _lblPressureLabel;
        private Label _lblPressure;

        private Panel _pnlAlarmSummaryCard;
        private Label lblAlarmSummaryTitle;
        private ListBox alarmListBox;

        // 分頁 2
        private Panel pnlEngineerScroll;
        private Panel _pnlSafetyWarningPanel;
        private Label _lblSafetyWarningIcon;
        private Label _lblSafetyWarning;
        private Panel _pnlRobotConnectionPanel;
        private Label _lblRobotConnectTitle;
        private Industrial.UI.Framework.IndustrialSafeButton _btnRobotConnect;
        private Button _btnRobotDisconnect;
        private Label _lblRobotConnectionState;

        // 分頁 3
        private Panel pnlVisionScroll;
        private Panel _pnlVisionModePanel;
        private Label _lblVisionModeTitle;
        private RadioButton _rbVisionModeSimulation;
        private RadioButton _rbVisionModeReal;
        private Label _lblVisionModeStatus;

        private Panel _pnlImageDisplayPanel;
        private Label _lblImageTitle;
        private Panel _pnlImageContainer;
        private PictureBox _imgCapturedImage;
        private Label _lblNoImagePlaceholder;

        private Panel _pnlInspectionResultPanel;
        private Label _lblInspectionTitle;
        private Label _lblInspectionSuccessLabel;
        private Label _lblInspectionSuccessValue;
        private Label _lblBarcodeResultLabel;
        private Label _lblBarcodeResultValue;
        private Label _lblConfidenceLabel;
        private Label _lblConfidenceValue;

        private Panel _pnlVisionControlPanel;
        private Label _lblVisionControlTitle;
        private Industrial.UI.Framework.IndustrialSafeButton _btnCaptureSingle;
        private Button _btnStartContinuous;
        private Button _btnStopContinuous;
        private Button _btnSaveImage;
        private Label _lblAlarmCode;
    }
}