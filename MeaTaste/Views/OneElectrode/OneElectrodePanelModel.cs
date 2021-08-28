using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;

namespace MEATaste.Views.OneElectrode
{
    public class OneElectrodePanelModel : INotifyPropertyChanged
    {
        private ushort[] rawSignalFromOneElectrode;
        public ushort[] RawSignalFromOneElectrode
        {
            get => rawSignalFromOneElectrode;
            set
            {
                rawSignalFromOneElectrode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RawSignalFromOneElectrode)));
            }
        }

        private ScottPlot.WpfPlot formsPlots;
        public ScottPlot.WpfPlot FormsPlots
        {
            get => formsPlots;
            set
            {
                formsPlots = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FormsPlots)));
            }
        }

        
        public OneElectrodePanelModel()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
