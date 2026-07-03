# 工業級視覺與手臂功能整合解決方案

## 概述

本專案提供完整的工業級鞋模控制系統，整合視覺解碼與機械手臂控制功能。系統設計支持**實體設備模式**與**模擬模式**雙重運行環境，確保在無硬體連接時仍可進行開發、測試與演示。

---

## 系統架構

### 核心組件

```
┌─────────────────────────────────────────────────────────────────┐
│                        UI Layer (WinForm)                        │
│  ┌─────────────┐  ┌──────────────┐  ┌───────────────────────┐  │
│  │ MainForm    │  │ MainWindowVM │  │ ConnectionStateDisplay │  │
│  └─────────────┘  └──────────────┘  └───────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│                     Application Layer                            │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ ShoeMoldWorkflow (生產流程協調器)                          │   │
│  │ - InitializeConnectionsAsync()                           │   │
│  │ - RunProductionCycleAsync()                              │   │
│  │ - SyncRobotOperationAsync()                              │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│                    Service Abstraction Layer                     │
│  ┌──────────────────┐         ┌──────────────────────────┐     │
│  │ IVisionService   │         │ IRobotController         │     │
│  ├──────────────────┤         ├──────────────────────────┤     │
│  │ TcpVisionService │         │ DobotCraController       │     │
│  │ MockVisionService│         │ MockRobotController      │     │
│  └──────────────────┘         └──────────────────────────┘     │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│                   Configuration & State Management               │
│  ┌──────────────────┐         ┌──────────────────────────┐     │
│  │ ISimulationConfig│         │ IConnectionStateManager  │     │
│  │ SimulationConfig │         │ ConnectionStateManager   │     │
│  └──────────────────┘         └──────────────────────────┘     │
└─────────────────────────────────────────────────────────────────┘
```

---

## 主要功能特性

### 1. 雙模式運行支持

#### 模擬模式 (Simulation Mode)
- **目的**: 開發測試、演示、無硬體環境
- **啟用方式**: 修改 `appsettings.json` 中的 `Simulation:EnableSimulation` 為 `true`
- **虛擬服務**:
  - `MockVisionService`: 模擬視覺解碼，產生模擬條碼數據
  - `MockRobotController`: 模擬機器人命令執行與狀態反饋

#### 生產模式 (Production Mode)
- **目的**: 實際工廠環境運行
- **啟用方式**: 設置 `Simulation:EnableSimulation` 為 `false`
- **實體服務**:
  - `TcpVisionService`: TCP 連線至真實視覺系統
  - `DobotCraController`: 連線至 DOBOT 機械手臂控制器

### 2. 智能連接管理

```csharp
// 連接狀態管理器實時追蹤設備狀態
public interface IConnectionStateManager
{
    bool IsVisionConnected { get; }
    bool IsRobotConnected { get; }
    bool IsSimulationMode { get; }
    void UpdateVisionConnectionStatus(bool isConnected);
    void UpdateRobotConnectionStatus(bool isConnected);
    string GetConnectionStatusSummary();
}
```

### 3. UI/UX 狀態顯示

- **窗口標題**: 動態顯示當前連接狀態
- **模擬指示器**: 綠色標籤顯示 "SIMULATION MODE" 或 "ALL CONNECTED"/"CONNECTION LOST"
- **定時更新**: 每秒自動刷新連接狀態

### 4. 彈性依賴注入

```csharp
// 根據配置自動註冊對應服務
services.AddSingleton<IVisionService>(sp => 
{
    if (useMockVision || enableSimulation)
        return new MockVisionService(mockOptions, logger);
    else
        return new TcpVisionService(...);
});
```

---

## 配置說明

### appsettings.json 完整配置

