using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using MEATaste.DataMEA.Models;

namespace MEATaste.Views.ElectrodesList
{
    public class ElectrodesListPanelModel : INotifyPropertyChanged
    {
        private ElectrodePropertiesExtended selectedElectrodeExtendedProperties;
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

       public ElectrodePropertiesExtended SelectedElectrodeExtendedProperties
       {
            get => selectedElectrodeExtendedProperties;
            set
            {
                if (selectedElectrodeExtendedProperties == value) return;
                selectedElectrodeExtendedProperties = value;
                OnPropertyChanged(nameof(SelectedElectrodeExtendedProperties));
            }
       }



       public event PropertyChangedEventHandler PropertyChanged;

       [NotifyPropertyChangedInvocator]
       protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}