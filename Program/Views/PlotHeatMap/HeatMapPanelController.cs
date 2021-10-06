using System.Collections.Generic;
using System.Windows;
using MEATaste.Infrastructure;
using ScottPlot;
using ScottPlot.Drawing;

namespace MEATaste.Views.PlotHeatMap
{
    class HeatMapPanelController
    {

        public HeatMapPanelModel Model { get; }
        private readonly ApplicationState state;
        private List<int> selectedChannels;

        public HeatMapPanelController(
            ApplicationState state,
            IEventSubscriber eventSubscriber)
        {
            this.state = state;
            
            Model = new HeatMapPanelModel();
            eventSubscriber.Subscribe(EventType.CurrentExperimentChanged, PlotElectrodesMap);
            eventSubscriber.Subscribe(EventType.SelectedChannelsChanged, ChangeSelectedElectrode);
        }

        private void PlotElectrodesMap()
        {
            var plot = Model.PlotControl.Plot;
            plot.Clear();

            var (xMin, xMax, yMin, yMax) = GetElectrodeLimits();
            var intensities = GetNbSpikes(xMin, xMax, yMin, yMax, 10);
            
            var hmc = plot.AddHeatmapCoordinated(intensities, xMin, xMax, yMin, yMax, Colormap.Turbo);
            var cb = plot.AddColorbar(hmc);
            plot.Render();
            Application.Current.Dispatcher.Invoke(() => { Model.PlotControl.Render(); });
        }

        private void ChangeSelectedElectrode()
        {
            selectedChannels = state.ListSelectedChannels.Get();
        }

        public void AttachControlToModel(WpfPlot wpfControl)
        {
            Model.PlotControl = wpfControl;
        }

        private (double, double, double, double) GetElectrodeLimits()
        {
            var meaExp = state.MeaExperiment.Get();
            var electroData0 = meaExp.Electrodes[0];
            var xMin = electroData0.Electrode.XuM;
            var xMax = xMin; 
            var yMin = electroData0.Electrode.YuM;
            var yMax = yMin;

            foreach (var electrode in meaExp.Electrodes)
            {
                if (xMin > electrode.Electrode.XuM) xMin = electrode.Electrode.XuM;
                if (yMin > electrode.Electrode.YuM) yMin = electrode.Electrode.YuM;
                if (xMax < electrode.Electrode.XuM) xMax = electrode.Electrode.XuM;
                if (yMax < electrode.Electrode.YuM) yMax = electrode.Electrode.YuM;
            }

            return (xMin, xMax, yMin, yMax);
        }

        private double[,] GetNbSpikes(double xMin, double xMax, double yMin, double yMax, double step)
        {
            var xitems = (int)((xMax - xMin) / step) + 1;
            var yitems = (int)((yMax - yMin) / step) + 1;
            var intensities = new double[xitems, yitems];
            var meaExp = state.MeaExperiment.Get();

            foreach (var electrode in meaExp.Electrodes)
            {
                var i = (int) ((electrode.Electrode.XuM - xMin) / step);
                var j = (int)((electrode.Electrode.YuM - yMin) / step);
                intensities[i, j] = 10;
                if (electrode.SpikeTimes != null) 
                    intensities[i, j] = electrode.SpikeTimes.Count + 10;
            }

            return intensities;
        }
    }
}
