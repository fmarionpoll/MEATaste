﻿using System;
using System.Diagnostics;
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
            eventSubscriber.Subscribe(EventType.ElectrodeRecordLoaded, FileHasChanged);
        }

        private void AxesChanged()
        {
            var axesMaxMin = state.AxesMaxMin.Get();
            if (axesMaxMin == null) return;

            Model.XFirst = axesMaxMin.XMin.ToString("0.###");
            Model.XLast = axesMaxMin.XMax.ToString("0.###");

            double diff = axesMaxMin.XMax - axesMaxMin.XMin;
            Model.ScrollViewPortSize = diff ; // /fileDuration;
            Model.ScrollValue = (axesMaxMin.XMax + axesMaxMin.XMin)/2;
        }

        private void FileHasChanged()
        {
            MeaExperiment meaExperiment = state.CurrentMeaExperiment.Get();
            if (meaExperiment.RawSignalDouble == null) return;

            fileDuration = meaExperiment.RawSignalDouble.Length / meaExperiment.Descriptors.SamplingRate;
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

        public void ScrollBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            var axesMaxMin = state.AxesMaxMin.Get();
            if (axesMaxMin == null) return; 
            
            double xFirst = Model.ScrollValue - Model.ScrollViewPortSize / 2;
            double xLast = Model.ScrollValue + Model.ScrollViewPortSize / 2;
            state.AxesMaxMin.Set(new AxesExtrema(xFirst, xLast, axesMaxMin.YMin, axesMaxMin.YMax));

        }
    }
}
