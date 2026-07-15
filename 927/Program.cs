using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShoeMoldControl;
using Serilog;
using System.Diagnostics.Eventing.Reader;

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
#if NET8_0_OR_GREATER
                    Log.Information("Requesting application cancellation asynchronously (.NET 8+)...");
                    try
                    {
                        // 使用 .NET 8 原生非同步取消，並透過 WaitAsync 實施 5 秒超時制動
                        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                        await _appCts.CancelAsync().WaitAsync(timeoutCts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        Log.Warning("Application cancellation timed out during async execution.");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Exception occurred during async application cancellation");
                    }
                    finally
                    {
                        _appCts.Dispose();
                    }
#else
                    Log.Information("Requesting application cancellation with timeout defense (.NET Framework 4.8)...");
                    try
                    {
                        // 將同步 Cancel 移至執行緒池執行，防止 UI 訊息循環終止引發的永久死鎖
                        var cancelTask = Task.Run(() => _appCts.Cancel());
                        if (!cancelTask.Wait(TimeSpan.FromSeconds(5)))
                        {
                            Log.Warning("Application cancellation timed out. Proceeding to force cleanup.");
                        }
                    }
                    catch (AggregateException ae)
                    {
                        // 逐項隔離並記錄所有來自註冊 Callback 的異常，防止中斷 ServiceProvider 的釋放
                        ae.Handle(ex =>
                        {
                            Log.Error(ex, "Exception detected in cancellation token registered callback");
                            return true; 
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Unexpected exception occurred during application cancellation");
                    }
                    finally
                    {
                        _appCts.Dispose();
                    }
#endif
                }

                // Dispose services
                if (_serviceProvider is IDisposable disposable)
                {
                    Log.Information("Disposing service provider...");
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Exception occurred while disposing service provider");
                    }
                }

                // Flush and close logging
                Log.Information("Application lifecycle cleanup completed. Flushing logs.");
                ShoeMoldControl.Infrastructure.Logging.LoggerConfigurator.FlushAndClose();
            }
        }
    }
}
