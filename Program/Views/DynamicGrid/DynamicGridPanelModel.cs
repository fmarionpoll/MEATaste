using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;

// adapted from Illya Reznykov https://github.com/IReznykov/Blog

namespace MEATaste.Views.DynamicGrid
{
    public class DynamicGridPanelModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
