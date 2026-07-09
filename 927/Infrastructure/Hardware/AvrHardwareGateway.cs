#if NET48
using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace ShoeMoldControl.Infrastructure.Hardware
{
    /// <summary>
    /// 執行緒安全硬體閘道器 - 管理廠商 ProgramMacrofilters 單一實體
    /// 防止 P/Invoke 非同步存取導致的死結與資源洩漏
    /// </summary>
    public class AvrHardwareGateway : IDisposable
    {
        private readonly SemaphoreSlim _hardwareLock = new SemaphoreSlim(1, 1);
        private readonly string _projectPath;
        private bool _isInitialized;
        private int _plcSocketId;
        private bool _disposed;

        /// <summary>
        /// 廠商原生 ProgramMacrofilters 實體 (僅供內部配接器使用)
        /// </summary>
        public ModbusConnectRobot.ProgramMacrofilters? RawMacros { get; private set; }

        /// <summary>
        /// PLC Socket ID (連線成功後有效)
        /// </summary>
        public int PlcSocketId => _plcSocketId;

        /// <summary>
        /// 是否已完成初始化
        /// </summary>
        public bool IsInitialized => _isInitialized;

        public AvrHardwareGateway(string projectPath)
        {
            _projectPath = !string.IsNullOrWhiteSpace(projectPath)
                ? projectPath
                : @"C:\TK_CC\AuroraVision 影像系統\TK_Sandbox\ModbusConnectAll_MarcoFilter\Program.avproj";
        }

        /// <summary>
        /// 初始化硬體閘道器 (連線機器人與 PLC)
        /// </summary>
        public async Task InitializeAsync(string robotIp, string plcIp, CancellationToken token)
        {
            await _hardwareLock.WaitAsync(token);
            try
            {
                if (_isInitialized) return;

                Log.Information("開始初始化 AVR 硬體閘道器，專案路徑：{ProjectPath}", _projectPath);

                RawMacros = new ModbusConnectRobot.ProgramMacrofilters(_projectPath);

                bool robotConnected = false;
                bool plcConnected = false;

                await Task.Run(() =>
                {
                    try
                    {
                        RawMacros!.ConnectToRobot(robotIp, out robotConnected);
                        Log.Information("機器人連線狀態：{Status}", robotConnected ? "成功" : "失敗");

                        RawMacros.ConnectToPLC(plcIp, out _plcSocketId, out plcConnected);
                        Log.Information("PLC 連線狀態：{Status}, SocketID: {SocketId}", 
                            plcConnected ? "成功" : "失敗", _plcSocketId);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "連線硬體設備時發生異常");
                        throw;
                    }
                }, token);

                // 工業級硬連線強制作業驗證
                if (!robotConnected || !plcConnected)
                {
                    var errorMsg = $"設備實體連線失敗。Robot 連線狀態：{robotConnected}, PLC 連線狀態：{plcConnected}";
                    Log.Error(errorMsg);
                    throw new InvalidOperationException(errorMsg);
                }

                _isInitialized = true;
                Log.Information("AVR 硬體閘道器初始化完成");
            }
            finally
            {
                _hardwareLock.Release();
            }
        }

        /// <summary>
        /// 執行緒安全的 Action 執行方法
        /// </summary>
        public async Task ExecuteSafeActionAsync(Action<ModbusConnectRobot.ProgramMacrofilters> action, CancellationToken token)
        {
            await _hardwareLock.WaitAsync(token);
            try
            {
                if (!_isInitialized || RawMacros == null)
                    throw new InvalidOperationException("硬體閘道器尚未初始化，拒絕執行底層巨集。");

                action(RawMacros);
            }
            finally
            {
                _hardwareLock.Release();
            }
        }

        /// <summary>
        /// 執行緒安全的 Func 執行方法 (帶回傳值)
        /// </summary>
        public async Task<T> ExecuteSafeFuncAsync<T>(Func<ModbusConnectRobot.ProgramMacrofilters, T> func, CancellationToken token)
        {
            await _hardwareLock.WaitAsync(token);
            try
            {
                if (!_isInitialized || RawMacros == null)
                    throw new InvalidOperationException("硬體閘道器尚未初始化，拒絕執行底層巨集。");

                return func(RawMacros);
            }
            finally
            {
                _hardwareLock.Release();
            }
        }

        /// <summary>
        /// 釋放硬體資源
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            _hardwareLock.Wait();
            try
            {
                if (RawMacros != null)
                {
                    Log.Information("開始釋放 AVR 硬體資源...");
                    
                    try
                    {
                        RawMacros.ReleaseRobot(out bool releaseResult);
                        Log.Information("機器人釋放結果：{Result}", releaseResult ? "成功" : "失敗");
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "釋放機器人資源時發生異常");
                    }

                    try
                    {
                        RawMacros.ResetConnectRobot();
                        Log.Information("機器人連線重設完成");
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "重設機器人連線時發生異常");
                    }
                }
            }
            finally
            {
                _hardwareLock.Release();
                _hardwareLock.Dispose();
                _disposed = true;
                Log.Information("AVR 硬體閘道器資源釋放完成");
            }
        }
    }
}
#else
namespace ShoeMoldControl.Core.Hardware { public class AvrHardwareGateway { } }
#endif