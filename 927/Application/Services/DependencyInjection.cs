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

            // Resilience Policies
            services.AddSingleton<IResiliencePolicyProvider, ResiliencePolicyProvider>();

            // Services
            services.AddSingleton<IVisionService, TcpVisionService>();
            services.AddSingleton<IRobotController, DobotCraController>();
            services.AddSingleton<IBarcodeParser, DefaultBarcodeParser>();
            services.AddSingleton<IShoeMoldWorkflow, ShoeMoldWorkflow>();

            return services.BuildServiceProvider();
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
