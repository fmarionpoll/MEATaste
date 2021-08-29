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
        private ICollectionView mySource;

        public ICollectionView MySource
        {
            get => mySource;
            set
            {
                if (mySource == value) return;
                mySource = value;
                OnPropertyChanged(nameof(MySource));
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
                selectedElectrodeRecord = value;
                OnPropertyChanged(nameof(SelectedElectrodeRecord));
            }
        }

       public int SelectedElectrodeIndex
       {
           get => selectedElectrodeIndex;
           set
           {
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