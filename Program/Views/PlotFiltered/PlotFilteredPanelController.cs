﻿using System;
using System.Linq;
using System.Windows.Input;
using MEATaste.DataMEA.MaxWell;
using MEATaste.DataMEA.Models;
using MEATaste.DataMEA.Utilities;
using MEATaste.Infrastructure;
using ScottPlot;

namespace MEATaste.Views.PlotFiltered
{
    public class PlotFilteredPanelController
    {
        public PlotFilteredPanelModel Model { get; }
        private readonly ApplicationState state;

        public PlotFilteredPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;

            Model = new PlotFilteredPanelModel();
            eventSubscriber.Subscribe(EventType.ElectrodeRecordLoaded, ChangeSelectedElectrode);
            eventSubscriber.Subscribe(EventType.AxesMaxMinChanged, AxesChanged);
        }


        public void AuthorizeReading(bool value)
        {
            Model.PlotFilteredData = value;
        }

        public void AttachControlToModel(WpfPlot wpfControl)
        {
            Model.PlotControl = wpfControl;
        }

        private void ChangeSelectedElectrode()
        {
            if (Model.PlotFilteredData)
            {
                var electrodeRecord = state.SelectedElectrode.Get();
                if (electrodeRecord != null)
                    UpdateSelectedChannel(electrodeRecord);
            }
        }

        private void UpdateSelectedChannel(ElectrodeRecord electrodeRecord)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var currentExperiment = state.CurrentMeaExperiment.Get();
            var rawSignalDouble = currentExperiment.rawSignalDouble;

            var plot = Model.PlotControl.Plot;
            plot.XLabel("Time (s)");
            plot.YLabel("Voltage (mV)");
            
            plot.Clear();

            double[] medianRow = Filter.BMedian(rawSignalDouble, rawSignalDouble.Length, 20);
            plot.AddSignal(medianRow, currentExperiment.Descriptors.SamplingRate, System.Drawing.Color.Green, label: "median");
            
            double[] derivRow = Filter.BDeriv(rawSignalDouble, rawSignalDouble.Length);
            plot.AddSignal(derivRow, currentExperiment.Descriptors.SamplingRate, System.Drawing.Color.Orange, label: "derivative");
            
            //Model.FormsPlots = new[] { RawSignal, FilteredSignal };
            //foreach (var fp in Model.FormsPlots)
            //    fp.AxesChanged += OnAxesChanged;

            Mouse.OverrideCursor = null;
            var legend = plot.Legend();
            legend.FontSize = 10; 
            plot.Render();

        }

        public void OnAxesChanged(object sender, EventArgs e)
        {
            var changedPlot = (WpfPlot)sender;
            var plot = Model.PlotControl;
            if (plot != changedPlot)
            {
                var newAxisLimits = changedPlot.Plot.GetAxisLimits();
                ChangeXAxes(Model.PlotControl, newAxisLimits.XMin, newAxisLimits.XMax);
            }
            UpdateAxesMaxMinFromScottPlot(plot.Plot.GetAxisLimits());
        }

        private void UpdateAxesMaxMinFromScottPlot(AxisLimits axisLimits)
        {
            state.AxesMaxMin.Set(new AxesMaxMin(axisLimits.XMin, axisLimits.XMax, axisLimits.YMin, axisLimits.YMax));
        }

        private void ChangeXAxes(WpfPlot plot, double XMin, double XMax)
        {
            plot.Configuration.AxesChangedEventEnabled = false;
            plot.Plot.SetAxisLimitsX(XMin, XMax);
            plot.Render();
            plot.Configuration.AxesChangedEventEnabled = true;

            Model.AxisLimitsForDataPlot = plot.Plot.GetAxisLimits();
        }

        private void AxesChanged()
        {
            if (Model.PlotFilteredData)
            {
                var axesMaxMin = state.AxesMaxMin.Get();
                if (axesMaxMin != null)
                    ChangeXAxes(Model.PlotControl, axesMaxMin.XMin, axesMaxMin.XMax);
            }
        }

    }

}
