using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VideoLooper.ViewModels.Base {
    public class RelayCommand : ICommand {
        public event EventHandler CanExecuteChanged = (sender,e) => { };

        /// <summary>
        /// the command to be executed
        /// </summary>
        private Action _action;

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="action"></param>
        public RelayCommand(Action action) {
            this._action = action;
        }


        /// <summary>
        /// relay command can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter) {
            return true;
        }

        /// <summary>
        /// invoke the command
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter) {
            _action();
        }
    }
}
