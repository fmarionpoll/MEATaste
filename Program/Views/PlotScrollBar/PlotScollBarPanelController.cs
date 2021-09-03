using System;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;


namespace MEATaste.Views.PlotScrollBar
{
    public class PlotScrollBarPanelController
    {
        public PlotScrollBarPanelModel Model { get; }

        private readonly ApplicationState state;
   


        public PlotScrollBarPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;

            Model = new PlotScrollBarPanelModel();
            eventSubscriber.Subscribe(EventType.AxesMaxMinChanged, AxesChanged);
        }

        private void AxesChanged()
        {
            var axesMaxMin = state.AxesMaxMin.Get();
            if (axesMaxMin == null) return;

            Model.XFirst = axesMaxMin.XMin.ToString("0.###");
            Model.XLast = axesMaxMin.XMax.ToString("0.###");
        }

        public void UpdateXAxisLimitsFromModelValues()
        {
            var xLast = Convert.ToDouble(Model.XLast);
            var xFirst = Convert.ToDouble(Model.XFirst);
            var axesMaxMin = state.AxesMaxMin.Get();
            state.AxesMaxMin.Set(new AxesExtrema(xFirst, xLast, axesMaxMin.YMin, axesMaxMin.YMax));
        }

        public void LeftLeftClick()
        {
            var axesMaxMin = state.AxesMaxMin.Get();
            var delta = axesMaxMin.XMax - axesMaxMin.XMin;
            MoveXAxis(-delta);
        }

        public void RightRightClick()
        {
            var axesMaxMin = state.AxesMaxMin.Get();
            var delta = axesMaxMin.XMax - axesMaxMin.XMin;
            MoveXAxis(delta);
        }

        public void MoveXAxis(double delta)
        {
            var axesMaxMin = state.AxesMaxMin.Get();
            state.AxesMaxMin.Set(new AxesExtrema(axesMaxMin.XMin + delta, axesMaxMin.XMax + delta, axesMaxMin.YMin, axesMaxMin.YMax));
        }
    }
}
