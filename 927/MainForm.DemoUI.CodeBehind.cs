using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;

using Avl;
using HMI.Controls;
using AuroraVision;
using System.Reflection;


namespace _927.DemoUI
{
    public partial class DemoMainForm : Form
    {
        private FormControl frmEngineer;                //工程測試視窗

        //設備
        public Equipment eq;
        private EquipmentConfig pShowEqParam = null;    //顯示的設備參數
        private RecipeData pShowRecipeData = null;      //顯示的Recipe

        private HMI.Controls.ActivityIndicator actBusy = new ActivityIndicator();
        public DemoMainForm()
        {
            InitializeComponent();

            frmEngineer = new FormControl(this);
            actBusy.Parent = btnAutoRun;
            actBusy.Dock = DockStyle.Fill;
            actBusy.Active = true;
            actBusy.Visible = false;

            tabPageRecipe.Parent = null;
            tabPageControl.Parent = null;

            dgvAlarm.RowCount = 37;
            dgvScanLog.RowCount = 26;

            eq = new Equipment(pnlCamera, pnlDetection);
            eq.eventAlarm += new EventHandler(OnAlarm); //警報更新事件
            eq.eventRecipeChanged += new EventHandler(OnRecipeChanged);   //切換Recipe事件
            eq.eventScanLog += new EventHandler(OnScanLog); //掃描記錄更新事件

            ShowRecipeInfo(true);
        }
        private void DemoMainForm_Load(object sender, EventArgs e)
        {
            #region 產生Log標題
            ScanLog tempLog = new ScanLog();
            string[] strTitles = tempLog.GetTitles();
            dgvScanLog.ColumnCount = strTitles.Length;            
            for (int i = 0; i < dgvScanLog.ColumnCount; i++)
            {

                dgvScanLog.Columns[i].HeaderText = strTitles[i];
                dgvScanLog.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            dgvRobotPose.RowCount = 2;

            DisplayRobotPose(0, eq.pCurrentEqParam.poseInitial);
            DisplayRobotPose(1, eq.pCurrentEqParam.poseInitial);
            #endregion

            btnInitial.Text = eq.pCurrentEqParam.blnIsOnLine ? "初始化" : "初始化\n模擬模式";
            tmrStatus.Enabled = true;
            tmrVision.Enabled = true;
            eq.WriteSysLog("Form Load --------------------");
        }
        private void DemoMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmEngineer.Close();

            eq.WriteSysLog("Form Closing");
            eq.Close();
            eq.WriteSysLog("Form Closed ------------------");
        }

        #region 系統狀態
        private uint uintCheckSystemCount = 0;
        private void tmrStatus_Tick(object sender, EventArgs e) 
        {
            uintCheckSystemCount++;

            SetAlarmButtonStyle();

            if (uintCheckSystemCount % 2 == 0)
            {
                SetOperationButtonStyle();
            }
            if (uintCheckSystemCount % 10 == 0)
            {
                txtDriverFreeSpace.Text = eq.DiskFreeGByte == 0.0 ? "NA" : string.Format("{0:F2}", eq.DiskFreeGByte);
            }
        }

