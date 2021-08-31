using System;
using System.Diagnostics;
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
            if (Model.PlotDataForSelectedElectrode)
            {
                ElectrodeRecord electrodeRecord = state.SelectedElectrode.Get();
                if (electrodeRecord != null)
                {
                    UpdateSelectedChannel(electrodeRecord);
                }
            }
        }

        private void UpdateSelectedChannel(ElectrodeRecord electrodeRecord)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var currentExperiment = state.CurrentMeaExperiment.Get();

            ElectrodeRecord loadedElectrode = state.LoadedElectrode.Get();
            if (loadedElectrode == null || electrodeRecord.Electrode != loadedElectrode.Electrode)
            {
                currentExperiment.rawSignalUShort = meaFileReader.ReadDataForOneElectrode(electrodeRecord);
                var rawSignalUShort = currentExperiment.rawSignalUShort;
                Trace.WriteLine("Transform data--------------");
                var gain = currentExperiment.Descriptors.Gain / 1000;
                currentExperiment.rawSignalDouble = rawSignalUShort.Select(x => x * gain).ToArray();
                state.LoadedElectrode.Set(electrodeRecord);
            }

            Trace.WriteLine("plot data--------------");

            var plot = Model.PlotControl.Plot;
            plot.Clear();
            plot.AddSignal(currentExperiment.rawSignalDouble, currentExperiment.Descriptors.SamplingRate);
            var title = $"channel: {electrodeRecord.Channel} electrode: {electrodeRecord.Electrode} (position : x={electrodeRecord.X_uM}, y={electrodeRecord.Y_uM} µm)";
            plot.Title(title);
            plot.XLabel("Time (s)");
            plot.YLabel("Voltage (µV)");
            plot.Render();
            
            Mouse.OverrideCursor = null;
            
        }

        public void OnAxesChanged(object sender, EventArgs e)
        {
            var changedPlot = (WpfPlot)sender;
            var plot = Model.PlotControl;
            if (plot == changedPlot)
                return;

            var newAxisLimits = changedPlot.Plot.GetAxisLimits();
          
            plot.Configuration.AxesChangedEventEnabled = false;
            plot.Plot.SetAxisLimitsX(newAxisLimits.XMin, newAxisLimits.XMax);
            plot.Render();
            plot.Configuration.AxesChangedEventEnabled = true;

            Model.AxisLimitsForDataPlot = newAxisLimits;
        }
    
    }
}
