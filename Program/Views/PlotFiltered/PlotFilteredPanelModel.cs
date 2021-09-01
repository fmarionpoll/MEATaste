
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using MEATaste.DataMEA.Models;
using ScottPlot;

namespace MEATaste.Views.PlotFiltered
{
    public class PlotFilteredPanelModel : INotifyPropertyChanged
    {
        private AxisLimits axisLimitsForDataPlot;
        private bool plotFilteredData;
        private WpfPlot plotControl;
        private ElectrodeRecord selectedElectrodeRecord;

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

        public ElectrodeRecord SelectedElectrodeRecord
        {
            get => selectedElectrodeRecord;
            set
            {
                if (selectedElectrodeRecord == value) return;
                selectedElectrodeRecord = value;
                OnPropertyChanged(nameof(SelectedElectrodeRecord));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
