
using System.Windows.Controls;

namespace VideoLooper {

    public interface IRemote {

        void Register(IControllable controllable);
        void Unregister(IControllable controllable);

    }
}
