# 927 + 927-Demo 工業級整合實作報告

## 執行摘要

本報告詳述將 **927-Demo 的優質 UI 交互體驗** 移植到 **927 工業級架構** 的手術刀式整合方案。目標是保留 927 的擴充性與強健性，同時融入 927-Demo 的直觀操作介面。

---

## 1. 現狀分析

### 1.1 927 專案優勢 (目標架構)
| 層級 | 組件 | 技術特點 |
|------|------|---------|
| **UI 層** | `MainForm.cs` + `MainWindowViewModel.cs` | MVVM 模式、HFC/HPC 高頻刷新 |
| **應用層** | `ShoeMoldWorkflow.cs` | 生產週期狀態機、依賴注入 |
| **基礎設施** | `DobotCraController.cs` | TPL 異步、Polly 斷路器 |
| **視覺層** | `AvlAdvancedServices.cs` | 記憶體池、零拷貝、GCHandle 定錨 |
| **硬體抽象** | `IAvrHardwareInterfaces` + `AvrHardwareGateway` | 依賴反轉、Adapter 模式 |

### 1.2 927-Demo 優勢 (UI 來源)
| 模組 | 組件 | UI 交互特點 |
|------|------|------------|
| **主視圖** | `FormMain.cs` | 多分頁導航、即時狀態著色、ActivityIndicator |
| **相機線程** | `CameraThread.cs` | 簡單直觀的取像循環 (但需重構為 TPL) |
| **設備控制** | `Equipment_Robot.cs` | Jog 拖曳模式、手動/自動切換 |
| **警報系統** | `Equipment_Alarm.cs` | DataGridView 即時更新、顏色編碼 |
| **配方管理** | `RecipeData.cs` | INI 檔案讀寫、參數克隆驗證 |

---

## 2. 整合策略矩陣

| 遷移項目 | 927-Demo 來源 | 927 目標位置 | 遷移模式 | 優先級 |
|---------|--------------|-------------|---------|--------|
| **UI 主題與配色** | `FormMain.Designer.cs` 色彩邏輯 | `MainForm.cs` 深色主題 | 提取色彩常數，增強對比度 | P0 |
| **分頁導航架構** | `tabControl` 5 分頁設計 | `MainForm` 3 分頁 | 擴展為 5 分頁 (新增 Recipe/Alarm) | P0 |
| **警報顯示組件** | `dgvAlarm` DataGridView | `alarmListBox` | 改用 DataGridView 支援排序/過濾 | P1 |
| **Jog 拖曳模式** | `btnChangeDrageMode` | `InitializeRobotEngineerPage()` | 整合現有 Jog 按鈕群組 | P1 |
| **配方管理 UI** | `tabPageRecipe` | 新增 `RecipeManagementPage` | 完整移植 + ViewModel 綁定 | P2 |
| **掃描記錄 UI** | `dgvScanLog` | 新增 `ScanHistoryPage` | 移植 + 分頁虛擬化優化 | P2 |
| **初始化流程** | `btnInitial` 雙模式 | `IndustrialSafeButton` | 擴展長按確認对话框 | P1 |
| **活動指示器** | `ActivityIndicator` | 新增 `UiBusyIndicator.cs` | 自製符合 ISO 9241 組件 | P2 |

---

## 3. 手術刀式移植步驟

### 階段一：UI 抽離與重構 (Week 1-2)

#### 步驟 1.1: 提取 927-Demo 視覺資產
```csharp
// 新建 /workspace/927/UI/DemoThemeConstants.cs
namespace Industrial.UI.Framework
{
    public static class DemoThemeConstants
    {
        // 從 FormMain.Designer.cs 提取的狀態色彩
        public static readonly Color StatusNotInitialed = Color.LightSkyBlue;
        public static readonly Color StatusInitializing = Color.White;
        public static readonly Color StatusIdle = Color.Transparent;
        public static readonly Color StatusAutoRunning = Color.SeaGreen;
        
        // 警報色彩
        public static readonly Color AlarmError = Color.Orange;
        public static readonly Color AlarmWarning = Color.Gold;
        
        // 按鈕交互色彩
        public static readonly Color ButtonEnabled_Foreground = Color.White;
        public static readonly Color ButtonDisabled_Background = Color.Transparent;
    }
}
```

