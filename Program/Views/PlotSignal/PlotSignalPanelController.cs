using System;
using System.Linq;
using System.Windows.Input;
using MEATaste.DataMEA.MaxWell;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;
using ScottPlot;

namespace MEATaste.Views.PlotSignal
{
    public class PlotSignalPanelController
    {
        public PlotSignalPanelModel Model { get; }

        private readonly ApplicationState state;
        private readonly MeaFileReader meaFileReader;

        public PlotSignalPanelController(ApplicationState state, MeaFileReader meaFileReader, IEventSubscriber eventSubscriber)
        {
            this.state = state;
            this.meaFileReader = meaFileReader;

            Model = new PlotSignalPanelModel();
            eventSubscriber.Subscribe(EventType.SelectedElectrodeChanged, ChangeSelectedElectrode);
            eventSubscriber.Subscribe(EventType.AxesMaxMinChanged, AxesChanged);
        }

        public void AuthorizeReading(bool value)
        {
            Model.PlotDataForSelectedElectrode = value;
        }

        public void AttachControlToModel(WpfPlot wpfControl)
        {
            Model.PlotControl = wpfControl;
        }

        private void ChangeSelectedElectrode()
        {
            if (!Model.PlotDataForSelectedElectrode) return;

            ElectrodeRecord electrodeRecord = state.SelectedElectrode.Get();
            if (electrodeRecord == null || electrodeRecord == Model.SelectedElectrodeRecord)
                return;
            Model.SelectedElectrodeRecord = electrodeRecord;
            UpdateSelectedElectrodeData(electrodeRecord);
        }

        private void UpdateSelectedElectrodeData(ElectrodeRecord electrodeRecord)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var currentExperiment = state.CurrentMeaExperiment.Get();
            currentExperiment.rawSignalUShort = meaFileReader.ReadDataForOneElectrode(electrodeRecord);
            var rawSignalUShort = currentExperiment.rawSignalUShort;
            var gain = currentExperiment.Descriptors.Gain / 1000;

            currentExperiment.rawSignalDouble = rawSignalUShort.Select(x => x * gain).ToArray();
            state.LoadedElectrode.Set(electrodeRecord);
            var plot = Model.PlotControl.Plot;
            plot.Clear();
            plot.AddSignal(currentExperiment.rawSignalDouble, currentExperiment.Descriptors.SamplingRate);
            var title = $"electrode: {electrodeRecord.Electrode} channel: {electrodeRecord.Channel} (position : x={electrodeRecord.XuM}, y={electrodeRecord.YuM} µm)";
            plot.Title(title);
            plot.XLabel("Time (s)");
            plot.YLabel("Voltage (µV)");
            
            Model.PlotControl.Plot.Render();
            Mouse.OverrideCursor = null;
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
            if (!Model.PlotDataForSelectedElectrode) return;
            var axesMaxMin = state.AxesMaxMin.Get();
            if (axesMaxMin != null)
                ChangeXAxes(Model.PlotControl, axesMaxMin.XMin, axesMaxMin.XMax);
        }


    }
}
