using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using MEATaste.DataMEA.Models;
using OxyPlot;

namespace MEATaste.Views.ElectrodesMap
{
    public class ElectrodesMapPanelModel : INotifyPropertyChanged
    {
        private PlotModel scatterPlotModel;

        public PlotModel ScatterPlotModel
        {
            get => scatterPlotModel;
            set
            {
                if (scatterPlotModel == value) return;
                scatterPlotModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScatterPlotModel)));
            }
        }

        public ElectrodesMapPanelModel()
        {
            ScatterPlotModel = new PlotModel();
        }

        //private int selectedElectrodeChannelNumber;
        //public int SelectedElectrodeChannelNumber
        //{
        //    get => selectedElectrodeChannelNumber;
        //    set
        //    {
        //        selectedElectrodeChannelNumber = value;
        //        OnPropertyChanged(nameof(SelectedElectrodeChannelNumber));
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
