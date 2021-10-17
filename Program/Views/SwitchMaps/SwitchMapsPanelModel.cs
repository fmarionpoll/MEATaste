using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;

namespace MEATaste.Views.SwitchMaps
{
    public class SwitchMapsPanelModel : INotifyPropertyChanged
    {
        private int selectedTabIndex;
        public int SelectedTabIndex
        {
            get => selectedTabIndex;
            set
            {
                if (selectedTabIndex == value) return;
                selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
