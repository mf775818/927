# 工業級視覺管線 BDD 規格文件

## 核心目標
在不修改既有 `GenericVisionService<TFrame>` 核心管線與 Polly 強健性策略的前提下，將 MV Viewer 相機 SDK 與 AVL 影像分析庫進行手術刀式的無縫解耦與接入，並徹底解決工廠長期運轉下常見的「非受控快取排空流失（Ring Buffer Exhaustion）」與「記憶體回收織顫（GC Jitter）導致生產節拍不穩」等工業痛點。

---

## BDD 修正前後對比表格

| 場景 (Scenario) | 修正前行為 (Before: Naive SDK Integration) | 修正後行為 (After: Managed Memory Pool & Isolation) | 工業價值 (Business Value) |
| :--- | :--- | :--- | :--- |
| **高頻取像下的記憶體狀態** | 每幀 `new byte[]` 或 `new Bitmap`，導致 Gen 0/Gen 1 GC 頻繁觸發，造成生產節拍出現隨機 50ms~200ms 的卡頓 (Jitter) | 啟動時預分配固定大小的 Ring Buffer (Object Pool)，運行期間**零配置 (Zero-Allocation)**，GC 幾乎不觸發，節拍穩定在微秒級波動 | **消除產線震動**：確保機械手臂同步信號不會因視覺卡頓而超時 |
| **相機硬體緩衝區 (Ring Buffer)** | 取得 Frame 後未立即釋放，直到上層分析完畢。若分析耗時 > 取像間隔，相機底層緩衝區填滿，驅動拋出 "Buffer Overflow" 或停止取像 | `CaptureFrameAsync` 內完成記憶體複製後，**毫秒級內**呼叫 SDK Release 將硬體幀歸還。上層分析使用副本，與硬體隊列完全解耦 | **7x24 小時穩定運行**：杜絕因軟體處理慢導致的相機死鎖，適應高速流水線 |
| **非受控記憶體訪問 (AVL/C++)** | 直接傳遞受控物件參考給 C++ DLL，若 GC 在運算期間移動記憶體，導致存取違規 (Access Violation) 或影像破損 | 使用 `fixed` 指標定錨 (Pinning) 技術，在 AVL 運算期間鎖定記憶體物理地址，運算結束後自動解鎖 | **資料完整性**：防止偶發性的記憶體錯誤導致誤判或當機 |
| **P0 (模擬) 到 P1 (實體) 切換** | 需要修改 Workflow 邏輯，註解掉相機代碼，改為讀取檔案路徑，容易引入回歸錯誤 | 透過 DI 容器切換 `ICameraDriver` 實作 (`FileImageDriver` vs `MvCameraDriver`)，**上層業務邏輯完全無需更動** | **敏捷部署**：離線開發與線上部署無縫接軌，降低整合風險 |
| **異常處理與復原** | 單次取像失敗可能導致整個執行緒阻塞，需重啟服務 | 結合 Polly 策略，驅動層失敗可自動重試或降級，且不會洩漏非受控資源 | **自我修復能力**：提升系統的容錯率與平均無故障時間 (MTBF) |

---

## 場景 1: 幀緩衝區池初始化 (FrameMemoryPool Initialization)

**Given** 系統啟動時指定相機解析度為 2448x2048，像素格式為 Mono8
**When** 創建 `FrameMemoryPool` 並設定初始容量為 5
**Then** 
- 池中應預先分配 5 個 `ManagedFrame` 實例
- 每個 `ManagedFrame` 的 `Payload` 大小應為 5,009,664 位元組 (2448 * 2048)
- `AvailableCount` 應返回 5
- 運行期間不應有任何 `new byte[]` 配置發生

```gherkin
Scenario: FrameMemoryPool pre-allocates buffers at startup
  Given camera resolution is 2448x2048 with Mono8 format
  When I create a FrameMemoryPool with initialCapacity = 5
  Then the pool should contain exactly 5 ManagedFrame instances
  And each ManagedFrame.Payload should be 5,009,664 bytes
  And AvailableCount should return 5
  And no heap allocations should occur during runtime operation
```

---

## 場景 2: ManagedFrame 生命週期驗證 (ManagedFrame Lifecycle)

**Given** 一個從池中借用的 `ManagedFrame`
**When** 使用完畢後呼叫 `Return()` 方法
**Then**
- `IsReturned` 屬性應設為 `true`
- 該幀應被歸還至池中供下次重用
- 若嘗試再次使用已歸還的幀，應拋出 `ObjectDisposedException`

```gherkin
Scenario: ManagedFrame lifecycle tracking prevents double-return
  Given a ManagedFrame rented from the pool
  When I call Return() on the frame
  Then IsReturned property should be set to true
  And the frame should be available in the pool for reuse
  When I attempt to use the returned frame again
  Then an ObjectDisposedException should be thrown
```

---

## 場景 3: AvlCameraDriver 取像流程 (Capture Flow)

