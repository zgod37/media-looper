using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoLooper.ViewModels.Base {

    [ImplementPropertyChanged]
    public class BaseViewModel : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged = (sender,e) => { };
    }
}
