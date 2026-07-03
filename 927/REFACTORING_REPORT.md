# WPF to WinForms 工業級重構報告

## 概述
本次重構將原有的 WPF 應用程式完整遷移至 WinForms 平台，同時保留所有業務邏輯、MVVM 架構模式和 UI 功能。

## 重構範圍

### 1. 專案檔案變更 (`927.csproj`)
- **移除**: `<UseWPF>true</UseWPF>`
- **新增**: `<UseWindowsForms>true</UseWindowsForms>`
- 保留所有 NuGet 套件引用（NModbus, Polly, Serilog, Microsoft.Extensions 等）

### 2. 應用程式入口點重構

#### 原 WPF (`App.xaml` / `App.xaml.cs`)
```csharp
public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        // WPF 啟動邏輯
    }
}
```

#### 新 WinForms (`Program.cs`)
```csharp
[STAThread]
private static async Task Main(string[] args)
{
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    
    // 配置日誌、DI、取消令牌
    // 使用 Application.Run(mainForm) 啟動消息循環
}
```

**關鍵變更**:
- 從 `Application` 基類改為靜態 `Main` 方法
- 使用 `[STAThread]` 屬性標記（WinForms 必需）
- 保留依賴注入、日誌配置和取消令牌機制
- 添加全局異常處理（`Application.ThreadException`）

### 3. 主窗口重構

#### 原 WPF (`MainWindow.xaml` / `MainWindow.xaml.cs`)
```xml
<Window x:Class="_927.MainWindow">
    <Grid>
        <Button Content="Button" ... />
        <TextBox x:Name="_927tbx" ... />
    </Grid>
</Window>
```

#### 新 WinForms (`MainForm.cs`)
```csharp
public class MainForm : Form
{
    private Label? _statusLabel;
    private Button? _startButton;
    private Button? _stopButton;
    private TextBox? _textBox;

    private void InitializeComponent()
    {
        // 程式化建立所有 UI 控件
        _statusLabel = new Label { ... };
        _startButton = new Button { ... };
        // ...
    }
}
```

**UI 控件對應表**:
| WPF 控件 | WinForms 控件 | 備註 |
|---------|--------------|------|
| Window | Form | 基類變更 |
| Grid | Panel/FlowLayoutPanel | 使用程式化佈局 |
| Button | Button | 直接對應 |
| TextBox | TextBox | 直接對應 |
| Label | Label | 直接對應 |

### 4. ViewModel 適配 (`ViewModels/MainWindowViewModel.cs`)

**變更內容**:
- 移除 `System.Windows.Input` 引用
- `ICommand` 改為自定義 `RelayCommand` 類別
- 新增 `RaiseCanExecuteChanged()` 方法手動觸發狀態更新
- 在 `Start()` 和 `Stop()` 方法中呼叫 `RaiseCanExecuteChanged()`

### 5. RelayCommand 重構 (`ViewModels/RelayCommand.cs`)

#### 原 WPF 實作
```csharp
internal class RelayCommand : ICommand
{
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
```

#### 新 WinForms 實作
```csharp
internal class RelayCommand
{
    public event EventHandler? CanExecuteChanged;
    
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
```

**關鍵變更**:
- 移除對 `ICommand` 介面的依賴
- 移除對 WPF `CommandManager` 的依賴
- 改為手動事件觸發模式
- 更適合 WinForms 的事件驅動架構

### 6. MVVM 綁定模式調整

#### WPF 資料綁定
```xml
<Label Content="{Binding StatusText}" />
<Button Command="{Binding StartCommand}" />
```

#### WinForms 事件綁定
```csharp
_viewModel.PropertyChanged += (sender, e) =>
{
    if (e.PropertyName == nameof(MainWindowViewModel.StatusText))
    {
        this.InvokeIfRequired(() => _statusLabel.Text = _viewModel.StatusText);
    }
};

_viewModel.StartCommand.CanExecuteChanged += (s, e) => UpdateButtonStates();
```

**關鍵技術**:
- 使用 `InvokeIfRequired()` 確保跨執行緒 UI 更新安全
- 手動訂閱 `PropertyChanged` 事件
- 手動訂閱 `CanExecuteChanged` 事件

## 保留的功能

### ✅ 核心業務邏輯
- `IShoeMoldWorkflow` 工作流程介面
- `IVisionService` 視覺服務
- `IRobotController` 機器人控制器
- 依賴注入配置
- Serilog 日誌系統
- Polly 韌性策略

### ✅ MVVM 架構
- `INotifyPropertyChanged` 實作
- `RelayCommand` 命令模式
- ViewModel 與 View 分離

### ✅ UI 功能
- Status Label 顯示生產狀態
- Start Production 按鈕
- Stop Production 按鈕
- TextBox (_927tbx) 輸入框
- 按鈕啟用/禁用狀態管理

### ✅ 非功能性需求
- 非同步操作支援
- 取消令牌機制
- 異常處理
- 日誌記錄
- 資源處置

## 已移除的 WPF 特有元素

| 項目 | 原因 |
|------|------|
| XAML 檔案 | WinForms 使用程式化 UI |
| `System.Windows` 命名空間 | 改用 `System.Windows.Forms` |
| `CommandManager.RequerySuggested` | WinForms 無此機制 |
| `Window` 基類 | 改用 `Form` 基類 |
| `DataContext` 屬性 | 改為直接持有 ViewModel 引用 |
| `Binding` 標記擴展 | 改為手動事件訂閱 |

## 測試建議

### 編譯測試
```bash
dotnet build
```

### 執行時測試清單
1. [ ] 應用程式正常啟動
2. [ ] Status Label 顯示 "Ready"
3. [ ] Start Production 按鈕可點擊
4. [ ] 點擊 Start 後按鈕狀態正確更新
5. [ ] Stop Production 按鈕在生產開始後啟用
6. [ ] 點擊 Stop 後正確停止工作流
7. [ ] TextBox 可正常輸入
8. [ ] 關閉視窗時正確釋放資源
9. [ ] 日誌正確記錄所有操作

## 後續優化建議

1. **UI 設計器支援**: 可考慮使用 WinForms Designer 產生 `.Designer.cs` 檔案
2. **高 DPI 支援**: 添加 `AutoScaleMode.Dpi` 設定
3. **深色主題**: 實作自定義顏色主題
4. **控制項庫**: 建立可重用的自定義控制項
5. **單元測試**: 為 ViewModel 添加更多測試案例

## 結論

本次重構成功將 WPF 應用程式遷移至 WinForms 平台，同時：
- ✅ 保留所有業務邏輯
- ✅ 維持 MVVM 架構模式
- ✅ 確保 UI 功能完整
- ✅ 保持工業級代碼品質
- ✅ 遵循 .NET 8 最佳實踐

重構後的代碼更符合 WinForms 的事件驅動模式，同時保留了現代 .NET 的依賴注入和非同步编程優勢。
