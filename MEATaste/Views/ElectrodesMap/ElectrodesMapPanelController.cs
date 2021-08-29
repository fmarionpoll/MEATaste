﻿
using System.Diagnostics;
using System.Linq;
using System.Windows;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace MEATaste.Views.ElectrodesMap
{
    public class ElectrodesMapPanelController
    {
        public ElectrodesMapPanelModel Model { get; }

        private readonly ApplicationState state;

        public ElectrodesMapPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;

            Model = new ElectrodesMapPanelModel();

            eventSubscriber.Subscribe(EventType.CurrentExperimentChanged, PlotElectrodesMap);
            eventSubscriber.Subscribe(EventType.SelectedElectrodeChanged, ChangeSelectedElectrode);
        }

        private void PlotElectrodesMap()
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

        private void ChangeSelectedElectrode()
        {
            var selectedElectode = state.SelectedElectrode.Get();
            var plotModel = Model.ScatterPlotModel;

            if (selectedElectode == null)
                SuppressSelectedPoint(plotModel);
            else
            {
                SetSelectedPoint(plotModel, selectedElectode);
                CenterPlotOnElectrode(plotModel, selectedElectode);
            }

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
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColors.LightBlue
            };

            foreach (var electrode in state.CurrentMeaExperiment.Get().Descriptors.Electrodes)
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

            const int deltaX = 150;
            xAxis.Minimum = electrode.XCoordinate - deltaX;
            xAxis.Maximum = electrode.XCoordinate + deltaX;

            var deltaY = deltaX; // * plotModel.Height / plotModel.Width;
            yAxis.Minimum = electrode.YCoordinate - deltaY;
            yAxis.Maximum = electrode.YCoordinate + deltaY;
        }

        private void SuppressSelectedPoint(PlotModel plotModel)
        {
            if ( plotModel.Series.Count > 1) 
                plotModel.Series.RemoveAt(1);
        }

        private void SetSelectedPoint(PlotModel plotModel, Electrode electrode)
        {
            if (plotModel.Series.Count < 2)
                AddSelectedSeries(plotModel);
            if (plotModel.Series.Count < 2)
                return;
            var series = (ScatterSeries) plotModel.Series[1];
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

        private void PlotModel_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            var plotModel = Model.ScatterPlotModel;
            var axisList = plotModel.Axes;

            var xAxis = axisList.FirstOrDefault(ax => ax.Position == AxisPosition.Bottom);
            var yAxis = axisList.FirstOrDefault(ax => ax.Position == AxisPosition.Left);

            var dataPointp = Axis.InverseTransform(e.Position, xAxis, yAxis);
            Trace.WriteLine($"coord ={dataPointp}");

            var currentExperiment = state.CurrentMeaExperiment.Get();
            var selectedElectrode = currentExperiment.Descriptors.Electrodes.FirstOrDefault(x => x.XCoordinate == dataPointp.X && x.YCoordinate == dataPointp.Y);
            Trace.WriteLine($"selected electrode is ={selectedElectrode}");

            state.SelectedElectrode.Set(selectedElectrode);
        }

    }
}