        private void SetAlarmButtonStyle()
        {
            if (eq.IsAlarm)
            {
                btnAlarm.Enabled = true;
                btnAlarm.BackColor = Color.Orange;

                btnAlarmReset.Enabled = true;
                btnAlarmReset.BackColor = Color.Orange;
            }
            else if (eq.IsWarning && !eq.IsAlarm)
            {
                btnAlarm.Enabled = true;
                btnAlarm.BackColor = Color.Gold;

                btnAlarmReset.Enabled = true;
                btnAlarmReset.BackColor = Color.Gold;
            }
            else
            {
                btnAlarm.Enabled = false;
                btnAlarm.BackColor = Color.Transparent;
                btnAlarm.ForeColor = Color.Black;

                btnAlarmReset.Enabled = false;
                btnAlarmReset.BackColor = Color.Transparent;
                btnAlarmReset.ForeColor = Color.Black;
            }
        }
        private bool flagPageInitialed = false;
        private void SetOperationButtonStyle()
        {
            if (eq.GetEquipmentStatus() >= EquipmentStatus.Eq03_Idle && !flagPageInitialed)
            {
                tabPageRecipe.Parent = tabControl;
                //tabPageControl.Parent = tabControl; //該頁面沒用到暫時不顯示
                flagPageInitialed = true;
            }

            switch (eq.GetEquipmentStatus())
            {
                case EquipmentStatus.Eq01_NotInitialed:
                    btnInitial.Enabled = true;
                    btnInitial.BackColor = Color.LightSkyBlue;
                    btnInitial.ForeColor = Color.Black;

                    btnAutoRun.BackColor = Color.Transparent;
                    btnAutoRun.ForeColor = Color.Black;

                    btnStop.BackColor = Color.Transparent;
                    btnStop.ForeColor = Color.Black;

                    btnAutoRun.Enabled = false;
                    btnStop.Enabled = false;

                    grpRobotPoseTeaching.Enabled = false;
                    grpRecipe.Enabled = false;
                    grpRecipeData.Enabled = false;
                    break;

                case EquipmentStatus.Eq02_Initializing:
                    btnInitial.Enabled = false;
                    btnInitial.FlatStyle = FlatStyle.Popup;
                    btnInitial.BackColor = Color.LightSkyBlue;
                    btnInitial.ForeColor = Color.White;

                    btnAutoRun.Enabled = false;
                    btnStop.Enabled = false;
                    break;

                case EquipmentStatus.Eq03_Idle:
                    btnInitial.Enabled = false;
                    btnInitial.FlatStyle = FlatStyle.Popup;
                    btnInitial.BackColor = Color.Transparent;
                    btnInitial.ForeColor = Color.White;
                    btnInitial.Text = eq.pCurrentEqParam.blnIsOnLine ? "實機模式" : "模擬模式";

                    btnAutoRun.BackColor = Color.LightSeaGreen;
                    btnAutoRun.ForeColor = Color.White;
                    
                    btnStop.BackColor = Color.HotPink;
                    btnStop.ForeColor = Color.Black;

                    btnAutoRun.Enabled = true;
                    btnStop.Enabled = false;
                    actBusy.Visible = false;

                    grpManualControl.Enabled = true;

                    grpRobotPoseTeaching.Enabled = true;
                    grpRecipe.Enabled = true;
                    grpRecipeData.Enabled = true;
                    break;

                case EquipmentStatus.Eq04_Preparing:
                    btnAutoRun.Enabled = false;
                    btnStop.Enabled = false;

                    grpManualControl.Enabled = false;

                    grpRobotPoseTeaching.Enabled = false;
                    grpRecipe.Enabled = false;
                    grpRecipeData.Enabled = false;
                    break;

                case EquipmentStatus.Eq05_AutoRunning:
                    btnAutoRun.BackColor = Color.SeaGreen;
                    btnAutoRun.ForeColor = Color.White;

                    btnStop.BackColor = Color.DeepPink;
                    btnStop.ForeColor = Color.White;

                    btnAutoRun.Enabled = false;
                    btnStop.Enabled = true;
                    actBusy.Visible = true;

                    grpManualControl.Enabled = false;

                    grpRobotPoseTeaching.Enabled = false;
                    grpRecipe.Enabled = false;
                    grpRecipeData.Enabled = false;
                    break;
            }
        }

        private void tmrVision_Tick(object sender, EventArgs e)
        {
            if (eq.GetEquipmentStatus() == EquipmentStatus.Eq03_Idle)
            {
                lock (eq.cameraThread.image)
                {
                    eq.ShowCameraView(eq.cameraThread.image);
                }
            }
            else if (eq.GetEquipmentStatus() == EquipmentStatus.Eq05_AutoRunning)
            {
                lock (eq.cameraThread.image)
                {
                    eq.ShowCameraView(eq.cameraThread.image);
                    eq.ShowDetectionView(eq.cameraThread.imageDetected);
                }
            }

            if (eq.GetEquipmentStatus() >= EquipmentStatus.Eq03_Idle)
            {
                if (eq.IsDragMode)
                {
                    btnChangeDrageMode.ImageIndex = 1;
                    btnRecipeChangeDrageMode.ImageIndex = 1;
                }
                else
                {
                    btnChangeDrageMode.ImageIndex = 0;
                    btnRecipeChangeDrageMode.ImageIndex = 0;
                }
            }
        }
        #endregion
        #region 分頁
        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            int pageIndex = int.Parse(tabControl.SelectedTab.Tag.ToString());

