using System.Collections.ObjectModel;
using MEATaste.DataMEA.MaxWell;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;
using MEATaste.Views.FileOpen;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;



namespace MEATaste.Views.ElectrodesList
{
    public class ElectrodesListPanelController
    {
        public ElectrodesListPanelModel Model { get; }

        private readonly MeaFileReader meaFileReader;
        private readonly ApplicationState state;

        public ElectrodesListPanelController(MeaFileReader meaFileReader, ApplicationState state)
        {
            this.meaFileReader = meaFileReader;
            this.state = state;

            Model = new ElectrodesListPanelModel();
            FileOpenPanelModel.NewFileIsLoadedAction += FillTable;
            FileOpenPanelModel.NewFileIsLoadedAction += FillMap;
        }

        public void FillTable()
        {
            Model.ElectrodesTableModel = new ObservableCollection<Electrode>();
            foreach (Electrode electrode in state.CurrentMeaExperiment.Descriptors.Electrodes)
            {
                Model.ElectrodesTableModel.Add(electrode);
            }
        }

        public void FillMap()
        {
            Model.ScatterPlotModel = new PlotModel { Title = "Electrodes position (µm)" };
            PlotModel plotModel = Model.ScatterPlotModel;
            AddAxes(plotModel);
            AddSeries(plotModel);
            plotModel.InvalidatePlot(true);
        }

        private void AddAxes(PlotModel plotModel)
        {
            var xAxis = new LinearAxis { Title = "x (µm)", Position = AxisPosition.Bottom };
            plotModel.Axes.Add(xAxis);

            var yAxis = new LinearAxis { Title = "y (µm)", Position = AxisPosition.Left };
            plotModel.Axes.Add(yAxis);
        }

        private void AddSeries(PlotModel plotModel)
        {
            var series = new ScatterSeries { MarkerType = MarkerType.Circle };
            foreach (Electrode electrode in state.CurrentMeaExperiment.Descriptors.Electrodes)
            {
                var point = new ScatterPoint(electrode.XCoordinate, electrode.YCoordinate);
                series.Points.Add(point);
            }
            Model.ScatterPlotModel.Series.Add(series);
        }

        public void SelectedRow(int selectedRowIndex)
        {
            Model.SelectedElectrodeIndex = selectedRowIndex;
        }

        public int GetElectrodeChannel(int index) =>
            state.CurrentMeaExperiment.Descriptors.Electrodes[index].ChannelNumber;
    }
}