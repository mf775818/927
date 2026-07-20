# Graph Report - .  (2026-07-20)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 583 nodes · 1012 edges · 25 communities (24 shown, 1 thin omitted)
- Extraction: 98% EXTRACTED · 2% INFERRED · 0% AMBIGUOUS · INFERRED: 20 edges (avg confidence: 0.8)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `f92e9de7`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- AvlAdvancedServices.cs
- GenericVisionService
- HardwareMotionResult
- ShoeMoldControl.Core
- AvlCameraDriver
- IndustrialSafeButton
- MainForm
- MainForm
- 927
- MachineDataModel
- LoggerConfigurator.cs
- DobotCraController
- AvrHardwareGateway
- HighPerformanceUiRefresher
- .ConfigureServices
- MainWindowViewModel
- SimulationConfig
- TcpVisionService
- MockRobotController
- AlarmLevel
- .InvokeIfRequired
- MainForm
- MockVisionService
- AppConfig.cs
- VisionInspectionResult

## God Nodes (most connected - your core abstractions)
1. `MainForm` - 46 edges
2. `MainForm` - 25 edges
3. `927` - 18 edges
4. `ShoeMoldControl.Core` - 18 edges
5. `DobotCraController` - 17 edges
6. `AvlCameraDriver` - 17 edges
7. `Serilog` - 16 edges
8. `IndustrialSafeButton` - 16 edges
9. `ManagedFrame` - 15 edges
10. `FileImageDriver` - 15 edges

## Surprising Connections (you probably didn't know these)
- `MainForm` --references--> `VisionSystemMode`  [EXTRACTED]
  927/MainForm.cs → 927/Core/Models/IndustrialDataModels.cs
- `MainForm` --references--> `JogType`  [EXTRACTED]
  927/MainForm.cs → 927/Core/Models/IndustrialDataModels.cs
- `AvlCameraDriver` --references--> `IVisionConfig`  [EXTRACTED]
  927/Vision/AvlImplementations.cs → 927/Core/Interfaces.cs
- `TcpVisionService` --references--> `IVisionConfig`  [EXTRACTED]
  927/Vision/TcpVisionService.cs → 927/Core/Interfaces.cs
- `DobotCraController` --references--> `IRobotConfig`  [EXTRACTED]
  927/Infrastructure/DobotCraController.cs → 927/Core/Interfaces.cs

## Import Cycles
- None detected.

## Communities (25 total, 1 thin omitted)

### Community 0 - "AvlAdvancedServices.cs"
Cohesion: 0.06
Nodes (29): ILogger, Image, List, object, X, Y, AvlGeometryMeasurer, AvlImagePreprocessor (+21 more)

### Community 1 - "GenericVisionService"
Cohesion: 0.07
Nodes (30): DefaultBarcodeParser, CancellationToken, ILogger, Task, ShoeMoldWorkflow, AppConfig, CancellationToken, Task (+22 more)

### Community 2 - "HardwareMotionResult"
Cohesion: 0.06
Nodes (30): List, HardwareMotionResult, RobotCoordinatePose, VisionInspectionResult, CancellationToken, Image, JogType, Task (+22 more)

### Community 3 - "ShoeMoldControl.Core"
Cohesion: 0.08
Nodes (25): IServiceCollection, AvlAdvancedServicesRegistration, AvlVisionServiceOptions, MockRobotOptions, FrameBufferPool, ILogger, AvlImageAnalyzer, MockVisionOptions (+17 more)

### Community 4 - "AvlCameraDriver"
Cohesion: 0.07
Nodes (23): ManagedFrame, PixelFormat, Bitmap, bool, CancellationToken, ILogger, int, object (+15 more)

### Community 5 - "IndustrialSafeButton"
Cohesion: 0.07
Nodes (22): MainForm, Button, Action, bool, Color, Control, EventArgs, int (+14 more)

### Community 6 - "MainForm"
Cohesion: 0.12
Nodes (9): MainForm, Bitmap, EventArgs, Image, MouseEventArgs, RobotCoordinatePose, RobotMode, Task (+1 more)

### Community 7 - "MainForm"
Cohesion: 0.09
Nodes (13): MainForm, bool, CancellationToken, CancellationTokenSource, Color, ILogger, Image, int (+5 more)

### Community 8 - "927"
Cohesion: 0.11
Nodes (18): 927, net48, net8.0-windows, AvlNet.Amr (5.5.2.78740), AvlNet.Kit (5.5.2.78740), AvlNet.Types (5.5.2.78740), CommunityToolkit.Mvvm (8.4.2), Microsoft.Extensions.Configuration (10.0.9) (+10 more)

