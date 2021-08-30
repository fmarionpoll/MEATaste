using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using ScottPlot;

namespace MEATaste.Views.OneElectrode
{
    public class OneElectrodePanelModel : INotifyPropertyChanged
    {
        private WpfPlot dataPlot;
        private AxisLimits axisLimitsForDataPlot;
        private bool authorizeReadingNewFile;

        public WpfPlot DataPlot
        {
            get => dataPlot;
            set
            {
                dataPlot = value;
                OnPropertyChanged(nameof(DataPlot));
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

        public OneElectrodePanelModel()
        {
            DataPlot = new WpfPlot();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