#### 步驟 1.2: 擴展 MainForm 為 5 分頁架構
```csharp
// 修改 MainForm.Designer.cs
// 原 3 分頁：_robotVisionMonitorPage, _robotEngineerPage, _visionIntegrationPage
// 新增 2 分頁：_recipeManagementPage, _scanHistoryPage

private TabPage _recipeManagementPage;
private TabPage _scanHistoryPage;

private void InitializeRecipeManagementPage()
{
    _recipeManagementPage = new TabPage("配方管理");
    _recipeManagementPage.Tag = "2";
    // ... 移植 FormMain.cs 的 Recipe UI 邏輯
}

private void InitializeScanHistoryPage()
{
    _scanHistoryPage = new TabPage("掃描記錄");
    _scanHistoryPage.Tag = "3";
    // ... 移植 dgvScanLog 邏輯
}
```

#### 步驟 1.3: 轉換 DataGridView 綁定
```csharp
// 新建 /workspace/927/ViewModels/AlarmEntryViewModel.cs
public class AlarmEntryViewModel : INotifyPropertyChanged
{
    public DateTime Timestamp { get; set; }
    public string AlarmCode { get; set; }
    public string Message { get; set; }
    public AlarmLevel Level { get; set; }
    
    // 實現 INotifyPropertyChanged 以支援 WPF 風格綁定
}

// 在 MainWindowViewModel 中添加 ObservableCollection<AlarmEntryViewModel>
public ObservableCollection<AlarmEntryViewModel> AlarmEntries { get; } 
    = new ObservableCollection<AlarmEntryViewModel>();
```

### 階段二：核心業務邏輯注入 (Week 3-4)

#### 步驟 2.1: 廢棄 CameraThread，改用 TPL
```csharp
// 927-Demo 的傳統 Thread 模型 (需廢棄)
public class CameraThread
{
    private Thread thread;  // ❌ 傳統執行緒
    private void Loop() { while(!stop) { Thread.Sleep(5); ... } }
}

// 927 的 TPL 模型 (目標架構)
// 已存在於 GenericVisionService.cs
public class GenericVisionService<TFrame> : IVisionService
{
    public async Task<VisionInspectionResult> InspectAsync(CancellationToken token)
    {
        var frame = await _cameraDriver.CaptureFrameAsync(token);  // ✅ TPL 異步
        return _imageAnalyzer.Analyze(frame);
    }
}
```

#### 步驟 2.2: 對接 Equipment 邏輯到 Application 服務
```csharp
// 新建 /workspace/927/Application/Services/RecipeManagementService.cs
public interface IRecipeManagementService
{
    Task<RecipeData> LoadRecipeAsync(uint recipeNo, CancellationToken token);
    Task SaveRecipeAsync(RecipeData recipe, CancellationToken token);
    Task<bool> ValidateRecipeAsync(RecipeData recipe, CancellationToken token);
}

public class RecipeManagementService : IRecipeManagementService
{
    private readonly IRecipeRepository _repository;
    private readonly IResiliencePolicyProvider _policyProvider;
    
    public async Task<RecipeData> LoadRecipeAsync(uint recipeNo, CancellationToken token)
    {
        return await _policyProvider.VisionRetryPolicy.ExecuteAsync(
            ctx => _repository.LoadAsync(recipeNo),
            new Context { ["RecipeNo"] = recipeNo }
        );
    }
}
```

#### 步驟 2.3: 配置 Autofac 依賴注入
```csharp
// 修改 /workspace/927/Application/Services/DependencyInjection.cs
public static IServiceCollection ConfigureServices()
{
    var services = new ServiceCollection();
    
    // 既有註冊
    services.AddSingleton<IVisionService, GenericVisionService<ManagedFrame>>();
    services.AddSingleton<IRobotController, DobotCraController>();
    
    // 新增 927-Demo 功能對應的服務
    services.AddSingleton<IRecipeManagementService, RecipeManagementService>();
    services.AddSingleton<IScanHistoryService, ScanHistoryService>();
    services.AddSingleton<IAlarmHistoryService, AlarmHistoryService>();
    
    // 註冊 UI 服務
    services.AddTransient<MainForm>();
    
    return services;
}
```

### 階段三：基礎設施層橋接 (Week 5-6)

