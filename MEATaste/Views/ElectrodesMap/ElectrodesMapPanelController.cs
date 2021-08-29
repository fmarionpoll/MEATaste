
using System.Diagnostics;
using System.Linq;
using System.Windows;
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
            FileOpenPanelModel.NewHdf5FileIsLoadedAction += PlotElectrodesMap;
            ElectrodesListPanelModel.NewCurrentElectrodeChannelAction += ChangeSelectedElectrode;
        }

        public void PlotElectrodesMap()
        {
            Model.ScatterPlotModel = new PlotModel
            {
                SelectionColor = OxyColors.Red
            }; 
            var plotModel = Model.ScatterPlotModel;
            PlotAddAxes(plotModel);
            PlotAddSeries(plotModel);
            plotModel.InvalidatePlot(true);

            plotModel.MouseDown += PlotModel_MouseDown;
        }

        public void ChangeSelectedElectrode()
        {

            int indexSelected = state.CurrentMeaExperiment.CurrentElectrodesIndex;
            Trace.WriteLine($"Map: selected index number has changed ={indexSelected}");

            var plotModel = Model.ScatterPlotModel;

            if (indexSelected < 0)
            {
                SuppressSelectedPoint(plotModel);
            }
            else
            {
                Electrode electrode = state.CurrentMeaExperiment.Descriptors.Electrodes[indexSelected];
                SetSelectedPoint(plotModel, electrode);
                CenterPlotOnElectrode(plotModel, electrode);
                Trace.WriteLine($"Map: electrode = {electrode}");
            }

            plotModel.InvalidatePlot(true);
        }

        private void PlotModel_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            var plotModel = Model.ScatterPlotModel;
            var s = plotModel.Series[0] as ScatterSeries;
            ElementCollection<Axis> axisList = plotModel.Axes;

            Axis xAxis = axisList.FirstOrDefault(ax => ax.Position == AxisPosition.Bottom);
            Axis yAxis = axisList.FirstOrDefault(ax => ax.Position == AxisPosition.Left);

            DataPoint dataPointp = Axis.InverseTransform(e.Position, xAxis, yAxis);
            Trace.WriteLine($"coord ={dataPointp}");
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
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColors.LightBlue
            };

            foreach (var electrode in state.CurrentMeaExperiment.Descriptors.Electrodes)
            {
                var point = new ScatterPoint(electrode.XCoordinate, electrode.YCoordinate);
                series.Points.Add(point);
            }
            plotModel.Series.Add(series);
        }
        
        private void CenterPlotOnElectrode(PlotModel plotModel, Electrode electrode)
        {
            var xAxis = plotModel.Axes[0];
            var yAxis = plotModel.Axes[1];
            xAxis.Reset();
            yAxis.Reset();

            var deltax = 150;
            xAxis.Minimum = electrode.XCoordinate - deltax;
            xAxis.Maximum = electrode.XCoordinate + deltax;

            var deltay = deltax * plotModel.Height / plotModel.Width;
            yAxis.Minimum = electrode.YCoordinate - deltay;
            yAxis.Maximum = electrode.YCoordinate + deltay;
        }

        private void SuppressSelectedPoint(PlotModel plotModel)
        {
            if ( plotModel.Series.Count > 1)
            {
                plotModel.Series.RemoveAt(1);
            }
        }

        private void SetSelectedPoint(PlotModel plotModel, Electrode electrode)
        {
            if (plotModel.Series.Count < 2)
                AddSelectedSeries(plotModel);
            if (plotModel.Series.Count < 2)
                return;
            ScatterSeries series = (ScatterSeries) plotModel.Series[1];
            series.Points.RemoveAt(0);
            var point = new ScatterPoint(electrode.XCoordinate, electrode.YCoordinate);
            series.Points.Add(point);

        }

        private void AddSelectedSeries(PlotModel plotModel)
        {
            var series = new ScatterSeries
            {
                SelectionMode = SelectionMode.Single,
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColors.Red
            };
            var point = new ScatterPoint(0, 0);
            series.Points.Add(point);
            plotModel.Series.Add(series);
        }

        public void MouseDown(Point p)
        {

        }
        
    }
}
