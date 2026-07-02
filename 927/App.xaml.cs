using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ShoeMoldControl;
using Serilog;

namespace _927
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider? _serviceProvider;
        private CancellationTokenSource? _appCts;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Configure logging first
                ShoeMoldControl.Infrastructure.Logging.LoggerConfigurator.Configure();
                Log.Information("Application starting...");

                // Setup dependency injection
                _serviceProvider = DependencyInjection.ConfigureServices();
                Log.Information("Dependency injection configured");

                // Initialize app cancellation token
                _appCts = new CancellationTokenSource();

                // Show main window
                var mainWindow = new MainWindow(_serviceProvider, _appCts.Token);
                mainWindow.Show();

                Log.Information("Application started successfully");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
                MessageBox.Show($"Application failed to start: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }

            await Task.CompletedTask;
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            Log.Information("Application shutting down...");

            if (_appCts != null)
            {
                Log.Information("Requesting application cancellation...");
                await _appCts.CancelAsync();
            }

            // Dispose services
            if (_serviceProvider is IDisposable disposable)
            {
                Log.Information("Disposing service provider...");
                disposable.Dispose();
            }

            // Flush and close logging
            ShoeMoldControl.Infrastructure.Logging.LoggerConfigurator.FlushAndClose();

            base.OnExit(e);
            await Task.CompletedTask;
        }
    }
}
