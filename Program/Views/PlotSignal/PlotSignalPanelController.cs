using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MEATaste.DataMEA.MaxWell;
using MEATaste.DataMEA.Models;
using MEATaste.DataMEA.Utilities;
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
            var samplingRate = meaExp.DataAcquisitionSettings.SamplingRate;
            PreparePlot();

            foreach (var channel in selectedElectrodes)
            {
                var electrodeData = meaExp.Electrodes.Single(x => x.Electrode.Channel == channel); 
                if( electrodeData.RawSignalUShort == null)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    electrodeData.RawSignalUShort = h5FileReader.ReadAllDataFromSingleChannel(channel);
                    Mouse.OverrideCursor = null;
                }
                var legend = "channel: " + electrodeData.Electrode.Channel
                                        + " electrode: " + electrodeData.Electrode.ElectrodeNumber
                                        + " (" + electrodeData.Electrode.XuM + ", " +
                                        electrodeData.Electrode.YuM + " µm)";
                AddPlot(ComputeFilteredData(electrodeData), samplingRate, legend);
            }
            DisplayPlot();
        }

        private double[] ComputeFilteredData(ElectrodeData electrodeData)
        {
            var meaExp = state.MeaExperiment.Get();
            var result = Filter.ConvertDataToMV(electrodeData.RawSignalUShort,
                meaExp.DataAcquisitionSettings.Lsb * 1000, 512);
            
            switch (Model.SelectedFilterIndex)
            {
                case 1:
                    result = Filter.BDerivFast2f3(result, result.Length);
                    break;
                case 2:
                    result = Filter.BMedian(result, result.Length, 20);
                    break;
                default:
                    break;
            }

            return result;
        }

        private void PreparePlot()
        {
            var plot = Model.PlotControl.Plot;
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
            var plot = Model.PlotControl.Plot;
            var legend = plot.Legend();
            legend.FontSize = 10;

            if (state.AxesMaxMin.Get() == null)
            {
                var acqSettings = state.MeaExperiment.Get().DataAcquisitionSettings;
                var duration = acqSettings.nDataAcquisitionPoints / acqSettings.SamplingRate;
                var newAxisLimits = plot.GetAxisLimits();
                state.AxesMaxMin.Set(new AxesExtrema(0, duration, newAxisLimits.YMin, newAxisLimits.YMax));
            }
            var axesMaxMin = state.AxesMaxMin.Get(); 
            plot.SetAxisLimits(axesMaxMin.XMin, axesMaxMin.XMax);

            plot.Render();
            Application.Current.Dispatcher.Invoke(() => { Model.PlotControl.Render(); });
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
            var axesMaxMin = state.AxesMaxMin.Get();
            if (axesMaxMin != null)
                ChangeXAxes(Model.PlotControl, axesMaxMin.XMin, axesMaxMin.XMax);
        }

        public void ChangeFilter(int selectedFilterIndex)
        {
            Model.SelectedFilterIndex = selectedFilterIndex;
            if (selectedElectrodes == null || selectedElectrodes.Count <= 0) return;
            UpdateSelectedElectrodeData(selectedElectrodes);
        }


    }
}
