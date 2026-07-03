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
            _startButton = new Button();
            _stopButton = new Button();
            _textBox = new TextBox();
            _simulationIndicatorLabel = new Label();
            SuspendLayout();
            // 
            // _statusLabel
            // 
            _statusLabel.AutoSize = true;
            _statusLabel.Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold);
            _statusLabel.Location = new Point(20, 20);
            _statusLabel.Name = "_statusLabel";
            _statusLabel.Size = new Size(67, 24);
            _statusLabel.TabIndex = 0;
            _statusLabel.Text = "Ready";
            // 
            // _startButton
            // 
            _startButton.Font = new Font("Microsoft JhengHei UI", 10F);
            _startButton.Location = new Point(75, 110);
            _startButton.Name = "_startButton";
            _startButton.Size = new Size(150, 40);
            _startButton.TabIndex = 1;
            _startButton.Text = "Start Production";
            _startButton.UseVisualStyleBackColor = true;
            _startButton.Click += StartButton_Click;
            // 
            // _stopButton
            // 
            _stopButton.Enabled = false;
            _stopButton.Font = new Font("Microsoft JhengHei UI", 10F);
            _stopButton.Location = new Point(75, 160);
            _stopButton.Name = "_stopButton";
            _stopButton.Size = new Size(150, 40);
            _stopButton.TabIndex = 2;
            _stopButton.Text = "Stop Production";
            _stopButton.UseVisualStyleBackColor = true;
            _stopButton.Click += StopButton_Click;
            // 
            // _textBox
            // 
            _textBox.Location = new Point(331, 12);
            _textBox.Name = "_textBox";
            _textBox.Size = new Size(200, 23);
            _textBox.TabIndex = 3;
            _textBox.Text = "TextBox";
            // 
            // _simulationIndicatorLabel
            // 
            _simulationIndicatorLabel.AutoSize = true;
            _simulationIndicatorLabel.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold);
            _simulationIndicatorLabel.Location = new Point(331, 50);
            _simulationIndicatorLabel.Name = "_simulationIndicatorLabel";
            _simulationIndicatorLabel.Padding = new Padding(10, 5, 10, 5);
            _simulationIndicatorLabel.Size = new Size(120, 25);
            _simulationIndicatorLabel.TabIndex = 4;
            _simulationIndicatorLabel.Text = "SIMULATION MODE";
            _simulationIndicatorLabel.BackColor = Color.LightGreen;
            _simulationIndicatorLabel.Visible = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 600);
            Controls.Add(_statusLabel);
            Controls.Add(_startButton);
            Controls.Add(_stopButton);
            Controls.Add(_textBox);
            Controls.Add(_simulationIndicatorLabel);
            MinimumSize = new Size(600, 400);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Shoe Mold Control System - Industrial Edition";
            FormClosing += MainForm_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        // UI Controls (對應原 XAML 中的控件)
        private Label _statusLabel;
        private Button _startButton;
        private Button _stopButton;
        private TextBox _textBox;  // 對應原 XAML 中的 _927tbx
        private Label _simulationIndicatorLabel;  // 模擬模式/連線狀態指示器
    }
}
