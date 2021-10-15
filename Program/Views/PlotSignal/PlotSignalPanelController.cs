using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private List<int> listSelectedChannels;
        private int selectedFilter;
        

        public PlotSignalPanelController(
            ApplicationState state,
            H5FileReader h5FileReader,
            IEventSubscriber eventSubscriber)
        {
            this.state = state;
            this.h5FileReader = h5FileReader;

            Model = new PlotSignalPanelModel();
            eventSubscriber.Subscribe(EventType.MeaExperimentChanged, LoadAcquisitionParameters);
            eventSubscriber.Subscribe(EventType.SelectedChannelsChanged, ChangeSelectedElectrode);
            eventSubscriber.Subscribe(EventType.AxesMaxMinChanged, AxesChanged);
            eventSubscriber.Subscribe(EventType.FilterChanged, ChangeFilter);
        }

        private void LoadAcquisitionParameters()
        {
            var meaExperiment = state.MeaExperiment.Get();
            Model.AcquisitionSettingsLabel = " High-pass=" + meaExperiment.DataAcquisitionSettings.Hpf + " Hz " 
                                           + " Sampling rate=" + meaExperiment.DataAcquisitionSettings.SamplingRate /1000 + " kHz "
                                           + " resolution=" + (meaExperiment.DataAcquisitionSettings.Lsb * 1000).ToString("0.###")  + " mV";
        }

        private void ChangeFilter()
        {
            selectedFilter = state.FilterProperty.Get();
            if (listSelectedChannels == null || listSelectedChannels.Count <= 0) return;
            UpdateSelectedElectrodeData(listSelectedChannels);
        }

        public void AttachControlToModel(WpfPlot wpfControl)
        {
            Model.PlotControl = wpfControl;
        }

        private void ChangeSelectedElectrode()
        {
            listSelectedChannels = state.DataSelected.Get().Channels.Keys.ToList();
            UpdateSelectedElectrodeData(listSelectedChannels);
        }

        private void UpdateSelectedElectrodeData(List<int> selectedChannels)
        {
           
            PreparePlot();
            Mouse.OverrideCursor = Cursors.Wait;
            //LoadDataFromFilev0(selectedChannels);
            //LoadDataFromFilev1();
            //LoadDataToPlot(selectedChannels, "v1");
            //DisplayPlot();

            LoadDataFromFilev2();
            LoadDataToPlot(selectedChannels, null);
            DisplayPlot();
            Mouse.OverrideCursor = null;
        }

        private void LoadDataFromFilev0(List<int> selectedChannels)
        {
            var sw = Stopwatch.StartNew();
            foreach (var i in selectedChannels)
            {
                var dataSelected = state.DataSelected.Get();
                dataSelected.Channels[i] = H5FileReader.ReadAllDataFromSingleChannel(i);
                
            }
            Trace.WriteLine("Load 1 channel at a time: "+ sw.Elapsed + " -- selectedCount=" + selectedChannels.Count);
        }

        private void LoadDataFromFilev1()
        {
            var sw = Stopwatch.StartNew();
            var dataSelected = state.DataSelected.Get();
            H5FileReader.A13ReadAllDataFromChannels(dataSelected);
            Trace.WriteLine("Load all channels together : " + sw.Elapsed + " -- selectedCount=" + dataSelected.Channels.Count);
        }

        private void LoadDataFromFilev2()
        {
            var sw = Stopwatch.StartNew();
            var dataSelected = state.DataSelected.Get();
            //H5FileReader.A13ReadAllDataFromChannelsPseudoParallel(dataSelected);
            H5FileReader.A13ReadAllDataFromChannelsParallel(dataSelected);
            Trace.WriteLine("Load all channels together : " + sw.Elapsed + " -- selectedCount=" + dataSelected.Channels.Count);
        }

        private void LoadDataToPlot(List<int> selectedChannels, string comment)
        {
            var meaExp = state.MeaExperiment.Get();
            var samplingRate = meaExp.DataAcquisitionSettings.SamplingRate;
            foreach (var i in selectedChannels)
            {
                var electrodeData = meaExp.Electrodes.Single(x => x.Electrode.Channel == i);
                var legend = "channel: " + electrodeData.Electrode.Channel 
                                         + " electrode: " + electrodeData.Electrode.ElectrodeNumber
                                         + " (" + electrodeData.Electrode.XuM + ", " + electrodeData.Electrode.YuM + " µm)"
                                         + " ** " + comment;

                var channel = state.DataSelected.Get().Channels[i];
                AddPlot(ComputeFilteredData(channel), samplingRate, legend);
            }
        }

        private double[] ComputeFilteredData(ushort[] array)
        {
            var meaExp = state.MeaExperiment.Get();
            var result = Filter.ConvertDataToMV(array,
                meaExp.DataAcquisitionSettings.Lsb * 1000, 512);
            
            switch (selectedFilter)
            {
                case 1:
                    result = Filter.BDerivFast2f3(result, result.Length);
                    break;
                case 2:
                    result = Filter.BMedian(result, result.Length, 20);
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
            var signalPlot = plot.AddSignal(result, samplingRate, null, legend);
            var acqSettings = state.MeaExperiment.Get().DataAcquisitionSettings;
            signalPlot.MaxRenderIndex = (int)(acqSettings.nDataAcquisitionPoints - 1);
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


    }
}
