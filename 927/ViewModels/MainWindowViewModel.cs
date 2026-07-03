using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using ShoeMoldControl.Core;
using Serilog;

namespace _927.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private Task? _workflowTask;
        private string _statusText = "Ready";

        public MainWindowViewModel(IServiceProvider serviceProvider, CancellationToken cancellationToken, ILogger logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _cancellationToken = cancellationToken;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            StartCommand = new RelayCommand(_ => Start(), _ => _workflowTask == null || _workflowTask.IsCompleted);
            StopCommand = new RelayCommand(async _ => await Stop(), _ => _workflowTask != null && !_workflowTask.IsCompleted);
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public string StatusText
        {
            get => _statusText;
            set
            {
                if (_statusText != value)
                {
                    _statusText = value;
                    OnPropertyChanged();
                }
            }
        }

        private void Start()
        {
            try
            {
                _logger.Information("Starting production workflow (VM)");
                var workflow = _serviceProvider.GetRequiredService<IShoeMoldWorkflow>();
                _workflowTask = Task.Run(() => workflow.RunProductionCycleAsync(_cancellationToken));
                StatusText = "Production started";
                _logger.Information("Production workflow started (VM)");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to start production workflow (VM)");
                StatusText = $"Error: {ex.Message}";
            }
        }

        private async Task Stop()
        {
            try
            {
                _logger.Information("Stopping production workflow (VM)");
                if (_workflowTask != null && !_workflowTask.IsCompleted)
                {
                    await _workflowTask;
                }
                StatusText = "Production stopped";
                _logger.Information("Production workflow stopped (VM)");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error stopping production workflow (VM)");
                StatusText = $"Error: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
