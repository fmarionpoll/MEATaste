
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
        private ElectrodeProperties selectedElectrodeProperties;
        private int selectedFilterIndex = 0;

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
        
        public ElectrodeProperties SelectedElectrodeProperties
        {
            get => selectedElectrodeProperties;
            set
            {
                if (selectedElectrodeProperties == value) return;
                selectedElectrodeProperties = value;
                OnPropertyChanged(nameof(SelectedElectrodeProperties));
            }
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

        public PlotFilteredPanelModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
