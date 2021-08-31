
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using ScottPlot;

namespace MEATaste.Views.PlotFiltered
{
    public class PlotFilteredPanelModel : INotifyPropertyChanged
    {
        private AxisLimits axisLimitsForDataPlot;
        private bool plotFilteredData;

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

        public bool PlotFilteredData
        {
            get => plotFilteredData;
            set
            {
                plotFilteredData = value;
                OnPropertyChanged(nameof(PlotFilteredData));
            }
        }

        public PlotFilteredPanelModel()
        {
            PlotControl = new WpfPlot();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
