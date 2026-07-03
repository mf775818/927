using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ShoeMoldControl.Core;
using Serilog;
using _927.ViewModels;
using System.Windows.Data;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

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
            // 建立 ViewModel 並設為 DataContext，遵循 MVVM
            DataContext = new MainWindowViewModel(_serviceProvider, _cancellationToken, _logger);
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
                FontSize = 14,
                FontWeight = FontWeights.Bold
            };
            // Bind label content to ViewModel.StatusText
            statusLabel.SetBinding(ContentProperty, new Binding("StatusText"));
            stackPanel.Children.Add(statusLabel);

            // Start button
            var startButton = new Button
            {
                Content = "Start Production",
                Width = 150,
                Height = 40,
                Margin = new Thickness(0, 10, 0, 0)
            };
            // 使用 ViewModel 的 Command（若 DataContext 是 MainWindowViewModel）
            if (DataContext is MainWindowViewModel vm)
            {
                startButton.Command = vm.StartCommand;
            }
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
            if (DataContext is MainWindowViewModel vm2)
            {
                stopButton.Command = vm2.StopCommand;
            }
            stackPanel.Children.Add(stopButton);

            mainGrid.Children.Add(stackPanel);
            Content = mainGrid;
        }

        // Event handlers removed: commands are handled in MainWindowViewModel (MVVM)
        // Provide an empty handler to satisfy any legacy XAML references
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // intentionally left blank
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}