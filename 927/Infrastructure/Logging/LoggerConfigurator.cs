using System;
using Serilog;
using Serilog.Events;

namespace ShoeMoldControl.Infrastructure.Logging
{
    public static class LoggerConfigurator
    {
        public static void Configure(string logFilePath = "logs/app-.log")
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: logFilePath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information("Application logging configured successfully");
        }

        public static void FlushAndClose()
        {
            Log.Information("Shutting down logging...");
            Log.CloseAndFlush();
        }
    }
}

// Extension methods for enrichers (simplified version to avoid extra dependencies)
namespace Serilog
{
    public static class LoggerEnrichmentConfigurationExtensions
    {
        public static LoggerConfiguration WithMachineName(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            return enrichmentConfiguration.With(new MachineNameEnricher());
        }

        public static LoggerConfiguration WithThreadId(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            return enrichmentConfiguration.With(new ThreadIdEnricher());
        }
    }

    public class MachineNameEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("MachineName", Environment.MachineName));
        }
    }

    public class ThreadIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ThreadId", System.Threading.Thread.CurrentThread.ManagedThreadId));
        }
    }
}
