using System;
using System.Linq;
using System.Windows.Input;
using MEATaste.DataMEA.MaxWell;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;
using ScottPlot;

namespace MEATaste.Views.OneElectrode
{
    public class OneElectrodePanelController
    {
        public OneElectrodePanelModel Model { get; }

        private readonly ApplicationState state;
        private readonly MeaFileReader meaFileReader;

        public OneElectrodePanelController(ApplicationState state, MeaFileReader meaFileReader, IEventSubscriber eventSubscriber)
        {
            this.state = state;
            this.meaFileReader = meaFileReader;

            Model = new OneElectrodePanelModel();
            eventSubscriber.Subscribe(EventType.SelectedElectrodeChanged, ChangeSelectedElectrode);
        }

        public void AuthorizeReading(bool value)
        {
            Model.AuthorizeReadingNewFile = value;
        }

        private void ChangeSelectedElectrode()
        {
            if (Model.AuthorizeReadingNewFile)
            {
                ElectrodeRecord electrodeRecord = state.SelectedElectrode.Get();
                if (electrodeRecord != null)
                    UpdateSelectedChannel(electrodeRecord);
            }
        }

        private void UpdateSelectedChannel(ElectrodeRecord electrodeRecord)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var currentExperiment = state.CurrentMeaExperiment.Get();
            currentExperiment.rawSignalFromOneElectrode = meaFileReader.ReadDataForOneElectrode(electrodeRecord);
            var rawSignal = currentExperiment.rawSignalFromOneElectrode;

            var plot = Model.DataPlot.Plot;
            plot.Clear();
            var myData = rawSignal.Select(x => (double)x).ToArray();
            plot.AddSignal(myData, currentExperiment.Descriptors.SamplingRate);
            var title = $"channel: {electrodeRecord.Channel} electrode: {electrodeRecord.Electrode} (position : x={electrodeRecord.X_uM}, y={electrodeRecord.Y_uM} µm)";
            plot.Title(title);
            plot.Render();

            //var plt2 = FilteredSignal.Plot;
            //plt2.Clear();

            //double[] medianRow = Filter.BMedian(myData, myData.Length, 20);
            //plt2.AddSignal(medianRow, state.CurrentMeaExperiment.Descriptors.SamplingRate, System.Drawing.Color.Green);
            //plt2.Title("derivRow + medianRow");

            //double[] derivRow = Filter.BDeriv(myData, myData.Length);
            //plt2.AddSignal(derivRow, state.CurrentMeaExperiment.Descriptors.SamplingRate, System.Drawing.Color.Orange);
            //plt2.Title("derivRow");

            //Model.FormsPlots = new[] { RawSignal, FilteredSignal };
            //foreach (var fp in Model.FormsPlots)
            //    fp.AxesChanged += OnAxesChanged;

            Mouse.OverrideCursor = null;
            
        }

        private void OnAxesChanged(object sender, EventArgs e)
        {
            var changedPlot = (WpfPlot)sender;
            var plot = Model.DataPlot;
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
