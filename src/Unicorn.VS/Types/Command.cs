using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Unicorn.VS.Types
{
    public class Command<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;
        public event EventHandler CanExecuteChanged;

        public Command(Action<T> execute, Func<T, bool> canexecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canexecute ?? (e => true);
        }

        [DebuggerStepThrough]
        public bool CanExecute(object p)
        {
            try
            {
                var value = (T)Convert.ChangeType(p, typeof(T));
                return _canExecute == null ? true : _canExecute(value);
            }
            catch { return false; }
        }

        public virtual void Execute(object p)
        {
            if (!CanExecute(p))
                return;
            var value = (T)Convert.ChangeType(p, typeof(T));
            _execute(value);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

    }

    public class Command : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        public event EventHandler CanExecuteChanged;

        public Command(Action execute, Func<bool> canexecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canexecute ?? (() => true);
        }

        [DebuggerStepThrough]
        public bool CanExecute(object p)
        {
            try { return _canExecute(); }
            catch { return false; }
        }

        public virtual void Execute(object p)
        {
            if (!CanExecute(p))
                return;
            try { _execute(); }
            catch { Debugger.Break(); }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        
    }
}