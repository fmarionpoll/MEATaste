using System.Collections.Generic;
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

            eventSubscriber.Subscribe(EventType.MeaExperimentChanged, PlotElectrodesMap);
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
            selectedChannels = state.DataSelected.Get().Channels.Keys.ToList();
            var plotModel = Model.ScatterPlotModel;

            if (selectedChannels.Count == 0)
            {
                SuppressSelectedPoints(plotModel);
            }
            else 
            {
                SetSelectedPoints(plotModel, selectedChannels);
                CenterPlotOnElectrodes(plotModel, selectedChannels);
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
        
        private void CenterPlotOnElectrodes(PlotModel plotModel, List<int> listSelectedChannels)
        {
            if (listSelectedChannels == null) return;

            var maxMin = GetSelectedElectrodesArea(listSelectedChannels); 
            
            var xAxis = plotModel.Axes[0];
            var yAxis = plotModel.Axes[1];
            xAxis.Reset();
            yAxis.Reset();

            const int delta = 150;

            xAxis.Minimum = maxMin[0] - delta;
            xAxis.Maximum = maxMin[1] + delta;

            yAxis.Minimum = maxMin[2] - delta;
            yAxis.Maximum = maxMin[3] + delta;
        }

        private double[] GetSelectedElectrodesArea(List<int> listSelectedChannels)
        {
            var meaExp = state.MeaExperiment.Get();
            var maxMin = new double[4];

            var electr = meaExp.Electrodes.Single(x => x.Electrode.Channel == listSelectedChannels[0]).Electrode;
            maxMin[0] = electr.XuM;
            maxMin[1] = electr.XuM;
            maxMin[2] = electr.YuM;
            maxMin[3] = electr.YuM;

            foreach (var channel in listSelectedChannels)
            {
                var electrode = meaExp.Electrodes.Single(x => x.Electrode.Channel == channel).Electrode;
                if (maxMin[0] > electrode.XuM) maxMin[0] = electrode.XuM;
                if (maxMin[1] < electrode.XuM) maxMin[1] = electrode.XuM;
                if (maxMin[2] > electrode.YuM) maxMin[2] = electrode.YuM;
                if (maxMin[3] < electrode.YuM) maxMin[3] = electrode.YuM;
            }

            var delta = maxMin[1] - maxMin[0];
            var deltaY = maxMin[2] - maxMin[3];
            if (delta < deltaY) delta = deltaY;
            maxMin[1] = maxMin[0] + delta;
            maxMin[3] = maxMin[2] + delta;

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
