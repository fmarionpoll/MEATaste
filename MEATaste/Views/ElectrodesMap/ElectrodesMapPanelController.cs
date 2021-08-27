
using System.Diagnostics;
using MEATaste.DataMEA.MaxWell;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;
using MEATaste.Views.ElectrodesList;
using MEATaste.Views.FileOpen;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace MEATaste.Views.ElectrodesMap
{
    public class ElectrodesMapPanelController
    {
        public ElectrodesMapPanelModel Model { get; }

        private readonly MeaFileReader meaFileReader;
        private readonly ApplicationState state;

        public ElectrodesMapPanelController(MeaFileReader meaFileReader, ApplicationState state)
        {
            this.meaFileReader = meaFileReader;
            this.state = state;

            Model = new ElectrodesMapPanelModel();
            FileOpenPanelModel.NewFileIsLoadedAction += PlotLoadData;
            ElectrodesListPanelModel.SelectedElectrodeChannelChanged += CurrentIndexHasChanged;
        }

        public void PlotLoadData()
        {
            Model.ScatterPlotModel = new PlotModel
            {
                SelectionColor = OxyColors.Red
            }; 
            var plotModel = Model.ScatterPlotModel;
            PlotAddAxes(plotModel);
            PlotAddSeries(plotModel);
            plotModel.InvalidatePlot(true);
        }

        private void PlotAddAxes(PlotModel plotModel)
        {
            var xAxis = new LinearAxis { Title = "x (µm)", Position = AxisPosition.Bottom };
            plotModel.Axes.Add(xAxis);

            var yAxis = new LinearAxis { Title = "y (µm)", Position = AxisPosition.Left };
            plotModel.Axes.Add(yAxis);
        }

        private void PlotAddSeries(PlotModel plotModel)
        {
            var series = new ScatterSeries
            {
                SelectionMode = SelectionMode.Single,
                MarkerType = MarkerType.Circle
            };

            foreach (var electrode in state.CurrentMeaExperiment.Descriptors.Electrodes)
            {
                var point = new ScatterPoint(electrode.XCoordinate, electrode.YCoordinate);
                series.Points.Add(point);
            }
            plotModel.Series.Add(series);
        }

        private void CurrentIndexHasChanged()
        {
            int indexSelected = state.CurrentMeaExperiment.CurrentElectrodesIndex;
            if (indexSelected >= 0)
            {
                Electrode electrode = state.CurrentMeaExperiment.Descriptors.Electrodes[indexSelected];
                Trace.WriteLine($"Map: electrode = {electrode}");
            }
            else
            {
                Trace.WriteLine($"Map: selected index number has changed ={indexSelected}");
            }

            int nbseries = Model.ScatterPlotModel.Series.Count;
            if (nbseries < 2)
            {
                // create new series with color red
                ScatterSeries series = (ScatterSeries)Model.ScatterPlotModel.Series[0];
            }
            
           
        }

       
        
    }
}
