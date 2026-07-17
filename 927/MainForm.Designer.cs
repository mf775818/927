namespace _927
{
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
            // 主分頁控制項 - 三分頁架構
            _mainTabControl = new TabControl();
            _robotVisionMonitorPage = new TabPage();
            _robotEngineerPage = new TabPage();
            _visionIntegrationPage = new TabPage();
            
            // 頂部狀態列
            _statusLabel = new Label();
            _simulationIndicatorLabel = new Label();
            _alarmIndicatorLabel = new Label();
            _startButton = new Industrial.UI.Framework.IndustrialSafeButton();
            _stopButton = new Button();
            
            SuspendLayout();
            
            // _mainTabControl
            _mainTabControl.Dock = DockStyle.Fill;
            _mainTabControl.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Regular);
            _mainTabControl.Location = new Point(0, 0);
            _mainTabControl.Name = "_mainTabControl";
            _mainTabControl.SelectedIndex = 0;
            _mainTabControl.Size = new Size(1400, 800);
            _mainTabControl.TabIndex = 0;
            _mainTabControl.Appearance = TabAppearance.FlatButtons;
            _mainTabControl.ItemSize = new Size(150, 35);
            _mainTabControl.SizeMode = TabSizeMode.Fixed;
            
            // _robotVisionMonitorPage
            _robotVisionMonitorPage.BackColor = Color.FromArgb(45, 45, 45);
            _robotVisionMonitorPage.Location = new Point(4, 39);
            _robotVisionMonitorPage.Name = "_robotVisionMonitorPage";
            _robotVisionMonitorPage.Padding = new Padding(15);
            _robotVisionMonitorPage.Size = new Size(1392, 757);
            _robotVisionMonitorPage.TabIndex = 0;
            _robotVisionMonitorPage.Text = "🤖 Robot+ 視覺監控";
            
            // _robotEngineerPage
            _robotEngineerPage.BackColor = Color.FromArgb(45, 45, 45);
            _robotEngineerPage.Location = new Point(4, 39);
            _robotEngineerPage.Name = "_robotEngineerPage";
            _robotEngineerPage.Padding = new Padding(15);
            _robotEngineerPage.Size = new Size(1392, 757);
            _robotEngineerPage.TabIndex = 1;
            _robotEngineerPage.Text = "🔧 Robot 工程師";
            
            // _visionIntegrationPage
            _visionIntegrationPage.BackColor = Color.FromArgb(45, 45, 45);
            _visionIntegrationPage.Location = new Point(4, 39);
            _visionIntegrationPage.Name = "_visionIntegrationPage";
            _visionIntegrationPage.Padding = new Padding(15);
            _visionIntegrationPage.Size = new Size(1392, 757);
            _visionIntegrationPage.TabIndex = 2;
            _visionIntegrationPage.Text = "👁️ 視覺虛實整合";
            
            // 頂部狀態列
            _statusLabel.AutoSize = true;
            _statusLabel.Font = new Font("Microsoft JhengHei UI", 10F);
            _statusLabel.ForeColor = Color.FromArgb(150, 150, 150);
            _statusLabel.Location = new Point(20, 10);
            _statusLabel.Name = "_statusLabel";
            _statusLabel.Size = new Size(67, 18);
            _statusLabel.Text = "Ready";
            
            _simulationIndicatorLabel.AutoSize = true;
            _simulationIndicatorLabel.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold);
            _simulationIndicatorLabel.Location = new Point(1100, 10);
            _simulationIndicatorLabel.Padding = new Padding(10, 5, 10, 5);
            _simulationIndicatorLabel.Size = new Size(120, 25);
            _simulationIndicatorLabel.Text = "SIMULATION";
            _simulationIndicatorLabel.BackColor = Color.LightGreen;
            _simulationIndicatorLabel.Visible = false;
            
            _alarmIndicatorLabel.AutoSize = true;
            _alarmIndicatorLabel.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold);
            _alarmIndicatorLabel.Location = new Point(950, 10);
            _alarmIndicatorLabel.Padding = new Padding(10, 5, 10, 5);
            _alarmIndicatorLabel.Size = new Size(100, 25);
            _alarmIndicatorLabel.Text = "ALARM";
            _alarmIndicatorLabel.BackColor = Color.Transparent;
            
            // _startButton
            _startButton.RequireLongPress = true;
            _startButton.TargetHoldTimeMs = 2000;
            _startButton.FlatAppearance.BorderSize = 2;
            _startButton.FlatStyle = FlatStyle.Flat;
            _startButton.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
            _startButton.ForeColor = Color.FromArgb(220, 220, 220);
            _startButton.Location = new Point(1200, 10);
            _startButton.MinimumSize = new Size(120, 50);
            _startButton.Name = "_startButton";
            _startButton.Size = new Size(140, 50);
            _startButton.TabIndex = 4;
            _startButton.Text = "▶ 啟動";
            _startButton.UseVisualStyleBackColor = true;
            
            // _stopButton
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
            
            // MainForm
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1400, 850);
            Controls.Add(_stopButton);
            Controls.Add(_startButton);
            Controls.Add(_alarmIndicatorLabel);
            Controls.Add(_simulationIndicatorLabel);
            Controls.Add(_statusLabel);
            Controls.Add(_mainTabControl);
            MinimumSize = new Size(1200, 750);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Shoe Mold Control System - Industrial 3-Tab Edition";
            FormClosing += MainForm_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        // ========== 主分頁控制項 ==========
        private TabControl _mainTabControl;
        private TabPage _robotVisionMonitorPage;
        private TabPage _robotEngineerPage;
        private TabPage _visionIntegrationPage;
        
        // ========== 頂部狀態列 ==========
        private Label _statusLabel;
        private Label _simulationIndicatorLabel;
        private Label _alarmIndicatorLabel;
        private Industrial.UI.Framework.IndustrialSafeButton _startButton;
        private Button _stopButton;
        
        // ========== 分頁 1: Robot+ 視覺監控 ==========
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
        private Label _lblAlarmTitle;
        private Label _lblActiveAlarmLabel;
        private Label _lblActiveAlarmValue;
        private ListView _alarmListViewMonitor;
        private ColumnHeader _colAlarmTime;
        private ColumnHeader _colAlarmMessage;
        private ColumnHeader _colAlarmCount;
        private ColumnHeader _colAlarmLevel;
        
        // ========== 分頁 2: Robot 工程師 ==========
        private Panel _pnlSafetyWarningPanel;
        private Label _lblSafetyWarningIcon;
        private Label _lblSafetyWarning;
        
        private Panel _pnlRobotConnectionPanel;
        private Label _lblRobotConnectTitle;
        private Industrial.UI.Framework.IndustrialSafeButton _btnRobotConnect;
        private Button _btnRobotDisconnect;
        private Label _lblRobotConnectionState;
        
        private Panel _pnlJogControlPanel;
        private Label _lblJogTitle;
        private Label _lblJogWarning;
        private FlowLayoutPanel _pnlJogButtons;
        private Button _btnJogPlusX, _btnJogMinusX;
        private Button _btnJogPlusY, _btnJogMinusY;
        private Button _btnJogPlusZ, _btnJogMinusZ;
        private Button _btnJogPlusR, _btnJogMinusR;
        private Industrial.UI.Framework.IndustrialSafeButton _btnJogStop;
        private CheckBox _chkJogContinuous;
        private Label _lblJogSpeedLabel;
        private NumericUpDown _numJogSpeed;
        private Label _lblJogSpeedUnit;
        
        private Panel _pnlPoseDisplayPanel;
        private Label _lblPoseTitle;
        private Label _lblPoseXLabel, _lblPoseXValue;
        private Label _lblPoseYLabel, _lblPoseYValue;
        private Label _lblPoseZLabel, _lblPoseZValue;
        private Label _lblPoseRLabel, _lblPoseRValue;
        private Button _btnRefreshPose;
        
        private Panel _pnlCommandPanel;
        private Label _lblCommandTitle;
        private TextBox _txtCommandInput;
        private Industrial.UI.Framework.IndustrialSafeButton _btnExecuteCommand;
        private ListBox _lstCommandHistory;
        
        // ========== 分頁 3: 視覺虛實整合 ==========
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
        private Label _lblMarkPositionsLabel;
        private Label _lblMarkPositionsValue;
        
        private Panel _pnlVisionControlPanel;
        private Label _lblVisionControlTitle;
        private Industrial.UI.Framework.IndustrialSafeButton _btnCaptureSingle;
        private Button _btnStartContinuous;
        private Button _btnStopContinuous;
        private Button _btnSaveImage;
        
        private Panel _pnlSimulationControlPanel;
        private Label _lblSimControlTitle;
        private Label _lblSimulatedPoseLabel;
        private NumericUpDown _numSimulatedX;
        private NumericUpDown _numSimulatedY;
        private NumericUpDown _numSimulatedZ;
        private NumericUpDown _numSimulatedR;
        private Button _btnApplyImage;
        private CheckBox _chkSimulateBarcode;
        private TextBox _txtSimulatedBarcode;
    }
}
