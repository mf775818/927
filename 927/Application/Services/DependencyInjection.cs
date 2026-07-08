using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoeMoldControl.Core;
using ShoeMoldControl.Core.Vision;
using ShoeMoldControl.Core.Hardware;
using ShoeMoldControl.Core.Domain;
using ShoeMoldControl.Application;
using ShoeMoldControl.Vision;
using ShoeMoldControl.Infrastructure;
using ShoeMoldControl.Infrastructure.Polly;
using ShoeMoldControl.Infrastructure.Vision;
using ShoeMoldControl.Infrastructure.Hardware;
using ShoeMoldControl.Infrastructure.Hardware.Adapters;
using Serilog;

namespace ShoeMoldControl
{
    public static class DependencyInjection
    {
        public static IServiceProvider ConfigureServices(string configPath = "appsettings.json")
        {
            var services = new ServiceCollection();

            // Configuration
            var configuration = BuildConfiguration(configPath);
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<AppConfig>();
            services.AddSingleton<IVisionConfig>(sp => sp.GetRequiredService<AppConfig>());
            services.AddSingleton<IRobotConfig>(sp => sp.GetRequiredService<AppConfig>());
            
            // Simulation Configuration
            services.AddSingleton<ISimulationConfig, SimulationConfig>();
            services.AddSingleton<IConnectionStateManager, ConnectionStateManager>();

            // Resilience Policies
            services.AddSingleton<IResiliencePolicyProvider, ResiliencePolicyProvider>();

            // Register Vision and Robot services based on configuration
            RegisterVisionService(services, configuration);
            RegisterRobotService(services, configuration);
            RegisterAvrHardwareServices(services, configuration);
            
            // Other Services
            services.AddSingleton<IBarcodeParser, DefaultBarcodeParser>();
            services.AddSingleton<IShoeMoldWorkflow, ShoeMoldWorkflow>();

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// 根據配置註冊視覺服務（實體或虛擬）
        /// 生產模式下若配置有誤或硬體不可用，將立即拋出異常而非退階至 Mock 服務
        /// </summary>
        private static void RegisterVisionService(IServiceCollection services, IConfiguration configuration)
        {
            var useMockVision = configuration.GetValue<bool>("Simulation:UseMockVision", false);
            var enableSimulation = configuration.GetValue<bool>("Simulation:EnableSimulation", false);

            if (useMockVision || enableSimulation)
            {
                var mockOptions = new MockVisionOptions
                {
                    SimulatedDelayMs = configuration.GetValue<int>("Simulation:MockVisionDelayMs", 500),
                    SuccessRate = configuration.GetValue<double>("Simulation:MockServiceSuccessRate", 1.0),
                    FixedBarcode = configuration.GetValue<string>("Simulation:FixedMockBarcode") ?? string.Empty
                };

                services.AddSingleton<IVisionService>(sp => 
                {
                    var logger = sp.GetService<Serilog.ILogger>();
                    return new MockVisionService(mockOptions, logger);
                });

                Log.Information("Vision Service: Using MOCK service (Simulation Mode)");
            }
            else
            {
                // 實體生產模式：嚴格註冊 AVL 泛型視覺服務
                // 若配置有誤、SDK 未安裝或硬體不可用，將立即拋出異常
                try
                {
                    // 驗證必要配置是否存在
                    var visionIp = configuration.GetValue<string>("Vision:IpAddress");
                    if (string.IsNullOrWhiteSpace(visionIp))
                    {
                        throw new InvalidOperationException(
                            "Production mode requires valid Vision:IpAddress configuration. " +
                            "Current value is null or empty. Please update appsettings.json or set SHOEMOLD_VISION_IPADDRESS environment variable.");
                    }

                    // 取得相機解析度配置（可選）
                    var imageWidth = configuration.GetValue<int>("Vision:ImageWidth", 2448);
                    var imageHeight = configuration.GetValue<int>("Vision:ImageHeight", 2048);
                    var pixelFormat = configuration.GetValue<string>("Vision:PixelFormat", "Mono8");
                    var bufferPoolSize = configuration.GetValue<int>("Vision:BufferPoolSize", 5);

                    // 根據像素格式計算 Stride
                    int stride = pixelFormat switch
                    {
                        "Mono8" => imageWidth,
                        "Bgr24" => imageWidth * 3,
                        "BayerRG8" => imageWidth,
                        _ => imageWidth
                    };

                    // 創建 FrameMemoryPool（單例）
                    var format = pixelFormat switch
                    {
                        "Mono8" => PixelFormat.Mono8,
                        "Bgr24" => PixelFormat.Bgr24,
                        "BayerRG8" => PixelFormat.BayerRG8,
                        _ => PixelFormat.Mono8
                    };

                    var bufferPool = new FrameMemoryPool(imageWidth, imageHeight, stride, format, bufferPoolSize);
                    services.AddSingleton(bufferPool);

                    // 註冊 AVL 泛型視覺服務鏈
                    services.AddSingleton<ICameraDriver<ManagedFrame>>(sp =>
                    {
                        var config = sp.GetRequiredService<IVisionConfig>();
                        var pool = sp.GetRequiredService<FrameMemoryPool>();
                        var logger = sp.GetService<Serilog.ILogger>();
                        return new AvlCameraDriver(config, pool, logger, imageWidth, imageHeight);
                    });

                    services.AddSingleton<IImageAnalyzer<ManagedFrame>, AvlImageAnalyzer>();
                    services.AddSingleton<IVisionService>(sp =>
                    {
                        var cameraDriver = sp.GetRequiredService<ICameraDriver<ManagedFrame>>();
                        var imageAnalyzer = sp.GetRequiredService<IImageAnalyzer<ManagedFrame>>();
                        var policyProvider = sp.GetRequiredService<IResiliencePolicyProvider>();
                        var bufferPool = sp.GetRequiredService<FrameMemoryPool>();
                        var logger = sp.GetService<Serilog.ILogger>();
                        return new GenericVisionService<ManagedFrame>(cameraDriver, imageAnalyzer, policyProvider, logger, bufferPool);
                    });

                    Log.Information("Vision Infrastructure: Production mode activated with AVL GenericVisionService<{Type}>", nameof(ManagedFrame));
                    Log.Information("Vision Service: Target device IP = {IP}, Resolution = {Width}x{Height}, Buffer Pool Size = {PoolSize}", 
                        visionIp, imageWidth, imageHeight, bufferPoolSize);
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "CRITICAL: Failed to initialize production vision service. Application cannot start without valid vision configuration.");
                    throw; // 立即拋出異常，阻止應用程式啟動
                }
            }
        }

