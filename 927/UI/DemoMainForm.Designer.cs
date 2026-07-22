namespace _927.UI
{
    partial class DemoMainForm
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            
            // 主要控制項初始化
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageMonitor = new System.Windows.Forms.TabPage();
            this.tabPageRecipe = new System.Windows.Forms.TabPage();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.tabPageAlarm = new System.Windows.Forms.TabPage();
            this.tabPageControl = new System.Windows.Forms.TabPage();
            
            // 狀態欄
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.lblStatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblCycleTime = new System.Windows.Forms.ToolStripStatusLabel();
            
            // 計時器
            this.tmrStatus = new System.Windows.Forms.Timer(this.components);
            this.tmrUIRefresh = new System.Windows.Forms.Timer(this.components);

            // TabControl 設定
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Font = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            
            // Page 1: Monitor (主監控頁)
            this.tabPageMonitor.Text = "主監控";
            this.tabPageMonitor.Padding = new System.Windows.Forms.Padding(10);
            
            // 模擬影像顯示區 (用 Panel + Label 替代 AuroraVision)
            this.pnlCameraView = new System.Windows.Forms.Panel();
            this.pnlCameraView.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlCameraView.Width = 640;
            this.pnlCameraView.BackColor = System.Drawing.Color.Black;
            this.pnlCameraView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            
            this.lblCameraPlaceholder = new System.Windows.Forms.Label();
            this.lblCameraPlaceholder.Text = "CAMERA VIEW\\n(No Signal)";
            this.lblCameraPlaceholder.ForeColor = System.Drawing.Color.Lime;
            this.lblCameraPlaceholder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCameraPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCameraPlaceholder.Font = new System.Drawing.Font("Consolas", 12F);
            this.pnlCameraView.Controls.Add(this.lblCameraPlaceholder);

            // 右側資訊區
            this.pnlInfoRight = new System.Windows.Forms.Panel();
            this.pnlInfoRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlInfoRight.Padding = new System.Windows.Forms.Padding(10);
            
            // 按鈕組
            this.flowLayoutButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutButtons.Height = 60;
            this.flowLayoutButtons.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            
            this.btnInit = new System.Windows.Forms.Button();
            this.btnInit.Text = "系統初始化";
            this.btnInit.Width = 100;
            this.btnInit.Height = 40;
            this.btnInit.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnInit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInit.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            
            this.btnRun = new System.Windows.Forms.Button();
            this.btnRun.Text = "自動運行";
            this.btnRun.Width = 100;
            this.btnRun.Height = 40;
            this.btnRun.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btnRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStop.Text = "緊急停止";
            this.btnStop.Width = 100;
            this.btnStop.Height = 40;
            this.btnStop.BackColor = System.Drawing.Color.Orange;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            
            this.flowLayoutButtons.Controls.AddRange(new System.Windows.Forms.Control[] { 
                this.btnInit, this.btnRun, this.btnStop 
            });

            // 狀態指示器 (模擬 ActivityIndicator)
            this.pnlStatusIndicator = new System.Windows.Forms.Panel();
            this.pnlStatusIndicator.Width = 40;
            this.pnlStatusIndicator.Height = 40;
            this.pnlStatusIndicator.BackColor = System.Drawing.Color.Gray;
            this.pnlStatusIndicator.Location = new System.Drawing.Point(350, 10);
            this.pnlStatusIndicator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            
            this.lblStatusTitle = new System.Windows.Forms.Label();
            this.lblStatusTitle.Text = "系統狀態：閒置";
            this.lblStatusTitle.AutoSize = true;
            this.lblStatusTitle.Location = new System.Drawing.Point(10, 60);
            this.lblStatusTitle.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold);

            // 數據網格 (掃描記錄)
            this.dgvScanLog = new System.Windows.Forms.DataGridView();
            this.dgvScanLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgvScanLog.Height = 200;
            this.dgvScanLog.AllowUserToAddRows = false;
            this.dgvScanLog.RowHeadersVisible = false;
            this.dgvScanLog.Columns.Add("Time", "時間");
            this.dgvScanLog.Columns.Add("Result", "結果");
            this.dgvScanLog.Columns.Add("Cycle", "週期 (ms)");

            // 組裝 Monitor 頁
            this.tabPageMonitor.Controls.Add(this.dgvScanLog);
            this.tabPageMonitor.Controls.Add(this.lblStatusTitle);
            this.tabPageMonitor.Controls.Add(this.pnlStatusIndicator);
            this.tabPageMonitor.Controls.Add(this.flowLayoutButtons);
            this.tabPageMonitor.Controls.Add(this.pnlInfoRight);
            this.tabPageMonitor.Controls.Add(this.pnlCameraView);

            // Page 2: Recipe (配方)
            this.tabPageRecipe.Text = "配方管理";
            this.dgvRecipe = new System.Windows.Forms.DataGridView();
            this.dgvRecipe.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRecipe.AllowUserToAddRows = false;
            this.dgvRecipe.Columns.Add("ID", "配方 ID");
            this.dgvRecipe.Columns.Add("Name", "名稱");
            this.dgvRecipe.Columns.Add("ParamA", "參數 A");
            this.tabPageRecipe.Controls.Add(this.dgvRecipe);

            // Page 3: Settings (設定)
            this.tabPageSettings.Text = "系統設定";
            this.txtLogOutput = new System.Windows.Forms.TextBox();
            this.txtLogOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLogOutput.Multiline = true;
            this.txtLogOutput.ReadOnly = true;
            this.txtLogOutput.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.txtLogOutput.ForeColor = System.Drawing.Color.Lime;
            this.txtLogOutput.Font = new System.Drawing.Font("Consolas", 9F);
            this.tabPageSettings.Controls.Add(this.txtLogOutput);

            // Page 4: Alarm (警報)
            this.tabPageAlarm.Text = "警報記錄";
            this.dgvAlarm = new System.Windows.Forms.DataGridView();
            this.dgvAlarm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAlarm.AllowUserToAddRows = false;
            this.dgvAlarm.Columns.Add("Time", "發生時間");
            this.dgvAlarm.Columns.Add("Level", "等級");
            this.dgvAlarm.Columns.Add("Message", "警報內容");
            this.tabPageAlarm.Controls.Add(this.dgvAlarm);

            // Page 5: Control (手動控制)
            this.tabPageControl.Text = "手動控制";
            this.lblManualControl = new System.Windows.Forms.Label();
            this.lblManualControl.Text = "機器人手動控制面板 (待實作)";
            this.lblManualControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblManualControl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tabPageControl.Controls.Add(this.lblManualControl);

            // 加入 TabControl
            this.tabControlMain.TabPages.AddRange(new System.Windows.Forms.TabPage[] {
                this.tabPageMonitor, this.tabPageRecipe, this.tabPageSettings, 
                this.tabPageAlarm, this.tabPageControl
            });

            // StatusStrip
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.lblStatusText, this.lblCycleTime
            });
            this.lblStatusText.Text = "就緒";
            this.lblCycleTime.Text = "CT: 0ms";
            this.lblCycleTime.Alignment = System.Drawing.ContentAlignment.MiddleRight;

            // Form 設定
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.statusStripMain);
            this.Text = "927 整合版 - 工業級 UI (Demo Mode)";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            
            // Timer 設定 (Design Time 不啟動)
            this.tmrStatus.Interval = 500;
            this.tmrUIRefresh.Interval = 100;

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        // 控制項宣告
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageMonitor;
        private System.Windows.Forms.TabPage tabPageRecipe;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.TabPage tabPageAlarm;
        private System.Windows.Forms.TabPage tabPageControl;
        
        private System.Windows.Forms.Panel pnlCameraView;
        private System.Windows.Forms.Label lblCameraPlaceholder;
        private System.Windows.Forms.Panel pnlInfoRight;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutButtons;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Panel pnlStatusIndicator;
        private System.Windows.Forms.Label lblStatusTitle;
        private System.Windows.Forms.DataGridView dgvScanLog;
        private System.Windows.Forms.DataGridView dgvRecipe;
        private System.Windows.Forms.TextBox txtLogOutput;
        private System.Windows.Forms.DataGridView dgvAlarm;
        private System.Windows.Forms.Label lblManualControl;
        
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusText;
        private System.Windows.Forms.ToolStripStatusLabel lblCycleTime;
        
        private System.Windows.Forms.Timer tmrStatus;
        private System.Windows.Forms.Timer tmrUIRefresh;
    }
}
