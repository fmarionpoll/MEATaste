using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using ScottPlot;

namespace MEATaste.Views.PlotSignal
{
    public class PlotSignalPanelModel : INotifyPropertyChanged
    {
        private AxisLimits axisLimitsForDataPlot;
        private bool plotDataForSelectedElectrode;
        private WpfPlot plotControl;

        public WpfPlot PlotControl
        {
            get => plotControl;
            set
            {
                if (plotControl == value) return;
                plotControl = value;
                OnPropertyChanged(nameof(PlotControl));
            }
        }

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

        public PlotSignalPanelModel()
        {
            PlotControl = new WpfPlot();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
