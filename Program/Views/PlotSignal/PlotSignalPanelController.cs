using System;
using System.Linq;
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
        private readonly H5FileReader h5FileReader;
        private WpfPlot plotControl;

        public PlotSignalPanelController(ApplicationState state,
            H5FileReader h5FileReader,
            IEventSubscriber eventSubscriber)
        {
            this.state = state;
            this.h5FileReader = h5FileReader;

            Model = new PlotSignalPanelModel();
            eventSubscriber.Subscribe(EventType.CurrentExperimentChanged, LoadAcquisitionParameters);
            eventSubscriber.Subscribe(EventType.SelectedElectrodeChanged, ChangeSelectedElectrode);
            eventSubscriber.Subscribe(EventType.AxesMaxMinChanged, AxesChanged);
        }

        private void LoadAcquisitionParameters()
        {
            var currentExperiment = state.CurrentExperiment.Get();
            Model.AcquisitionSettingsLabel = " High-pass=" + currentExperiment.DataAcquisitionSettings.Hpf + " Hz " 
                                           + " Sampling rate=" + currentExperiment.DataAcquisitionSettings.SamplingRate /1000 + " kHz "
                                           + " resolution=" + (currentExperiment.DataAcquisitionSettings.Lsb * 1000).ToString("0.###")  + " mV";
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

            var electrodeProperties = state.CurrentElectrode.Get();
            if (electrodeProperties == null || electrodeProperties == Model.SelectedElectrodeProperties)
                return;
            Model.SelectedElectrodeProperties = electrodeProperties;

            //var cancellationTokenSource = new CancellationTokenSource();
            //var task = new Task(() => UpdateSelectedElectrodeData(electrodeProperties), cancellationTokenSource.Token);
            UpdateSelectedElectrodeData(electrodeProperties);
        }

        private void UpdateSelectedElectrodeData(ElectrodeProperties electrodeProperties)
        {
            
            var electrodeBuffer = state.ElectrodeBuffer.Get();
            if (electrodeBuffer == null)
            {
                state.ElectrodeBuffer.Set(new ElectrodeDataBuffer());
                electrodeBuffer = state.ElectrodeBuffer.Get();
            }
            electrodeBuffer.RawSignalUShort = h5FileReader.ReadAllFromOneChannelAsInt(electrodeProperties.Channel);

            DisplayNewData(electrodeProperties);
        }

        private void DisplayNewData(ElectrodeProperties electrodeProperties)
        {
            var currentExperiment = state.CurrentExperiment.Get();
            state.CurrentElectrode.Set(electrodeProperties);
            var plot = plotControl.Plot;
            plot.Clear();
            plot.AddSignal(TransferDataToElectrodeBuffer(), currentExperiment.DataAcquisitionSettings.SamplingRate);
            var title = "electrode: "+ electrodeProperties.Electrode
            + " channel: " +electrodeProperties.Channel
            + $" (position : x={electrodeProperties.XuM}, y={electrodeProperties.YuM} µm)";
            plot.Title(title);
            plot.XLabel("Time (s)");
            plot.YLabel("Voltage (mV)");

            plotControl.Plot.Render();
        }

        private double[] TransferDataToElectrodeBuffer()
        {
            var currentExperiment = state.CurrentExperiment.Get();
            var electrodeBuffer = state.ElectrodeBuffer.Get();
            var rawSignalUShort = electrodeBuffer.RawSignalUShort;
            var lsb = currentExperiment.DataAcquisitionSettings.Lsb * 1000;
            electrodeBuffer.RawSignalDouble = rawSignalUShort.Select(x => (x - 512) * lsb).ToArray();
            return electrodeBuffer.RawSignalDouble;
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
