using System;
using System.Drawing;
using System.Windows.Forms;

namespace Industrial.UI.Framework
{
    /// <summary>
    /// 工業級防呆設計：長按充能確認按鈕 (Custom Control)
    /// 解決誤觸痛點，需長按指定時間才能觸發事件，並提供視覺回饋。
    /// 符合人因工程規範 (最小 40x40 像素點擊熱區)。
    /// </summary>
    public class IndustrialSafeButton : Button
    {
        private System.Windows.Forms.Timer _pressTimer;
        private int _currentHoldTimeMs = 0;
        private int _targetHoldTimeMs = 2000; // 預設長按 2 秒
        private bool _isTriggered = false;
        private bool _requireLongPress = true;
        
        // 工業級配色定義 (符合 ISO 標準)
        private static readonly Color ProgressColor = Color.FromArgb(243, 156, 18); // 工業黃
        private static readonly Color SuccessColor = Color.FromArgb(39, 174, 96);   // 成功綠
        private static readonly Color DangerColor = Color.FromArgb(231, 76, 60);    // 危險紅

        /// <summary>
        /// 安全點擊事件 (取代傳統的 Click 事件)
        /// </summary>
        public event EventHandler SafeClick;

        /// <summary>
        /// 長按時間設定 (毫秒)
        /// </summary>
        public int TargetHoldTimeMs
        {
            get => _targetHoldTimeMs;
            set
            {
                _targetHoldTimeMs = Math.Max(500, value); // 最小 500ms
            }
        }

        /// <summary>
        /// 是否需要長按確認 (可切換為一般按鈕)
        /// </summary>
        public bool RequireLongPress
        {
            get => _requireLongPress;
            set => _requireLongPress = value;
        }

        public IndustrialSafeButton()
        {
            // 設定預設樣式以符合工業規範
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 2;
            this.Font = new Font(this.Font.FontFamily, 12F, FontStyle.Bold);
            this.MinimumSize = new Size(120, 50); // 符合人因工程最小尺寸
            this.Cursor = Cursors.Hand;
            
            _pressTimer = new System.Windows.Forms.Timer { Interval = 50 }; // 20Hz 更新進度
            _pressTimer.Tick += PressTimer_Tick;

            this.MouseDown += IndustrialSafeButton_MouseDown;
            this.MouseUp += IndustrialSafeButton_MouseUp;
            this.Leave += IndustrialSafeButton_Leave;
            this.EnabledChanged += IndustrialSafeButton_EnabledChanged;
        }

        private void IndustrialSafeButton_EnabledChanged(object sender, EventArgs e)
        {
            if (!this.Enabled)
            {
                ResetPressState();
            }
        }