**Given** 相機已連線並處於軟體觸發模式
**When** 呼叫 `CaptureFrameAsync()`
**Then**
1. 發送軟體觸發指令至相機
2. 等待 SDK 返回原生幀 (`AvlNet.Image`)
3. 從 `FrameMemoryPool` 借用一個 `ManagedFrame`
4. 使用 `Buffer.MemoryCopy` 將資料從原生指標複製到受控 `Payload`
5. **立即**呼叫 `ReleaseNativeFrame()` 將硬體幀歸還至相機 Ring Buffer
6. 返回受控 `ManagedFrame` 至上層管線

```gherkin
Scenario: AvlCameraDriver captures and releases hardware frame immediately
  Given camera is connected in software trigger mode
  When CaptureFrameAsync() is called
  Then a software trigger pulse should be sent
  And a native frame should be received from AVL SDK
  And a ManagedFrame should be rented from the pool
  And pixel data should be copied via Buffer.MemoryCopy
  And the native frame should be released immediately after copy
  And the ManagedFrame should be returned to the caller
```

---

## 場景 4: 記憶體複製效能驗證 (Memory Copy Performance)

**Given** 一個 5MB 的原生影像幀
**When** 使用 `Buffer.MemoryCopy` 進行複製
**Then**
- 複製時間應小於 2ms
- 不應有任何中間緩衝區配置
- 複製完成後，受控 `Payload` 內容應與原生資料完全一致

```gherkin
Scenario: Memory copy completes within industrial timing constraints
  Given a 5MB native image frame
  When copying via Buffer.MemoryCopy
  Then the operation should complete in under 2ms
  And no intermediate buffer allocations should occur
  And the destination Payload should match source data exactly
```

---

## 場景 5: AVL GCHandle 定錨技術 (Memory Pinning)

**Given** 一個包含影像資料的 `ManagedFrame`
**When** 傳遞至 `AvlImageAnalyzer.Analyze()` 方法
**Then**
- 應使用 `GCHandle.Alloc(payload, Pinned)` 固定記憶體
- AVL SDK 應直接透過固定指標訪問記憶體（零拷貝）
- 分析完成後，`finally` 區塊必須呼叫 `handle.Free()` 解鎖

```gherkin
Scenario: AVL analyzer pins memory during C++ interop
  Given a ManagedFrame with valid image payload
  When Analyze() is called on AvlImageAnalyzer
  Then GCHandle.Alloc should be called with GCHandleType.Pinned
  And AVL SDK should access memory directly via pinned pointer (zero-copy)
  And handle.Free() must be called in finally block after analysis
```

---

## 場景 6: AVL 暫時性物件清理 (Transient Object Cleanup)

**Given** AVL 分析已完成並產生 `DecodeResult`
**When** 離開 `Analyze()` 方法範圍
**Then**
- 所有 `AvlNet.Image` 物件應被 `Dispose()`
- 不應有任何非受控記憶體洩漏
- `DecodeResult` 應包含完整的解碼資訊（成功/失敗、內容、置信度）

```gherkin
Scenario: AVL transient objects are properly disposed
  Given barcode analysis has completed
  When exiting the Analyze() method scope
  Then all AvlNet.Image instances should be Disposed
  And no unmanaged memory leaks should occur
  And DecodeResult should contain complete decoding information
```

---

## 場景 7: GenericVisionService 自動歸還 (Automatic Frame Return)

**Given** `GenericVisionService<ManagedFrame>` 已注入 `FrameMemoryPool`
**When** `GrabAndDecodeAsync()` 完成（無論成功或失敗）
**Then**
- `finally` 區塊必須呼叫 `_bufferPool.Return(managedFrame)`
- 即使分析器拋出例外，幀仍應被歸還
- 日誌應記錄歸還操作的成功/失敗狀態

```gherkin
Scenario: Vision service automatically returns frames in all cases
  Given GenericVisionService is configured with a FrameMemoryPool
  When GrabAndDecodeAsync() completes (success or failure)
  Then the finally block must call _bufferPool.Return(managedFrame)
  And frames should be returned even if analyzer throws exceptions
  And logging should record the return operation status
```

---

## 場景 8: DI 容器註冊配置 (Dependency Injection Registration)

**Given** 應用程式啟動並載入 `appsettings.json`
**When** `DependencyInjection.ConfigureServices()` 被呼叫
**Then**
- `FrameMemoryPool` 應註冊為 Singleton
- `ICameraDriver<ManagedFrame>` 應解析為 `AvlCameraDriver`
- `IImageAnalyzer<ManagedFrame>` 應解析為 `AvlImageAnalyzer`
- `IVisionService` 應解析為 `GenericVisionService<ManagedFrame>`
- 所有依賴應正確注入相同的 `FrameMemoryPool` 實例

