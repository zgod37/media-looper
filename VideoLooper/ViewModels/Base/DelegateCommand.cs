using System;
using System.Windows.Input;

namespace VideoLooper.ViewModels.Base {
    public class DelegateCommand : ICommand {

        private Predicate<object> _canExecute;
        private Action _action;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action action, Predicate<object> canExecute) {
            _action = action;
            _canExecute = canExecute;
        }

        public void RaiseCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter) {
            if (_canExecute == null) {
                return true;
            }

            return _canExecute(parameter);
        }

        public void Execute(object parameter) {
            _action();
        }
    }
}
