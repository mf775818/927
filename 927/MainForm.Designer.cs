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
            this.components = new System.ComponentModel.Container();
            
            // WinForm 容器屬性設定（對應原 XAML: Title="MainWindow" Height="450" Width="800"）
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 600);
            this.Text = "Shoe Mold Control System - Industrial Edition";
            this.MinimumSize = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);

            // Status Label（對應原 InitializeUi 中的 statusLabel，綁定 ViewModel.StatusText）
            this._statusLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold),
                Location = new Point(20, 20),
                Name = "statusLabel",
                TabIndex = 0,
                Text = "Ready"
            };

            // Start Button（對應原 InitializeUi 中的 startButton）
            this._startButton = new Button
            {
                Location = new Point(20, 60),
                Name = "startButton",
                Size = new Size(150, 40),
                TabIndex = 1,
                Text = "Start Production",
                UseVisualStyleBackColor = true,
                Font = new Font("Microsoft JhengHei UI", 10F)
            };
            this._startButton.Click += new System.EventHandler(this.StartButton_Click);

            // Stop Button（對應原 InitializeUi 中的 stopButton）
            this._stopButton = new Button
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
            this._stopButton.Click += new System.EventHandler(this.StopButton_Click);

            // TextBox（對應原 XAML 中的 _927tbx）
            // 原 XAML: <TextBox x:Name="_927tbx" TextWrapping="Wrap" Text="TextBox" Margin="60,33,481,226" Grid.ColumnSpan="2"/>
            this._textBox = new TextBox
            {
                Location = new Point(60, 33),
                Name = "_927tbx",
                Size = new Size(200, 23),
                TabIndex = 3,
                Text = "TextBox",
                Multiline = false
            };

            // Add all controls to Form
            this.Controls.Add(this._statusLabel);
            this.Controls.Add(this._startButton);
            this.Controls.Add(this._stopButton);
            this.Controls.Add(this._textBox);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // UI Controls (對應原 XAML 中的控件)
        private Label _statusLabel;
        private Button _startButton;
        private Button _stopButton;
        private TextBox _textBox;  // 對應原 XAML 中的 _927tbx
    }
}
