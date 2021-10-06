using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private List<int> selectedChannels;

        public ElectrodesMapPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;

            Model = new ElectrodesMapPanelModel();

            eventSubscriber.Subscribe(EventType.CurrentExperimentChanged, PlotElectrodesMap);
            eventSubscriber.Subscribe(EventType.SelectedChannelsChanged, ChangeSelectedElectrode);
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

            //plotModel.MouseDown += PlotModel_MouseDown;
        }

        private void ChangeSelectedElectrode()
        {
            var listSelectedChannels = state.ListSelectedChannels.Get();
            if (listSelectedChannels == null) return;

            var plotModel = Model.ScatterPlotModel;

            if (listSelectedChannels.Count == 0)
            {
                SuppressSelectedPoints(plotModel);
            }
            else 
            {
                SetSelectedPoints(plotModel, listSelectedChannels);
                CenterPlotOnElectrodes(plotModel, listSelectedChannels);
            }
            selectedChannels = state.ListSelectedChannels.Get();

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
        
        private void CenterPlotOnElectrodes(PlotModel plotModel, List<int> listSelectedChannels)
        {
            return;
            var xAxis = plotModel.Axes[0];
            var yAxis = plotModel.Axes[1];
            xAxis.Reset();
            yAxis.Reset();

            const int delta = 150;
            var maxMin = GetSelectedElectrodesArea(listSelectedChannels);

            xAxis.Minimum = maxMin[0] - delta;
            xAxis.Maximum = maxMin[1] + delta;

            yAxis.Minimum = maxMin[0] - delta;
            yAxis.Maximum = maxMin[1] + delta;
        }

        private double[] GetSelectedElectrodesArea(List<int> listSelectedChannels)
        {
            var meaExp = state.MeaExperiment.Get();
            var maxMin = new double[2];

            if (listSelectedChannels == null) return maxMin;
            var electr = meaExp.Electrodes.Single(x => x.Electrode.Channel == listSelectedChannels[0]).Electrode;
            maxMin[0] = electr.XuM;
            maxMin[1] = electr.XuM;

            foreach (var channel in listSelectedChannels)
            {
                var electrode = meaExp.Electrodes.Single(x => x.Electrode.Channel == channel).Electrode;
                if (maxMin[0] > electrode.XuM) maxMin[0] = electrode.XuM;
                if (maxMin[1] < electrode.XuM) maxMin[1] = electrode.XuM;
                if (maxMin[0] > electrode.YuM) maxMin[0] = electrode.YuM;
                if (maxMin[1] < electrode.YuM) maxMin[1] = electrode.YuM;
            }

            return maxMin;
        }

        private void SuppressSelectedPoints(PlotModel plotModel)
        {
            if ( plotModel.Series.Count > 1) 
                plotModel.Series.RemoveAt(1);
        }

        private void SetSelectedPoints(PlotModel plotModel, List<int> listSelectedChannels)
        {
            if (plotModel.Series.Count < 2)
                AddSelectedSeries(plotModel);
            if (plotModel.Series.Count < 2)
                return;
            var series = (ScatterSeries) plotModel.Series[1];
            if (series.Points.Count > 0)
                series.Points.Clear();
            var meaExp = state.MeaExperiment.Get();
            foreach (var channel in listSelectedChannels)
            {
                var electrode = meaExp.Electrodes.Single(x => x.Electrode.Channel == channel).Electrode;
                var point = new ScatterPoint(electrode.XuM, electrode.YuM);
                series.Points.Add(point);
            }
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

        //private void PlotModel_MouseDown(object sender, OxyMouseDownEventArgs e)
        //{
        //    if (e.HitTestResult == null)
        //        return;
        //    var indexOfNearestPoint = (int)Math.Round(e.HitTestResult.Index);
        //    var currentExperiment = state.MeaExperiment.Get();
        //    var selectedElectrode = currentExperiment.Electrodes[indexOfNearestPoint];
        //    SelectElectrode(selectedElectrode.Electrode);
        //}

        //public void SelectElectrode(ElectrodeProperties electrodeProperties)
        //{
        //    if (state.ListSelectedChannels.Get() != null &&
        //        electrodeProperties.ElectrodeNumber == state.ListSelectedChannels.Get().ElectrodeNumber) return;
        //    state.ListSelectedChannels.Set(electrodeProperties);
        //}

    }
}
