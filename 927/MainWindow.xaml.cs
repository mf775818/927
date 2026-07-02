using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ShoeMoldControl.Core;
using Serilog;

namespace _927
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private Task? _workflowTask;

        public MainWindow(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _cancellationToken = cancellationToken;
            _logger = Log.ForContext<MainWindow>();

            InitializeComponent();
            InitializeUi();
            
            _logger.Information("MainWindow initialized");
        }

        private void InitializeUi()
        {
            // Setup basic UI controls (this would be expanded in a real implementation)
            Title = "Shoe Mold Control System - Industrial Edition";
            Width = 800;
            Height = 600;

            var mainGrid = new Grid();
            mainGrid.Margin = new Thickness(10);

            var stackPanel = new StackPanel();
            
            // Status label
            var statusLabel = new Label
            {
                Content = "Status: Ready",
                FontSize = 14,
                FontWeight = FontWeights.Bold
            };
            stackPanel.Children.Add(statusLabel);

            // Start button
            var startButton = new Button
            {
                Content = "Start Production",
                Width = 150,
                Height = 40,
                Margin = new Thickness(0, 10, 0, 0)
            };
            startButton.Click += StartButton_Click;
            stackPanel.Children.Add(startButton);

            // Stop button
            var stopButton = new Button
            {
                Content = "Stop Production",
                Width = 150,
                Height = 40,
                Margin = new Thickness(0, 10, 0, 0),
                IsEnabled = false
            };
            stopButton.Click += StopButton_Click;
            stackPanel.Children.Add(stopButton);

            mainGrid.Children.Add(stackPanel);
            Content = mainGrid;
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.Information("Starting production workflow");
                
                var workflow = _serviceProvider.GetRequiredService<IShoeMoldWorkflow>();
                _workflowTask = Task.Run(() => workflow.RunProductionCycleAsync(_cancellationToken));
                
                _logger.Information("Production workflow started");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to start production workflow");
                MessageBox.Show($"Error starting production: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.Information("Stopping production workflow");
                
                if (_workflowTask != null && !_workflowTask.IsCompleted)
                {
                    await _workflowTask;
                }
                
                _logger.Information("Production workflow stopped");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error stopping production workflow");
            }
        }
    }
}