#### 步驟 3.1: 整合硬體 Gateway 與 Adapter
```csharp
// 927-Demo 的 Equipment 直接操作硬體 (需解耦)
public partial class Equipment
{
    public Procedure avsAction;  // ❌ 緊耦合 AVL 巨集
    
    public void ManualGoTo(Pose4 pose, int speed, int acc)
    {
        avsAction.ManualGoTo(pose, speed, acc, true, out result);  // 直接調用
    }
}

// 927 的抽象層 (目標架構)
// 已存在於 IAvrHardwareInterfaces.cs
public interface IAvrRobotMotion
{
    Task<HardwareMotionResult> MoveLinearAsync(RobotCoordinatePose pose, ...);
}

// 實作 Adapter 對接 AVL
public class AvrRobotMotionAdapter : IAvrRobotMotion
{
    private readonly Procedure _avsMacro;
    
    public async Task<HardwareMotionResult> MoveLinearAsync(...)
    {
        // 透過 AvrHardwareGateway 安全調用
        return await _gateway.ExecuteSafeFuncAsync(macros => 
        {
            macros.ManualGoTo(pose, speed, acc, out bool success);
            return success ? HardwareMotionResult.Success() : HardwareMotionResult.Failed();
        }, token);
    }
}
```

#### 步驟 3.2: 套用 Polly 容錯策略到 UI 操作
```csharp
// 擴展 ResiliencePolicyProvider
public interface IResiliencePolicyProvider
{
    // 既有策略
    AsyncRetryPolicy<DecodeResult> VisionRetryPolicy { get; }
    
    // 新增 UI 相關策略
    AsyncRetryPolicy<RecipeData> RecipeLoadRetryPolicy { get; }
    AsyncCircuitBreakerPolicy HardwareCommunicationBreaker { get; }
}

// 在 MainForm 中使用
private async void btnInitial_Click(object sender, EventArgs e)
{
    try
    {
        var policy = _policyProvider.HardwareCommunicationBreaker;
        await policy.ExecuteAsync(async () =>
        {
            await _robotController.ConnectAsync();
            await _visionService.InitializeAsync(_cancellationToken);
        });
    }
    catch (BrokenCircuitException)
    {
        _logger.Warning("硬體通訊斷路器開啟，請檢查連線");
        ShowAlarm("硬體通訊失敗，請重試");
    }
}
```

#### 步驟 3.3: 橋接 Vision 視覺模組
```csharp
// 927-Demo 的 AVS Macro 調用模式
eq.avsAction.Grab_ReadQRRecipe(image, imageDetected, out ok, out recipeNo);

// 927 的泛型視覺服務 (已存在)
var visionService = _serviceProvider.GetRequiredService<IVisionService>();
var result = await visionService.InspectAsync(_cancellationToken);

// 結果綁定到 UI
_imageRefresher.UpdateImage(result.Frame);
_barcodeLabel.Text = result.BarcodeData;
```

---

## 4. 技術難點與解決方案

### 4.1 UI 執行緒與資料綁定瓶頸

**問題**: 927-Demo 的 UI 緊密耦合控制邏輯，直接操作 Control.Invoke

**解決方案**:
```csharp
// 使用 HighPerformanceUiRefresher 進行節流更新
_dataRefresher = new HighPerformanceUiRefresher<MachineDataModel>(
    _machineDataModel,
    this,
    UpdateDashboardFromModel,
    throttleIntervalMs: 33  // 30 FPS
);

// ViewModel 屬性變更自動觸發 UI 更新
_model.PropertyChanged += (s, e) => 
{
    if (e.PropertyName == nameof(MachineDataModel.AlarmCode))
    {
        _alarmSuppressor.SubmitAlarm(
            $"ALM-{_model.AlarmCode}", 
            GetAlarmMessage(_model.AlarmCode), 
            AlarmLevel.Error
        );
    }
};
```

### 4.2 多執行緒與資源競爭

**問題**: CameraThread 使用 lock(image) 造成阻塞

**解決方案**:
```csharp
// 使用 Channel<T> 進行非同步訊息傳遞
private readonly Channel<ManagedFrame> _frameChannel = Channel.CreateBounded<ManagedFrame>(10);

// 生產者 (視覺服務)
await _frameChannel.Writer.WriteAsync(capturedFrame, token);

// 消費者 (UI 刷新)
while (await _frameChannel.Reader.WaitToReadAsync(token))
{
    var frame = await _frameChannel.Reader.ReadAsync(token);
    _uiRefresher.ScheduleUpdate(() => RenderFrame(frame));
}
```

### 4.3 硬體通訊解耦

**問題**: Equipment 類別直接依賴 AVL Macro

