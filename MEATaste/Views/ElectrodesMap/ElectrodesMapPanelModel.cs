using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using MEATaste.Annotations;
using OxyPlot;
using OxyPlot.Series;

namespace MEATaste.Views.ElectrodesMap
{
    public class ElectrodesMapPanelModel : INotifyPropertyChanged
    {
        private PlotModel scatterPlotModel;

        public PlotModel ScatterPlotModel
        {
            get => scatterPlotModel;
            set
            {
                if (scatterPlotModel == value) return;
                scatterPlotModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScatterPlotModel)));
            }
        }

        public ElectrodesMapPanelModel()
        {
            ScatterPlotModel = new PlotModel();
            ScatterPlotModel.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Bottom });
            ScatterPlotModel.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Left, Maximum = 10, Minimum = 0 });
        }

       

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
