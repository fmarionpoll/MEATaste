using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using MEATaste.Annotations;
using MEATaste.DataMEA.Models;

namespace MEATaste.Views.ElectrodesList
{
    public class ElectrodesListPanelModel : INotifyPropertyChanged
    {
        private ObservableCollection<ElectrodeRecord> electrodes;
        private ElectrodeRecord selectedElectrodeRecord;
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

        public ObservableCollection<ElectrodeRecord> Electrodes
        {
            get => electrodes;
            set
            {
                if (electrodes == value) return;
                electrodes = value;
                OnPropertyChanged(nameof(Electrodes));
            }
        }
        
       public ElectrodeRecord SelectedElectrodeRecord
        {
            get => selectedElectrodeRecord;
            set
            {
                if (selectedElectrodeRecord == value) return;
                selectedElectrodeRecord = value;
                OnPropertyChanged(nameof(SelectedElectrodeRecord));
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