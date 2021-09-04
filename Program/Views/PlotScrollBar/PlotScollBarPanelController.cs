using System;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;


namespace MEATaste.Views.PlotScrollBar
{
    public class PlotScrollBarPanelController
    {
        public PlotScrollBarPanelModel Model { get; }

        private readonly ApplicationState state;
        private double fileDuration;
        public double Delta;


        public PlotScrollBarPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;

            Model = new PlotScrollBarPanelModel();
            eventSubscriber.Subscribe(EventType.AxesMaxMinChanged, AxesChanged);
            eventSubscriber.Subscribe(EventType.ElectrodeRecordLoaded, FileHasChanged);
        }

        private void AxesChanged()
        {
            var axesMaxMin = state.AxesMaxMin.Get();
            if (axesMaxMin == null) return;

            Model.XFirst = axesMaxMin.XMin.ToString("0.###");
            Model.XLast = axesMaxMin.XMax.ToString("0.###");
            Model.ViewPortWidth = axesMaxMin.XMax - axesMaxMin.XMin;
            Model.ScrollPosition = (axesMaxMin.XMax + axesMaxMin.XMin)/2;
        }

        private void FileHasChanged()
        {
            MeaExperiment meaExperiment = state.CurrentMeaExperiment.Get();
            if (meaExperiment.RawSignalDouble != null)
            {
                fileDuration = meaExperiment.RawSignalDouble.Length / meaExperiment.Descriptors.SamplingRate;
                Model.ScrollableMinimum = 0;
                Model.ScrollableMaximum = fileDuration;
            }
                
        }

        public void UpdateXAxisLimitsFromModelValues()
        {
            var xLast = Convert.ToDouble(Model.XLast);
            var xFirst = Convert.ToDouble(Model.XFirst);
            var axesMaxMin = state.AxesMaxMin.Get();
            Delta = xLast - xFirst;
            state.AxesMaxMin.Set(new AxesExtrema(xFirst, xLast, axesMaxMin.YMin, axesMaxMin.YMax));
        }

        public void LeftLeftClick()
        {
            var axesMaxMin = state.AxesMaxMin.Get();
            Delta = axesMaxMin.XMax - axesMaxMin.XMin;
            MoveXAxis(-Delta);
        }

        public void RightRightClick()
        {
            var axesMaxMin = state.AxesMaxMin.Get();
            Delta = axesMaxMin.XMax - axesMaxMin.XMin;
            MoveXAxis(Delta);
        }

        public void MoveXAxis(double delta)
        {
            var axesMaxMin = state.AxesMaxMin.Get();
            if (axesMaxMin.XMin + delta < 0)
            {
                delta = - axesMaxMin.XMin;
            }

            if (axesMaxMin.XMax + delta > fileDuration)
            {
                delta = fileDuration - axesMaxMin.XMax;
            }
            state.AxesMaxMin.Set(new AxesExtrema(axesMaxMin.XMin + delta, axesMaxMin.XMax + delta, axesMaxMin.YMin, axesMaxMin.YMax));
        }

        public void ScrollBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            
        }
    }
}
