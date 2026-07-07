using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoeMoldControl.Core;
using ShoeMoldControl.Core.Vision;
using ShoeMoldControl.Application;
using ShoeMoldControl.Vision;
using ShoeMoldControl.Infrastructure;
using ShoeMoldControl.Infrastructure.Polly;
using ShoeMoldControl.Infrastructure.Vision;
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
