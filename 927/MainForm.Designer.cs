namespace _927
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _statusLabel = new Label();
            _mainTabControl = new TabControl();
            _dashboardPage = new TabPage();
            _alarmPage = new TabPage();
            _settingsPage = new TabPage();
            _trendPage = new TabPage();
            _simulationIndicatorLabel = new Label();
            _alarmIndicatorLabel = new Label();
            
            // 儀表板頁面控制項
            _lblTemperature = new Label();
            _lblPressure = new Label();
            _lblProductionCount = new Label();
            _lblStatus = new Label();
            _lblLastUpdate = new Label();
            _lblAlarmCode = new Label();
            _tempValueLabel = new Label();
            _pressureValueLabel = new Label();
            _countValueLabel = new Label();
            
            // 警報頁面控制項
            _alarmListView = new ListView();
            
            // 設定頁面控制項
            _btnNightMode = new Industrial.UI.Framework.IndustrialSafeButton();
            _btnStartSimulation = new Button();
            _startButton = new Industrial.UI.Framework.IndustrialSafeButton();
            _stopButton = new Button();
            
            SuspendLayout();
            
            // 
            // _mainTabControl
            // 
            _mainTabControl.Dock = DockStyle.Fill;
            _mainTabControl.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular);
            _mainTabControl.Location = new Point(0, 0);
            _mainTabControl.Name = "_mainTabControl";
            _mainTabControl.SelectedIndex = 0;
            _mainTabControl.Size = new Size(1024, 650);
            _mainTabControl.TabIndex = 0;
            _mainTabControl.TabStop = false;
            
            // 
            // _dashboardPage - 監控儀表板
            // 
            _dashboardPage.BackColor = Color.FromArgb(45, 45, 45);
            _dashboardPage.Controls.Add(_lblAlarmCode);
            _dashboardPage.Controls.Add(_lblLastUpdate);
            _dashboardPage.Controls.Add(_lblStatus);
            _dashboardPage.Controls.Add(_countValueLabel);
            _dashboardPage.Controls.Add(_pressureValueLabel);
            _dashboardPage.Controls.Add(_tempValueLabel);
            _dashboardPage.Controls.Add(_lblProductionCount);
            _dashboardPage.Controls.Add(_lblPressure);
            _dashboardPage.Controls.Add(_lblTemperature);
            _dashboardPage.Font = new Font("Microsoft JhengHei UI", 12F);
            _dashboardPage.Location = new Point(4, 31);
            _dashboardPage.Name = "_dashboardPage";
            _dashboardPage.Padding = new Padding(20);
            _dashboardPage.Size = new Size(1016, 615);
            _dashboardPage.TabIndex = 0;
            _dashboardPage.Text = "📊 監控儀表板";
            _dashboardPage.UseVisualStyleBackColor = true;
            
            // 
            // _lblTemperature
            // 
            _lblTemperature.AutoSize = true;
            _lblTemperature.Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold);
            _lblTemperature.ForeColor = Color.FromArgb(220, 220, 220);
            _lblTemperature.Location = new Point(30, 30);
            _lblTemperature.Name = "_lblTemperature";
            _lblTemperature.Size = new Size(120, 25);
            _lblTemperature.TabIndex = 0;
            _lblTemperature.Text = "爐溫監控:";
            
            // 
            // _tempValueLabel
            // 
            _tempValueLabel.AutoSize = true;
            _tempValueLabel.Font = new Font("Consolas", 18F, FontStyle.Bold);
            _tempValueLabel.ForeColor = Color.FromArgb(64, 169, 255);
            _tempValueLabel.Location = new Point(160, 25);
            _tempValueLabel.Name = "_tempValueLabel";
            _tempValueLabel.Size = new Size(140, 28);
            _tempValueLabel.TabIndex = 1;
            _tempValueLabel.Text = "25.00 °C";
            
            // 
            // _lblPressure
            // 
            _lblPressure.AutoSize = true;
            _lblPressure.Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold);
            _lblPressure.ForeColor = Color.FromArgb(220, 220, 220);
            _lblPressure.Location = new Point(30, 80);
            _lblPressure.Name = "_lblPressure";
            _lblPressure.Size = new Size(120, 25);
            _lblPressure.TabIndex = 2;
            _lblPressure.Text = "氣壓監控:";
            
            // 
            // _pressureValueLabel
            // 
            _pressureValueLabel.AutoSize = true;
            _pressureValueLabel.Font = new Font("Consolas", 18F, FontStyle.Bold);
            _pressureValueLabel.ForeColor = Color.FromArgb(64, 169, 255);
            _pressureValueLabel.Location = new Point(160, 75);
            _pressureValueLabel.Name = "_pressureValueLabel";
            _pressureValueLabel.Size = new Size(140, 28);
            _pressureValueLabel.TabIndex = 3;
            _pressureValueLabel.Text = "0.000 MPa";
            
            // 
            // _lblProductionCount
            // 
            _lblProductionCount.AutoSize = true;
            _lblProductionCount.Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold);
            _lblProductionCount.ForeColor = Color.FromArgb(220, 220, 220);
            _lblProductionCount.Location = new Point(30, 130);
            _lblProductionCount.Name = "_lblProductionCount";
            _lblProductionCount.Size = new Size(120, 25);
            _lblProductionCount.TabIndex = 4;
            _lblProductionCount.Text = "生產計數:";
            
            // 
            // _countValueLabel
            // 
            _countValueLabel.AutoSize = true;
            _countValueLabel.Font = new Font("Consolas", 18F, FontStyle.Bold);
            _countValueLabel.ForeColor = Color.FromArgb(39, 174, 96);
            _countValueLabel.Location = new Point(160, 125);
            _countValueLabel.Name = "_countValueLabel";
            _countValueLabel.Size = new Size(100, 28);
            _countValueLabel.TabIndex = 5;
            _countValueLabel.Text = "0";
            
            // 
            // _lblStatus
            // 
            _lblStatus.AutoSize = true;
            _lblStatus.Font = new Font("Microsoft JhengHei UI", 16F, FontStyle.Bold);
            _lblStatus.ForeColor = Color.FromArgb(150, 150, 150);
            _lblStatus.Location = new Point(30, 190);
            _lblStatus.Name = "_lblStatus";
            _lblStatus.Size = new Size(100, 28);
            _lblStatus.TabIndex = 6;
            _lblStatus.Text = "待機中";
            
            // 
            // _lblLastUpdate
            // 
            _lblLastUpdate.AutoSize = true;
            _lblLastUpdate.Font = new Font("Microsoft JhengHei UI", 10F);
            _lblLastUpdate.ForeColor = Color.FromArgb(150, 150, 150);
            _lblLastUpdate.Location = new Point(30, 230);
            _lblLastUpdate.Name = "_lblLastUpdate";
            _lblLastUpdate.Size = new Size(150, 18);
            _lblLastUpdate.TabIndex = 7;
            _lblLastUpdate.Text = "更新時間：--:--:--";
            
            // 
            // _lblAlarmCode
            // 
            _lblAlarmCode.AutoSize = true;
            _lblAlarmCode.Font = new Font("Consolas", 20F, FontStyle.Bold);
            _lblAlarmCode.ForeColor = Color.FromArgb(231, 76, 60);
            _lblAlarmCode.Location = new Point(30, 280);
            _lblAlarmCode.Name = "_lblAlarmCode";
            _lblAlarmCode.Size = new Size(180, 32);
            _lblAlarmCode.TabIndex = 8;
            _lblAlarmCode.Text = "ALM-000";
            _lblAlarmCode.Visible = false;
            
            // 
            // _alarmPage - 警報中心
            // 
            _alarmPage.BackColor = Color.FromArgb(45, 45, 45);
            _alarmPage.Controls.Add(_alarmListView);
            _alarmPage.Font = new Font("Microsoft JhengHei UI", 12F);
            _alarmPage.Location = new Point(4, 31);
            _alarmPage.Name = "_alarmPage";
            _alarmPage.Padding = new Padding(20);
            _alarmPage.Size = new Size(1016, 615);
            _alarmPage.TabIndex = 1;
            _alarmPage.Text = "🔔 警報中心";
            _alarmPage.UseVisualStyleBackColor = true;
            
            // 
            // _alarmListView
            // 
            _alarmListView.BackColor = Color.FromArgb(35, 35, 35);
            _alarmListView.Columns.AddRange(new ColumnHeader[] {
                new ColumnHeader() { Text = "時間", Width = 120 },
                new ColumnHeader() { Text = "警報內容", Width = 400 },
                new ColumnHeader() { Text = "次數", Width = 80 },
                new ColumnHeader() { Text = "等級", Width = 100 }
            });
            _alarmListView.Dock = DockStyle.Fill;
            _alarmListView.Font = new Font("Consolas", 11F);
            _alarmListView.ForeColor = Color.FromArgb(220, 220, 220);
            _alarmListView.FullRowSelect = true;
            _alarmListView.GridLines = true;
            _alarmListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            _alarmListView.HideSelection = false;
            _alarmListView.Location = new Point(20, 20);
            _alarmListView.MultiSelect = false;
            _alarmListView.Name = "_alarmListView";
            _alarmListView.Size = new Size(976, 575);
            _alarmListView.TabIndex = 0;
            _alarmListView.UseCompatibleStateImageBehavior = false;
            _alarmListView.View = View.Details;
            
            // 
            // _settingsPage - 參數設定
            // 
            _settingsPage.BackColor = Color.FromArgb(45, 45, 45);
            _settingsPage.Controls.Add(_btnNightMode);
            _settingsPage.Controls.Add(_btnStartSimulation);
            _settingsPage.Font = new Font("Microsoft JhengHei UI", 12F);
            _settingsPage.Location = new Point(4, 31);
            _settingsPage.Name = "_settingsPage";
            _settingsPage.Padding = new Padding(20);
            _settingsPage.Size = new Size(1016, 615);
            _settingsPage.TabIndex = 2;
            _settingsPage.Text = "⚙️ 參數設定";
            _settingsPage.UseVisualStyleBackColor = true;
            
            // 
            // _btnNightMode - 夜班模式切換 (工業防呆按鈕)
            // 
            _btnNightMode.RequireLongPress = true;
            _btnNightMode.TargetHoldTimeMs = 1000;
            _btnNightMode.FlatAppearance.BorderSize = 2;
            _btnNightMode.FlatStyle = FlatStyle.Flat;
            _btnNightMode.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
            _btnNightMode.ForeColor = Color.FromArgb(220, 220, 220);
            _btnNightMode.Location = new Point(30, 30);
            _btnNightMode.MinimumSize = new Size(150, 60);
            _btnNightMode.Name = "_btnNightMode";
            _btnNightMode.Size = new Size(200, 60);
            _btnNightMode.TabIndex = 0;
            _btnNightMode.Text = "🌙 夜班模式";
            _btnNightMode.UseVisualStyleBackColor = true;
            _btnNightMode.SafeClick += BtnNightMode_SafeClick;
            
            // 
            // _btnStartSimulation
            // 
            _btnStartSimulation.FlatAppearance.BorderSize = 2;
            _btnStartSimulation.FlatStyle = FlatStyle.Flat;
            _btnStartSimulation.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
            _btnStartSimulation.ForeColor = Color.FromArgb(220, 220, 220);
            _btnStartSimulation.Location = new Point(30, 110);
            _btnStartSimulation.Name = "_btnStartSimulation";
            _btnStartSimulation.Size = new Size(200, 50);
            _btnStartSimulation.TabIndex = 1;
            _btnStartSimulation.Text = "▶ 啟動數據模擬";
            _btnStartSimulation.UseVisualStyleBackColor = true;
            _btnStartSimulation.Click += BtnStartSimulation_Click;
            
            // 
            // _trendPage - 趨勢圖
            // 
            _trendPage.BackColor = Color.FromArgb(45, 45, 45);
            _trendPage.Font = new Font("Microsoft JhengHei UI", 12F);
            _trendPage.Location = new Point(4, 31);
            _trendPage.Name = "_trendPage";
            _trendPage.Padding = new Padding(20);
            _trendPage.Size = new Size(1016, 615);
            _trendPage.TabIndex = 3;
            _trendPage.Text = "📈 趨勢圖";
            _trendPage.UseVisualStyleBackColor = true;
            
            // 
            // _simulationIndicatorLabel
            // 
            _simulationIndicatorLabel.AutoSize = true;
            _simulationIndicatorLabel.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold);
            _simulationIndicatorLabel.Location = new Point(800, 10);
            _simulationIndicatorLabel.Name = "_simulationIndicatorLabel";
            _simulationIndicatorLabel.Padding = new Padding(10, 5, 10, 5);
            _simulationIndicatorLabel.Size = new Size(120, 25);
            _simulationIndicatorLabel.TabIndex = 2;
            _simulationIndicatorLabel.Text = "SIMULATION MODE";
            _simulationIndicatorLabel.BackColor = Color.LightGreen;
            _simulationIndicatorLabel.Visible = false;
            
            // 
            // _alarmIndicatorLabel
            // 
            _alarmIndicatorLabel.AutoSize = true;
            _alarmIndicatorLabel.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold);
            _alarmIndicatorLabel.Location = new Point(650, 10);
            _alarmIndicatorLabel.Name = "_alarmIndicatorLabel";
            _alarmIndicatorLabel.Padding = new Padding(10, 5, 10, 5);
            _alarmIndicatorLabel.Size = new Size(100, 25);
            _alarmIndicatorLabel.TabIndex = 3;
            _alarmIndicatorLabel.Text = "ALARM";
            _alarmIndicatorLabel.BackColor = Color.Transparent;
            _alarmIndicatorLabel.Visible = true;
            
            // 
            // _statusLabel
            // 
            _statusLabel.AutoSize = true;
            _statusLabel.Font = new Font("Microsoft JhengHei UI", 10F);
            _statusLabel.ForeColor = Color.FromArgb(150, 150, 150);
            _statusLabel.Location = new Point(20, 10);
            _statusLabel.Name = "_statusLabel";
            _statusLabel.Size = new Size(67, 18);
            _statusLabel.TabIndex = 1;
            _statusLabel.Text = "Ready";
            
            // 
            // _startButton - 工業防呆啟動按鈕
            // 
            _startButton.RequireLongPress = true;
            _startButton.TargetHoldTimeMs = 2000;
            _startButton.FlatAppearance.BorderSize = 2;
            _startButton.FlatStyle = FlatStyle.Flat;
            _startButton.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
            _startButton.ForeColor = Color.FromArgb(220, 220, 220);
            _startButton.Location = new Point(850, 10);
            _startButton.MinimumSize = new Size(120, 50);
            _startButton.Name = "_startButton";
            _startButton.Size = new Size(140, 50);
            _startButton.TabIndex = 4;
            _startButton.Text = "▶ 啟動";
            _startButton.UseVisualStyleBackColor = true;
            _startButton.SafeClick += StartButton_Click;
            
            // 
            // _stopButton
            // 
            _stopButton.Enabled = false;
            _stopButton.FlatAppearance.BorderSize = 2;
            _stopButton.FlatStyle = FlatStyle.Flat;
            _stopButton.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
            _stopButton.ForeColor = Color.FromArgb(220, 220, 220);
            _stopButton.Location = new Point(1000, 10);
            _stopButton.Name = "_stopButton";
            _stopButton.Size = new Size(140, 50);
            _stopButton.TabIndex = 5;
            _stopButton.Text = "⏹ 停止";
            _stopButton.UseVisualStyleBackColor = true;
            _stopButton.Click += StopButton_Click;
            
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1024, 700);
            Controls.Add(_stopButton);
            Controls.Add(_startButton);
            Controls.Add(_alarmIndicatorLabel);
            Controls.Add(_simulationIndicatorLabel);
            Controls.Add(_statusLabel);
            Controls.Add(_mainTabControl);
            MinimumSize = new Size(800, 600);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Shoe Mold Control System - Industrial Edition";
            FormClosing += MainForm_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        // 主分頁控制項
        private TabControl _mainTabControl;
        
        // 分頁頁面
        private TabPage _dashboardPage;
        private TabPage _alarmPage;
        private TabPage _settingsPage;
        private TabPage _trendPage;
        
        // 狀態顯示
        private Label _statusLabel;
        private Label _simulationIndicatorLabel;
        private Label _alarmIndicatorLabel;
        
        // 儀表板控制項
        private Label _lblTemperature;
        private Label _lblPressure;
        private Label _lblProductionCount;
        private Label _lblStatus;
        private Label _lblLastUpdate;
        private Label _lblAlarmCode;
        private Label _tempValueLabel;
        private Label _pressureValueLabel;
        private Label _countValueLabel;
        
        // 警報列表
        private ListView _alarmListView;
        
        // 設定頁面控制項
        private Industrial.UI.Framework.IndustrialSafeButton _btnNightMode;
        private Button _btnStartSimulation;
        
        // 操作按鈕 (使用工業防呆按鈕)
        private Industrial.UI.Framework.IndustrialSafeButton _startButton;
        private Button _stopButton;
    }
}
