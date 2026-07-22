# 927-Demo UI 移植說明

## 檔案結構

```
927/UI/
├── DemoMainForm.cs              # 主程式碼 (行為邏輯)
├── DemoMainForm.Designer.cs     # 設計器代碼 (UI 佈局)
├── DemoThemeConstants.cs        # UI 主題色彩
├── HighPerformanceUiRefresher.cs # 高效能 UI 刷新工具
└── IndustrialSafeButton.cs      # 工業級安全按鈕
```

## 快速開始

### 1. 直接使用 (無依賴注入)

```csharp
using _927.UI;

// 演示模式 - 不需要任何服務
var form = new DemoMainForm();
Application.Run(form);
```

### 2. 依賴注入模式

```csharp
using _927.UI;
using _927.Core.Interfaces;

// 註冊服務
var robotService = serviceProvider.GetRequiredService<IAvrRobotMotion>();
var visionService = serviceProvider.GetRequiredService<IAvrVisionService>();

// 注入服務
var form = new DemoMainForm(robotService, visionService);
Application.Run(form);
```

## 功能特點

### ✅ 已完整移植的 927-Demo 功能

1. **5 分頁架構**
   - 主監控 (Monitor)
   - 配方管理 (Recipe)
   - 系統設定 (Settings)
   - 警報記錄 (Alarm)
   - 手動控制 (Control)

2. **操作按鈕組**
   - 系統初始化 (藍色)
   - 自動運行 (綠色)
   - 緊急停止 (橘色)
   - 按鈕狀態機 (Enable/Disable/Color)

3. **視覺元素**
   - 狀態指示器 (模擬 ActivityIndicator)
   - 掃描記錄 DataGridView
   - 警報記錄 DataGridView
   - 配方管理 DataGridView
   - 日誌輸出 TextBox (深色主題)

4. **交互邏輯**
   - tmrStatus (500ms) - 狀態輪詢
   - tmrUIRefresh (100ms) - UI 更新節流
   - 按鈕點擊事件處理
   - 狀態色彩變化

### 🛡️ 工業級增強

1. **雙緩衝繪製** - 解決閃爍問題
2. **UI 節流機制** - 防止高頻更新卡頓
3. **非同步操作** - 不阻塞 UI 執行緒
4. **例外處理** - 完整的 try-catch 保護
5. **線程安全** - 所有 UI 更新透過 Invoke

## 編譯測試

### 最低需求
- .NET Framework 4.7.2+
- System.Windows.Forms
- System.Drawing

### 編譯命令

```bash
csc /target:winexe /out:DemoUI.exe ^
    /reference:System.Windows.Forms.dll ^
    /reference:System.Drawing.dll ^
    DemoMainForm.cs DemoMainForm.Designer.cs
```

## 與 927 核心整合

若您的 927 專案已有這些介面，請刪除本檔案中的介面定義：

- `IAvrRobotMotion`
- `IAvrVisionService`

然後在 `DemoMainForm.cs` 頂部添加：

```csharp
using _927.Core.Interfaces;
```

並移除文件底部的介面定義區塊。

## 已知限制

1. **相機預覽** - 使用 Label 模擬，實際使用時需替換為真實影像控制項
2. **硬體通訊** - 演示模式下使用 Task.Delay 模擬
3. **第三方控制項** - AuroraVision/HMI.Controls 已替換為原生控制項

## 下一步

1. 將 `DemoMainForm` 設為啟動表單
2. 實作真實的 `IAvrRobotMotion` 和 `IAvrVisionService`
3. 連接實際硬體設備
4. 調整 UI 佈局以符合現場需求
