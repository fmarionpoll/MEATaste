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
        private WpfPlot plotControl;

        public PlotSignalPanelController(ApplicationState state, 
            MeaFileReader meaFileReader,
            IEventSubscriber eventSubscriber)
        {
            this.state = state;
            this.meaFileReader = meaFileReader;

            Model = new PlotSignalPanelModel();
            eventSubscriber.Subscribe(EventType.CurrentExperimentChanged, LoadAcquisitionParameters);
            eventSubscriber.Subscribe(EventType.SelectedElectrodeChanged, ChangeSelectedElectrode);
            eventSubscriber.Subscribe(EventType.AxesMaxMinChanged, AxesChanged);
        }

        private void LoadAcquisitionParameters()
        {
            var currentExperiment = state.CurrentExperiment.Get();
            Model.AcquisitionSettingsLabel = " High-pass filter(Hz)=" + currentExperiment.Descriptors.Hpf 
                                           + " Sampling rate(Hz)=" + currentExperiment.Descriptors.SamplingRate
                                           + " resolution (mV) =" + currentExperiment.Descriptors.Lsb 
                                           * currentExperiment.Descriptors.Gain * 1000;
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
            //var gain = currentExperiment.Descriptors.Gain / 1000;
            //electrodeBuffer.RawSignalDouble = rawSignalUShort.Select(x => x * gain).ToArray();
            var Lsb = currentExperiment.Descriptors.Lsb * 1000;
            electrodeBuffer.RawSignalDouble = rawSignalUShort.Select(x => (x -512)* Lsb).ToArray();

            state.CurrentElectrode.Set(electrodeProperties);
            var plot = plotControl.Plot;
            plot.Clear();
            plot.AddSignal(electrodeBuffer.RawSignalDouble, currentExperiment.Descriptors.SamplingRate);
            var title =
                $"electrode: {electrodeProperties.Electrode} channel: {electrodeProperties.Channel} (position : x={electrodeProperties.XuM}, y={electrodeProperties.YuM} µm)";
            plot.Title(title);
            plot.XLabel("Time (s)");
            plot.YLabel("Voltage (mV)");
            
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

    }
}
