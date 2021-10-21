using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using ScottPlot;

namespace MEATaste.Views.PlotSignal
{
    public class PlotSignalPanelModel : INotifyPropertyChanged
    {
        private string acquisitionSettingsLabel = string.Empty;
        private WpfPlot plotControl;
        private int id;


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

        public int Id
        {
            get => id;
            set
            {
                if (id == value) return;
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public PlotSignalPanelModel()
        {
            plotControl = new WpfPlot();
        }

        public string AcquisitionSettingsLabel
        {
            get => acquisitionSettingsLabel;
            set
            {
                if (acquisitionSettingsLabel == value) return;
                acquisitionSettingsLabel = value;
                OnPropertyChanged(nameof(AcquisitionSettingsLabel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
