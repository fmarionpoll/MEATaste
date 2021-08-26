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
        }

        public void FillTable()
        {
            Model.ElectrodesTableModel = new ObservableCollection<Electrode>();
            Model.XYPlotDataModel = new PlotModel {Title = "Electrodes position (µm)"};
            var scatterSeries = new ScatterSeries {MarkerType = MarkerType.Circle};
            int pointsize = 5;
            int colorValue = 128;
            foreach (Electrode electrode in state.CurrentMeaExperiment.Descriptors.Electrodes)
            {
                Model.ElectrodesTableModel.Add(electrode);
                var point = new ScatterPoint(electrode.XCoordinate, electrode.YCoordinate, pointsize, colorValue);
                scatterSeries.Points.Add(point);
            }

            Model.XYPlotDataModel.Series.Add(scatterSeries);
            Model.XYPlotDataModel.Axes.Add(new LinearColorAxis
                {Position = AxisPosition.Right, Palette = OxyPalettes.Jet(200)});
            Model.XYPlotDataModel.InvalidatePlot(true);
        }

        public void SelectedRow(int selectedRowIndex)
        {
            Model.SelectedElectrodeIndex = selectedRowIndex;
        }

        public int GetElectrodeChannel(int index) =>
            state.CurrentMeaExperiment.Descriptors.Electrodes[index].ChannelNumber;
    }
}