**解決方案**:
```csharp
// 透過 DI 注入抽象介面
public class MainForm
{
    private readonly IAvrRobotMotion _robotMotion;
    private readonly IAvrCameraDriver _cameraDriver;
    
    public MainForm(
        IAvrRobotMotion robotMotion,
        IAvrCameraDriver cameraDriver,
        ...)
    {
        _robotMotion = robotMotion;
        _cameraDriver = cameraDriver;
    }
    
    private async void btnGoHome_Click(object sender, EventArgs e)
    {
        var result = await _robotMotion.MoveLinearAsync(
            RobotCoordinatePose.Home,
            speed: 50,
            acceleration: 80,
            token: _cancellationToken
        );
        
        if (!result.IsSuccess)
        {
            _alarmSuppressor.SubmitAlarm("ROBOT_HOME_FAIL", result.ErrorMessage, AlarmLevel.Error);
        }
    }
}
```

---

## 5. 擴充性保護機制

### 5.1 依賴反轉原則
```csharp
// ✅ 正確：UI 層依賴抽象介面
public class MainForm : Form
{
    private readonly IVisionService _visionService;  // 抽象介面
    
    public MainForm(IVisionService visionService)
    {
        _visionService = visionService;  // 由 DI 容器注入
    }
}

// ❌ 錯誤：UI 層直接實例化具體類別
public class MainForm : Form
{
    private readonly AvlCameraDriver _camera = new AvlCameraDriver();  // 禁止!
}
```

### 5.2 Adapter 擴展模式
```csharp
// 新增感測器時，僅需實作新的 Adapter
public class AvrTemperatureSensorAdapter : ITemperatureSensor
{
    private readonly AvrHardwareGateway _gateway;
    
    public async Task<double> ReadTemperatureAsync(CancellationToken token)
    {
        return await _gateway.ExecuteSafeFuncAsync(
            macros => macros.ReadTemperatureRegister(),
            token
        );
    }
}

// 註冊至 DI 容器
services.AddSingleton<ITemperatureSensor, AvrTemperatureSensorAdapter>();
```

### 5.3 狀態統一管理
```csharp
// 所有硬體狀態匯入 IndustrialDataModels
public class MachineDataModel : INotifyPropertyChanged
{
    // 既有屬性
    public double Temperature { get; set; }
    public double Pressure { get; set; }
    
    // 擴展屬性 (來自 927-Demo)
    public EquipmentStatus EquipmentStatus { get; set; }
    public uint CurrentRecipeNo { get; set; }
    public ulong TotalScanCount { get; set; }
    public bool IsDragMode { get; set; }
}
```

---

## 6. 實作檢查清單

### 階段一交付物
- [ ] `DemoThemeConstants.cs` - 色彩常數提取
- [ ] `MainForm.Designer.cs` - 5 分頁架構
- [ ] `AlarmEntryViewModel.cs` - 警報 ViewModel
- [ ] `UiBusyIndicator.cs` - 活動指示器組件

### 階段二交付物
- [ ] `RecipeManagementService.cs` - 配方服務
- [ ] `ScanHistoryService.cs` - 掃描歷史服務
- [ ] `DependencyInjection.cs` - 完整 DI 配置
- [ ] `MainWindowViewModel` - 擴展 Recipe/Alarm 屬性

### 階段三交付物
- [ ] `AvrRobotMotionAdapter.cs` - 機器人運動 Adapter
- [ ] `ResiliencePolicyProvider` - 擴展 UI 策略
- [ ] `HighPerformanceUiRefresher` - 優化影像刷新
- [ ] 單元測試覆蓋率 > 80%

---

## 7. 風險評估與緩解

| 風險 | 影響程度 | 發生機率 | 緩解措施 |
|------|---------|---------|---------|
| UI 綁定效能下降 | 高 | 中 | 使用 HFC 節流機制，限制 30 FPS |
| 硬體相容性問題 | 高 | 低 | 透過 Adapter 封裝，提供 Mock 實作 |
| 記憶體洩漏 | 中 | 中 | 實施 IDisposable 模式，定期 GC 監控 |
| 學習曲線陡峭 | 中 | 高 | 提供詳細程式碼註解與文件 |

---

## 8. 結論

本整合方案遵循以下核心原則：

1. **UI 層**: 移植 927-Demo 的直觀交互，但以無狀態 View 模式實作
2. **業務層**: 保留 927 的 TPL + Polly 強健性架構
3. **基礎層**: 維持 IAvrHardwareInterfaces 抽象，透過 Adapter 對接 AVL
4. **擴充性**: 嚴守 DIP 原則，新功能透過 DI 注入而非硬編碼

此方案可在 6 週內完成整合，並保持 927 原有的工業級強健性與可測試性。

---

*報告生成日期：2025*
*版本：v1.0 (整合規劃版)*
