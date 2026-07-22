using System.Windows.Forms;

namespace _927.DemoUI
{
    partial class DemoMainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DemoMainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlCamera = new System.Windows.Forms.Panel();
            this.grpOperation = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtRecipeName_Main = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCurrentRecipeNO_Main = new System.Windows.Forms.TextBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnAlarm = new System.Windows.Forms.Button();
            this.btnAutoRun = new System.Windows.Forms.Button();
            this.btnInitial = new System.Windows.Forms.Button();
            this.tmrStatus = new System.Windows.Forms.Timer(this.components);
            this.grpRecipe = new System.Windows.Forms.GroupBox();
            this.lblDisplayRecipeFile = new System.Windows.Forms.Label();
            this.lblCurrentRecipeNO = new System.Windows.Forms.Label();
            this.txtDisplayRecipeFile = new System.Windows.Forms.TextBox();
            this.txtCurrentRecipeNO = new System.Windows.Forms.TextBox();
            this.btnSaveRecipe = new System.Windows.Forms.Button();
            this.btnUseRecipe = new System.Windows.Forms.Button();
            this.txtRecipeName = new System.Windows.Forms.TextBox();
            this.lblRecipeName = new System.Windows.Forms.Label();
            this.cboRecipeNO = new System.Windows.Forms.ComboBox();
            this.lblRecipeNo = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageMain = new System.Windows.Forms.TabPage();
            this.pnlDetection = new System.Windows.Forms.Panel();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dgvScanLog = new System.Windows.Forms.DataGridView();
            this.pic_927.DemoUI = new System.Windows.Forms.PictureBox();
            this.tabPageSetting = new System.Windows.Forms.TabPage();
            this.grpParam = new System.Windows.Forms.GroupBox();
            this.grpParamRobot = new System.Windows.Forms.GroupBox();
            this.grpRobotPoseTeaching = new System.Windows.Forms.GroupBox();
            this.btnChangeDrageMode = new System.Windows.Forms.Button();
            this.imgListDragMode = new System.Windows.Forms.ImageList(this.components);
            this.btnToHomePose = new System.Windows.Forms.Button();
            this.btnToScanPose = new System.Windows.Forms.Button();
            this.dgvRobotPose = new System.Windows.Forms.DataGridView();
            this.點位 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.X = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Y = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Z = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.R = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSetHomePose = new System.Windows.Forms.Button();
            this.btnSetScanPose = new System.Windows.Forms.Button();
            this.nupdParamWaitTimeInScanPos = new System.Windows.Forms.NumericUpDown();
            this.lblParamWaitTimeInScanPos = new System.Windows.Forms.Label();
            this.txtParamRobotIP = new System.Windows.Forms.TextBox();
            this.lblParamRobotIP = new System.Windows.Forms.Label();
            this.grpParamVision = new System.Windows.Forms.GroupBox();
            this.btnSaveEqSetting = new System.Windows.Forms.Button();
            this.grpParamFolders = new System.Windows.Forms.GroupBox();
            this.nupdScanCleanDays = new System.Windows.Forms.NumericUpDown();
            this.lblLogCleanDays = new System.Windows.Forms.Label();
            this.lblDriverFreeSpace = new System.Windows.Forms.Label();
            this.txtDriverFreeSpace = new System.Windows.Forms.TextBox();
            this.nupdLogCleanDays = new System.Windows.Forms.NumericUpDown();
            this.lblParamLogFolder = new System.Windows.Forms.Label();
            this.txtParamLogFolder = new System.Windows.Forms.TextBox();
            this.lblParamScanFolder = new System.Windows.Forms.Label();
            this.txtParamScanFolder = new System.Windows.Forms.TextBox();
            this.tabPageRecipe = new System.Windows.Forms.TabPage();
            this.grpRecipeData = new System.Windows.Forms.GroupBox();
            this.btnRecipeChangeDrageMode = new System.Windows.Forms.Button();
            this.btnRecipePathGoTo = new System.Windows.Forms.Button();
            this.btnRecipePathDelete = new System.Windows.Forms.Button();
            this.btnRecipePathAddNew = new System.Windows.Forms.Button();
            this.btnRecipePathModify = new System.Windows.Forms.Button();
            this.dgvRecipePath = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageControl = new System.Windows.Forms.TabPage();
            this.grpManualControl = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tabPageAlarm = new System.Windows.Forms.TabPage();
            this.btnAlarmReset = new System.Windows.Forms.Button();
            this.dgvAlarm = new System.Windows.Forms.DataGridView();
            this.dgvSystemInfoColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvSystemInfoColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvSystemInfoColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tmrVision = new System.Windows.Forms.Timer(this.components);
            this.nupdRecipeRunPathSpeed = new System.Windows.Forms.NumericUpDown();
            this.lblRecipeRunPathSpeed = new System.Windows.Forms.Label();
            this.nupdRecipeRunPathAcc = new System.Windows.Forms.NumericUpDown();
            this.lblRecipeRunPathAcc = new System.Windows.Forms.Label();
            this.nupdRecipePointNoToSendModbus = new System.Windows.Forms.NumericUpDown();
            this.lblRecipePointNoToSendModbus = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.grpParamPLC = new System.Windows.Forms.GroupBox();
            this.txtParamModbusIP = new System.Windows.Forms.TextBox();
            this.lblParamModbusIP = new System.Windows.Forms.Label();
            this.nupdModbusCoilAddress = new System.Windows.Forms.NumericUpDown();
            this.lblModbusCoilAddress = new System.Windows.Forms.Label();
            this.grpOperation.SuspendLayout();
            this.grpRecipe.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageMain.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScanLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_927.DemoUI)).BeginInit();
            this.tabPageSetting.SuspendLayout();
            this.grpParam.SuspendLayout();
            this.grpParamRobot.SuspendLayout();
            this.grpRobotPoseTeaching.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRobotPose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupdParamWaitTimeInScanPos)).BeginInit();
            this.grpParamFolders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupdScanCleanDays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupdLogCleanDays)).BeginInit();
            this.tabPageRecipe.SuspendLayout();
            this.grpRecipeData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecipePath)).BeginInit();
            this.tabPageControl.SuspendLayout();
            this.grpManualControl.SuspendLayout();
            this.tabPageAlarm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlarm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupdRecipeRunPathSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupdRecipeRunPathAcc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupdRecipePointNoToSendModbus)).BeginInit();
            this.grpParamPLC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupdModbusCoilAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlCamera
            // 
            this.pnlCamera.BackColor = System.Drawing.Color.Black;
            this.pnlCamera.Location = new System.Drawing.Point(687, 2);
            this.pnlCamera.Name = "pnlCamera";
            this.pnlCamera.Size = new System.Drawing.Size(1200, 470);
            this.pnlCamera.TabIndex = 0;
            // 
            // grpOperation
            // 
            this.grpOperation.Controls.Add(this.label3);
            this.grpOperation.Controls.Add(this.txtRecipeName_Main);
            this.grpOperation.Controls.Add(this.label1);
            this.grpOperation.Controls.Add(this.txtCurrentRecipeNO_Main);
            this.grpOperation.Controls.Add(this.btnStop);
            this.grpOperation.Controls.Add(this.btnAlarm);
            this.grpOperation.Controls.Add(this.btnAutoRun);
            this.grpOperation.Controls.Add(this.btnInitial);
            this.grpOperation.Location = new System.Drawing.Point(8, 6);
            this.grpOperation.Name = "grpOperation";
            this.grpOperation.Size = new System.Drawing.Size(673, 144);
            this.grpOperation.TabIndex = 4;
            this.grpOperation.TabStop = false;
            this.grpOperation.Text = "操作";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(413, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 20);
            this.label3.TabIndex = 248;
            this.label3.Text = "名稱";
            // 
            // txtRecipeName_Main
            // 
            this.txtRecipeName_Main.BackColor = System.Drawing.Color.White;
            this.txtRecipeName_Main.ForeColor = System.Drawing.Color.SteelBlue;
            this.txtRecipeName_Main.Location = new System.Drawing.Point(477, 83);
            this.txtRecipeName_Main.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtRecipeName_Main.Name = "txtRecipeName_Main";
            this.txtRecipeName_Main.ReadOnly = true;
            this.txtRecipeName_Main.Size = new System.Drawing.Size(185, 29);
            this.txtRecipeName_Main.TabIndex = 247;
            this.txtRecipeName_Main.Text = "--";
            this.txtRecipeName_Main.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(413, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 20);
            this.label1.TabIndex = 246;
            this.label1.Text = "Recipe";
            // 
            // txtCurrentRecipeNO_Main
            // 
            this.txtCurrentRecipeNO_Main.BackColor = System.Drawing.Color.White;
            this.txtCurrentRecipeNO_Main.ForeColor = System.Drawing.Color.SteelBlue;
            this.txtCurrentRecipeNO_Main.Location = new System.Drawing.Point(477, 48);
            this.txtCurrentRecipeNO_Main.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCurrentRecipeNO_Main.Name = "txtCurrentRecipeNO_Main";
            this.txtCurrentRecipeNO_Main.ReadOnly = true;
            this.txtCurrentRecipeNO_Main.Size = new System.Drawing.Size(185, 29);
            this.txtCurrentRecipeNO_Main.TabIndex = 245;
            this.txtCurrentRecipeNO_Main.Text = "999";
            this.txtCurrentRecipeNO_Main.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnStop
            // 
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnStop.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnStop.Location = new System.Drawing.Point(203, 37);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(86, 82);
            this.btnStop.TabIndex = 31;
            this.btnStop.Text = "停止";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnAlarm
            // 
            this.btnAlarm.BackColor = System.Drawing.Color.Orange;
            this.btnAlarm.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAlarm.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAlarm.Location = new System.Drawing.Point(314, 37);
            this.btnAlarm.Name = "btnAlarm";
            this.btnAlarm.Size = new System.Drawing.Size(86, 82);
            this.btnAlarm.TabIndex = 30;
            this.btnAlarm.Text = "清除警報";
            this.btnAlarm.UseVisualStyleBackColor = false;
            this.btnAlarm.Click += new System.EventHandler(this.btnAlarm_Click);
            // 
            // btnAutoRun
            // 
            this.btnAutoRun.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAutoRun.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnAutoRun.Location = new System.Drawing.Point(113, 37);
            this.btnAutoRun.Name = "btnAutoRun";
            this.btnAutoRun.Size = new System.Drawing.Size(86, 82);
            this.btnAutoRun.TabIndex = 29;
            this.btnAutoRun.Text = "運行";
            this.btnAutoRun.UseVisualStyleBackColor = true;
            this.btnAutoRun.Click += new System.EventHandler(this.btnAutoRun_Click);
            // 
            // btnInitial
            // 
            this.btnInitial.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnInitial.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnInitial.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnInitial.Location = new System.Drawing.Point(6, 37);
            this.btnInitial.Name = "btnInitial";
            this.btnInitial.Size = new System.Drawing.Size(101, 82);
            this.btnInitial.TabIndex = 28;
            this.btnInitial.Text = "初始化";
            this.btnInitial.UseVisualStyleBackColor = false;
            this.btnInitial.Click += new System.EventHandler(this.btnInitial_Click);
            // 
            // tmrStatus
            // 
            this.tmrStatus.Tick += new System.EventHandler(this.tmrStatus_Tick);
            // 
            // grpRecipe
            // 
            this.grpRecipe.Controls.Add(this.lblDisplayRecipeFile);
            this.grpRecipe.Controls.Add(this.lblCurrentRecipeNO);
            this.grpRecipe.Controls.Add(this.txtDisplayRecipeFile);
            this.grpRecipe.Controls.Add(this.txtCurrentRecipeNO);
            this.grpRecipe.Controls.Add(this.btnSaveRecipe);
            this.grpRecipe.Controls.Add(this.btnUseRecipe);
            this.grpRecipe.Controls.Add(this.txtRecipeName);
            this.grpRecipe.Controls.Add(this.lblRecipeName);
            this.grpRecipe.Controls.Add(this.cboRecipeNO);
            this.grpRecipe.Controls.Add(this.lblRecipeNo);
            this.grpRecipe.Location = new System.Drawing.Point(22, 21);
            this.grpRecipe.Name = "grpRecipe";
            this.grpRecipe.Size = new System.Drawing.Size(865, 148);
            this.grpRecipe.TabIndex = 15;
            this.grpRecipe.TabStop = false;
            this.grpRecipe.Text = "選單";
            // 
            // lblDisplayRecipeFile
            // 
            this.lblDisplayRecipeFile.AutoSize = true;
            this.lblDisplayRecipeFile.Location = new System.Drawing.Point(12, 114);
            this.lblDisplayRecipeFile.Name = "lblDisplayRecipeFile";
            this.lblDisplayRecipeFile.Size = new System.Drawing.Size(95, 20);
            this.lblDisplayRecipeFile.TabIndex = 245;
            this.lblDisplayRecipeFile.Text = "Recipe 路徑";
            // 
            // lblCurrentRecipeNO
            // 
            this.lblCurrentRecipeNO.AutoSize = true;
            this.lblCurrentRecipeNO.Location = new System.Drawing.Point(12, 25);
            this.lblCurrentRecipeNO.Name = "lblCurrentRecipeNO";
            this.lblCurrentRecipeNO.Size = new System.Drawing.Size(137, 20);
            this.lblCurrentRecipeNO.TabIndex = 244;
            this.lblCurrentRecipeNO.Text = "現在系統執行的是";
            // 
            // txtDisplayRecipeFile
            // 
            this.txtDisplayRecipeFile.BackColor = System.Drawing.Color.White;
            this.txtDisplayRecipeFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDisplayRecipeFile.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtDisplayRecipeFile.ForeColor = System.Drawing.Color.Black;
            this.txtDisplayRecipeFile.Location = new System.Drawing.Point(155, 113);
            this.txtDisplayRecipeFile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtDisplayRecipeFile.Name = "txtDisplayRecipeFile";
            this.txtDisplayRecipeFile.ReadOnly = true;
            this.txtDisplayRecipeFile.Size = new System.Drawing.Size(390, 23);
            this.txtDisplayRecipeFile.TabIndex = 243;
            // 
            // txtCurrentRecipeNO
            // 
            this.txtCurrentRecipeNO.BackColor = System.Drawing.Color.White;
            this.txtCurrentRecipeNO.ForeColor = System.Drawing.Color.SteelBlue;
            this.txtCurrentRecipeNO.Location = new System.Drawing.Point(155, 22);
            this.txtCurrentRecipeNO.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCurrentRecipeNO.Name = "txtCurrentRecipeNO";
            this.txtCurrentRecipeNO.ReadOnly = true;
            this.txtCurrentRecipeNO.Size = new System.Drawing.Size(124, 29);
            this.txtCurrentRecipeNO.TabIndex = 242;
            this.txtCurrentRecipeNO.Text = "999";
            this.txtCurrentRecipeNO.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnSaveRecipe
            // 
            this.btnSaveRecipe.BackColor = System.Drawing.Color.LightBlue;
            this.btnSaveRecipe.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSaveRecipe.Location = new System.Drawing.Point(551, 53);
            this.btnSaveRecipe.Name = "btnSaveRecipe";
            this.btnSaveRecipe.Size = new System.Drawing.Size(117, 82);
            this.btnSaveRecipe.TabIndex = 241;
            this.btnSaveRecipe.Text = "儲存";
            this.btnSaveRecipe.UseVisualStyleBackColor = false;
            this.btnSaveRecipe.Click += new System.EventHandler(this.btnSaveRecipe_Click);
            // 
            // btnUseRecipe
            // 
            this.btnUseRecipe.BackColor = System.Drawing.Color.LightBlue;
            this.btnUseRecipe.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnUseRecipe.Location = new System.Drawing.Point(674, 54);
            this.btnUseRecipe.Name = "btnUseRecipe";
            this.btnUseRecipe.Size = new System.Drawing.Size(117, 82);
            this.btnUseRecipe.TabIndex = 240;
            this.btnUseRecipe.Text = "切換";
            this.btnUseRecipe.UseVisualStyleBackColor = false;
            this.btnUseRecipe.Click += new System.EventHandler(this.btnUseRecipe_Click);
            // 
            // txtRecipeName
            // 
            this.txtRecipeName.Location = new System.Drawing.Point(155, 83);
            this.txtRecipeName.Name = "txtRecipeName";
            this.txtRecipeName.Size = new System.Drawing.Size(390, 29);
            this.txtRecipeName.TabIndex = 24;
            // 
            // lblRecipeName
            // 
            this.lblRecipeName.AutoSize = true;
            this.lblRecipeName.Location = new System.Drawing.Point(12, 88);
            this.lblRecipeName.Name = "lblRecipeName";
            this.lblRecipeName.Size = new System.Drawing.Size(95, 20);
            this.lblRecipeName.TabIndex = 23;
            this.lblRecipeName.Text = "Recipe 名稱";
            // 
            // cboRecipeNO
            // 
            this.cboRecipeNO.FormattingEnabled = true;
            this.cboRecipeNO.Location = new System.Drawing.Point(155, 54);
            this.cboRecipeNO.Name = "cboRecipeNO";
            this.cboRecipeNO.Size = new System.Drawing.Size(390, 28);
            this.cboRecipeNO.TabIndex = 22;
            this.cboRecipeNO.DropDown += new System.EventHandler(this.cboRecipeNO_DropDown);
            this.cboRecipeNO.SelectedIndexChanged += new System.EventHandler(this.cboRecipeNO_SelectedIndexChanged);
            // 
            // lblRecipeNo
            // 
            this.lblRecipeNo.AutoSize = true;
            this.lblRecipeNo.Location = new System.Drawing.Point(12, 57);
            this.lblRecipeNo.Name = "lblRecipeNo";
            this.lblRecipeNo.Size = new System.Drawing.Size(95, 20);
            this.lblRecipeNo.TabIndex = 21;
            this.lblRecipeNo.Text = "查看 Recipe";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageMain);
            this.tabControl.Controls.Add(this.tabPageSetting);
            this.tabControl.Controls.Add(this.tabPageRecipe);
            this.tabControl.Controls.Add(this.tabPageControl);
            this.tabControl.Controls.Add(this.tabPageAlarm);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1904, 981);
            this.tabControl.TabIndex = 16;
            this.tabControl.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl_DrawItem);
            this.tabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl_Selected);
            // 
            // tabPageMain
            // 
            this.tabPageMain.Controls.Add(this.pnlDetection);
            this.tabPageMain.Controls.Add(this.tabControlMain);
            this.tabPageMain.Controls.Add(this.pic_927.DemoUI);
            this.tabPageMain.Controls.Add(this.grpOperation);
            this.tabPageMain.Controls.Add(this.pnlCamera);
            this.tabPageMain.Location = new System.Drawing.Point(4, 29);
            this.tabPageMain.Name = "tabPageMain";
            this.tabPageMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMain.Size = new System.Drawing.Size(1896, 948);
            this.tabPageMain.TabIndex = 0;
            this.tabPageMain.Tag = "0";
            this.tabPageMain.Text = "首頁";
            this.tabPageMain.UseVisualStyleBackColor = true;
            // 
            // pnlDetection
            // 
            this.pnlDetection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.pnlDetection.Location = new System.Drawing.Point(687, 475);
            this.pnlDetection.Name = "pnlDetection";
            this.pnlDetection.Size = new System.Drawing.Size(1200, 470);
            this.pnlDetection.TabIndex = 8;
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPage1);
            this.tabControlMain.Controls.Add(this.tabPage2);
            this.tabControlMain.Location = new System.Drawing.Point(7, 156);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(674, 698);
            this.tabControlMain.TabIndex = 7;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(666, 665);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "功能";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dgvScanLog);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(666, 665);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Log";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dgvScanLog
            // 
            this.dgvScanLog.AllowUserToAddRows = false;
            this.dgvScanLog.AllowUserToDeleteRows = false;
            this.dgvScanLog.AllowUserToResizeColumns = false;
            this.dgvScanLog.AllowUserToResizeRows = false;
            this.dgvScanLog.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvScanLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvScanLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvScanLog.Location = new System.Drawing.Point(3, 3);
            this.dgvScanLog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvScanLog.Name = "dgvScanLog";
            this.dgvScanLog.ReadOnly = true;
            this.dgvScanLog.RowHeadersVisible = false;
            this.dgvScanLog.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvScanLog.RowTemplate.Height = 24;
            this.dgvScanLog.Size = new System.Drawing.Size(660, 659);
            this.dgvScanLog.TabIndex = 6;
            // 
            // pic_927.DemoUI
            // 
            this.pic_927.DemoUI.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pic_927.DemoUI.BackgroundImage")));
            this.pic_927.DemoUI.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pic_927.DemoUI.Location = new System.Drawing.Point(8, 860);
            this.pic_927.DemoUI.Name = "pic_927.DemoUI";
            this.pic_927.DemoUI.Size = new System.Drawing.Size(307, 82);
            this.pic_927.DemoUI.TabIndex = 5;
            this.pic_927.DemoUI.TabStop = false;
            this.pic_927.DemoUI.Click += new System.EventHandler(this.pic_927.DemoUI_Click);
            // 
            // tabPageSetting
            // 
            this.tabPageSetting.Controls.Add(this.grpParam);
            this.tabPageSetting.Location = new System.Drawing.Point(4, 29);
            this.tabPageSetting.Name = "tabPageSetting";
            this.tabPageSetting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSetting.Size = new System.Drawing.Size(1896, 948);
            this.tabPageSetting.TabIndex = 1;
            this.tabPageSetting.Tag = "1";
            this.tabPageSetting.Text = "設定";
            this.tabPageSetting.UseVisualStyleBackColor = true;
            // 
            // grpParam
            // 
            this.grpParam.Controls.Add(this.grpParamPLC);
            this.grpParam.Controls.Add(this.grpParamRobot);
            this.grpParam.Controls.Add(this.grpParamVision);
            this.grpParam.Controls.Add(this.btnSaveEqSetting);
            this.grpParam.Controls.Add(this.grpParamFolders);
            this.grpParam.Location = new System.Drawing.Point(1, 0);
            this.grpParam.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpParam.Name = "grpParam";
            this.grpParam.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpParam.Size = new System.Drawing.Size(1895, 948);
            this.grpParam.TabIndex = 10;
            this.grpParam.TabStop = false;
            this.grpParam.Text = "系統參數";
            // 
            // grpParamRobot
            // 
            this.grpParamRobot.Controls.Add(this.grpRobotPoseTeaching);
            this.grpParamRobot.Controls.Add(this.nupdParamWaitTimeInScanPos);
            this.grpParamRobot.Controls.Add(this.lblParamWaitTimeInScanPos);
            this.grpParamRobot.Controls.Add(this.txtParamRobotIP);
            this.grpParamRobot.Controls.Add(this.lblParamRobotIP);
            this.grpParamRobot.Location = new System.Drawing.Point(34, 330);
            this.grpParamRobot.Name = "grpParamRobot";
            this.grpParamRobot.Size = new System.Drawing.Size(941, 434);
            this.grpParamRobot.TabIndex = 13;
            this.grpParamRobot.TabStop = false;
            this.grpParamRobot.Text = "Robot";
            // 
            // grpRobotPoseTeaching
            // 
            this.grpRobotPoseTeaching.Controls.Add(this.btnChangeDrageMode);
            this.grpRobotPoseTeaching.Controls.Add(this.btnToHomePose);
            this.grpRobotPoseTeaching.Controls.Add(this.btnToScanPose);
            this.grpRobotPoseTeaching.Controls.Add(this.dgvRobotPose);
            this.grpRobotPoseTeaching.Controls.Add(this.btnSetHomePose);
            this.grpRobotPoseTeaching.Controls.Add(this.btnSetScanPose);
            this.grpRobotPoseTeaching.Location = new System.Drawing.Point(21, 142);
            this.grpRobotPoseTeaching.Name = "grpRobotPoseTeaching";
            this.grpRobotPoseTeaching.Size = new System.Drawing.Size(899, 286);
            this.grpRobotPoseTeaching.TabIndex = 244;
            this.grpRobotPoseTeaching.TabStop = false;
            this.grpRobotPoseTeaching.Text = "Robot固定點位示教";
            // 
            // btnChangeDrageMode
            // 
            this.btnChangeDrageMode.ImageIndex = 0;
            this.btnChangeDrageMode.ImageList = this.imgListDragMode;
            this.btnChangeDrageMode.Location = new System.Drawing.Point(20, 38);
            this.btnChangeDrageMode.Name = "btnChangeDrageMode";
            this.btnChangeDrageMode.Size = new System.Drawing.Size(140, 70);
            this.btnChangeDrageMode.TabIndex = 246;
            this.btnChangeDrageMode.UseVisualStyleBackColor = true;
            this.btnChangeDrageMode.Click += new System.EventHandler(this.btnChangeDrageMode_Click);
            // 
            // imgListDragMode
            // 
            this.imgListDragMode.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListDragMode.ImageStream")));
            this.imgListDragMode.TransparentColor = System.Drawing.Color.Transparent;
            this.imgListDragMode.Images.SetKeyName(0, "DragOff.jpg");
            this.imgListDragMode.Images.SetKeyName(1, "DragOn.jpg");
            // 
            // btnToHomePose
            // 
            this.btnToHomePose.BackColor = System.Drawing.Color.LightBlue;
            this.btnToHomePose.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnToHomePose.Location = new System.Drawing.Point(156, 125);
            this.btnToHomePose.Name = "btnToHomePose";
            this.btnToHomePose.Size = new System.Drawing.Size(131, 51);
            this.btnToHomePose.TabIndex = 245;
            this.btnToHomePose.Text = "To Home點";
            this.btnToHomePose.UseVisualStyleBackColor = false;
            this.btnToHomePose.Click += new System.EventHandler(this.btnToHomePose_Click);
            // 
            // btnToScanPose
            // 
            this.btnToScanPose.BackColor = System.Drawing.Color.LightBlue;
            this.btnToScanPose.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnToScanPose.Location = new System.Drawing.Point(156, 210);
            this.btnToScanPose.Name = "btnToScanPose";
            this.btnToScanPose.Size = new System.Drawing.Size(131, 51);
            this.btnToScanPose.TabIndex = 246;
            this.btnToScanPose.Text = "To 拍照點";
            this.btnToScanPose.UseVisualStyleBackColor = false;
            this.btnToScanPose.Click += new System.EventHandler(this.btnToScanPose_Click);
            // 
            // dgvRobotPose
            // 
            this.dgvRobotPose.AllowUserToAddRows = false;
            this.dgvRobotPose.AllowUserToDeleteRows = false;
            this.dgvRobotPose.AllowUserToResizeColumns = false;
            this.dgvRobotPose.AllowUserToResizeRows = false;
            this.dgvRobotPose.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvRobotPose.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRobotPose.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.點位,
            this.X,
            this.Y,
            this.Z,
            this.R});
            this.dgvRobotPose.Location = new System.Drawing.Point(312, 38);
            this.dgvRobotPose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvRobotPose.Name = "dgvRobotPose";
            this.dgvRobotPose.ReadOnly = true;
            this.dgvRobotPose.RowHeadersVisible = false;
            this.dgvRobotPose.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvRobotPose.RowTemplate.Height = 24;
            this.dgvRobotPose.Size = new System.Drawing.Size(516, 223);
            this.dgvRobotPose.TabIndex = 244;
            // 
            // 點位
            // 
            this.點位.HeaderText = "點位";
            this.點位.Name = "點位";
            this.點位.ReadOnly = true;
            this.點位.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.點位.Width = 47;
            // 
            // X
            // 
            this.X.HeaderText = "X";
            this.X.Name = "X";
            this.X.ReadOnly = true;
            this.X.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.X.Width = 25;
            // 
            // Y
            // 
            this.Y.HeaderText = "Y";
            this.Y.Name = "Y";
            this.Y.ReadOnly = true;
            this.Y.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Y.Width = 25;
            // 
            // Z
            // 
            this.Z.HeaderText = "Z";
            this.Z.Name = "Z";
            this.Z.ReadOnly = true;
            this.Z.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Z.Width = 25;
            // 
            // R
            // 
            this.R.HeaderText = "R";
            this.R.Name = "R";
            this.R.ReadOnly = true;
            this.R.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.R.Width = 25;
            // 
            // btnSetHomePose
            // 
            this.btnSetHomePose.BackColor = System.Drawing.Color.LightBlue;
            this.btnSetHomePose.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSetHomePose.Location = new System.Drawing.Point(19, 125);
            this.btnSetHomePose.Name = "btnSetHomePose";
            this.btnSetHomePose.Size = new System.Drawing.Size(131, 51);
            this.btnSetHomePose.TabIndex = 242;
            this.btnSetHomePose.Text = "示教Home點";
            this.btnSetHomePose.UseVisualStyleBackColor = false;
            this.btnSetHomePose.Click += new System.EventHandler(this.btnSetHomePose_Click);
            // 
            // btnSetScanPose
            // 
            this.btnSetScanPose.BackColor = System.Drawing.Color.LightBlue;
            this.btnSetScanPose.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSetScanPose.Location = new System.Drawing.Point(19, 210);
            this.btnSetScanPose.Name = "btnSetScanPose";
            this.btnSetScanPose.Size = new System.Drawing.Size(131, 51);
            this.btnSetScanPose.TabIndex = 243;
            this.btnSetScanPose.Text = "示教拍照點";
            this.btnSetScanPose.UseVisualStyleBackColor = false;
            this.btnSetScanPose.Click += new System.EventHandler(this.btnSetScanPose_Click);
            // 
            // nupdParamWaitTimeInScanPos
            // 
            this.nupdParamWaitTimeInScanPos.Location = new System.Drawing.Point(217, 98);
            this.nupdParamWaitTimeInScanPos.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nupdParamWaitTimeInScanPos.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.nupdParamWaitTimeInScanPos.Name = "nupdParamWaitTimeInScanPos";
            this.nupdParamWaitTimeInScanPos.Size = new System.Drawing.Size(70, 29);
            this.nupdParamWaitTimeInScanPos.TabIndex = 11;
            this.nupdParamWaitTimeInScanPos.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // lblParamWaitTimeInScanPos
            // 
            this.lblParamWaitTimeInScanPos.AutoSize = true;
            this.lblParamWaitTimeInScanPos.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblParamWaitTimeInScanPos.Location = new System.Drawing.Point(26, 100);
            this.lblParamWaitTimeInScanPos.Name = "lblParamWaitTimeInScanPos";
            this.lblParamWaitTimeInScanPos.Size = new System.Drawing.Size(185, 20);
            this.lblParamWaitTimeInScanPos.TabIndex = 10;
            this.lblParamWaitTimeInScanPos.Text = "到拍照位後等待時間(ms)";
            // 
            // txtParamRobotIP
            // 
            this.txtParamRobotIP.Location = new System.Drawing.Point(80, 37);
            this.txtParamRobotIP.Name = "txtParamRobotIP";
            this.txtParamRobotIP.Size = new System.Drawing.Size(131, 29);
            this.txtParamRobotIP.TabIndex = 9;
            this.txtParamRobotIP.Text = "192.168.1.6";
            // 
            // lblParamRobotIP
            // 
            this.lblParamRobotIP.AutoSize = true;
            this.lblParamRobotIP.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblParamRobotIP.Location = new System.Drawing.Point(26, 40);
            this.lblParamRobotIP.Name = "lblParamRobotIP";
            this.lblParamRobotIP.Size = new System.Drawing.Size(24, 20);
            this.lblParamRobotIP.TabIndex = 8;
            this.lblParamRobotIP.Text = "IP";
            // 
            // grpParamVision
            // 
            this.grpParamVision.Location = new System.Drawing.Point(34, 185);
            this.grpParamVision.Name = "grpParamVision";
            this.grpParamVision.Size = new System.Drawing.Size(533, 139);
            this.grpParamVision.TabIndex = 12;
            this.grpParamVision.TabStop = false;
            this.grpParamVision.Text = "Vision";
            // 
            // btnSaveEqSetting
            // 
            this.btnSaveEqSetting.BackColor = System.Drawing.Color.LightBlue;
            this.btnSaveEqSetting.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSaveEqSetting.Location = new System.Drawing.Point(1053, 85);
            this.btnSaveEqSetting.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSaveEqSetting.Name = "btnSaveEqSetting";
            this.btnSaveEqSetting.Size = new System.Drawing.Size(242, 95);
            this.btnSaveEqSetting.TabIndex = 7;
            this.btnSaveEqSetting.Text = "儲存系統參數";
            this.btnSaveEqSetting.UseVisualStyleBackColor = false;
            this.btnSaveEqSetting.Click += new System.EventHandler(this.btnSaveEqSetting_Click);
            // 
            // grpParamFolders
            // 
            this.grpParamFolders.Controls.Add(this.nupdScanCleanDays);
            this.grpParamFolders.Controls.Add(this.lblLogCleanDays);
            this.grpParamFolders.Controls.Add(this.lblDriverFreeSpace);
            this.grpParamFolders.Controls.Add(this.txtDriverFreeSpace);
            this.grpParamFolders.Controls.Add(this.nupdLogCleanDays);
            this.grpParamFolders.Controls.Add(this.lblParamLogFolder);
            this.grpParamFolders.Controls.Add(this.txtParamLogFolder);
            this.grpParamFolders.Controls.Add(this.lblParamScanFolder);
            this.grpParamFolders.Controls.Add(this.txtParamScanFolder);
            this.grpParamFolders.Location = new System.Drawing.Point(34, 35);
            this.grpParamFolders.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpParamFolders.Name = "grpParamFolders";
            this.grpParamFolders.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpParamFolders.Size = new System.Drawing.Size(941, 145);
            this.grpParamFolders.TabIndex = 10;
            this.grpParamFolders.TabStop = false;
            this.grpParamFolders.Text = "資料夾";
            // 
            // nupdScanCleanDays
            // 
            this.nupdScanCleanDays.BackColor = System.Drawing.Color.White;
            this.nupdScanCleanDays.Location = new System.Drawing.Point(554, 93);
            this.nupdScanCleanDays.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.nupdScanCleanDays.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.nupdScanCleanDays.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nupdScanCleanDays.Name = "nupdScanCleanDays";
            this.nupdScanCleanDays.ReadOnly = true;
            this.nupdScanCleanDays.Size = new System.Drawing.Size(101, 29);
            this.nupdScanCleanDays.TabIndex = 9;
            this.nupdScanCleanDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nupdScanCleanDays.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblLogCleanDays
            // 
            this.lblLogCleanDays.AutoSize = true;
            this.lblLogCleanDays.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblLogCleanDays.Location = new System.Drawing.Point(550, 34);
            this.lblLogCleanDays.Name = "lblLogCleanDays";
            this.lblLogCleanDays.Size = new System.Drawing.Size(105, 20);
            this.lblLogCleanDays.TabIndex = 3;
            this.lblLogCleanDays.Text = "自動刪檔天數";
            // 
            // lblDriverFreeSpace
            // 
            this.lblDriverFreeSpace.AutoSize = true;
            this.lblDriverFreeSpace.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblDriverFreeSpace.Location = new System.Drawing.Point(37, 29);
            this.lblDriverFreeSpace.Name = "lblDriverFreeSpace";
            this.lblDriverFreeSpace.Size = new System.Drawing.Size(109, 20);
            this.lblDriverFreeSpace.TabIndex = 12;
            this.lblDriverFreeSpace.Text = "硬碟空間 (GB)";
            // 
            // txtDriverFreeSpace
            // 
            this.txtDriverFreeSpace.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDriverFreeSpace.Location = new System.Drawing.Point(154, 26);
            this.txtDriverFreeSpace.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtDriverFreeSpace.Name = "txtDriverFreeSpace";
            this.txtDriverFreeSpace.ReadOnly = true;
            this.txtDriverFreeSpace.Size = new System.Drawing.Size(145, 26);
            this.txtDriverFreeSpace.TabIndex = 11;
            this.txtDriverFreeSpace.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // nupdLogCleanDays
            // 
            this.nupdLogCleanDays.BackColor = System.Drawing.Color.White;
            this.nupdLogCleanDays.Location = new System.Drawing.Point(554, 58);
            this.nupdLogCleanDays.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.nupdLogCleanDays.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.nupdLogCleanDays.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nupdLogCleanDays.Name = "nupdLogCleanDays";
            this.nupdLogCleanDays.ReadOnly = true;
            this.nupdLogCleanDays.Size = new System.Drawing.Size(101, 29);
            this.nupdLogCleanDays.TabIndex = 7;
            this.nupdLogCleanDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nupdLogCleanDays.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblParamLogFolder
            // 
            this.lblParamLogFolder.AutoSize = true;
            this.lblParamLogFolder.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblParamLogFolder.Location = new System.Drawing.Point(37, 62);
            this.lblParamLogFolder.Name = "lblParamLogFolder";
            this.lblParamLogFolder.Size = new System.Drawing.Size(57, 20);
            this.lblParamLogFolder.TabIndex = 2;
            this.lblParamLogFolder.Text = "記錄檔";
            // 
            // txtParamLogFolder
            // 
            this.txtParamLogFolder.Location = new System.Drawing.Point(154, 58);
            this.txtParamLogFolder.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtParamLogFolder.Name = "txtParamLogFolder";
            this.txtParamLogFolder.ReadOnly = true;
            this.txtParamLogFolder.Size = new System.Drawing.Size(355, 29);
            this.txtParamLogFolder.TabIndex = 3;
            // 
            // lblParamScanFolder
            // 
            this.lblParamScanFolder.AutoSize = true;
            this.lblParamScanFolder.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblParamScanFolder.Location = new System.Drawing.Point(37, 97);
            this.lblParamScanFolder.Name = "lblParamScanFolder";
            this.lblParamScanFolder.Size = new System.Drawing.Size(73, 20);
            this.lblParamScanFolder.TabIndex = 0;
            this.lblParamScanFolder.Text = "掃描圖片";
            // 
            // txtParamScanFolder
            // 
            this.txtParamScanFolder.Location = new System.Drawing.Point(154, 93);
            this.txtParamScanFolder.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtParamScanFolder.Name = "txtParamScanFolder";
            this.txtParamScanFolder.ReadOnly = true;
            this.txtParamScanFolder.Size = new System.Drawing.Size(355, 29);
            this.txtParamScanFolder.TabIndex = 1;
            // 
            // tabPageRecipe
            // 
            this.tabPageRecipe.Controls.Add(this.grpRecipeData);
            this.tabPageRecipe.Controls.Add(this.grpRecipe);
            this.tabPageRecipe.Location = new System.Drawing.Point(4, 29);
            this.tabPageRecipe.Name = "tabPageRecipe";
            this.tabPageRecipe.Size = new System.Drawing.Size(1896, 948);
            this.tabPageRecipe.TabIndex = 2;
            this.tabPageRecipe.Tag = "2";
            this.tabPageRecipe.Text = "Recipe";
            this.tabPageRecipe.UseVisualStyleBackColor = true;
            // 
            // grpRecipeData
            // 
            this.grpRecipeData.Controls.Add(this.label2);
            this.grpRecipeData.Controls.Add(this.nupdRecipePointNoToSendModbus);
            this.grpRecipeData.Controls.Add(this.lblRecipePointNoToSendModbus);
            this.grpRecipeData.Controls.Add(this.nupdRecipeRunPathAcc);
            this.grpRecipeData.Controls.Add(this.lblRecipeRunPathAcc);
            this.grpRecipeData.Controls.Add(this.nupdRecipeRunPathSpeed);
            this.grpRecipeData.Controls.Add(this.lblRecipeRunPathSpeed);
            this.grpRecipeData.Controls.Add(this.btnRecipeChangeDrageMode);
            this.grpRecipeData.Controls.Add(this.btnRecipePathGoTo);
            this.grpRecipeData.Controls.Add(this.btnRecipePathDelete);
            this.grpRecipeData.Controls.Add(this.btnRecipePathAddNew);
            this.grpRecipeData.Controls.Add(this.btnRecipePathModify);
            this.grpRecipeData.Controls.Add(this.dgvRecipePath);
            this.grpRecipeData.Location = new System.Drawing.Point(22, 175);
            this.grpRecipeData.Name = "grpRecipeData";
            this.grpRecipeData.Size = new System.Drawing.Size(1047, 417);
            this.grpRecipeData.TabIndex = 16;
            this.grpRecipeData.TabStop = false;
            this.grpRecipeData.Text = "手臂路徑";
            // 
            // btnRecipeChangeDrageMode
            // 
            this.btnRecipeChangeDrageMode.ImageIndex = 0;
            this.btnRecipeChangeDrageMode.ImageList = this.imgListDragMode;
            this.btnRecipeChangeDrageMode.Location = new System.Drawing.Point(36, 37);
            this.btnRecipeChangeDrageMode.Name = "btnRecipeChangeDrageMode";
            this.btnRecipeChangeDrageMode.Size = new System.Drawing.Size(140, 70);
            this.btnRecipeChangeDrageMode.TabIndex = 250;
            this.btnRecipeChangeDrageMode.UseVisualStyleBackColor = true;
            this.btnRecipeChangeDrageMode.Click += new System.EventHandler(this.btnChangeDrageMode_Click);
            // 
            // btnRecipePathGoTo
            // 
            this.btnRecipePathGoTo.BackColor = System.Drawing.Color.LightBlue;
            this.btnRecipePathGoTo.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRecipePathGoTo.Location = new System.Drawing.Point(200, 130);
            this.btnRecipePathGoTo.Name = "btnRecipePathGoTo";
            this.btnRecipePathGoTo.Size = new System.Drawing.Size(131, 51);
            this.btnRecipePathGoTo.TabIndex = 249;
            this.btnRecipePathGoTo.Text = "GoTo路徑點";
            this.btnRecipePathGoTo.UseVisualStyleBackColor = false;
            this.btnRecipePathGoTo.Click += new System.EventHandler(this.btnRecipePathGoTo_Click);
            // 
            // btnRecipePathDelete
            // 
            this.btnRecipePathDelete.BackColor = System.Drawing.Color.LightBlue;
            this.btnRecipePathDelete.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRecipePathDelete.Location = new System.Drawing.Point(337, 187);
            this.btnRecipePathDelete.Name = "btnRecipePathDelete";
            this.btnRecipePathDelete.Size = new System.Drawing.Size(131, 51);
            this.btnRecipePathDelete.TabIndex = 248;
            this.btnRecipePathDelete.Text = "刪除路徑點";
            this.btnRecipePathDelete.UseVisualStyleBackColor = false;
            this.btnRecipePathDelete.Click += new System.EventHandler(this.btnRecipePathDelete_Click);
            // 
            // btnRecipePathAddNew
            // 
            this.btnRecipePathAddNew.BackColor = System.Drawing.Color.LightBlue;
            this.btnRecipePathAddNew.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRecipePathAddNew.Location = new System.Drawing.Point(36, 130);
            this.btnRecipePathAddNew.Name = "btnRecipePathAddNew";
            this.btnRecipePathAddNew.Size = new System.Drawing.Size(131, 51);
            this.btnRecipePathAddNew.TabIndex = 246;
            this.btnRecipePathAddNew.Text = "新增路徑點";
            this.btnRecipePathAddNew.UseVisualStyleBackColor = false;
            this.btnRecipePathAddNew.Click += new System.EventHandler(this.btnRecipePathAddNew_Click);
            // 
            // btnRecipePathModify
            // 
            this.btnRecipePathModify.BackColor = System.Drawing.Color.LightBlue;
            this.btnRecipePathModify.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRecipePathModify.Location = new System.Drawing.Point(200, 187);
            this.btnRecipePathModify.Name = "btnRecipePathModify";
            this.btnRecipePathModify.Size = new System.Drawing.Size(131, 51);
            this.btnRecipePathModify.TabIndex = 247;
            this.btnRecipePathModify.Text = "修改路徑點";
            this.btnRecipePathModify.UseVisualStyleBackColor = false;
            this.btnRecipePathModify.Click += new System.EventHandler(this.btnRecipePathModify_Click);
            // 
            // dgvRecipePath
            // 
            this.dgvRecipePath.AllowUserToAddRows = false;
            this.dgvRecipePath.AllowUserToDeleteRows = false;
            this.dgvRecipePath.AllowUserToResizeColumns = false;
            this.dgvRecipePath.AllowUserToResizeRows = false;
            this.dgvRecipePath.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvRecipePath.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecipePath.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5});
            this.dgvRecipePath.Location = new System.Drawing.Point(497, 27);
            this.dgvRecipePath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvRecipePath.MultiSelect = false;
            this.dgvRecipePath.Name = "dgvRecipePath";
            this.dgvRecipePath.ReadOnly = true;
            this.dgvRecipePath.RowHeadersVisible = false;
            this.dgvRecipePath.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvRecipePath.RowTemplate.Height = 24;
            this.dgvRecipePath.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRecipePath.Size = new System.Drawing.Size(529, 366);
            this.dgvRecipePath.TabIndex = 245;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "點位";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Width = 47;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "X";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn2.Width = 25;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Y";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn3.Width = 25;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Z";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn4.Width = 25;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "R";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn5.Width = 25;
            // 
            // tabPageControl
            // 
            this.tabPageControl.Controls.Add(this.grpManualControl);
            this.tabPageControl.Location = new System.Drawing.Point(4, 29);
            this.tabPageControl.Name = "tabPageControl";
            this.tabPageControl.Size = new System.Drawing.Size(1896, 948);
            this.tabPageControl.TabIndex = 3;
            this.tabPageControl.Tag = "3";
            this.tabPageControl.Text = "手動控制";
            this.tabPageControl.UseVisualStyleBackColor = true;
            // 
            // grpManualControl
            // 
            this.grpManualControl.Controls.Add(this.button1);
            this.grpManualControl.Location = new System.Drawing.Point(22, 24);
            this.grpManualControl.Name = "grpManualControl";
            this.grpManualControl.Size = new System.Drawing.Size(723, 523);
            this.grpManualControl.TabIndex = 1;
            this.grpManualControl.TabStop = false;
            this.grpManualControl.Text = "控制操作";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(65, 67);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(151, 65);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // tabPageAlarm
            // 
            this.tabPageAlarm.Controls.Add(this.btnAlarmReset);
            this.tabPageAlarm.Controls.Add(this.dgvAlarm);
            this.tabPageAlarm.Location = new System.Drawing.Point(4, 29);
            this.tabPageAlarm.Name = "tabPageAlarm";
            this.tabPageAlarm.Size = new System.Drawing.Size(1896, 948);
            this.tabPageAlarm.TabIndex = 4;
            this.tabPageAlarm.Tag = "4";
            this.tabPageAlarm.Text = "警報";
            this.tabPageAlarm.UseVisualStyleBackColor = true;
            // 
            // btnAlarmReset
            // 
            this.btnAlarmReset.BackColor = System.Drawing.Color.Orange;
            this.btnAlarmReset.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAlarmReset.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAlarmReset.Location = new System.Drawing.Point(53, 17);
            this.btnAlarmReset.Name = "btnAlarmReset";
            this.btnAlarmReset.Size = new System.Drawing.Size(86, 82);
            this.btnAlarmReset.TabIndex = 31;
            this.btnAlarmReset.Text = "清除警報";
            this.btnAlarmReset.UseVisualStyleBackColor = false;
            this.btnAlarmReset.Click += new System.EventHandler(this.btnAlarm_Click);
            // 
            // dgvAlarm
            // 
            this.dgvAlarm.AllowUserToAddRows = false;
            this.dgvAlarm.AllowUserToDeleteRows = false;
            this.dgvAlarm.AllowUserToResizeRows = false;
            this.dgvAlarm.BackgroundColor = System.Drawing.Color.White;
            this.dgvAlarm.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAlarm.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvSystemInfoColumn1,
            this.dgvSystemInfoColumn3,
            this.dgvSystemInfoColumn7});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAlarm.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAlarm.Enabled = false;
            this.dgvAlarm.EnableHeadersVisualStyles = false;
            this.dgvAlarm.Location = new System.Drawing.Point(220, 17);
            this.dgvAlarm.Name = "dgvAlarm";
            this.dgvAlarm.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAlarm.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvAlarm.RowHeadersVisible = false;
            this.dgvAlarm.RowHeadersWidth = 61;
            this.dgvAlarm.RowTemplate.Height = 24;
            this.dgvAlarm.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvAlarm.Size = new System.Drawing.Size(1108, 923);
            this.dgvAlarm.TabIndex = 9;
            this.dgvAlarm.TabStop = false;
            // 
            // dgvSystemInfoColumn1
            // 
            this.dgvSystemInfoColumn1.Frozen = true;
            this.dgvSystemInfoColumn1.HeaderText = "時間";
            this.dgvSystemInfoColumn1.Name = "dgvSystemInfoColumn1";
            this.dgvSystemInfoColumn1.ReadOnly = true;
            // 
            // dgvSystemInfoColumn3
            // 
            this.dgvSystemInfoColumn3.HeaderText = "警報";
            this.dgvSystemInfoColumn3.Name = "dgvSystemInfoColumn3";
            this.dgvSystemInfoColumn3.ReadOnly = true;
            this.dgvSystemInfoColumn3.Width = 200;
            // 
            // dgvSystemInfoColumn7
            // 
            this.dgvSystemInfoColumn7.HeaderText = "異常";
            this.dgvSystemInfoColumn7.Name = "dgvSystemInfoColumn7";
            this.dgvSystemInfoColumn7.ReadOnly = true;
            this.dgvSystemInfoColumn7.Width = 800;
            // 
            // tmrVision
            // 
            this.tmrVision.Interval = 50;
            this.tmrVision.Tick += new System.EventHandler(this.tmrVision_Tick);
            // 
            // nupdRecipeRunPathSpeed
            // 
            this.nupdRecipeRunPathSpeed.Location = new System.Drawing.Point(191, 266);
            this.nupdRecipeRunPathSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nupdRecipeRunPathSpeed.Name = "nupdRecipeRunPathSpeed";
            this.nupdRecipeRunPathSpeed.Size = new System.Drawing.Size(70, 29);
            this.nupdRecipeRunPathSpeed.TabIndex = 252;
            this.nupdRecipeRunPathSpeed.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // lblRecipeRunPathSpeed
            // 
            this.lblRecipeRunPathSpeed.AutoSize = true;
            this.lblRecipeRunPathSpeed.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblRecipeRunPathSpeed.Location = new System.Drawing.Point(32, 270);
            this.lblRecipeRunPathSpeed.Name = "lblRecipeRunPathSpeed";
            this.lblRecipeRunPathSpeed.Size = new System.Drawing.Size(153, 20);
            this.lblRecipeRunPathSpeed.TabIndex = 251;
            this.lblRecipeRunPathSpeed.Text = "跑路徑時的速度比例";
            // 
            // nupdRecipeRunPathAcc
            // 
            this.nupdRecipeRunPathAcc.Location = new System.Drawing.Point(191, 305);
            this.nupdRecipeRunPathAcc.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nupdRecipeRunPathAcc.Name = "nupdRecipeRunPathAcc";
            this.nupdRecipeRunPathAcc.Size = new System.Drawing.Size(70, 29);
            this.nupdRecipeRunPathAcc.TabIndex = 254;
            this.nupdRecipeRunPathAcc.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // lblRecipeRunPathAcc
            // 
            this.lblRecipeRunPathAcc.AutoSize = true;
            this.lblRecipeRunPathAcc.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblRecipeRunPathAcc.Location = new System.Drawing.Point(32, 309);
            this.lblRecipeRunPathAcc.Name = "lblRecipeRunPathAcc";
            this.lblRecipeRunPathAcc.Size = new System.Drawing.Size(153, 20);
            this.lblRecipeRunPathAcc.TabIndex = 253;
            this.lblRecipeRunPathAcc.Text = "跑路徑時加速度比例";
            // 
            // nupdRecipePointNoToSendModbus
            // 
            this.nupdRecipePointNoToSendModbus.Location = new System.Drawing.Point(317, 354);
            this.nupdRecipePointNoToSendModbus.Name = "nupdRecipePointNoToSendModbus";
            this.nupdRecipePointNoToSendModbus.Size = new System.Drawing.Size(70, 29);
            this.nupdRecipePointNoToSendModbus.TabIndex = 256;
            this.nupdRecipePointNoToSendModbus.ValueChanged += new System.EventHandler(this.nupdRecipePointNoToSendModbus_ValueChanged);
            // 
            // lblRecipePointNoToSendModbus
            // 
            this.lblRecipePointNoToSendModbus.AutoSize = true;
            this.lblRecipePointNoToSendModbus.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblRecipePointNoToSendModbus.Location = new System.Drawing.Point(32, 356);
            this.lblRecipePointNoToSendModbus.Name = "lblRecipePointNoToSendModbus";
            this.lblRecipePointNoToSendModbus.Size = new System.Drawing.Size(279, 20);
            this.lblRecipePointNoToSendModbus.TabIndex = 255;
            this.lblRecipePointNoToSendModbus.Text = "跑到第幾個點要確認並送Modbus訊號";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(203, 376);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 20);
            this.label2.TabIndex = 257;
            this.label2.Text = "(0表示不確認)";
            // 
            // grpParamPLC
            // 
            this.grpParamPLC.Controls.Add(this.nupdModbusCoilAddress);
            this.grpParamPLC.Controls.Add(this.lblModbusCoilAddress);
            this.grpParamPLC.Controls.Add(this.txtParamModbusIP);
            this.grpParamPLC.Controls.Add(this.lblParamModbusIP);
            this.grpParamPLC.Location = new System.Drawing.Point(588, 185);
            this.grpParamPLC.Name = "grpParamPLC";
            this.grpParamPLC.Size = new System.Drawing.Size(387, 139);
            this.grpParamPLC.TabIndex = 14;
            this.grpParamPLC.TabStop = false;
            this.grpParamPLC.Text = "PLC Modbus";
            // 
            // txtParamModbusIP
            // 
            this.txtParamModbusIP.Location = new System.Drawing.Point(84, 37);
            this.txtParamModbusIP.Name = "txtParamModbusIP";
            this.txtParamModbusIP.Size = new System.Drawing.Size(131, 29);
            this.txtParamModbusIP.TabIndex = 11;
            this.txtParamModbusIP.Text = "127.0.0.1";
            // 
            // lblParamModbusIP
            // 
            this.lblParamModbusIP.AutoSize = true;
            this.lblParamModbusIP.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblParamModbusIP.Location = new System.Drawing.Point(30, 40);
            this.lblParamModbusIP.Name = "lblParamModbusIP";
            this.lblParamModbusIP.Size = new System.Drawing.Size(24, 20);
            this.lblParamModbusIP.TabIndex = 10;
            this.lblParamModbusIP.Text = "IP";
            // 
            // nupdModbusCoilAddress
            // 
            this.nupdModbusCoilAddress.Location = new System.Drawing.Point(161, 79);
            this.nupdModbusCoilAddress.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.nupdModbusCoilAddress.Name = "nupdModbusCoilAddress";
            this.nupdModbusCoilAddress.Size = new System.Drawing.Size(70, 29);
            this.nupdModbusCoilAddress.TabIndex = 13;
            this.nupdModbusCoilAddress.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblModbusCoilAddress
            // 
            this.lblModbusCoilAddress.AutoSize = true;
            this.lblModbusCoilAddress.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblModbusCoilAddress.Location = new System.Drawing.Point(34, 83);
            this.lblModbusCoilAddress.Name = "lblModbusCoilAddress";
            this.lblModbusCoilAddress.Size = new System.Drawing.Size(121, 20);
            this.lblModbusCoilAddress.TabIndex = 12;
            this.lblModbusCoilAddress.Text = "線圈寄存器點位";
            // 
            // DemoMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1904, 981);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DemoMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Demo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DemoMainForm_FormClosing);
            this.Load += new System.EventHandler(this.DemoMainForm_Load);
            this.grpOperation.ResumeLayout(false);
            this.grpOperation.PerformLayout();
            this.grpRecipe.ResumeLayout(false);
            this.grpRecipe.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageMain.ResumeLayout(false);
            this.tabControlMain.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvScanLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_927.DemoUI)).EndInit();
            this.tabPageSetting.ResumeLayout(false);
            this.grpParam.ResumeLayout(false);
            this.grpParamRobot.ResumeLayout(false);
            this.grpParamRobot.PerformLayout();
            this.grpRobotPoseTeaching.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRobotPose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupdParamWaitTimeInScanPos)).EndInit();
            this.grpParamFolders.ResumeLayout(false);
            this.grpParamFolders.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupdScanCleanDays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupdLogCleanDays)).EndInit();
            this.tabPageRecipe.ResumeLayout(false);
            this.grpRecipeData.ResumeLayout(false);
            this.grpRecipeData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecipePath)).EndInit();
            this.tabPageControl.ResumeLayout(false);
            this.grpManualControl.ResumeLayout(false);
            this.tabPageAlarm.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlarm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupdRecipeRunPathSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupdRecipeRunPathAcc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupdRecipePointNoToSendModbus)).EndInit();
            this.grpParamPLC.ResumeLayout(false);
            this.grpParamPLC.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupdModbusCoilAddress)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel pnlCamera;
        private GroupBox grpOperation;
        private Timer tmrStatus;
        private GroupBox grpRecipe;
        private TextBox txtRecipeName;
        private Label lblRecipeName;
        private ComboBox cboRecipeNO;
        private Label lblRecipeNo;
        private TextBox txtCurrentRecipeNO;
        private Button btnSaveRecipe;
        private Button btnUseRecipe;
        private TextBox txtDisplayRecipeFile;
        private Label lblCurrentRecipeNO;
        private TabControl tabControl;
        private TabPage tabPageMain;
        private TabPage tabPageSetting;
        private TabPage tabPageRecipe;
        private TabPage tabPageControl;
        private TabPage tabPageAlarm;
        private Button btnInitial;
        private PictureBox pic_927.DemoUI;
        private Button btnAlarm;
        private Button btnAutoRun;
        private Button btnStop;
        private DataGridView dgvAlarm;
        private DataGridViewTextBoxColumn dgvSystemInfoColumn1;
        private DataGridViewTextBoxColumn dgvSystemInfoColumn3;
        private DataGridViewTextBoxColumn dgvSystemInfoColumn7;
        private Button btnAlarmReset;
        private GroupBox grpParam;
        private GroupBox grpParamVision;
        private Button btnSaveEqSetting;
        private GroupBox grpParamFolders;
        private Label lblDriverFreeSpace;
        private TextBox txtDriverFreeSpace;
        private Label lblParamLogFolder;
        private TextBox txtParamLogFolder;
        private Label lblParamScanFolder;
        private TextBox txtParamScanFolder;
        private NumericUpDown nupdScanCleanDays;
        private NumericUpDown nupdLogCleanDays;
        private Label lblLogCleanDays;
        private DataGridView dgvScanLog;
        private GroupBox grpRecipeData;
        private Label label1;
        private TextBox txtCurrentRecipeNO_Main;
        private Label lblDisplayRecipeFile;
        private Label label3;
        private TextBox txtRecipeName_Main;
        private TabControl tabControlMain;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button button1;
        private GroupBox grpManualControl;
        private GroupBox grpParamRobot;
        private Label lblParamRobotIP;
        private TextBox txtParamRobotIP;
        private Panel pnlDetection;
        private NumericUpDown nupdParamWaitTimeInScanPos;
        private Label lblParamWaitTimeInScanPos;
        private Button btnSetHomePose;
        private Button btnSetScanPose;
        private GroupBox grpRobotPoseTeaching;
        private DataGridView dgvRobotPose;
        private DataGridViewTextBoxColumn 點位;
        private DataGridViewTextBoxColumn X;
        private DataGridViewTextBoxColumn Y;
        private DataGridViewTextBoxColumn Z;
        private DataGridViewTextBoxColumn R;
        private Timer tmrVision;
        private Button btnToHomePose;
        private Button btnToScanPose;
        private DataGridView dgvRecipePath;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private Button btnRecipePathDelete;
        private Button btnRecipePathAddNew;
        private Button btnRecipePathModify;
        private Button btnRecipePathGoTo;
        private Button btnChangeDrageMode;
        private Button btnRecipeChangeDrageMode;
        private ImageList imgListDragMode;
        private NumericUpDown nupdRecipeRunPathSpeed;
        private Label lblRecipeRunPathSpeed;
        private NumericUpDown nupdRecipeRunPathAcc;
        private Label lblRecipeRunPathAcc;
        private NumericUpDown nupdRecipePointNoToSendModbus;
        private Label lblRecipePointNoToSendModbus;
        private Label label2;
        private GroupBox grpParamPLC;
        private TextBox txtParamModbusIP;
        private Label lblParamModbusIP;
        private NumericUpDown nupdModbusCoilAddress;
        private Label lblModbusCoilAddress;
    }
}

