using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private List<int> selectedElectrodes;
        

        public PlotSignalPanelController(
            ApplicationState state,
            H5FileReader h5FileReader,
            IEventSubscriber eventSubscriber)
        {
            this.state = state;
            this.h5FileReader = h5FileReader;

            Model = new PlotSignalPanelModel();
            eventSubscriber.Subscribe(EventType.CurrentExperimentChanged, LoadAcquisitionParameters);
            eventSubscriber.Subscribe(EventType.SelectedChannelsChanged, ChangeSelectedElectrode);
            eventSubscriber.Subscribe(EventType.AxesMaxMinChanged, AxesChanged);
        }

        private void LoadAcquisitionParameters()
        {
            var currentExperiment = state.MeaExperiment.Get();
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
                || state.ListSelectedChannels.Get() != selectedElectrodes))
                ChangeSelectedElectrode();
        }

        public void KeepCurveChecked(bool value)
        {
            if (Model.KeepChecked != value)
                MakeCurvesVisible(value);
            Model.KeepChecked = value;

            if (value &&
                (Model.PlotControl.Plot.GetPlottables().Length == 0
                 || state.ListSelectedChannels.Get() != selectedElectrodes))
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
            var listSelectedChannels = state.ListSelectedChannels.Get();
            if (listSelectedChannels == null)
                return;

            if (listSelectedChannels == selectedElectrodes)
                return;

            UpdateSelectedElectrodeData(listSelectedChannels);
        }

        private void UpdateSelectedElectrodeData(List<int> listSelectedChannels)
        {
            selectedElectrodes = listSelectedChannels;
            var meaExp = state.MeaExperiment.Get();
            double samplingRate = meaExp.DataAcquisitionSettings.SamplingRate;
            PreparePlot();

            foreach (var channel in selectedElectrodes)
            {
                var electrodeData = meaExp.Electrodes.Single(x => x.Electrode.Channel == channel); 
                if( electrodeData.RawSignalUShort == null)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    electrodeData.RawSignalUShort = h5FileReader.ReadChannelDataAll(channel);
                    Mouse.OverrideCursor = null;
                }
                TransferDataToElectrodeBuffer(electrodeData);
                string legend = "channel " + electrodeData.Electrode.Channel
                                           + "(" + electrodeData.Electrode.XuM + ", " +
                                           electrodeData.Electrode.YuM + ")";
                AddPlot(electrodeData.RawSignalDouble, samplingRate, legend);
            }
            DisplayPlot();
        }

        private void PreparePlot()
        {
            var plot = Model.PlotControl.Plot;
            if (!Model.KeepChecked)
                plot.Clear();
            plot.XLabel("Time (s)");
            plot.YLabel("Voltage (mV)");
        }

        private void AddPlot(double[] result, double samplingRate, string legend)
        {
            var plot = Model.PlotControl.Plot;
            var sig = plot.AddSignal(result, samplingRate, null, legend);
            var acqSettings = state.MeaExperiment.Get().DataAcquisitionSettings;
            sig.MaxRenderIndex = (int)(acqSettings.nDataAcquisitionPoints - 1);
        }

        private void DisplayPlot()
        {
            var acqSettings = state.MeaExperiment.Get().DataAcquisitionSettings;
            var duration = acqSettings.nDataAcquisitionPoints / acqSettings.SamplingRate;
            var plot = Model.PlotControl.Plot;
            var legend = plot.Legend();
            legend.FontSize = 10; 
            plot.SetAxisLimits(0, duration);
            plot.Render();
            Application.Current.Dispatcher.Invoke(() => { Model.PlotControl.Render(); });
        }

        private void TransferDataToElectrodeBuffer(ElectrodeData electrodeData)
        {
            var meaExp = state.MeaExperiment.Get();
            var lsb = meaExp.DataAcquisitionSettings.Lsb * 1000;
            electrodeData.RawSignalDouble = electrodeData.RawSignalUShort.Select(x => (x - 512) * lsb).ToArray();
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
