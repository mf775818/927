using AuroraVision;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShoeMoldControl.Infrastructure.Hardware
{
    public class AvrHardwareGateway : IDisposable
    {
        private readonly SemaphoreSlim _hardwareLock = new SemaphoreSlim(1, 1);
        private readonly string _projectPath;
        private bool _isInitialized;
        private int _plcSocketId;
        private bool _disposed;

        public ProgramMacrofilters RawMacros { get; private set; }
        public int PlcSocketId => _plcSocketId;

        public bool IsInitialized { get => _isInitialized; set => _isInitialized = value; }

        public AvrHardwareGateway(string projectPath)
        {
            if (string.IsNullOrWhiteSpace(projectPath))
            {
                throw new ArgumentNullException(nameof(projectPath), "專案路徑不得為空。");
            }
            _projectPath = projectPath;
        }

        public async Task InitializeAsync(string robotIp, string plcIp, CancellationToken token)
        {
            await _hardwareLock.WaitAsync(token);
            try
            {
                if (_isInitialized)
                    return;

                RawMacros = new ProgramMacrofilters(_projectPath);

                bool robotConnected = false;
                bool plcConnected = false;
                int socketId = 0;

                await Task.Run(() => {
                    RawMacros.ConnectToRobot(robotIp, out robotConnected);
                    RawMacros.ConnectToPLC(plcIp, out socketId, out plcConnected);
                }, token);

                if (!robotConnected || !plcConnected)
                {
                    RawMacros.ResetConnectRobot();
                    throw new InvalidOperationException($"連線硬體失敗。機器人:{robotConnected}, PLC:{plcConnected}");
                }

                _plcSocketId = socketId;
                _isInitialized = true;
            }
            catch (Exception)
            {
                _isInitialized = false;
                throw;
            }
            finally
            {
                _hardwareLock.Release();
            }
        }

        public async Task<T> ExecuteSafeFuncAsync<T>(Func<ProgramMacrofilters, T> func, CancellationToken token)
        {
            await _hardwareLock.WaitAsync(token);
            try
            {
                if (!_isInitialized || RawMacros == null)
                {
                    throw new InvalidOperationException("硬件閘道器未初始化。");
                }
                return func(RawMacros);
            }
            finally
            {
                _hardwareLock.Release();
            }
        }

        public async Task ExecuteSafeActionAsync(Action<ProgramMacrofilters> action, CancellationToken token)
        {
            await _hardwareLock.WaitAsync(token);
            try
            {
                if (!_isInitialized || RawMacros == null)
                {
                    throw new InvalidOperationException("硬件閘道器未初始化。");
                }
                action(RawMacros);
            }
            finally
            {
                _hardwareLock.Release();
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _hardwareLock.Wait();
            try
            {
                if (RawMacros != null)
                {
                    RawMacros.ReleaseRobot(out _);
                    RawMacros.ResetConnectRobot();
                }
                _disposed = true;
            }
            finally
            {
                _hardwareLock.Release();
                _hardwareLock.Dispose();
            }
        }
    }
}