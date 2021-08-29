using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using MEATaste.Annotations;
using MEATaste.DataMEA.Models;




namespace MEATaste.Views.ElectrodesList
{
    public class ElectrodesListPanelModel : INotifyPropertyChanged
    {
        private ObservableCollection<Electrode> electrodesTableModel;
        private int selectedElectrodeChannelNumber;
        private Electrode selectedElectrodeItem;
        private int selectedElectrodeIndex;

        public ObservableCollection<Electrode> ElectrodesTableModel
        {
            get => electrodesTableModel;
            set
            {
                if (electrodesTableModel == value) return;
                electrodesTableModel = value;
                OnPropertyChanged(nameof(ElectrodesTableModel));
            }
        }
        
       public Electrode SelectedElectrodeItem
        {
            get => selectedElectrodeItem;
            set
            {
                selectedElectrodeItem = value;
                OnPropertyChanged(nameof(SelectedElectrodeItem));
            }
        }

       public int SelectedElectrodeChannelNumber
        {
            get => selectedElectrodeChannelNumber;
            set
            {
                selectedElectrodeChannelNumber = value;
                OnPropertyChanged(nameof(SelectedElectrodeChannelNumber));
                if (NewCurrentElectrodeChannelAction != null)
                    NewCurrentElectrodeChannelAction();
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

        public static event Action NewCurrentElectrodeChannelAction;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}