            switch (pageIndex)
            {
                case 0: // Main Page
                    {
                        pShowEqParam = null;
                        pShowRecipeData = null;
                    }
                    break;
                case 1: // Setting
                    {
                        pShowRecipeData = null;
                        ShowEquipmentSetting();
                    }
                    break;
                case 2: // Recipe
                    {
                        uint iRecipeNO = eq.pCurrentRecipe.iRecipeNO;
                        cboRecipeNO.SelectedIndex = (int)iRecipeNO - 1;
                        RecipeData pRecipeData = new RecipeData(iRecipeNO, eq.strSystemFolder);
                        ShowRecipeData(pRecipeData);
                    }
                    break;
                case 3: //Control
                    {
                        pShowEqParam = null;
                        pShowRecipeData = null;
                    }
                    break;
                case 4: //Alarm
                    {
                        pShowEqParam = null;
                        pShowRecipeData = null;
                    }
                    break;
                default:
                    {
                        pShowEqParam = null;
                        pShowRecipeData = null;
                    }
                    break;
            }
        }
        private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            SolidBrush fillbrush = new SolidBrush(Color.White);

            Rectangle lasttabrect = tabControl.GetTabRect(tabControl.TabPages.Count - 1);
            Rectangle background = new Rectangle();
            background.Location = new Point(lasttabrect.Right, 0);

            //pad the rectangle to cover the 1 pixel line between the top of the tabpage and the start of the tabs
            background.Size = new System.Drawing.Size(tabControl.Right - background.Left, lasttabrect.Height + 1);
            e.Graphics.FillRectangle(fillbrush, background);

            TabPage page = tabControl.TabPages[e.Index];
            int index = int.Parse(page.Tag.ToString());

            if(index == 4 && (eq.IsAlarm || eq.IsWarning))
            {
                //有警報的時候警報頁面(index 4)的頁籤顯示顏色
                e.Graphics.FillRectangle(new SolidBrush(Color.Orange), e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);
            }
            

            Rectangle paddedBounds = e.Bounds;
            int yOffset = (e.State == DrawItemState.Selected) ? -2 : 1;
            paddedBounds.Offset(1, yOffset);

