
using System;
using System.Threading.Channels;
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
            var selectedElectrode = state.CurrentElectrode.Get();
            var plotModel = Model.ScatterPlotModel;

            if (selectedElectrode == null)
                SuppressSelectedPoint(plotModel);
            else 
            {
                SetSelectedPoint(plotModel, selectedElectrode);
                CenterPlotOnElectrode(plotModel, selectedElectrode);
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

            foreach (var item in state.MeaExperiment.Get().Electrodes)
            {
                var point = new ScatterPoint(item.Electrode.XuM, item.Electrode.YuM);
                series.Points.Add(point);
            }

            plotModel.Series.Add(series);
        }
        
        private void CenterPlotOnElectrode(PlotModel plotModel, ElectrodeProperties electrodeProperties)
        {
            var xAxis = plotModel.Axes[0];
            var yAxis = plotModel.Axes[1];
            xAxis.Reset();
            yAxis.Reset();

            const int deltaX = 150;
            xAxis.Minimum = electrodeProperties.XuM - deltaX;
            xAxis.Maximum = electrodeProperties.XuM + deltaX;

            var deltaY = deltaX; 
            yAxis.Minimum = electrodeProperties.YuM - deltaY;
            yAxis.Maximum = electrodeProperties.YuM + deltaY;
        }

        private void SuppressSelectedPoint(PlotModel plotModel)
        {
            if ( plotModel.Series.Count > 1) 
                plotModel.Series.RemoveAt(1);
        }

        private void SetSelectedPoint(PlotModel plotModel, ElectrodeProperties electrodeProperties)
        {
            if (plotModel.Series.Count < 2)
                AddSelectedSeries(plotModel);
            if (plotModel.Series.Count < 2)
                return;
            var series = (ScatterSeries) plotModel.Series[1];
            series.Points.RemoveAt(0);
            var point = new ScatterPoint(electrodeProperties.XuM, electrodeProperties.YuM);
            series.Points.Add(point);

        }

        private void AddSelectedSeries(PlotModel plotModel)
        {
            var series = new ScatterSeries
            {
                SelectionMode = SelectionMode.Single,
                MarkerType = MarkerType.Circle,
                MarkerStroke = OxyColors.Red,
                MarkerFill = OxyColors.Transparent
            };
            var point = new ScatterPoint(0, 0);
            series.Points.Add(point);
            plotModel.Series.Add(series);
        }

        private void PlotModel_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (e.HitTestResult == null)
                return;
            var indexOfNearestPoint = (int)Math.Round(e.HitTestResult.Index);
            var currentExperiment = state.MeaExperiment.Get();
            var selectedElectrode = currentExperiment.Electrodes[indexOfNearestPoint];
            SelectElectrode(selectedElectrode.Electrode);
        }

        public void SelectElectrode(ElectrodeProperties electrodeProperties)
        {
            if (state.CurrentElectrode.Get() != null &&
                electrodeProperties.ElectrodeNumber == state.CurrentElectrode.Get().ElectrodeNumber) return;
            state.CurrentElectrode.Set(electrodeProperties);
        }

    }
}
