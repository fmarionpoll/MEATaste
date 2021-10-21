using System;
using System.Collections.Generic;
using System.Linq;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;


namespace MEATaste.Views.PlotScrollBar
{
    public class PlotScrollBarPanelController
    {
        public PlotScrollBarPanelModel Model { get; }
        private readonly ApplicationState state;
        private double fileDuration;
        private List<int> initialSelectedChannelsList;
        private int expandLevel;



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
            var meaExperiment = state.MeaExperiment.Get();
            fileDuration = meaExperiment.DataAcquisitionSettings.nDataAcquisitionPoints / meaExperiment.DataAcquisitionSettings.SamplingRate;
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

        public void ExpandSelectionOneLevel()
        {
            expandLevel++;
            if (expandLevel == 1)
                initialSelectedChannelsList = new List<int>(state.DataSelected.Get().Channels.Keys.ToList());

            ChangeSelectionLevel();
        }

        public void ReduceSelectionOneLevel()
        {
            expandLevel--;
            if (expandLevel < 0)
                expandLevel = 0;
            ChangeSelectionLevel();
        }

        private void ChangeSelectionLevel()
        {
            var expandedSelectedChannelsList = GetAllElectrodesAroundCurrentSelection(initialSelectedChannelsList, 20);
            state.DataSelected.Get().TrimDictionaryToList(expandedSelectedChannelsList);
            state.DataSelected.SetChanged();
        }
        private List<int> GetAllElectrodesAroundCurrentSelection(List<int> currentChannelsList, double delta)
        {
            var expandedSelectedChannelsList = new List<int>(currentChannelsList);
            var niterations = expandLevel;
            while (niterations > 0)
            {
                expandedSelectedChannelsList = ExpandCurrentSelectionOneLevel(expandedSelectedChannelsList, delta);
                niterations--;
            }

            return expandedSelectedChannelsList;
        }

        private List<int> ExpandCurrentSelectionOneLevel(List<int> currentChannelsList, double delta)
        {
            List<int> expandedSelectedChannelsList = new();

            var meaExp = state.MeaExperiment.Get();
            foreach (var channel in currentChannelsList)
            {
                var electrode = meaExp.Electrodes.Single(x => x.Electrode.Channel == channel).Electrode;
                var xMax = electrode.XuM + delta;
                var xMin = electrode.XuM - delta;
                var yMax = electrode.YuM + delta;
                var yMin = electrode.YuM - delta;
                expandedSelectedChannelsList.Add(channel);
                expandedSelectedChannelsList.AddRange(from electrodeData in meaExp.Electrodes
                    where electrodeData.Electrode.Channel != channel
                    where !(electrodeData.Electrode.XuM > xMax)
                    where !(electrodeData.Electrode.XuM < xMin)
                    where !(electrodeData.Electrode.YuM > yMax)
                    where !(electrodeData.Electrode.YuM < yMin)
                    select electrodeData.Electrode.Channel);
            }

            return expandedSelectedChannelsList;
        }
    }
}
