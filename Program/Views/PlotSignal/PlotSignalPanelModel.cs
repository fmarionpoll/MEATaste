using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using ScottPlot;

namespace MEATaste.Views.PlotSignal
{
    public class PlotSignalPanelModel : INotifyPropertyChanged
    {
        private WpfPlot plotControl;
        private AxisLimits axisLimitsForDataPlot;
        private bool plotDataForSelectedElectrode;

        public WpfPlot PlotControl
        {
            get => plotControl;
            set
            {
                plotControl = value;
                OnPropertyChanged(nameof(PlotControl));
            }
        }

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