### Community 9 - "MachineDataModel"
Cohesion: 0.16
Nodes (12): bool, DateTime, int, string, AlarmDataModel, IndustrialDataModelBase, JogType, MachineDataModel (+4 more)

### Community 10 - "LoggerConfigurator.cs"
Cohesion: 0.17
Nodes (10): LoggerEnrichmentConfigurationExtensions, MachineNameEnricher, ThreadIdEnricher, LoggerConfigurator, ShoeMoldControl.Infrastructure.Logging, ILogEventEnricher, ILogEventPropertyFactory, LogEvent (+2 more)

### Community 11 - "DobotCraController"
Cohesion: 0.22
Nodes (9): bool, CancellationToken, ILogger, NetworkStream, SemaphoreSlim, Task, TcpClient, DobotCraController (+1 more)

### Community 12 - "AvrHardwareGateway"
Cohesion: 0.17
Nodes (11): Action, bool, CancellationToken, Func, int, SemaphoreSlim, string, Task (+3 more)

### Community 13 - "HighPerformanceUiRefresher"
Cohesion: 0.14
Nodes (10): Action, bool, Control, EventArgs, int, Timer, AlarmStormSuppressor, HighPerformanceUiRefresher (+2 more)

### Community 14 - ".ConfigureServices"
Cohesion: 0.22
Nodes (9): IConfiguration, IServiceCollection, IServiceProvider, DependencyInjection, Program, CancellationTokenSource, IServiceProvider, Task (+1 more)

### Community 15 - "MainWindowViewModel"
Cohesion: 0.16
Nodes (9): MainWindowViewModel, CancellationToken, ILogger, IServiceProvider, string, Task, RelayCommand, Action (+1 more)

### Community 16 - "SimulationConfig"
Cohesion: 0.21
Nodes (5): bool, IConfiguration, ConnectionStateManager, ISimulationConfig, SimulationConfig

### Community 17 - "TcpVisionService"
Cohesion: 0.27
Nodes (8): bool, CancellationToken, ILogger, NetworkStream, SemaphoreSlim, Task, TcpClient, TcpVisionService

### Community 18 - "MockRobotController"
Cohesion: 0.27
Nodes (6): bool, CancellationToken, ILogger, int, Task, MockRobotController

### Community 19 - "AlarmLevel"
Cohesion: 0.27
Nodes (5): AlarmLevel, DateTime, AlarmEventArgs, AlarmRecord, ShoeMoldControl.Core.Models

### Community 20 - ".InvokeIfRequired"
Cohesion: 0.22
Nodes (3): Action, EventArgs, Random

### Community 21 - "MainForm"
Cohesion: 0.25
Nodes (4): MainForm, Button, JogType, Size

### Community 22 - "MockVisionService"
Cohesion: 0.25
Nodes (6): bool, CancellationToken, ILogger, int, Task, MockVisionService

### Community 24 - "VisionInspectionResult"
Cohesion: 0.50
Nodes (4): List, X, Y, VisionInspectionResult

## Knowledge Gaps
- **26 isolated node(s):** `net8.0-windows`, `net48`, `NModbus (3.0.83)`, `Polly (8.7.0)`, `Polly.Extensions.Http (3.0.0)` (+21 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **1 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `MainForm` connect `MainForm` to `GenericVisionService`, `ShoeMoldControl.Core`, `MachineDataModel`, `HighPerformanceUiRefresher`, `MainWindowViewModel`, `AlarmLevel`, `.InvokeIfRequired`, `VisionInspectionResult`?**
  _High betweenness centrality (0.269) - this node is a cross-community bridge._
- **Why does `Serilog` connect `ShoeMoldControl.Core` to `AvlAdvancedServices.cs`, `LoggerConfigurator.cs`?**
  _High betweenness centrality (0.219) - this node is a cross-community bridge._
- **Why does `ShoeMoldControl.Core` connect `ShoeMoldControl.Core` to `SimulationConfig`, `GenericVisionService`, `AvlAdvancedServices.cs`, `AppConfig.cs`?**
  _High betweenness centrality (0.155) - this node is a cross-community bridge._
- **What connects `net8.0-windows`, `net48`, `NModbus (3.0.83)` to the rest of the system?**
  _26 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `AvlAdvancedServices.cs` be split into smaller, more focused modules?**
  _Cohesion score 0.06001984126984127 - nodes in this community are weakly interconnected._
- **Should `GenericVisionService` be split into smaller, more focused modules?**
  _Cohesion score 0.06704260651629072 - nodes in this community are weakly interconnected._
- **Should `HardwareMotionResult` be split into smaller, more focused modules?**
  _Cohesion score 0.0593990216631726 - nodes in this community are weakly interconnected._