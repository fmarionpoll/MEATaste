using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using MEATaste.DataMEA.Models;




namespace MEATaste.Views.ElectrodesList
{
    public class ElectrodesListPanelModel : INotifyPropertyChanged
    {
        private ObservableCollection<Electrode> electrodesTableModel;
        public ObservableCollection<Electrode> ElectrodesTableModel
        {
            get => electrodesTableModel;
            set
            {
                if (electrodesTableModel == value) return;
                electrodesTableModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ElectrodesTableModel)));
            }
        }
        
        private Electrode selectedElectrodeItem;
        public Electrode SelectedElectrodeItem
        {
            get => selectedElectrodeItem;
            set
            {
                selectedElectrodeItem = value;
                OnPropertyChanged(nameof(SelectedElectrodeItem));
            }
        }

        private int selectedElectrodeChannelNumber;
        public int SelectedElectrodeChannelNumber
        {
            get => selectedElectrodeChannelNumber;
            set
            {
                selectedElectrodeChannelNumber = value;
                OnPropertyChanged(nameof(SelectedElectrodeChannelNumber));
                SelectedElectrodeChannelChanged?.Invoke();
            }
        }

        public static event Action SelectedElectrodeChannelChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}