```gherkin
Scenario: DI container registers all vision components correctly
  Given application startup with valid appsettings.json
  When ConfigureServices() is executed
  Then FrameMemoryPool should be registered as Singleton
  And ICameraDriver<ManagedFrame> should resolve to AvlCameraDriver
  And IImageAnalyzer<ManagedFrame> should resolve to AvlImageAnalyzer
  And IVisionService should resolve to GenericVisionService<ManagedFrame>
  And all dependencies should share the same FrameMemoryPool instance
```

---

## 場景 9: P0 到 P1 平滑演進 (Simulation to Production Transition)

**Given** 系統處於 P0 階段（模擬模式）
**When** 切換至 P1 階段（生產模式）
**Then**
- 只需修改 DI 配置，將 `ICameraDriver` 從 `FileImageDriver` 改為 `AvlCameraDriver`
- `ShoeMoldWorkflow` 與上位機邏輯**完全不需要修改**
- Polly 斷路器與重試策略保持不變
- 記憶體池機制自動啟用

```gherkin
Scenario: Zero-code-change transition from simulation to production
  Given system is running in P0 simulation mode with FileImageDriver
  When switching to P1 production mode with AvlCameraDriver
  Then only DI configuration should change
  And ShoeMoldWorkflow should require zero modifications
  And Polly resilience policies should remain unchanged
  And memory pooling should activate automatically
```

---

## 技術指標驗收標準

| 指標 | 目標值 | 測量方法 | 驗收閾值 |
|------|--------|----------|----------|
| **GC Jitter** | < 1ms | BenchmarkDotNet Alloc/Op | 通過：Gen 0 收集次數/秒 < 10 |
| **Ring Buffer 耗盡** | 0 次 | 24 小時長期運行測試 | 通過：無 "Buffer Overflow" 錯誤 |
| **記憶體複製延遲** | < 2ms (5MB) | Stopwatch 測量 | 通過：P99 < 2ms |
| **AVL 定錨開銷** | < 0.1ms | 對比有/無 Pinning | 通過：差異 < 0.1ms |
| **緩衝區歸還率** | 100% | 日誌分析 Return 次數 | 通過：Rented == Returned |
| **零拷貝達成率** | 100% | 代碼審查無 Array.Copy | 通過：僅使用 Buffer.MemoryCopy |

---

## 關鍵設計決策記錄 (ADR)

### ADR-001: 選擇 `FrameMemoryPool` 而非 `ArrayPool<T>`
- **情境**: 需要管理包含元數據的完整 `ManagedFrame` 物件
- **決策**: 實作專屬的 `FrameMemoryPool` 而非使用 .NET 內建 `ArrayPool<byte>`
- **理由**: 
  - `ManagedFrame` 包含 Width、Height、Stride、Format 等元數據
  - 需要追蹤 `IsReturned` 狀態以防止重複歸還
  - 需要提供監控指標（TotalRented、TotalReturned）

### ADR-002: 立即釋放硬體幀策略
- **情境**: 相機 SDK 的 Ring Buffer 有限，易在高頻取像下耗盡
- **決策**: 在 `CaptureFrameAsync` 內完成複製後立即釋放原生幀
- **理由**:
  - 解耦硬體隊列與軟體處理管線
  - 允許上層分析耗時超過取像間隔而不影響相機
  - 符合工業級 7x24 小時穩定性要求

### ADR-003: GCHandle Pinning 而非 unsafe 指標
- **情境**: 需要將受控記憶體傳遞給 AVL C++ SDK
- **決策**: 使用 `GCHandle.Alloc(Pinned)` 而非純 unsafe 程式碼
- **理由**:
  - 更安全：GC 會自動追蹤定錨物件
  - 更易維護：不需要持續的 unsafe 上下文
  - 效能損失可忽略（< 0.1ms）

---

## 附錄：核心元件清單

| 檔案路徑 | 元件名稱 | 職責 |
|----------|----------|------|
| `src/Core/Vision/ManagedFrame.cs` | `ManagedFrame` | TFrame 泛型物料定義（受控記憶體描述符） |
| `src/Infrastructure/Vision/FrameBufferPool.cs` | `FrameMemoryPool` | 記憶體預分配池（消除 GC Jitter） |
| `src/Vision/AvlImplementations.cs` | `AvlCameraDriver` | MV Viewer 相機驅動（立即釋放硬體幀） |
| `src/Vision/AvlImplementations.cs` | `AvlImageAnalyzer` | AVL 分析器（零拷貝 Memory Pinning） |
| `src/Drivers/FileImageDriver.cs` | `FileImageDriver` | P0 模擬驅動（雙模式切換） |
| `src/Vision/GenericVisionService.cs` | `GenericVisionService<TFrame>` | 通用視覺服務（整合 Polly 策略） |
| `src/Core/SimulationConfig.cs` | `ConnectionStateManager` | 狀態管理器 |
| `src/Application/Services/DependencyInjection.cs` | `DependencyInjection` | DI 容器配置 |

---

## 版本歷史

| 版本 | 日期 | 作者 | 變更說明 |
|------|------|------|----------|
| 1.0 | 2025-01-XX | System | 初始版本：完成所有核心元件實作 |
