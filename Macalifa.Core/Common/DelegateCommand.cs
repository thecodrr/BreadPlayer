using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Macalifa

{
    public class DelegateCommand : ICommand
    {
        private readonly Action action;
        private bool enabled;

        public DelegateCommand(Action action)
        {
            this.action = action;
            this.enabled = true;
        }

        public bool IsEnabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    OnCanExecuteChanged();
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return enabled;
        }

        public void Execute(object parameter)
        {
            action();
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
    public class RelayCommand : ICommand
    {
        #region Fields 
        readonly Action<object> _execute; readonly Predicate<object> _canExecute;
        #endregion
        // Fields 
        #region Constructors 
        public RelayCommand(Action<object> execute) : this(execute, null) { } public RelayCommand(Action<object> execute, Predicate<object> canExecute) { if (execute == null) throw new ArgumentNullException("execute"); _execute = execute; _canExecute = canExecute; }
        #endregion // Constructors 
        #region ICommand Members 
        [DebuggerStepThrough]
        public bool CanExecute(object parameter) { return _canExecute == null ? true : _canExecute(parameter); }
        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        public void Execute(object parameter) { _execute(parameter); }
        #endregion // ICommand Members 
    }
    }