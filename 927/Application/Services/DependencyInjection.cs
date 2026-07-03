using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoeMoldControl.Core;
using ShoeMoldControl.Application;
using ShoeMoldControl.Vision;
using ShoeMoldControl.Infrastructure;
using ShoeMoldControl.Infrastructure.Polly;
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
                services.AddSingleton<IVisionService, TcpVisionService>();
                Log.Information("Vision Service: Using TCP service (Production Mode)");
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
