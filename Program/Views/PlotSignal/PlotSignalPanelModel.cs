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

        public WpfPlot PlotControl { get; set; }

        public AxisLimits AxisLimitsForDataPlot
        {
            get => axisLimitsForDataPlot;
            set
            {
                axisLimitsForDataPlot = value;
                OnPropertyChanged(nameof(AxisLimitsForDataPlot));
            }
        }

        public bool PlotDataForSelectedElectrode
        {
            get => plotDataForSelectedElectrode;
            set
            {
                if (plotDataForSelectedElectrode != value)
                {
                    plotDataForSelectedElectrode = value;
                    OnPropertyChanged(nameof(PlotDataForSelectedElectrode));
                }
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
