using System;
using System.Linq;
using System.Windows;
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
        private readonly H5FileReader h5FileReader;
        private ElectrodeProperties selectedElectrode;
        

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

        public void DisplayCurveChecked(bool value)
        {
            if (Model.DisplayChecked != value)
                MakeCurvesVisible(value);
            Model.DisplayChecked = value;

            if (value && 
                (Model.PlotControl.Plot.GetPlottables().Length == 0
                || state.CurrentElectrode.Get() != selectedElectrode))
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
            Model.PlotControl = wpfControl;
        }

        private void ChangeSelectedElectrode()
        {
            if (!Model.DisplayChecked) return;
            
            var properties = state.CurrentElectrode.Get();
            if (properties == null || properties == selectedElectrode)
                return;

            UpdateSelectedElectrodeData(properties);
        }

        private void UpdateSelectedElectrodeData(ElectrodeProperties properties)
        {
            var electrodeBuffer = state.ElectrodeBuffer.Get();
            var flag = electrodeBuffer == null;
            if (flag)
            {
                state.ElectrodeBuffer.Set(new ElectrodeDataBuffer());
                electrodeBuffer = state.ElectrodeBuffer.Get();
            }

            if (properties != selectedElectrode || flag)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                electrodeBuffer.RawSignalUShort = h5FileReader.ReadAllFromOneChannelAsInt(properties.Channel);
                selectedElectrode = properties;
                Mouse.OverrideCursor = null;
            }

            var result = TransferDataToElectrodeBuffer();
            DisplayNewData(properties, result);
        }

        private void DisplayNewData(ElectrodeProperties properties, double[] result)
        {
            var currentExperiment = state.CurrentExperiment.Get();
            var plot = Model.PlotControl.Plot;
            plot.Clear();
            var sig = plot.AddSignal(result, currentExperiment.DataAcquisitionSettings.SamplingRate);

            var acqSettings = currentExperiment.DataAcquisitionSettings;
            var duration = acqSettings.nDataAcquisitionPoints / acqSettings.SamplingRate;
            var title = "electrode: "+ properties.Electrode
                                     + " channel: " +properties.Channel
                                     + $" (position : x={properties.XuM}, y={properties.YuM} µm)";
            plot.Title(title);
            plot.XLabel("Time (s)");
            plot.YLabel("Voltage (mV)");

            plot.SetAxisLimits(0, duration);
            sig.MaxRenderIndex = (int)(acqSettings.nDataAcquisitionPoints - 1);
            plot.Render();
            Application.Current.Dispatcher.Invoke(() => { Model.PlotControl.Render(); });
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
            ChangeXAxes(changedPlot, newAxisLimits.XMin, newAxisLimits.XMax);
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
                ChangeXAxes(Model.PlotControl, axesMaxMin.XMin, axesMaxMin.XMax);
        }

    }
}
