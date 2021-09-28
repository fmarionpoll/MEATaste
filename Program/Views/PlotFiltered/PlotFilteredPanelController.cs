using System;
using System.Windows;
using System.Windows.Input;
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
        private WpfPlot plotControl;
        private ElectrodeProperties selectedElectrode;


        public PlotFilteredPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;

            Model = new PlotFilteredPanelModel();
            eventSubscriber.Subscribe(EventType.ElectrodeRecordLoaded, ChangeSelectedElectrode);
            eventSubscriber.Subscribe(EventType.AxesMaxMinChanged, AxesChanged);
        }

        public void DisplayCurveChecked(bool value)
        {
            if (Model.DisplayChecked != value)
                MakeCurvesVisible(value);
            Model.DisplayChecked = value;
            ChangeSelectedElectrode();
        }

        private void MakeCurvesVisible(bool visible)
        {
            var plot = Model.PlotControl.Plot;
            var plottables = plot.GetPlottables();
            foreach (var t in plottables)
            {
                t.IsVisible = visible;
            }
            Application.Current.Dispatcher.Invoke(() => { Model.PlotControl.Render(); });
        }

        public void AttachControlToModel(WpfPlot wpfControl)
        {
            plotControl = wpfControl;
        }

        private void ChangeSelectedElectrode()
        {
            if (!Model.DisplayChecked)
            {
                selectedElectrode = null;
                return;
            }

            var properties = state.CurrentElectrode.Get();
            if (properties == null || properties == selectedElectrode)
                return;
            selectedElectrode = properties;
            UpdateSelectedElectrodeFilteredData();
        }

        private void UpdateSelectedElectrodeFilteredData()
        {
            var currentExperiment = state.CurrentExperiment.Get();
            if (currentExperiment == null) 
                return;

            var electrodeBuffer = state.ElectrodeBuffer.Get();
            if (electrodeBuffer == null)
                return;
            var rawSignalDouble = electrodeBuffer.RawSignalDouble;
            if (rawSignalDouble == null) 
                return;

            PlotData(ComputeFilteredData(rawSignalDouble));
        }

        private double[] ComputeFilteredData(double[] rawSignalDouble)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var result = Model.SelectedFilterIndex switch
            {
                1 => Filter.BMedian(rawSignalDouble, rawSignalDouble.Length, 20),
                _ => Filter.BDeriv(rawSignalDouble, rawSignalDouble.Length)
            };
            Mouse.OverrideCursor = null;
            return result;
        }

        private void PlotData(double[] result)
        {
            var currentExperiment = state.CurrentExperiment.Get();
            var plot = plotControl.Plot;
            plot.Clear();
            plot.AddSignal(result, currentExperiment.DataAcquisitionSettings.SamplingRate,
                System.Drawing.Color.Orange, label: Model.SelectedFilterIndex.ToString());

            plot.XLabel("Time (s)");
            plot.YLabel("Voltage (µV)");

            var legend = plot.Legend();
            legend.FontSize = 10;
            plot.Render();
            Application.Current.Dispatcher.Invoke(() => { Model.PlotControl.Render(); });
        }

        public void OnAxesChanged(object sender, EventArgs e)
        {
            var changedPlot = (WpfPlot)sender;
            var newAxisLimits = changedPlot.Plot.GetAxisLimits();
            ChangeXAxes(plotControl, newAxisLimits.XMin, newAxisLimits.XMax);
            state.AxesMaxMin.Set(new AxesExtrema(newAxisLimits.XMin, newAxisLimits.XMax, newAxisLimits.YMin, newAxisLimits.YMax));
        }

        private void ChangeXAxes(WpfPlot plot, double xMin, double xMax)
        {
            plot.Configuration.AxesChangedEventEnabled = false;
            plot.Plot.SetAxisLimitsX(xMin, xMax);
            plot.Render();
            plot.Configuration.AxesChangedEventEnabled = true;
        }

        private void AxesChanged()
        {
            if (!Model.DisplayChecked) return;
            var axesMaxMin = state.AxesMaxMin.Get();
            if (axesMaxMin != null)
                ChangeXAxes(plotControl, axesMaxMin.XMin, axesMaxMin.XMax);
        }

        public void ChangeFilter(int selectedFilterIndex)
        {
            Model.SelectedFilterIndex = selectedFilterIndex;
            if (Model.SelectedElectrodeProperties != null)
                UpdateSelectedElectrodeFilteredData();
        }

    }

}