```json
{
  "Robot": {
    "IpAddress": "192.168.1.10",
    "DashboardPort": 29999,
    "ModbusPort": 502,
    "ModbusStatusRegister": 1012,
    "CommandTimeoutMs": 5000
  },
  "Vision": {
    "IpAddress": "192.168.1.20",
    "Port": 5000,
    "TimeoutMs": 3000,
    "TriggerCommand": "T1\r\n"
  },
  "Logging": {
    "FilePath": "logs/app-.log",
    "MinimumLevel": "Debug"
  },
  "Simulation": {
    "EnableSimulation": true,
    "UseMockVision": true,
    "UseMockRobot": true,
    "MockVisionDelayMs": 500,
    "MockRobotCommandDelayMs": 300,
    "MockServiceSuccessRate": 1.0,
    "FixedMockBarcode": ""
  }
}
```

### 配置參數說明

| 參數 | 類型 | 預設值 | 說明 |
|------|------|--------|------|
| `EnableSimulation` | bool | false | 啟用模擬模式（優先級最高） |
| `UseMockVision` | bool | false | 單獨使用虛擬視覺服務 |
| `UseMockRobot` | bool | false | 單獨使用虛擬機器人服務 |
| `MockVisionDelayMs` | int | 500 | 模擬視覺處理延遲（毫秒） |
| `MockRobotCommandDelayMs` | int | 300 | 模擬機器人命令執行延遲（毫秒） |
| `MockServiceSuccessRate` | double | 1.0 | 模擬成功率 (0.0-1.0)，用於測試失敗處理 |
| `FixedMockBarcode` | string | "" | 固定模擬條碼，留空則自動生成 |

---

## 使用指南

### 場景 1: 純軟體開發測試

```json
{
  "Simulation": {
    "EnableSimulation": true,
    "UseMockVision": true,
    "UseMockRobot": true,
    "MockServiceSuccessRate": 1.0
  }
}
```

### 場景 2: 混合測試（實體視覺 + 虛擬手臂）

```json
{
  "Simulation": {
    "EnableSimulation": false,
    "UseMockVision": false,
    "UseMockRobot": true
  }
}
```

### 場景 3: 失敗情境測試

```json
{
  "Simulation": {
    "EnableSimulation": true,
    "MockServiceSuccessRate": 0.7
  }
}
```

### 場景 4: 正式生產環境

```json
{
  "Simulation": {
    "EnableSimulation": false,
    "UseMockVision": false,
    "UseMockRobot": false
  }
}
```

---

## 檔案結構

```
/workspace/927/
├── Core/
│   ├── Interfaces.cs           # 服務接口定義
│   ├── Models.cs               # 數據模型
│   ├── AppConfig.cs            # 應用配置
│   └── SimulationConfig.cs     # 模擬配置與連接狀態管理 ⭐新增
├── Infrastructure/
│   ├── DobotCraController.cs   # 實體機器人控制器
│   ├── MockRobotController.cs  # 虛擬機器人控制器 ⭐新增
│   └── Polly/
│       └── ResiliencePolicyProvider.cs
├── Vision/
│   ├── TcpVisionService.cs     # 實體視覺服務
│   └── MockVisionService.cs    # 虛擬視覺服務 ⭐新增
├── Application/
│   ├── ShoeMoldWorkflow.cs     # 生產流程協調器
│   ├── DefaultBarcodeParser.cs
│   └── Services/
│       └── DependencyInjection.cs  # 依賴注入配置 ⭐已更新
├── ViewModels/
│   ├── MainWindowViewModel.cs
│   └── RelayCommand.cs
├── MainForm.cs                 # 主窗口邏輯 ⭐已更新
├── MainForm.Designer.cs        # 主窗口 UI 設計 ⭐已更新
├── Program.cs                  # 應用程式入口
└── appsettings.json            # 配置文件 ⭐已更新
```

---

## 關鍵代碼片段

### 1. 虛擬視覺服務 (MockVisionService.cs)

