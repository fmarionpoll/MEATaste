using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using ScottPlot;

namespace MEATaste.Views.PlotSignal
{
    public class PlotSignalPanelModel : INotifyPropertyChanged
    {
        private int selectedFilterIndex = 0;
        private string acquisitionSettingsLabel = string.Empty;
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

        public PlotSignalPanelModel()
        {
            PlotControl = new WpfPlot();
        }

        public int SelectedFilterIndex
        {
            get => selectedFilterIndex;
            set
            {
                if (selectedFilterIndex == value) return;
                selectedFilterIndex = value;
                OnPropertyChanged(nameof(SelectedFilterIndex));
            }
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
