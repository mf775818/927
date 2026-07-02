# 鞋模自動化控制系統 - 工業級架構 BDD 規格

## 特性：生產週期管理

### 場景 1: 正常生產流程
```gherkin
Feature: 生產週期執行
  為了確保鞋模具生產線自動化運行
  作為系統操作員
  我希望系統能夠自動讀取條碼並控制機械臂執行對應程序

  Scenario: 正常生產流程
    Given 視覺系統已連接且就緒
      And 機械臂處於 ENABLE 模式
      And 系統配置已正確加載
    When 系統觸發視覺掃描
      And 視覺系統返回有效條碼 "ABC-123"
      And 條碼解析生成命令 "RunScript(\"blockly_ABC-123\")"
      And 機械臂成功執行命令並返回 commandId > 0
    Then 系統應等待機械臂完成執行
      And 記錄成功的生產紀錄
      And 自動開始下一個生產週期
```

### 場景 2: 視覺系統失敗重試
```gherkin
  Scenario: 視覺系統臨時失敗時的指數退避重試
    Given 視覺系統已連接
    When 系統觸發視覺掃描
      And 視覺系統返回超时錯誤
    Then 系統應記錄警告日誌
      And 等待 1 秒後重試
      And 如果連續失敗 3 次，應升級為錯誤日誌
      And 如果連續失敗 5 次，應觸發電路斷路器
    
  Scenario Outline: 指數退避策略
    Given 連續失敗次數為 <failures>
    When 系統決定下次重試延遲
    Then 延遲時間應為 <expected_delay> 毫秒
    
    Examples:
      | failures | expected_delay |
      | 1        | 1000           |
      | 2        | 2000           |
      | 3        | 4000           |
      | 4        | 8000           |
      | 5+       | 16000          |
```

### 場景 3: 電路斷路器模式
```gherkin
  Scenario: 電路斷路器狀態轉換
    Given 電路斷路器初始狀態為 CLOSED
      And 失敗閾值設置為 5 次
      And 恢復時間窗口為 30 秒
    When 連續發生 5 次視覺系統連接失敗
    Then 電路斷路器應切換到 OPEN 狀態
      And 系統應立即拒絕新的請求
      And 記錄嚴重錯誤日誌
    
    When 30 秒恢復時間過去
    Then 電路斷路器應切換到 HALF_OPEN 狀態
      And 允許一次測試請求通過
    
    When 測試請求成功
    Then 電路斷路器應切換回 CLOSED 狀態
      And 重置失敗計數器
```

## 特性：配置管理

### 場景 4: 配置加載與驗證
```gherkin
Feature: 配置管理
  為了確保系統在不同環境下正確運行
  作為系統維護工程師
  我希望系統能夠從外部配置文件加載並驗證配置

  Scenario: 從 appsettings.json 加載配置
    Given 存在有效的 appsettings.json 文件
      And 文件包含 Vision 和 Robot 配置節點
    When 系統啟動時
    Then 應從配置文件加載所有必要參數
      And 驗證 IP 地址格式正確
      And 驗證端口號在有效範圍內 (1-65535)
      And 驗證超時時間為正數
  
  Scenario: 配置驗證失敗
    Given appsettings.json 中的 VisionIpAddress 為無效格式
    When 系統嘗試加載配置
    Then 應拋出 ConfigurationValidationException
      And 記錄詳細的驗證錯誤信息
      And 系統不應啟動
  
  Scenario: 環境變量覆蓋配置
    Given 基礎配置來自 appsettings.json
      And 環境變量 VISION_IP 設置為 "10.0.0.100"
    When 系統加載配置
    Then 環境變量應優先於配置文件
      And VisionIpAddress 應為 "10.0.0.100"
```

## 特性：連接管理

### 場景 5: 視覺系統連接管理
```gherkin
Feature: 連接健康管理
  為了確保與外部系統的穩定連接
  作為系統操作員
  我希望系統能夠自動管理連接狀態和健康檢查

  Scenario: 視覺系統自動重連
    Given 視覺系統連接已建立
    When 連接意外中斷
      And 系統檢測到連接失敗
    Then 應自動嘗試重新連接
      And 使用指數退避策略
      And 最多重試 3 次
      And 每次重試間記錄日誌
    
  Scenario: 連接超時處理
    Given 視覺系統響應緩慢
    When 單次請求超過配置的超時時間 (VisionTimeoutMs)
    Then 應取消當前請求
      And 關閉並重建連接
      And 記錄超时警告
```

