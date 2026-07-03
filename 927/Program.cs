using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShoeMoldControl;
using Serilog;

namespace _927
{
    /// <summary>
    /// WinForm 應用程式入口點 - 從 WPF App.xaml 重構而來
    /// 使用 STAThread 模式，保留所有依賴注入、日誌和取消令牌功能
    /// </summary>
    internal static class Program
    {
        private static IServiceProvider? _serviceProvider;
        private static CancellationTokenSource? _appCts;

        [STAThread]
        private static async Task Main(string[] args)
        {
            // Enable visual styles for modern Windows look
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Configure logging first
                ShoeMoldControl.Infrastructure.Logging.LoggerConfigurator.Configure();
                Log.Information("Application starting (WinForms)...");

                // Setup dependency injection
                _serviceProvider = DependencyInjection.ConfigureServices();
                Log.Information("Dependency injection configured");

                // Initialize app cancellation token
                _appCts = new CancellationTokenSource();

                // Set up unhandled exception handlers
                Application.ThreadException += (sender, e) =>
                {
                    Log.Error(e.Exception, "Unhandled UI thread exception");
                };
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    Log.Fatal((Exception)e.ExceptionObject!, "Unhandled exception in AppDomain");
                };

                // Show main form and run message loop
                using var mainForm = new MainForm(_serviceProvider, _appCts.Token);
                Application.Run(mainForm);

                Log.Information("Application exited successfully");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
                MessageBox.Show($"Application failed to start: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Cleanup
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
            }
        }
    }
}
