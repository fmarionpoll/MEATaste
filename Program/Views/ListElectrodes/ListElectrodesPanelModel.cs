using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;

namespace MEATaste.Views.ListElectrodes
{
    public class ListElectrodesPanelModel : INotifyPropertyChanged
    {
        private ICollectionView electrodeListView;

        public ICollectionView ElectrodeListView
        {
            get => electrodeListView;
            set
            {
                if (electrodeListView == value) return;
                electrodeListView = value;
                OnPropertyChanged(nameof(ElectrodeListView));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

       [NotifyPropertyChangedInvocator]
       protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}