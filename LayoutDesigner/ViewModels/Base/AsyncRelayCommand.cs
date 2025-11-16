using System.Windows.Input;

namespace LayoutDesigner.ViewModels.Base
{
    /// <summary>
    /// Async command implementation that prevents concurrent execution
    /// Best Practice: Use for async operations like Save, Load, Export to keep UI responsive
    /// </summary>
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;
        private bool _isExecuting;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Creates a new async command that can always execute
        /// </summary>
        /// <param name="execute">The async execution logic</param>
        public AsyncRelayCommand(Func<Task> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new async command
        /// </summary>
        /// <param name="execute">The async execution logic</param>
        /// <param name="canExecute">The execution status logic</param>
        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            // Prevent concurrent execution - Best Practice for async commands
            if (_isExecuting)
                return false;

            return _canExecute == null || _canExecute();
        }

        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            _isExecuting = true;
            RaiseCanExecuteChanged();

            try
            {
                await _execute();
            }
            catch (Exception ex)
            {
                // Log error using ErrorHandlingService
                var errorService = ServiceContainer.GetService<Services.Interfaces.IErrorHandlingService>();
                errorService?.HandleError(ex, "AsyncRelayCommand error", showMessageBox: false);
                throw; // Re-throw to allow handling at higher level
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Raises the CanExecuteChanged event
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    /// <summary>
    /// Generic async command with parameter support
    /// Best Practice: Use for async operations that need parameters
    /// </summary>
    /// <typeparam name="T">Parameter type</typeparam>
    public class AsyncRelayCommand<T> : ICommand
    {
        private readonly Func<T?, Task> _execute;
        private readonly Predicate<T?>? _canExecute;
        private bool _isExecuting;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Creates a new async command that can always execute
        /// </summary>
        /// <param name="execute">The async execution logic</param>
        public AsyncRelayCommand(Func<T?, Task> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new async command
        /// </summary>
        /// <param name="execute">The async execution logic</param>
        /// <param name="canExecute">The execution status logic</param>
        public AsyncRelayCommand(Func<T?, Task> execute, Predicate<T?>? canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            // Prevent concurrent execution
            if (_isExecuting)
                return false;

            return _canExecute == null || _canExecute((T?)parameter);
        }

        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            _isExecuting = true;
            RaiseCanExecuteChanged();

            try
            {
                await _execute((T?)parameter);
            }
            catch (Exception ex)
            {
                var errorService = ServiceContainer.GetService<Services.Interfaces.IErrorHandlingService>();
                errorService?.HandleError(ex, $"AsyncRelayCommand<{typeof(T).Name}> error", showMessageBox: false);
                throw;
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Raises the CanExecuteChanged event
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
