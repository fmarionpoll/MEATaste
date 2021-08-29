using System;
using System.Linq;
using System.Windows.Input;
using MEATaste.DataMEA.MaxWell;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;

namespace MEATaste.Views.OneElectrode
{
    public class OneElectrodePanelController
    {
        public OneElectrodePanelModel Model { get; }

        private readonly ApplicationState state;

        public OneElectrodePanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;

            Model = new OneElectrodePanelModel();
            eventSubscriber.Subscribe(EventType.SelectedElectrodeChanged, ChangeSelectedElectrode);
        }

        public void AuthorizeReading(bool value)
        {
            Model.AuthorizeReadingNewFile = value;
        }

        private void  ChangeSelectedElectrode()
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
            var fileReader = new FileReader(); // todo: delete this and use MeaFileReader

            Mouse.OverrideCursor = Cursors.Wait;
            var currentExperiment = state.CurrentMeaExperiment.Get();


            currentExperiment.rawSignalFromOneElectrode = fileReader.ReadAll_OneElectrodeAsInt(electrodeRecord);
            var rawSignal = currentExperiment.rawSignalFromOneElectrode;
            var plt = Model.DataPlot.Plot;
            plt.Clear();
            var myData = rawSignal.Select(x => (double)x).ToArray();
            plt.AddSignal(myData, currentExperiment.Descriptors.SamplingRate);
            var title = $"channel: {electrodeRecord.Channel} electrode: {electrodeRecord.Electrode} (position : x={electrodeRecord.X_uM}, y={electrodeRecord.Y_uM} µm)";
            plt.Title(title);
            plt.Render();

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

        //private void OnAxesChanged(object sender, EventArgs e)
        //{
        //    ScottPlot.WpfPlot changedPlot = (ScottPlot.WpfPlot)sender;
        //    var newAxisLimits = changedPlot.Plot.GetAxisLimits();

        //    foreach (var fp in Model.FormsPlots)
        //    {
        //        if (fp == changedPlot)
        //            continue;

        //        // disable events briefly to avoid an infinite loop
        //        fp.Configuration.AxesChangedEventEnabled = false;
        //        fp.Plot.SetAxisLimitsX(newAxisLimits.XMin, newAxisLimits.XMax);
        //        fp.Render();
        //        fp.Configuration.AxesChangedEventEnabled = true;
        //    }
        //}
    }
}
