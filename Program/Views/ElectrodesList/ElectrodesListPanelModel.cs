using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using MEATaste.DataMEA.Models;

namespace MEATaste.Views.ElectrodesList
{
    public class ElectrodesListPanelModel : INotifyPropertyChanged
    {
        private ElectrodePropertiesExtended selectedElectrodeProperties;
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

       public ElectrodePropertiesExtended SelectedElectrodeProperties
        {
            get => selectedElectrodeProperties;
            set
            {
                if (selectedElectrodeProperties == value) return;
                selectedElectrodeProperties = value;
                OnPropertyChanged(nameof(SelectedElectrodeProperties));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}