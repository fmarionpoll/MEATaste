using System;
using System.Linq;
using System.Windows.Input;
using MEATaste.DataMEA.dbWave;
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
        private readonly DataFileWriter dataFileWriter;
        private WpfPlot plotControl;

        public PlotSignalPanelController(ApplicationState state, MeaFileReader meaFileReader, DataFileWriter dataFileWriter, IEventSubscriber eventSubscriber)
        {
            this.state = state;
            this.meaFileReader = meaFileReader;
            this.dataFileWriter = dataFileWriter;

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
            plotControl = wpfControl;
        }

        private void ChangeSelectedElectrode()
        {
            if (!Model.PlotDataForSelectedElectrode) return;

            ElectrodeProperties electrodeProperties = state.CurrentElectrode.Get();
            if (electrodeProperties == null || electrodeProperties == Model.SelectedElectrodeProperties)
                return;
            Model.SelectedElectrodeProperties = electrodeProperties;
            UpdateSelectedElectrodeData(electrodeProperties);
        }

        private void UpdateSelectedElectrodeData(ElectrodeProperties electrodeProperties)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var currentExperiment = state.CurrentExperiment.Get();

            var electrodeBuffer = state.ElectrodeBuffer.Get();
            if (electrodeBuffer == null)
            {
                state.ElectrodeBuffer.Set(new ElectrodeDataBuffer());
                electrodeBuffer = state.ElectrodeBuffer.Get();
            }
            electrodeBuffer.RawSignalUShort = meaFileReader.ReadDataForOneElectrode(electrodeProperties);
            var rawSignalUShort = electrodeBuffer.RawSignalUShort;
            var gain = currentExperiment.Descriptors.Gain / 1000;
            electrodeBuffer.RawSignalDouble = rawSignalUShort.Select(x => x * gain).ToArray();

            state.CurrentElectrode.Set(electrodeProperties);
            var plot = plotControl.Plot;
            plot.Clear();
            plot.AddSignal(electrodeBuffer.RawSignalDouble, currentExperiment.Descriptors.SamplingRate);
            var title = $"electrode: {electrodeProperties.Electrode} channel: {electrodeProperties.Channel} (position : x={electrodeProperties.XuM}, y={electrodeProperties.YuM} µm)";
            plot.Title(title);
            plot.XLabel("Time (s)");
            plot.YLabel("Voltage (µV)");
            
            plotControl.Plot.Render();
            Mouse.OverrideCursor = null;
        }

        public void OnAxesChanged(object sender, EventArgs e)
        {
            var changedPlot = (WpfPlot)sender;
            var newAxisLimits = changedPlot.Plot.GetAxisLimits();
            ChangeXAxes(plotControl, newAxisLimits.XMin, newAxisLimits.XMax);
            UpdateAxesMaxMinFromScottPlot(plotControl.Plot.GetAxisLimits());
        }

        private void UpdateAxesMaxMinFromScottPlot(AxisLimits axisLimits)
        {
            state.AxesMaxMin.Set(new AxesExtrema(axisLimits.XMin, axisLimits.XMax, axisLimits.YMin, axisLimits.YMax));
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
                ChangeXAxes(plotControl, axesMaxMin.XMin, axesMaxMin.XMax);
        }

        public void SaveDataFile()
        {
            var experiment = state.CurrentExperiment.Get();
            var electrode = state.CurrentElectrode.Get();
            var electrodeData = state.ElectrodeBuffer.Get();
            dataFileWriter.SaveCurrentElectrodeDataToFile(experiment, electrode, electrodeData);
        }

    }
}
