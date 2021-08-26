using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using MEATaste.DataMEA.Models;
using OxyPlot;



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

        private int selectedElectrodeIndex;
        public int SelectedElectrodeIndex
        {
            get => selectedElectrodeIndex;

            set
            {
                selectedElectrodeIndex = value;
                OnPropertyChanged(nameof(SelectedElectrodeIndex));
            }
        }

        private Electrode selectedElectrode;
        public Electrode SelectedElectrode
        {
            get => selectedElectrode;
            set
            {
                selectedElectrode = value;
                OnPropertyChanged(nameof(SelectedElectrode));
            }
        }

        private PlotModel xyPlotDataModel;

        public PlotModel XYPlotDataModel
        {
            get => xyPlotDataModel;
            set
            {
                if (xyPlotDataModel == value) return;
                xyPlotDataModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(XYPlotDataModel)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}