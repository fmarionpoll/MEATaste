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
        private bool checkDisplayDataSelectedElectrode;

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

        public bool CheckDisplayDataSlectedElectrode
        {
            get => checkDisplayDataSelectedElectrode;
            set
            {
                checkDisplayDataSelectedElectrode = value;
                OnPropertyChanged(nameof(CheckDisplayDataSlectedElectrode));
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
