
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using ScottPlot;

namespace MEATaste.Views.PlotFiltered
{
    public class PlotFilteredPanelModel : INotifyPropertyChanged
    {
        private WpfPlot plotControl;
        private AxisLimits axisLimitsForDataPlot;
        private bool authorizeReadingNewFile;

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

        public bool AuthorizeReadingNewFile
        {
            get => authorizeReadingNewFile;
            set
            {
                authorizeReadingNewFile = value;
                OnPropertyChanged(nameof(AuthorizeReadingNewFile));
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
