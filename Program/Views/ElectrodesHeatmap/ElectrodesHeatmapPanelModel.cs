using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using ScottPlot;

namespace MEATaste.Views.ElectrodesHeatmap
{
    public class ElectrodesHeatmapPanelModel : INotifyPropertyChanged
    {

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

        public ElectrodesHeatmapPanelModel()
        {
            PlotControl = new WpfPlot();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
