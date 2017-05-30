using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VideoLooper.ViewModels.Base;

namespace VideoLooper.ViewModels {
    public class RemoteViewModel : BaseViewModel {

        public ICommand OpenNewCommand { get; set; }

        //public ObservableCollection<> Children { get; set; }        

        public RemoteViewModel() {
            OpenNewCommand = new RelayCommand(OpenNewVideoWindow);
        }

        private void OpenNewVideoWindow() {

        }

    }
}
