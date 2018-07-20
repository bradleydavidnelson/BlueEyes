using System;
using System.Windows.Input;

namespace BlueEyes.Utilities
{
    public class RelayCommand : RelayCommand<Object>
    {
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            : base(execute, canExecute)
        { }
    }

    public class RelayCommand<T> : ICommand
    {
        #region Fields
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the RelayCommand class
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<T> execute) : this(execute, null)
        { }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion

        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }
            else
            {
                return _canExecute((T)parameter);
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
        #endregion
    }
}
