using System;

namespace _927.ViewModels
{
    /// <summary>
    /// WinForms 相容的 RelayCommand 實作
    /// 取代 WPF 的 ICommand，使用事件驅動模式
    /// </summary>
    internal class RelayCommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);

        /// <summary>
        /// 當 CanExecute 狀態改變時觸發的事件
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// 手動觸發 CanExecuteChanged 事件
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
