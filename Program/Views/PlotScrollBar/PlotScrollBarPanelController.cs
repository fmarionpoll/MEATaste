using System;
using System.Linq;
using System.Windows.Xps;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;


namespace MEATaste.Views.PlotScrollBar
{
    public class PlotScrollBarPanelController
    {
        public PlotScrollBarPanelModel Model { get; }
        private readonly ApplicationState state;
        private double fileDuration;


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

            Model.ScrollViewPortSize = axesMaxMin.XMax - axesMaxMin.XMin; 
            Model.ScrollValue = (axesMaxMin.XMax + axesMaxMin.XMin)/2;

            if (fileDuration == 0)
                FileHasChanged();
        }

        private void FileHasChanged()
        {
            var dictionary = state.DataSelected.Get().Channels;
            var listSelectedChannels = dictionary.Keys.ToList();
            if (listSelectedChannels.Count == 0)
                return;
            var channel = listSelectedChannels.First();

            // TODO: read length of data acquisition directly from file...
            var data = dictionary[channel];
            if (data == null) return;

            var meaExperiment = state.MeaExperiment.Get();
            fileDuration = data.Length / meaExperiment.DataAcquisitionSettings.SamplingRate;
            Model.ScrollMinimum = 0;
            Model.ScrollMaximum = fileDuration;
        }

        public void UpdateXAxisLimitsFromModelValues()
        {
            var xLast = Convert.ToDouble(Model.XLast);
            var xFirst = Convert.ToDouble(Model.XFirst);
            var axesMaxMin = state.AxesMaxMin.Get();
            state.AxesMaxMin.Set(new AxesExtrema(xFirst, xLast, axesMaxMin.YMin, axesMaxMin.YMax));
        }

        public void UpdateZoomFromModelValues()
        {
            var xFirst = Convert.ToDouble(Model.XFirst);
            var xLast = xFirst + Convert.ToDouble(Model.ScrollViewPortSize);
            var axesMaxMin = state.AxesMaxMin.Get();
            state.AxesMaxMin.Set(new AxesExtrema(xFirst, xLast, axesMaxMin.YMin, axesMaxMin.YMax));
        }

        public void ScrollBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            var axesMaxMin = state.AxesMaxMin.Get();
            if (axesMaxMin == null) return; 
            
            var xFirst = Model.ScrollValue - Model.ScrollViewPortSize / 2;
            var xLast = Model.ScrollValue + Model.ScrollViewPortSize / 2;
            state.AxesMaxMin.Set(new AxesExtrema(xFirst, xLast, axesMaxMin.YMin, axesMaxMin.YMax));
        }

        public void ChangeFilter(int selectedFilterIndex)
        {
            Model.SelectedFilterIndex = selectedFilterIndex;
            state.FilterProperty.Set(selectedFilterIndex);
        }

        public void ChangeSelectedChannels(int option)
        {

        }
    }
}