### 場景 6: 機械臂連接健康檢查
```gherkin
  Scenario: 定期健康檢查
    Given 機械臂連接已建立
      And 健康檢查間隔設置為 10 秒
    When 系統運行時
    Then 應每 10 秒檢查一次機械臂模式
      And 如果返回 ERROR 模式，應記錄警告
      And 如果連續 3 次健康檢查失敗，應嘗試重連
  
  Scenario: 機械臂模式異常處理
    Given 機械臂處於 ERROR 模式
    When 系統檢測到 ERROR 模式
    Then 應暫停生產流程
      And 向 UI 發送警報通知
      And 等待操作員干預
```

## 特性：日誌與監控

### 場景 7: 結構化日誌記錄
```gherkin
Feature: 可觀測性
  為了便於故障排查和性能分析
  作為系統維護工程師
  我希望系統能夠記錄詳細的結構化日誌

  Scenario: 生產週期成功日誌
    Given 生產週期成功完成
      And 條碼為 "ABC-123"
      And commandId 為 1001
      And 執行耗時 2.5 秒
    When 記錄日誌
    Then 日誌應包含以下字段:
      | 字段 | 值 |
      | Timestamp | ISO8601 格式時間戳 |
      | Level | Information |
      | Event | ProductionCycleCompleted |
      | Barcode | ABC-123 |
      | CommandId | 1001 |
      | DurationMs | 2500 |
      | CorrelationId | 唯一關聯 ID |
  
  Scenario: 異常情況日誌
    Given 視覺系統連接失敗
      And 異常類型為 SocketException
      And 錯誤消息為 "Connection refused"
    When 記錄錯誤日誌
    Then 日誌應包含:
      | 字段 | 值 |
      | Timestamp | ISO8601 格式時間戳 |
      | Level | Error |
      | Event | VisionConnectionFailed |
      | ExceptionType | SocketException |
      | Message | Connection refused |
      | StackTrace | 完整堆棧追蹤 |
      | RetryCount | 當前重試次數 |
```

### 場景 8: 性能指標收集
```gherkin
  Scenario: 生產效率指標
    Given 系統運行中
    When 完成一個生產週期
    Then 應更新以下指標:
      | 指標名稱 | 類型 | 描述 |
      | production_cycles_total | Counter | 總生產週期數 |
      | production_success_total | Counter | 成功生產數 |
      | production_failure_total | Counter | 失敗生產數 |
      | cycle_duration_seconds | Histogram | 週期耗時分佈 |
      | vision_response_time_ms | Histogram | 視覺響應時間 |
      | robot_command_latency_ms | Histogram | 機械臂命令延遲 |
  
  Scenario: 連接狀態指標
    Given 系統運行中
    Then 應暴露以下狀態指標:
      | 指標名稱 | 類型 | 描述 |
      | vision_connected | Gauge | 視覺連接狀態 (1=連接，0=斷開) |
      | robot_connected | Gauge | 機械臂連接狀態 |
      | circuit_breaker_state | Gauge | 斷路器狀態 (0=CLOSED, 1=OPEN, 2=HALF_OPEN) |
```

## 特性：錯誤處理與恢復

### 場景 9: 條碼解析失敗處理
```gherkin
Feature: 錯誤處理策略
  為了確保系統在異常情況下仍能穩定運行
  作為系統操作員
  我希望系統能夠優雅地處理各種錯誤場景

  Scenario: 無效條碼格式
    Given 視覺系統返回條碼 "INVALID_FORMAT"
      And 條碼不符合預期的 "XXX-YYY" 格式
    When 條碼解析器處理該條碼
    Then 應返回空命令字符串
      And 記錄警告日誌 "Invalid barcode format"
      And 跳過當前週期
      And 等待 1 秒後繼續
  
  Scenario: 條碼對應腳本不存在
    Given 條碼解析生成命令 "RunScript(\"blockly_UNKNOWN\")"
      And 機械臂返回錯誤 "Script not found"
    When 命令執行失敗
    Then 應記錄錯誤日誌包含條碼信息
      And 通知操作員檢查腳本配置
      And 暫停生產等待確認
```

### 場景 10: 網絡波動處理
```gherkin
  Scenario: 瞬時網絡抖動
    Given 系統正在執行生產流程
    When 發生單次網絡請求超時
      And 下一次請求成功
    Then 系統應繼續正常運行
      And 記錄單次警告但不中斷流程
  
  Scenario: 持續網絡故障
    Given 網絡持續不可達
    When 連續 10 次請求失敗
    Then 系統應進入安全模式
      And 停止自動生產循環
      And 發出聲光警報
      And 等待管理員重置
```

## 特性：用戶界面集成