```csharp
public class MockVisionService : IVisionService
{
    public async Task<DecodeResult> GrabAndDecodeAsync(CancellationToken token)
    {
        await Task.Delay(_options.SimulatedDelayMs, token);
        
        // 生成模擬條碼：A01-0001, B02-0002, ...
        string simulatedBarcode = GenerateSimulatedBarcode(_cycleCount);
        
        return new DecodeResult 
        { 
            IsSuccess = true, 
            DecodedText = simulatedBarcode 
        };
    }
}
```

### 2. 虛擬機器人控制器 (MockRobotController.cs)

```csharp
public class MockRobotController : IRobotController
{
    public async Task<int> ExecuteCommandAsync(string command, CancellationToken token)
    {
        await Task.Delay(_options.SimulatedCommandDelayMs, token);
        
        _currentCommandId++;
        return _currentCommandId;  // 返回模擬的命令 ID
    }
}
```

### 3. 連接狀態管理 (SimulationConfig.cs)

```csharp
public class ConnectionStateManager : IConnectionStateManager
{
    public string GetConnectionStatusSummary()
    {
        if (_isSimulationMode)
        {
            return $"[SIMULATION] Vision: Connected (Mock), Robot: Connected (Mock)";
        }
        else
        {
            return $"[PRODUCTION] Vision: {(IsVisionConnected ? "Connected" : "Disconnected")}, " +
                   $"Robot: {(IsRobotConnected ? "Connected" : "Disconnected")}";
        }
    }
}
```

### 4. 依賴注入自動切換 (DependencyInjection.cs)

```csharp
private static void RegisterVisionService(IServiceCollection services, IConfiguration configuration)
{
    var useMockVision = configuration.GetValue<bool>("Simulation:UseMockVision", false);
    var enableSimulation = configuration.GetValue<bool>("Simulation:EnableSimulation", false);

    if (useMockVision || enableSimulation)
    {
        // 註冊虛擬服務
        services.AddSingleton<IVisionService>(sp => new MockVisionService(mockOptions, logger));
        Log.Information("Vision Service: Using MOCK service (Simulation Mode)");
    }
    else
    {
        // 註冊實體服務
        services.AddSingleton<IVisionService, TcpVisionService>();
        Log.Information("Vision Service: Using TCP service (Production Mode)");
    }
}
```

---

## 最佳實踐建議

### 1. 開發階段
- 啟用模擬模式進行單元測試和集成測試
- 使用 `MockServiceSuccessRate < 1.0` 測試異常處理邏輯
- 利用 `FixedMockBarcode` 驗證特定條碼格式處理

### 2. 測試階段
- 混合模式測試：實體視覺 + 虛擬手臂，或反之
- 壓力測試：調整模擬延遲模擬高負載情況
- 斷線恢復測試：動態切換模擬/生產模式

### 3. 部署階段
- 禁用所有模擬選項
- 配置正確的 IP 地址和端口
- 啟用詳細日誌記錄以便故障排查

### 4. 運維階段
- 監控連接狀態指示器
- 定期檢查日誌文件
- 保留模擬配置作為緊急備用方案

---

## 錯誤處理與韌性

系統內建 Polly 韌性策略：

| 服務 | 重試策略 | 電路斷路器 |
|------|----------|------------|
| Vision | 3 次指數退避 (1s, 2s, 4s) | 50% 失敗率開啟 60 秒 |
| Robot | 3 次固定延遲 (500ms) | 40% 失敗率開啟 45 秒 |

---

## 總結

本解決方案提供：

✅ **完整的虛實切換機制** - 通過配置即可切換模擬/生產模式  
✅ **工業級錯誤處理** - 內建重試、電路斷路器、超時控制  
✅ **直觀的 UX 設計** - 實時連接狀態顯示、模擬模式指示器  
✅ **可擴展架構** - 基於接口的設計，易於添加新設備支持  
✅ **測試友好** - 支持各種失敗情境模擬  

此架構確保系統在任何環境下（有無硬體連接）都能穩定運行，大幅提升開發效率和系統可靠性。
