using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;

namespace MEATaste.Views.OneElectrode
{
    public class OneElectrodePanelModel : INotifyPropertyChanged
    {
        private ScottPlot.WpfPlot dataPlot;
        public ScottPlot.WpfPlot DataPlot
        {
            get => dataPlot;
            set
            {
                dataPlot = value;
                OnPropertyChanged(nameof(DataPlot));
            }
        }

        
        public OneElectrodePanelModel()
        {
            DataPlot = new ScottPlot.WpfPlot();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
