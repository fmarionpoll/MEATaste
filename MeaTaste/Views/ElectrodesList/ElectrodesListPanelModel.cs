using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using MEATaste.Annotations;
using MEATaste.DataMEA.Models;

namespace MEATaste.Views.ElectrodesList
{
    public class ElectrodesListPanelModel : INotifyPropertyChanged
    {
        private ObservableCollection<Electrode> electrodes;
        private int selectedElectrodeChannelNumber;
        private Electrode selectedElectrode;
        private int selectedElectrodeIndex;


        public ObservableCollection<Electrode> Electrodes
        {
            get => electrodes;
            set
            {
                if (electrodes == value) return;
                electrodes = value;
                OnPropertyChanged(nameof(Electrodes));
            }
        }
        
       public Electrode SelectedElectrode
        {
            get => selectedElectrode;
            set
            {
                selectedElectrode = value;
                OnPropertyChanged(nameof(SelectedElectrode));
            }
        }

       public int SelectedElectrodeChannelNumber
        {
            get => selectedElectrodeChannelNumber;
            set
            {
                selectedElectrodeChannelNumber = value;
                OnPropertyChanged(nameof(SelectedElectrodeChannelNumber));
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