        private void IndustrialSafeButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.Enabled && _requireLongPress)
            {
                _isTriggered = false;
                _currentHoldTimeMs = 0;
                _pressTimer.Start();
                this.Invalidate(); // 立即重繪以顯示按下狀態
            }
        }

        private void IndustrialSafeButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (_requireLongPress)
            {
                ResetPressState();
            }
        }

        private void IndustrialSafeButton_Leave(object sender, EventArgs e)
        {
            // 當滑鼠離開控制項時，重置計時，防止卡住
            ResetPressState();
        }

        private void ResetPressState()
        {
            if (_pressTimer.Enabled)
            {
                _pressTimer.Stop();
                _currentHoldTimeMs = 0;
                _isTriggered = false;
                this.Invalidate(); // 清除進度條
            }
        }

        private void PressTimer_Tick(object sender, EventArgs e)
        {
            _currentHoldTimeMs += _pressTimer.Interval;
            
            if (_currentHoldTimeMs >= _targetHoldTimeMs)
            {
                _pressTimer.Stop();
                if (!_isTriggered)
                {
                    _isTriggered = true;
                    // 觸發安全事件
                    SafeClick?.Invoke(this, EventArgs.Empty);
                    
                    // 視覺回饋：閃爍綠色表示已成功觸發
                    var originalBackColor = this.BackColor;
                    this.BackColor = SuccessColor;
                    
                    // 非同步恢復背景色 (避免阻塞 UI)
                    Task.Delay(300).ContinueWith(t => 
                    {
                        if (!this.IsDisposed && this.IsHandleCreated)
                        {
                            this.Invoke(new Action(() => this.BackColor = originalBackColor));
                        }
                    });
                }
            }
            else
            {
                // 局部重繪進度動畫，避免全螢幕刷新
                this.Invalidate(); 
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            // 繪製長按進度條
            if (_currentHoldTimeMs > 0 && !_isTriggered && _requireLongPress)
            {
                double progress = (double)_currentHoldTimeMs / _targetHoldTimeMs;
                int width = (int)(this.ClientRectangle.Width * progress);
                int height = 8; // 進度條高度

                // 繪製於按鈕底部
                Rectangle progressRect = new Rectangle(
                    0, 
                    this.ClientRectangle.Height - height, 
                    width, 
                    height
                );
                
                using (Brush progressBrush = new SolidBrush(ProgressColor))
                {
                    pevent.Graphics.FillRectangle(progressBrush, progressRect);
                }
                
                // 繪製邊框以示區別
                using (Pen pen = new Pen(ProgressColor, 2))
                {
                    pevent.Graphics.DrawRectangle(pen, 
                        0, 0, 
                        this.ClientRectangle.Width - 1, 
                        this.ClientRectangle.Height - 1);
                }

                // 顯示倒數文字
                string remainingText = $"{(_targetHoldTimeMs - _currentHoldTimeMs) / 1000.0:F1}s";
                using (Font textFont = new Font(this.Font.FontFamily, 10F, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    SizeF textSize = pevent.Graphics.MeasureString(remainingText, textFont);
                    PointF textPos = new PointF(
                        (this.ClientRectangle.Width - textSize.Width) / 2,
                        (this.ClientRectangle.Height - textSize.Height) / 2
                    );
                    pevent.Graphics.DrawString(remainingText, textFont, textBrush, textPos);
                }
            }
        }

        /// <summary>
        /// 快速觸發模式 (用於緊急停止等特殊場景)
        /// </summary>
        public void TriggerImmediate()
        {
            SafeClick?.Invoke(this, EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pressTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// 雙重確認對話框 (非阻塞式)
    /// </summary>
    public class SafetyConfirmationDialog
    {
        /// <summary>
        /// 顯示非阻塞式確認對話框
        /// </summary>
        public static void ShowAsync(Control owner, string message, string title, Action<bool> callback)
        {
            if (owner.InvokeRequired)
            {
                owner.BeginInvoke(new Action(() => ShowAsync(owner, message, title, callback)));
                return;
            }

            var form = new Form
            {
                Text = title,
                Size = new Size(400, 200),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                StartPosition = FormStartPosition.CenterParent,
                TopMost = true
            };

            var label = new Label
            {
                Text = message,
                AutoSize = true,
                Location = new Point(20, 20),
                Font = new Font("Microsoft JhengHei UI", 12F),
                MaximumSize = new Size(350, 0)
            };

            var confirmButton = new IndustrialSafeButton
            {
                Text = "確認執行",
                Location = new Point(50, 80),
                Size = new Size(130, 50),
                TargetHoldTimeMs = 1500 // 1.5 秒確認
            };

            var cancelButton = new Button
            {
                Text = "取消",
                Location = new Point(200, 80),
                Size = new Size(130, 50),
                DialogResult = DialogResult.Cancel
            };

            confirmButton.SafeClick += (s, e) =>
            {
                form.DialogResult = DialogResult.OK;
                form.Close();
                callback?.Invoke(true);
            };

            cancelButton.Click += (s, e) =>
            {
                form.Close();
                callback?.Invoke(false);
            };

            form.Controls.Add(label);
            form.Controls.Add(confirmButton);
            form.Controls.Add(cancelButton);
            form.AcceptButton = cancelButton;

            // 非阻塞式顯示
            form.Show(owner);
        }
    }
}
