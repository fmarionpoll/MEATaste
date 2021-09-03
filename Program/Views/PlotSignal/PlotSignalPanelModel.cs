using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using MEATaste.DataMEA.Models;
using ScottPlot;

namespace MEATaste.Views.PlotSignal
{
    public class PlotSignalPanelModel : INotifyPropertyChanged
    {
        private AxisLimits axisLimitsForDataPlot;
        private bool plotDataForSelectedElectrode;
        private ElectrodeRecord selectedElectrodeRecord;

        public AxisLimits AxisLimitsForDataPlot
        {
            get => axisLimitsForDataPlot;
            set
            {
                if (axisLimitsForDataPlot.XMin == value.XMin
                && AxisLimitsForDataPlot.XMax == value.XMax
                && AxisLimitsForDataPlot.YMin == value.YMin
                && AxisLimitsForDataPlot.YMax == value.YMax
                ) return;
                axisLimitsForDataPlot = value;
                OnPropertyChanged(nameof(AxisLimitsForDataPlot));
            }
        }

        public bool PlotDataForSelectedElectrode
        {
            get => plotDataForSelectedElectrode;
            set
            {
                if (plotDataForSelectedElectrode == value) return;
                plotDataForSelectedElectrode = value;
                OnPropertyChanged(nameof(PlotDataForSelectedElectrode));
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
        
        public PlotSignalPanelModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
