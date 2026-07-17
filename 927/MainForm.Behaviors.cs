using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serilog;
using ShoeMoldControl.Core.Hardware;
using Industrial.UI.Framework;

namespace _927
{
    partial class MainForm
    {
        // ========== Robot 工程師分頁事件處理器 ==========

        /// <summary>
        /// Robot 連接按鈕點擊 (防呆長按)
        /// </summary>
        private async void BtnRobotConnect_SafeClick(object? sender, EventArgs e)
        {
            try
            {
                _btnRobotConnect.Enabled = false;
                _lblRobotConnectionState.Text = "狀態：連接中...";
                _lblRobotConnectionState.ForeColor = AlarmWarning;

                await Task.Run(async () =>
                {
                    await _robotController.ConnectAsync(_cancellationToken);
                });

                if (_robotController.IsConnected)
                {
                    _lblRobotConnectionState.Text = "狀態：已連接";
                    _lblRobotConnectionState.ForeColor = AccentGreen;
                    _btnRobotConnect.Enabled = false;
                    _btnRobotDisconnect.Enabled = true;
                    _logger.Information("Robot connected successfully");
                }
                else
                {
                    _lblRobotConnectionState.Text = "狀態：連接失敗";
                    _lblRobotConnectionState.ForeColor = AlarmError;
                    _btnRobotConnect.Enabled = true;
                    _logger.Error("Robot connection failed");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error connecting to robot");
                _lblRobotConnectionState.Text = "狀態：錯誤";
                _lblRobotConnectionState.ForeColor = AlarmError;
                _btnRobotConnect.Enabled = true;
                MessageBox.Show(this, $"Robot 連接失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Robot 斷開連接按鈕點擊
        /// </summary>
        private async void BtnRobotDisconnect_Click(object? sender, EventArgs e)
        {
            try
            {
                await _robotController.DisconnectAsync();
                _lblRobotConnectionState.Text = "狀態：未連接";
                _lblRobotConnectionState.ForeColor = TextSecondary;
                _btnRobotConnect.Enabled = true;
                _btnRobotDisconnect.Enabled = false;
                _logger.Information("Robot disconnected");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error disconnecting robot");
                MessageBox.Show(this, $"Robot 斷開失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Jog 按鈕按下 (開始移動)
        /// </summary>
        private async void BtnJog_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is Button btn && btn.Tag is JogType jogType)
            {
                try
                {
                    _isJogActive = true;
                    _currentJogType = jogType;
                    btn.BackColor = Color.FromArgb(41, 128, 185); // 按下效果

                    var speedPercent = (int)_numJogSpeed.Value;
                    
                    if (_chkJogContinuous.Checked)
                    {
                        // 連續模式：持續發送 Jog 命令
                        while (_isJogActive && !_cancellationToken.IsCancellationRequested)
                        {
                            await _robotController.JogAsync(jogType, speedPercent, _cancellationToken);
                            await Task.Delay(100, _cancellationToken); // 10Hz 更新
                        }
                    }
                    else
                    {
                        // 單步模式：執行一次 Jog
                        await _robotController.JogAsync(jogType, speedPercent, _cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    // 正常取消
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Jog error: {jogType}");
                    _isJogActive = false;
                }
            }
        }

        /// <summary>
        /// Jog 按鈕放開 (停止移動)
        /// </summary>
        private void BtnJog_MouseUp(object? sender, MouseEventArgs e)
        {
            _isJogActive = false;
            
            if (sender is Button btn)
            {
                btn.BackColor = JogButtonColor; // 恢復原色
            }
        }

        /// <summary>
        /// Jog 按鈕失去焦點 (安全停止)
        /// </summary>
        private void BtnJog_Leave(object? sender, EventArgs e)
        {
            _isJogActive = false;
            
            if (sender is Button btn)
            {
                btn.BackColor = JogButtonColor;
            }
        }

        /// <summary>
        /// Jog 急停按鈕 (防呆)
        /// </summary>
        private async void BtnJogStop_SafeClick(object? sender, EventArgs e)
        {
            try
            {
                _isJogActive = false;
                await _robotController.StopJogAsync();
                _logger.Information("Jog stopped by emergency button");
                
                // 視覺回饋
                _btnJogStop.BackColor = Color.FromArgb(192, 57, 43);
                await Task.Delay(200);
                _btnJogStop.BackColor = AlarmError;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error stopping jog");
            }
        }

        /// <summary>
        /// 刷新座標顯示
        /// </summary>
        private async void BtnRefreshPose_Click(object? sender, EventArgs e)
        {
            try
            {
                _btnRefreshPose.Enabled = false;
                _btnRefreshPose.Text = "🔄 刷新中...";

                var pose = await _robotController.GetPositionAsync(_cancellationToken);
                
                if (pose != null)
                {
                    _cachedRobotPose = pose;
                    UpdatePoseDisplay(pose);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error refreshing pose");
                MessageBox.Show(this, $"獲取座標失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _btnRefreshPose.Enabled = true;
                _btnRefreshPose.Text = "🔄 刷新";
            }
        }

        /// <summary>
        /// 更新座標顯示 UI
        /// </summary>
        private void UpdatePoseDisplay(RobotCoordinatePose pose)
        {
            this.InvokeIfRequired(() =>
            {
                _lblPoseXValue.Text = pose.X.ToString("F3");
                _lblPoseYValue.Text = pose.Y.ToString("F3");
                _lblPoseZValue.Text = pose.Z.ToString("F3");
                _lblPoseRValue.Text = pose.R.ToString("F3");
            });
        }

        /// <summary>
        /// 執行命令按鈕 (防呆)
        /// </summary>
        private async void BtnExecuteCommand_SafeClick(object? sender, EventArgs e)
        {
            var command = _txtCommandInput.Text.Trim();
            
            if (string.IsNullOrEmpty(command))
            {
                MessageBox.Show(this, "請輸入命令", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                _btnExecuteCommand.Enabled = false;
                _lstCommandHistory.Items.Insert(0, $"> {command} [{DateTime.Now:HH:mm:ss}]");

                // 解析並執行命令
                await ExecuteRobotCommand(command);
                
                _lstCommandHistory.Items.Insert(0, $"✓ 執行成功 [{DateTime.Now:HH:mm:ss}]");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Command execution failed: {command}");
                _lstCommandHistory.Items.Insert(0, $"✗ 錯誤：{ex.Message} [{DateTime.Now:HH:mm:ss}]");
                MessageBox.Show(this, $"命令執行失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _btnExecuteCommand.Enabled = true;
            }
        }

        /// <summary>
        /// 執行 Robot 命令
        /// </summary>
        private async Task ExecuteRobotCommand(string command)
        {
            var cmdUpper = command.ToUpper();
            
            if (cmdUpper.StartsWith("MOVE "))
            {
                // MOVE X Y Z R
                var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 5)
                {
                    double x = double.Parse(parts[1]);
                    double y = double.Parse(parts[2]);
                    double z = double.Parse(parts[3]);
                    double r = double.Parse(parts[4]);
                    await _robotController.MoveToAsync(x, y, z, r, _cancellationToken);
                }
                else
                {
                    throw new ArgumentException("命令格式：MOVE X Y Z R");
                }
            }
            else if (cmdUpper == "HOME")
            {
                await _robotController.HomeAsync(_cancellationToken);
            }
            else if (cmdUpper == "STOP")
            {
                await _robotController.StopAsync();
            }
            else
            {
                throw new NotImplementedException($"未知命令：{command}");
            }
        }


        // ========== 視覺虛實整合分頁事件處理器 ==========

        /// <summary>
        /// 切換模擬/實際模式
        /// </summary>
        private void RbVisionMode_CheckedChanged(object? sender, EventArgs e)
        {
            if (_rbVisionModeSimulation.Checked)
            {
                _visionSystemMode = VisionSystemMode.Simulation;
                _lblVisionModeStatus.Text = "當前：模擬模式";
                _lblVisionModeStatus.ForeColor = AccentColor;
                _pnlSimulationControlPanel.Enabled = true;
                _logger.Information("Vision mode switched to Simulation");
            }
            else if (_rbVisionModeReal.Checked)
            {
                _visionSystemMode = VisionSystemMode.Real;
                _lblVisionModeStatus.Text = "當前：實際模式";
                _lblVisionModeStatus.ForeColor = AccentGreen;
                _pnlSimulationControlPanel.Enabled = false;
                _logger.Information("Vision mode switched to Real");
            }
        }

        /// <summary>
        /// 單張影像擷取 (防呆按鈕)
        /// </summary>
        private async void BtnCaptureSingle_SafeClick(object? sender, EventArgs e)
        {
            await CaptureAndProcessImage();
        }

        /// <summary>
        /// 開始連續擷取
        /// </summary>
        private async void BtnStartContinuous_Click(object? sender, EventArgs e)
        {
            try
            {
                _isContinuousCaptureRunning = true;
                _btnStartContinuous.Enabled = false;
                _btnStopContinuous.Enabled = true;
                _btnCaptureSingle.Enabled = false;

                _continuousCaptureCts = new CancellationTokenSource();

                while (_isContinuousCaptureRunning && !_continuousCaptureCts.Token.IsCancellationRequested)
                {
                    await CaptureAndProcessImage();
                    await Task.Delay(500, _continuousCaptureCts.Token); // 2 FPS
                }
            }
            catch (OperationCanceledException)
            {
                // 正常停止
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in continuous capture");
            }
            finally
            {
                _btnStartContinuous.Enabled = true;
                _btnStopContinuous.Enabled = false;
                _btnCaptureSingle.Enabled = true;
            }
        }

        /// <summary>
        /// 停止連續擷取
        /// </summary>
        private void BtnStopContinuous_Click(object? sender, EventArgs e)
        {
            _isContinuousCaptureRunning = false;
            _continuousCaptureCts?.Cancel();
            _logger.Information("Continuous capture stopped");
        }

        /// <summary>
        /// 儲存影像
        /// </summary>
        private void BtnSaveImage_Click(object? sender, EventArgs e)
        {
            if (_lastCapturedImage == null)
            {
                MessageBox.Show(this, "無影像可儲存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using var saveDialog = new SaveFileDialog
                {
                    Filter = "PNG 影像|*.png|JPEG 影像|*.jpg|BMP 影像|*.bmp",
                    FileName = $"Vision_{DateTime.Now:yyyyMMdd_HHmmss}.png"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // 將 Avl.Image 轉換為 Bitmap 並儲存
                    var bitmap = ConvertAvlImageToBitmap(_lastCapturedImage);
                    bitmap.Save(saveDialog.FileName);
                    
                    _logger.Information($"Image saved to {saveDialog.FileName}");
                    MessageBox.Show(this, $"影像已儲存至:\n{saveDialog.FileName}", "成功", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error saving image");
                MessageBox.Show(this, $"儲存失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 套用模擬參數至影像
        /// </summary>
        private async void BtnApplyImage_Click(object? sender, EventArgs e)
        {
            if (_visionSystemMode != VisionSystemMode.Simulation)
            {
                MessageBox.Show(this, "請先切換至模擬模式", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // 更新模擬 Robot 座標
                var simulatedPose = new RobotCoordinatePose
                {
                    X = (double)_numSimulatedX.Value,
                    Y = (double)_numSimulatedY.Value,
                    Z = (double)_numSimulatedZ.Value,
                    R = (double)_numSimulatedR.Value
                };

                _cachedRobotPose = simulatedPose;
                UpdatePoseDisplay(simulatedPose);

                // 生成模擬影像
                await CaptureAndProcessImage();

                _logger.Information($"Applied simulated pose: {simulatedPose}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error applying simulated pose");
            }
        }

        /// <summary>
        /// 擷取並處理影像 (支援模擬/實際模式)
        /// </summary>
        private async Task CaptureAndProcessImage()
        {
            try
            {
                Avl.Image image;
                VisionInspectionResult result;

                if (_visionSystemMode == VisionSystemMode.Simulation)
                {
                    // 模擬模式：生成虛擬數據
                    var mockVision = _visionService as MockVisionService;
                    if (mockVision != null)
                    {
                        var barcode = _chkSimulateBarcode.Checked ? _txtSimulatedBarcode.Text : null;
                        var simPose = new RobotCoordinatePose
                        {
                            X = (double)_numSimulatedX.Value,
                            Y = (double)_numSimulatedY.Value,
                            Z = (double)_numSimulatedZ.Value,
                            R = (double)_numSimulatedR.Value
                        };
                        
                        image = mockVision.GenerateTestImage(simPose, barcode);
                        result = new VisionInspectionResult
                        {
                            Success = true,
                            Barcode = barcode ?? "SIM-001",
                            Confidence = 95.5,
                            MarkPositions = new System.Drawing.PointF[]
                            {
                                new PointF((float)_numSimulatedX.Value, (float)_numSimulatedY.Value)
                            }
                        };
                    }
                    else
                    {
                        // Fallback: 使用預設模擬
                        image = await _visionService.CaptureAsync(_cancellationToken);
                        result = await _visionService.InspectAsync(image, _cancellationToken);
                    }
                }
                else
                {
                    // 實際模式：串接硬體
                    image = await _visionService.CaptureAsync(_cancellationToken);
                    result = await _visionService.InspectAsync(image, _cancellationToken);
                }

                // 更新 UI
                _lastCapturedImage = image;
                _lastInspectionResult = result;
                UpdateVisionDisplay(image, result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error capturing and processing image");
                this.InvokeIfRequired(() =>
                {
                    _imgCapturedImage.Visible = false;
                    _lblNoImagePlaceholder.Visible = true;
                    _lblNoImagePlaceholder.Text = $"❌ 錯誤\n{ex.Message}";
                });
            }
        }

        /// <summary>
        /// 更新視覺顯示 UI
        /// </summary>
        private void UpdateVisionDisplay(Avl.Image image, VisionInspectionResult result)
        {
            this.InvokeIfRequired(() =>
            {
                // 顯示影像
                var bitmap = ConvertAvlImageToBitmap(image);
                _imgCapturedImage.Image = bitmap;
                _imgCapturedImage.Visible = true;
                _lblNoImagePlaceholder.Visible = false;

                // 更新檢測結果
                _lblInspectionSuccessValue.Text = result.Success ? "OK ✓" : "NG ✗";
                _lblInspectionSuccessValue.ForeColor = result.Success ? AccentGreen : AlarmError;

                _lblBarcodeResultValue.Text = result.Barcode ?? "N/A";
                _lblConfidenceValue.Text = $"{result.Confidence:F1}%";
                
                if (result.MarkPositions != null && result.MarkPositions.Length > 0)
                {
                    var pos = result.MarkPositions[0];
                    _lblMarkPositionsValue.Text = $"({pos.X:F2}, {pos.Y:F2})";
                }
                else
                {
                    _lblMarkPositionsValue.Text = "(--, --)";
                }
            });
        }

        /// <summary>
        /// 將 Avl.Image 轉換為 Bitmap
        /// </summary>
        private Bitmap ConvertAvlImageToBitmap(Avl.Image avlImage)
        {
            if (avlImage == null) return null;

            // 假設 Avl.Image 有 ToBitmap() 方法或類似 API
            // 實際實現需根據 Avl.Image 的具體結構調整
            try
            {
                // 嘗試直接轉換 (如果 Avl.Image 支援)
                if (avlImage is Bitmap bmp)
                    return bmp;

                // 或使用 MemoryStream 轉換
                using var ms = new MemoryStream();
                avlImage.Save(ms); // 假設有 Save 方法
                ms.Position = 0;
                return new Bitmap(ms);
            }
            catch
            {
                // Fallback: 創建測試圖案
                return CreateTestPatternImage();
            }
        }

        /// <summary>
        /// 創建測試圖案 (當無法轉換影像時使用)
        /// </summary>
        private Bitmap CreateTestPatternImage()
        {
            var bmp = new Bitmap(640, 480);
            using var g = Graphics.FromImage(bmp);
            
            // 背景
            g.Clear(Color.FromArgb(20, 20, 20));
            
            // 網格線
            using var pen = new Pen(Color.FromArgb(50, 50, 50), 1);
            for (int i = 0; i < 640; i += 40)
            {
                g.DrawLine(pen, i, 0, i, 480);
            }
            for (int i = 0; i < 480; i += 40)
            {
                g.DrawLine(pen, 0, i, 640, i);
            }
            
            // 文字
            using var font = new Font("Microsoft JhengHei UI", 16F);
            using var brush = new SolidBrush(TextPrimary);
            var text = _visionSystemMode == VisionSystemMode.Simulation 
                ? "🎭 模擬影像" 
                : "🔌 實際影像";
            g.DrawString(text, font, brush, new PointF(20, 20));
            
            // 時間戳
            using var smallFont = new Font("Consolas", 12F);
            g.DrawString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), smallFont, brush, new PointF(20, 440));
            
            return bmp;
        }


        // ========== 儀表板分頁輔助方法 ==========

        /// <summary>
        /// 啟動狀態輪詢 (定期更新 Robot 和視覺狀態)
        /// </summary>
        private void StartStatusPolling()
        {
            Task.Run(async () =>
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await PollRobotStatus();
                        await PollVisionStatus();
                        await Task.Delay(500, _cancellationToken); // 2Hz 更新
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Error in status polling");
                        await Task.Delay(2000); // 錯誤時降頻
                    }
                }
            }, _cancellationToken);
        }

        /// <summary>
        /// 輪詢 Robot 狀態
        /// </summary>
        private async Task PollRobotStatus()
        {
            if (!_robotController.IsConnected) return;

            try
            {
                var mode = await _robotController.GetModeAsync(_cancellationToken);
                var cmdId = await _robotController.GetCurrentCommandIdAsync(_cancellationToken);
                var pose = await _robotController.GetPositionAsync(_cancellationToken);

                // 快取檢查：僅在變化時更新 UI
                if (mode != _cachedRobotMode || cmdId != _cachedCommandId)
                {
                    _cachedRobotMode = mode;
                    _cachedCommandId = cmdId;
                    _cachedRobotPose = pose;

                    this.InvokeIfRequired(() =>
                    {
                        UpdateDashboardRobotStatus(mode, cmdId, pose);
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Error polling robot status");
            }
        }

        /// <summary>
        /// 輪詢視覺狀態
        /// </summary>
        private async Task PollVisionStatus()
        {
            // 實際應用中可從此處獲取視覺系統心跳、條碼讀取結果等
            await Task.CompletedTask;
        }

        /// <summary>
        /// 更新儀表板 Robot 狀態顯示
        /// </summary>
        private void UpdateDashboardRobotStatus(RobotMode mode, int commandId, RobotCoordinatePose pose)
        {
            _lblRobotModeValue.Text = mode.ToString();
            _lblRobotCommandIdValue.Text = commandId.ToString();
            
            var connStatus = _robotController.IsConnected ? "已連接" : "未連接";
            _lblRobotConnectionStatus.Text = connStatus;
            _lblRobotConnectionStatus.ForeColor = _robotController.IsConnected ? AccentGreen : StatusError;

            // 同步至工程師分頁
            if (pose != null)
            {
                UpdatePoseDisplay(pose);
            }
        }
    }
}