            TextRenderer.DrawText(e.Graphics, page.Text, e.Font, paddedBounds, page.ForeColor);
        }
        #endregion
        #region Alarm
        private void OnAlarm(object sender, EventArgs e)
        {
            AlarmArgs alarmData = (AlarmArgs)e;

            if (this.InvokeRequired)
            {
                pShowAlarm ptr = new pShowAlarm(ShowAlarm);
                this.Invoke(ptr, new object[] { alarmData.code, alarmData.strText, alarmData.strTime });
            }
            else
            {
                ShowAlarm(alarmData.code, alarmData.strText, alarmData.strTime);
            }
        }
        private delegate void pShowAlarm(AlarmCode code, string strText = "", string strTime = "");
        private void ShowAlarm(AlarmCode code, string strText = "", string strTime = "")
        {
            dgvAlarm.Rows.RemoveAt(dgvAlarm.RowCount - 1);
            dgvAlarm.Rows.Insert(0, 1);
            dgvAlarm.Rows[0].Cells[0].Value = strTime;
            dgvAlarm.Rows[0].Cells[1].Value = eq.alarmCode[(int)code];
            dgvAlarm.Rows[0].Cells[2].Value = eq.alarmText[(int)code] + ": "+ strText;
            this.Refresh();
        }
        private void btnAlarm_Click(object sender, EventArgs e)
        {
            eq.AlarmReset();
            this.Refresh();
        }
        #endregion
        #region Setting
        private void btnSaveEqSetting_Click(object sender, EventArgs e)
        {
            if (ApplyEquipmentSetting())
            {
                eq.SaveEquipmentSetting(pShowEqParam);
            }
            else
            {
                MessageBox.Show("因部分設定超出合理範圍，儲存失敗，先恢復到上次的設定", "參數存檔前檢查");
                ShowEquipmentSetting();
            }
        }
        private bool ApplyEquipmentSetting()
        {
            if (pShowEqParam == null)
                return false;
            try
            {
                #region Log
                pShowEqParam.intLOGKeepFileDays = (int)nupdLogCleanDays.Value;
                pShowEqParam.intScanKeepFileDays = (int)nupdScanCleanDays.Value;
                #endregion

                #region Robot
                pShowEqParam.strRobotIP = txtParamRobotIP.Text;
                pShowEqParam.intWaitTimeInScanPos = (int)nupdParamWaitTimeInScanPos.Value;

                pShowEqParam.poseHome = GetDisplayedRobotPose(0);
                pShowEqParam.poseScan = GetDisplayedRobotPose(1);
                #endregion

                #region PLC
                pShowEqParam.strModbusIP = txtParamModbusIP.Text;
                pShowEqParam.intModbusCoilAddress = (int)nupdModbusCoilAddress.Value;
                #endregion
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void ShowEquipmentSetting()
        {
            pShowEqParam = eq.pCurrentEqParam.Clone();    //複製一份並顯示出來

            #region Folders
            txtParamLogFolder.Text = eq.strLogFolder;
            txtParamScanFolder.Text = eq.strScanFolder;
            #endregion
            #region Log
            nupdLogCleanDays.Value = pShowEqParam.intLOGKeepFileDays;
            nupdScanCleanDays.Value = pShowEqParam.intScanKeepFileDays;
            #endregion

            #region Robot
            txtParamRobotIP.Text = pShowEqParam.strRobotIP;
            nupdParamWaitTimeInScanPos.Value = pShowEqParam.intWaitTimeInScanPos;

            DisplayRobotPose(0, pShowEqParam.poseHome);
            DisplayRobotPose(1, pShowEqParam.poseScan);
            #endregion

            #region PLC
            txtParamModbusIP.Text = pShowEqParam.strModbusIP;
            nupdModbusCoilAddress.Value = pShowEqParam.intModbusCoilAddress;
            #endregion
        }

        #endregion
        #region Recipe
        private void OnRecipeChanged(object sender, EventArgs e)
        {
            RecipeEventArgs eventData = (RecipeEventArgs)e;

            if (InvokeRequired)
            {
                pOnRecipeChangedEx ptr = new pOnRecipeChangedEx(OnRecipeChangedEx);
                Invoke(ptr, new object[] { eventData.pChangedRecipe });
            }
            else
            {
                OnRecipeChangedEx(eventData.pChangedRecipe);
            }
        }
        private delegate void pOnRecipeChangedEx(RecipeData pChangeRecipeData);
        private void OnRecipeChangedEx(RecipeData pChangeRecipeData)
        {
            //當頁面停留在Recipe時，更新顯示畫面
            if (int.Parse(tabControl.SelectedTab.Tag.ToString()) == 2)
            {
                pShowRecipeData = pChangeRecipeData;
                ShowRecipeData(pShowRecipeData);
            }
            ShowRecipeNumber();
        }
        private void cboRecipeNO_DropDown(object sender, EventArgs e)
        {
            ShowRecipeInfo();
        }
        private void cboRecipeNO_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetRecipeData((uint)cboRecipeNO.SelectedIndex + 1);
        }
        private void btnSaveRecipe_Click(object sender, EventArgs e)
        {
            if (pShowRecipeData == null)
            {
                UserFunc.ErrorMessage("btnSaveRecipe", "pShowRecipeData = null", "");
                return;
            }

            bool bResult = Update_ShowRecipeData();
            if (!bResult)
            {
                UserFunc.ErrorMessage("btnSaveRecipe", "Update_ShowRecipeData Error", "");
                return;
            }

            bResult = eq.SaveRecipeData(pShowRecipeData);
            if (!bResult)
            {
                UserFunc.ErrorMessage("btnSaveRecipe", "SaveRecipeData Error", "");
            }
        }

        private void btnUseRecipe_Click(object sender, EventArgs e)
        {
            if (pShowRecipeData == null)
            {
                UserFunc.ErrorMessage("btnUseRecipe", "pShowRecipeData = null", "");
                return;
            }

            bool bResult = Update_ShowRecipeData();
            if (!bResult)
            {
                UserFunc.ErrorMessage("btnUseRecipe", "Update_ShowRecipeData Error", "");
                return;
            }

            bResult = eq.SaveRecipeData(pShowRecipeData);
            if (!bResult)
            {
                UserFunc.ErrorMessage("btnUseRecipe", "SaveRecipeData Error", "");
            }

            bResult = eq.LoadRecipe(pShowRecipeData.iRecipeNO);
            if (!bResult)
            {
                UserFunc.ErrorMessage("btnUseRecipe", "LoadRecipe Error", "");
            }
        }

        /// <summary>
        /// 根據號碼讀取Recipe並顯示出來
        /// </summary>
        private void GetRecipeData(uint iRecipeNO)
        {
            RecipeData pRecipeData = new RecipeData(iRecipeNO, eq.strSystemFolder);
            ShowRecipeData(pRecipeData);
        }

        private void ShowRecipeInfo(bool blnInitial = false)
        {
            string strRecipeDirectory = eq.strSystemFolder + "\\" + RecipeData.RecipeRootFolderName;
            for (int i = 0; i < 999; ++i)
            {
                uint iRecipeNO = (uint)i + 1;
                string strINIFolder = strRecipeDirectory + "\\" + RecipeData.RecipeSubFolderName + string.Format("{0:D3}", iRecipeNO);
                string strINIFile = strINIFolder + "\\" + RecipeData.RecipeFileName;

                if (Directory.Exists(strINIFolder) && File.Exists(strINIFile))
                {
                    string strName = "";
                    bool ok = UserFile.ReadIniValue(strINIFile, "Information", "Name", out strName, "");

                    if (blnInitial)
                    {
                        if (ok)
                            cboRecipeNO.Items.Add(string.Format("{0:D3}- {1}", iRecipeNO, strName));
                        else
                            cboRecipeNO.Items.Add(string.Format("{0:D3}", iRecipeNO));
                    }
                    else
                    {
                        if (ok)
                            cboRecipeNO.Items[i] = string.Format("{0:D3}- {1}", iRecipeNO, strName);
                        else
                            cboRecipeNO.Items[i] = string.Format("{0:D3}", iRecipeNO);
                    }
                }
                else
                {
                    if (blnInitial)
                    {
                        cboRecipeNO.Items.Add(string.Format("{0:D3}", iRecipeNO));
                    }
                    else
                    {
                        cboRecipeNO.Items[i] = string.Format("{0:D3}", iRecipeNO);
                    }
                }
            }
        }
        /// <summary>
        /// 顯示Recipe編號
        /// </summary>
        private void ShowRecipeNumber()
        {
            uint recipeNO = eq.pCurrentRecipe.iRecipeNO;
            string recipeName = eq.pCurrentRecipe.str_RecipeName;
            txtCurrentRecipeNO.Text = string.Format("{0:D3}", recipeNO);
            txtCurrentRecipeNO_Main.Text = string.Format("{0:D3}", recipeNO);
            txtRecipeName_Main.Text = recipeName;
        }

        /// <summary>
        /// 將指定Recipe顯示出來
        /// </summary>
        private void ShowRecipeData(RecipeData pRecipeData)
        {
            pShowRecipeData = pRecipeData;

            txtRecipeName.Text = pShowRecipeData.str_RecipeName.ToString();
            txtDisplayRecipeFile.Text = pShowRecipeData.iniFile;

            //參數
            nupdRecipeRunPathSpeed.Value = pShowRecipeData.RunPathSpeed;
            nupdRecipeRunPathAcc.Value = pShowRecipeData.RunPathAcc;
            pShowRecipeData.PointNoToSendModbus = Math.Min(pShowRecipeData.PointNoToSendModbus, pShowRecipeData.RobotPath.Count);
            nupdRecipePointNoToSendModbus.Maximum = pShowRecipeData.RobotPath.Count;
            nupdRecipePointNoToSendModbus.Value = pShowRecipeData.PointNoToSendModbus;
            
            DisplayRecipeRobotPath(pShowRecipeData.RobotPath);
        }       

        /// <summary>
        /// 將畫面文字寫到RecipeData
        /// </summary>
        private bool Update_ShowRecipeData()
        {
            try
            {
                pShowRecipeData.str_RecipeName = txtRecipeName.Text;

                // 參數
                pShowRecipeData.RunPathSpeed = (int)nupdRecipeRunPathSpeed.Value;
                pShowRecipeData.RunPathAcc = (int)nupdRecipeRunPathAcc.Value;
                pShowRecipeData.PointNoToSendModbus = (int)nupdRecipePointNoToSendModbus.Value;

                pShowRecipeData.RobotPath = GetDisplayedRecipeRobotPath();
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion
        #region Log
        private void OnScanLog(object sender, EventArgs e)
        {
            ScanLog log = (ScanLog)sender;
            pShowScanLog pfunc = new pShowScanLog(ShowScanLog);
            BeginInvoke(pfunc, new object[] { log });
        }

        private delegate void pShowScanLog(ScanLog log);
        private void ShowScanLog(ScanLog log)
        {
            object[] values = log.GetValues();
            dgvScanLog.Rows.RemoveAt(dgvScanLog.RowCount - 1);
            dgvScanLog.Rows.Insert(0, 1);
            for (int i = 0; i < dgvScanLog.ColumnCount; i++)
            {
                dgvScanLog.Rows[0].Cells[i].Value = values[i];
                if (log.logResult != "識別成功")
                {
                    dgvScanLog.Rows[0].Cells[i].Style.ForeColor = Color.White;
                    dgvScanLog.Rows[0].Cells[i].Style.BackColor = Color.DarkRed;
                }
            }
        }

        #endregion

        #region RobotPose表單
        private void nupdRecipePointNoToSendModbus_ValueChanged(object sender, EventArgs e)
        {
            nupdRecipePointNoToSendModbus.Maximum = dgvRecipePath.RowCount;
        }
        private void DisplayRobotPose(int index, Pose4 value)
        {
            string name = "";
            if (index == 0)
                name = "Home";
            else if(index == 1) 
                name = "拍照";

            dgvRobotPose.Rows[index].Cells[0].Value = name;
            dgvRobotPose.Rows[index].Cells[1].Value = value.X;
            dgvRobotPose.Rows[index].Cells[2].Value = value.Y;
            dgvRobotPose.Rows[index].Cells[3].Value = value.Z;
            dgvRobotPose.Rows[index].Cells[4].Value = value.R;
        }
        private Pose4 GetDisplayedRobotPose(int index)
        {
            Pose4 pos = new Pose4();
            pos.X = (float)dgvRobotPose.Rows[index].Cells[1].Value;
            pos.Y = (float)dgvRobotPose.Rows[index].Cells[2].Value;
            pos.Z = (float)dgvRobotPose.Rows[index].Cells[3].Value;
            pos.R = (float)dgvRobotPose.Rows[index].Cells[4].Value;
            return pos;
        }

        private void DisplayRecipeRobotPath(List<Pose4> poses)
        {
            dgvRecipePath.RowCount = poses.Count;
            for (int i = 0; i < poses.Count; i++)
            {
                dgvRecipePath.Rows[i].Cells[0].Value = (i + 1).ToString();
                dgvRecipePath.Rows[i].Cells[1].Value = poses[i].X;
                dgvRecipePath.Rows[i].Cells[2].Value = poses[i].Y;
                dgvRecipePath.Rows[i].Cells[3].Value = poses[i].Z;
                dgvRecipePath.Rows[i].Cells[4].Value = poses[i].R;
            }
        }
        private void AddNewToDisplayedRecipeRobotPath(Pose4 pose)
        {
            int last = dgvRecipePath.RowCount;
            dgvRecipePath.Rows.Add();
            dgvRecipePath.Rows[last].Cells[0].Value = (last + 1).ToString();
            dgvRecipePath.Rows[last].Cells[1].Value = pose.X;
            dgvRecipePath.Rows[last].Cells[2].Value = pose.Y;
            dgvRecipePath.Rows[last].Cells[3].Value = pose.Z;
            dgvRecipePath.Rows[last].Cells[4].Value = pose.R;

            nupdRecipePointNoToSendModbus.Maximum = dgvRecipePath.RowCount;
        }
        private void ModifySelectedRecipeRobotPath(int selectedIndex, Pose4 pose)
        {
            dgvRecipePath.Rows[selectedIndex].Cells[1].Value = pose.X;
            dgvRecipePath.Rows[selectedIndex].Cells[2].Value = pose.Y;
            dgvRecipePath.Rows[selectedIndex].Cells[3].Value = pose.Z;
            dgvRecipePath.Rows[selectedIndex].Cells[4].Value = pose.R;
        }
        private void RemoveSelectedRecipeRobotPath(int selectedIndex)
        {
            List<Pose4> poses = GetDisplayedRecipeRobotPath();
            poses.RemoveAt(selectedIndex);
            DisplayRecipeRobotPath(poses);

            nupdRecipePointNoToSendModbus.Maximum = dgvRecipePath.RowCount;
        }
        private List<Pose4> GetDisplayedRecipeRobotPath()
        {
            int count = dgvRecipePath.RowCount;
            List<Pose4> poses = new List<Pose4>();
            for (int i = 0; i < count; i++)
            {
                Pose4 pos = new Pose4();
                pos.X = (float)dgvRecipePath.Rows[i].Cells[1].Value;
                pos.Y = (float)dgvRecipePath.Rows[i].Cells[2].Value;
                pos.Z = (float)dgvRecipePath.Rows[i].Cells[3].Value;
                pos.R = (float)dgvRecipePath.Rows[i].Cells[4].Value;
                poses.Add(pos);
            }

            return poses;
        }
        private void btnSetHomePose_Click(object sender, EventArgs e)
        {
            bool ok = false;
            Pose4 pose = new Pose4();
            eq.avsAction.GetRobotPose(out ok, out pose);
            if (ok)
            {
                DisplayRobotPose(0, pose);
            }
        }
        private void btnSetScanPose_Click(object sender, EventArgs e)
        {
            bool ok = false;
            Pose4 pose = new Pose4();
            eq.avsAction.GetRobotPose(out ok, out pose);
            if (ok)
            {
                DisplayRobotPose(1, pose);
            }
        }
        private void btnToHomePose_Click(object sender, EventArgs e)
        {
            eq.ManualGoHome(10, 10);
        }
        private void btnToScanPose_Click(object sender, EventArgs e)
        {
            eq.ManualGoScan(10, 10);
        }
        private void btnRecipePathAddNew_Click(object sender, EventArgs e)
        {
            bool ok = false;
            Pose4 pose = new Pose4();
            eq.avsAction.GetRobotPose(out ok, out pose);
            if (ok)
            {
                AddNewToDisplayedRecipeRobotPath(pose);
            }
        }
        private void btnRecipePathGoTo_Click(object sender, EventArgs e)
        {
            if (dgvRecipePath.RowCount == 0)
                return;
            int index = dgvRecipePath.SelectedRows[0].Index;

            List<Pose4> poses = GetDisplayedRecipeRobotPath();
            eq.ManualGoTo(poses[index], 10, 10);
        }
        private void btnRecipePathModify_Click(object sender, EventArgs e)
        {
            if (dgvRecipePath.RowCount == 0)
                return;
            int index = dgvRecipePath.SelectedRows[0].Index;

            bool ok = false;
            Pose4 pose = new Pose4();
            eq.avsAction.GetRobotPose(out ok, out pose);
            if (ok)
            {
                ModifySelectedRecipeRobotPath(index, pose);
            }
        }
        private void btnRecipePathDelete_Click(object sender, EventArgs e)
        {
            if (dgvRecipePath.RowCount == 0)
                return;
            int index = dgvRecipePath.SelectedRows[0].Index;

            RemoveSelectedRecipeRobotPath(index);
        }
        private void btnChangeDrageMode_Click(object sender, EventArgs e)
        {
            if (eq.IsDragMode)
            {
                eq.SetDragMode(false);
            }
            else
            {
                eq.SetDragMode(true);
            }
        }
        #endregion
        private void btnInitial_Click(object sender, EventArgs e)
        {
            btnInitial.Enabled = false;
            this.Refresh();

            eq.Initial();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            eq.AutoRunStop();
        }

        private void btnAutoRun_Click(object sender, EventArgs e)
        {
            eq.AutoRunStart();
        }
        private void picURVISION_Click(object sender, EventArgs e)
        {
            frmEngineer.ShowDialog();
        }


    }
}
