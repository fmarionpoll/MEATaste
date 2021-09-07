using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using MEATaste.Annotations;
using MEATaste.DataMEA.Models;

namespace MEATaste.Views.ElectrodesList
{
    public class ElectrodesListPanelModel : INotifyPropertyChanged
    {
        private ObservableCollection<ElectrodeProperties> electrodes;
        private ElectrodeProperties selectedElectrodeProperties;
        private int selectedElectrodeIndex;
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

        public ObservableCollection<ElectrodeProperties> Electrodes
        {
            get => electrodes;
            set
            {
                if (electrodes == value) return;
                electrodes = value;
                OnPropertyChanged(nameof(Electrodes));
            }
        }
        
       public ElectrodeProperties SelectedElectrodeProperties
        {
            get => selectedElectrodeProperties;
            set
            {
                if (selectedElectrodeProperties == value) return;
                selectedElectrodeProperties = value;
                OnPropertyChanged(nameof(SelectedElectrodeProperties));
            }
        }

       public int SelectedElectrodeIndex
       {
           get => selectedElectrodeIndex;
           set
           {
               if (selectedElectrodeIndex == value) return;
               selectedElectrodeIndex = value;
               OnPropertyChanged(nameof(SelectedElectrodeIndex));
           }
       }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}