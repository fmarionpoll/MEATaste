using System;
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
            if (value)
                ChangeSelectedElectrode();
            else
            {
                Model.SelectedElectrodeRecord = null; 
                var plotControl = Model.PlotControl;
                plotControl.Plot.Clear();
            }
        }

        public void AttachControlToModel(WpfPlot wpfControl)
        {
            Model.PlotControl = wpfControl;
        }

        private void ChangeSelectedElectrode()
        {
            if (!Model.PlotFilteredData) return;
            var electrodeRecord = state.SelectedElectrode.Get();
            if (electrodeRecord == null) return;
            if (electrodeRecord == Model.SelectedElectrodeRecord)
                return;
            Model.SelectedElectrodeRecord = electrodeRecord;
            UpdateSelectedElectrodeFilteredData();
        }

        private void UpdateSelectedElectrodeFilteredData()
        {
            var currentExperiment = state.CurrentMeaExperiment.Get();
            if (currentExperiment == null) 
                return;
            var rawSignalDouble = currentExperiment.RawSignalDouble;
            if (rawSignalDouble == null) 
                return;

            Mouse.OverrideCursor = Cursors.Wait;
            var plot = Model.PlotControl.Plot;
            plot.XLabel("Time (s)");
            plot.YLabel("Voltage (µV)");
            
            plot.Clear();
            switch (Model.SelectedFilterIndex)
            {
            case 1:
                var medianRow = Filter.BMedian(rawSignalDouble, rawSignalDouble.Length, 20);
                plot.AddSignal(medianRow, currentExperiment.Descriptors.SamplingRate, System.Drawing.Color.Green, label: "median");
                break;
            default:
                var derivRow = Filter.BDeriv(rawSignalDouble, rawSignalDouble.Length);
                plot.AddSignal(derivRow, currentExperiment.Descriptors.SamplingRate, System.Drawing.Color.Orange, label: "derivative");
                break;
            }
            
            Mouse.OverrideCursor = null;
            var legend = plot.Legend();
            legend.FontSize = 10; 
            plot.Render();
        }

        public void OnAxesChanged(object sender, EventArgs e)
        {
            var changedPlot = (WpfPlot)sender;
            var plot = Model.PlotControl;
            var newAxisLimits = changedPlot.Plot.GetAxisLimits();
            ChangeXAxes(Model.PlotControl, newAxisLimits.XMin, newAxisLimits.XMax);
            UpdateAxesMaxMinFromScottPlot(plot.Plot.GetAxisLimits());
        }

        private void UpdateAxesMaxMinFromScottPlot(AxisLimits axisLimits)
        {
            state.AxesMaxMin.Set(new AxesMaxMin(axisLimits.XMin, axisLimits.XMax, axisLimits.YMin, axisLimits.YMax));
        }

        private void ChangeXAxes(WpfPlot plot, double xMin, double xMax)
        {
            plot.Configuration.AxesChangedEventEnabled = false;
            plot.Plot.SetAxisLimitsX(xMin, xMax);
            plot.Render();
            plot.Configuration.AxesChangedEventEnabled = true;

            Model.AxisLimitsForDataPlot = plot.Plot.GetAxisLimits();
        }

        private void AxesChanged()
        {
            if (!Model.PlotFilteredData) return;

            var axesMaxMin = state.AxesMaxMin.Get();
            if (axesMaxMin != null)
                ChangeXAxes(Model.PlotControl, axesMaxMin.XMin, axesMaxMin.XMax);
        }

        public void ChangeFilter(int selectedFilterIndex)
        {
            Model.SelectedFilterIndex = selectedFilterIndex;
            if (Model.SelectedElectrodeRecord != null)
                UpdateSelectedElectrodeFilteredData();
        }

    }

}
