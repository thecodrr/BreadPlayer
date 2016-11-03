/* 
	BreadPlayer. A music player made for Windows 10 store.
    Copyright (C) 2016  theweavrs (Abdullah Atta)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace BreadPlayer

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
        private bool enabled;
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;
        #endregion

        public bool IsEnabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    RaiseCanExecuteChanged();
                }
            }
        }
        #region Constructors 
        public RelayCommand(Action<object> execute) : this(execute, null)
        {
            enabled = true;
        }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            enabled = true;
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion // Constructors 
        #region ICommand Members 
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return enabled; //_canExecute == null ? true : _canExecute(parameter);
        }
        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        public void Execute(object parameter) { _execute(parameter); }
        #endregion // ICommand Members 
    }
    }