### 場景 11: 實時狀態顯示
```gherkin
Feature: 用戶界面
  為了讓操作員實時了解系統狀態
  作為車間操作員
  我希望 UI 能夠清晰顯示系統運行狀態

  Scenario: 生產狀態實時更新
    Given 系統正在運行生產
    When 完成一個生產週期
    Then UI 應更新以下信息:
      | 顯示項 | 內容 |
      | 當前條碼 | ABC-123 |
      | 生產計數 | +1 |
      | 最後完成時間 | HH:mm:ss |
      | 系統狀態 | 運行中 (綠色) |
  
  Scenario: 異常狀態警報
    Given 視覺系統連接失敗
    When 檢測到錯誤
    Then UI 應:
      | 動作 | 描述 |
      | 狀態指示器 | 變為紅色 |
      | 彈出對話框 | 顯示錯誤詳情 |
      | 日誌面板 | 滾動顯示錯誤信息 |
      | 聲音提示 | 播放警報音 (可配置) |
```

### 場景 12: 手動控制功能
```gherkin
  Scenario: 緊急停止
    Given 系統正在自動運行
    When 操作員點擊"緊急停止"按鈕
    Then 應立即取消所有進行中的操作
      And 斷開與設備的連接
      And UI 狀態變為"已停止"
      And 記錄緊急停止事件
  
  Scenario: 手動觸發單次生產
    Given 系統處於待機狀態
    When 操作員點擊"單次運行"按鈕
    Then 應執行一次完整的生產週期
      And 不進入自動循環
      And 顯示本次執行結果
  
  Scenario: 配置管理界面
    Given 操作員有管理員權限
    When 打開配置頁面
    Then 應顯示所有可配置參數
      And 允許修改並保存
      And 保存前驗證輸入有效性
      And 保存後無需重啟即可生效
```

## 特性：系統啟動與關閉

### 場景 13: 優雅啟動
```gherkin
Feature: 生命週期管理
  為了確保系統可靠啟動和關閉
  作為系統操作員
  我希望系統能夠有序地啟動和關閉

  Scenario: 正常啟動流程
    Given 系統配置文件存在且有效
    When 應用程序啟動
    Then 應按順序執行:
      | 步驟 | 描述 |
      | 1 | 加載並驗證配置 |
      | 2 | 初始化日誌系統 |
      | 3 | 創建依賴注入容器 |
      | 4 | 註冊所有服務 |
      | 5 | 連接視覺系統 |
      | 6 | 連接機械臂 |
      | 7 | 執行初始健康檢查 |
      | 8 | 顯示主界面 |
      | 9 | 記錄啟動成功日誌 |
  
  Scenario: 啟動時設備不可用
    Given 視覺系統無法連接
    When 應用程序啟動
    Then 應重試連接 3 次
      And 如果仍然失敗，顯示錯誤對話框
      And 提供"僅 UI 模式"選項
      And 記錄啟動警告
```

### 場景 14: 優雅關閉
```gherkin
  Scenario: 正常關閉流程
    Given 系統正在運行生產
    When 用戶關閉應用程序
    Then 應按順序執行:
      | 步驟 | 描述 |
      | 1 | 停止接收新生產任務 |
      | 2 | 等待當前命令完成 (最多 10 秒) |
      | 3 | 取消所有 pending 的异步操作 |
      | 4 | 斷開視覺系統連接 |
      | 5 | 斷開機械臂連接 |
      | 6 | 刷新並關閉日誌 |
      | 7 | 釋放所有托管資源 |
      | 8 | 記錄關閉完成日誌 |
  
  Scenario: 強制關閉處理
    Given 系統卡死在某个操作
    When 用戶強制關閉 (超過 5 秒未完成優雅關閉)
    Then 應立即終止所有線程
      And 強制釋放資源
      And 記錄非正常關閉事件
```

## 特性：測試與質量保證

### 場景 15: 單元測試覆蓋
```gherkin
Feature: 測試策略
  為了確保代碼質量和回歸測試
  作為開發人員
  我希望有完善的測試覆蓋

  Scenario: 核心邏輯單元測試
    Given 存在單位測試項目
    Then 以下組件應有單元測試:
      | 組件 | 最低覆蓋率 |
      | DefaultBarcodeParser | 90% |
      | AppConfig 驗證邏輯 | 90% |
      | 狀態機轉換邏輯 | 95% |
    
  Scenario: 接口 Mock 測試
    Given 測試 IVisionService
    When 使用 Moq 創建 mock 對象
    Then 應能模擬:
      | 場景 | 預期行為 |
      | 成功解碼 | 返回 IsSuccess=true 的 DecodeResult |
      | 連接失敗 | 返回 IsSuccess=false 的 DecodeResult |
      | 超時 | 拋出 OperationCanceledException |
```