        /// <summary>
        /// 根據配置註冊機器人服務（實體或虛擬）
        /// </summary>
        private static void RegisterRobotService(IServiceCollection services, IConfiguration configuration)
        {
            var useMockRobot = configuration.GetValue<bool>("Simulation:UseMockRobot", false);
            var enableSimulation = configuration.GetValue<bool>("Simulation:EnableSimulation", false);

            if (useMockRobot || enableSimulation)
            {
                var mockOptions = new MockRobotOptions
                {
                    SimulatedConnectionDelayMs = 200,
                    SimulatedCommandDelayMs = configuration.GetValue<int>("Simulation:MockRobotCommandDelayMs", 300),
                    SuccessRate = configuration.GetValue<double>("Simulation:MockServiceSuccessRate", 1.0)
                };

                services.AddSingleton<IRobotController>(sp => 
                {
                    var logger = sp.GetService<Serilog.ILogger>();
                    return new MockRobotController(mockOptions, logger);
                });

                Log.Information("Robot Controller: Using MOCK service (Simulation Mode)");
            }
            else
            {
                services.AddSingleton<IRobotController, DobotCraController>();
                Log.Information("Robot Controller: Using DOBOT CRA controller (Production Mode)");
            }
        }

        /// <summary>
        /// 註冊 AVR 硬體相關服務（廠商巨集解耦架構）
        /// 包含硬體閘道器與各項配接器服務
        /// </summary>
        private static void RegisterAvrHardwareServices(IServiceCollection services, IConfiguration configuration)
        {
            var useAvrHardware = configuration.GetValue<bool>("Hardware:UseAvrHardware", false);
            
            if (!useAvrHardware)
            {
                Log.Information("AVR Hardware Services: Disabled (set Hardware:UseAvrHardware=true to enable)");
                return;
            }

            try
            {
                // 取得 AVR 專案路徑與設備 IP 配置
                var avrProjectPath = configuration.GetValue<string>("Hardware:AvrProjectPath");
                var robotIp = configuration.GetValue<string>("Hardware:RobotIpAddress");
                var plcIp = configuration.GetValue<string>("Hardware:PlcIpAddress");

                if (string.IsNullOrWhiteSpace(robotIp) || string.IsNullOrWhiteSpace(plcIp))
                {
                    throw new InvalidOperationException(
                        "AVR Hardware mode requires valid Hardware:RobotIpAddress and Hardware:PlcIpAddress configuration.");
                }

                // 註冊硬體閘道器為單例 (管理 ProgramMacrofilters 實體生命週期)
                services.AddSingleton(sp =>
                {
                    var gateway = new AvrHardwareGateway(avrProjectPath);
                    // 注意：InitializeAsync 需在外部明確呼叫，此處僅建立實體
                    return gateway;
                });

                // 註冊配接器服務
                services.AddSingleton<IAvrRobotMotion>(sp =>
                {
                    var gateway = sp.GetRequiredService<AvrHardwareGateway>();
                    return new AvrRobotMotionAdapter(gateway);
                });

                services.AddSingleton<IAvrRobotInstructionalJog>(sp =>
                {
                    var gateway = sp.GetRequiredService<AvrHardwareGateway>();
                    return new AvrRobotInstructionalJogAdapter(gateway);
                });

                services.AddSingleton<IAvrPlcCommunicator>(sp =>
                {
                    var gateway = sp.GetRequiredService<AvrHardwareGateway>();
                    return new AvrPlcCommunicationAdapter(gateway);
                });

                services.AddSingleton<IAvrCameraDriver>(sp =>
                {
                    var gateway = sp.GetRequiredService<AvrHardwareGateway>();
                    return new AvrCameraDriverAdapter(gateway);
                });

                services.AddSingleton<IAvrImageAnalyzer>(sp =>
                {
                    var gateway = sp.GetRequiredService<AvrHardwareGateway>();
                    return new AvrImageAnalyzerAdapter(gateway);
                });

                Log.Information("AVR Hardware Services: Production mode activated with SOLID adapter architecture");
                Log.Information("AVR Hardware: Robot IP = {RobotIp}, PLC IP = {PlcIp}, Project Path = {ProjectPath}",
                    robotIp, plcIp, avrProjectPath ?? "(default)");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "CRITICAL: Failed to initialize AVR hardware services. Application cannot start without valid hardware configuration.");
                throw;
            }
        }

        private static IConfiguration BuildConfiguration(string configPath)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory);

            builder.AddJsonFile(configPath, optional: true, reloadOnChange: true);
            builder.AddEnvironmentVariables(prefix: "SHOEMOLD_");

            return builder.Build();
        }
    }
}