### 場景 16: 集成測試
```gherkin
  Scenario: 端到端工作流測試
    Given 測試環境配置完成
      And Mock 視覺服務返回預定條碼
      And Mock 機械臂服務接受所有命令
    When 執行完整生產流程
    Then 應驗證:
      | 驗證點 | 預期結果 |
      | 視覺服務被調用 | 至少 1 次 |
      | 條碼解析器被調用 | 1 次 |
      | 機械臂 ExecuteCommandAsync 被調用 | 1 次 |
      | SyncRobotOperationAsync 被調用 | 1 次 |
      | 生產記錄被保存 | 1 條記錄 |
```

---

## 技術實施建議

### 推薦的 NuGet 包
```xml
<ItemGroup>
  <!-- 現有 -->
  <PackageReference Include="NModbus" Version="3.0.83" />
  
  <!-- 新增：配置管理 -->
  <PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
  <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="8.0.0" />
  <PackageReference Include="Microsoft.Extensions.Configuration.EnvironmentVariables" Version="8.0.0" />
  <PackageReference Include="Microsoft.Extensions.Options" Version="8.0.0" />
  
  <!-- 新增：依賴注入 -->
  <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
  <PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
  
  <!-- 新增：日誌 -->
  <PackageReference Include="Serilog" Version="3.1.1" />
  <PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
  <PackageReference Include="Serilog.Sinks.Console" Version="5.0.1" />
  <PackageReference Include="Serilog.Extensions.Hosting" Version="8.0.0" />
  
  <!-- 新增：監控 -->
  <PackageReference Include="prometheus-net" Version="8.2.1" />
  
  <!-- 新增：策略處理 -->
  <PackageReference Include="Polly" Version="8.2.0" />
  
  <!-- 新增：測試 -->
  <PackageReference Include="xunit" Version="2.6.2" />
  <PackageReference Include="Moq" Version="4.20.70" />
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
</ItemGroup>
```

### 建議的目錄結構
```
927/
├── Core/
│   ├── Interfaces.cs
│   ├── Models.cs
│   ├── AppConfig.cs
│   └── ConfigValidator.cs          # 新增：配置驗證
├── Infrastructure/
│   ├── DobotCraController.cs
│   ├── Vision/
│   │   └── TcpVisionService.cs
│   └── Connectivity/               # 新增：連接管理
│       ├── CircuitBreaker.cs
│       ├── HealthChecker.cs
│       └── ConnectionPool.cs
├── Application/
│   ├── ShoeMoldWorkflow.cs
│   ├── DefaultBarcodeParser.cs
│   ├── Services/                   # 新增：應用服務
│   │   ├── ProductionService.cs
│   │   └── AlertService.cs
│   └── Policies/                   # 新增：策略定義
│       ├── RetryPolicies.cs
│       └── TimeoutPolicies.cs
├── Presentation/
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   ├── ViewModels/                 # 新增：MVVM
│   │   └── MainViewModel.cs
│   └── Converters/                 # 新增：值轉換器
├── Diagnostics/                    # 新增：診斷
│   ├── Logging/
│   │   └── SerilogConfig.cs
│   └── Metrics/
│       └── PrometheusMetrics.cs
├── Tests/                          # 新增：測試項目
│   ├── Unit/
│   └── Integration/
├── appsettings.json                # 新增：配置文件
├── appsettings.Development.json    # 新增：開發環境
└── appsettings.Production.json     # 新增：生產環境
```

### 關鍵改進點總結

1. **可靠性**: 添加 Polly 重試策略和電路斷路器
2. **可觀測性**: 集成 Serilog 結構化日誌和 Prometheus 指標
3. **配置管理**: 使用 Microsoft.Extensions.Configuration 支持多環境
4. **依賴注入**: 使用 Microsoft.Extensions.DependencyInjection
5. **測試覆蓋**: 添加 xUnit + Moq 測試框架
6. **UI 架構**: 引入 MVVM 模式改善 UI 可維護性
7. **資源管理**: 改進 Dispose 模式，避免 async void
8. **錯誤處理**: 統一的異常處理策略和錯誤分級

---

## 驗收標準

- [ ] 所有 BDD 場景都有對應的測試用例
- [ ] 核心業務邏輯單元測試覆蓋率 >= 85%
- [ ] 系統能在配置錯誤時給出明確的錯誤提示
- [ ] 網絡故障時系統能自動恢復或安全降級
- [ ] 日誌包含足夠信息用於故障排查
- [ ] UI 能實時反映系統狀態
- [ ] 支持優雅啟動和關閉
- [ ] 關鍵指標可被監控系統采集
