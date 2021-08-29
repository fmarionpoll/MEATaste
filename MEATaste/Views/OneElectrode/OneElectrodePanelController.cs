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

        public OneElectrodePanelController(ApplicationState state)
        {
            this.state = state;

            Model = new OneElectrodePanelModel();
        }

        public void ClearCurrentData()
        {

        }

        public void CurrentDataHasChanged()
        {

        }

        private void UpdateSelectedElectrode(Electrode electrode)
        {
            UpdateSelectedChannel(electrode);
        }

        private void UpdateSelectedChannel(Electrode electrode)
        {
            var fileReader = new FileReader(); // todo: delete this and use MeaFileReader

            Mouse.OverrideCursor = Cursors.Wait;
            var currentExperiment = state.CurrentMeaExperiment.Get();

            try
            {
                currentExperiment.rawSignalFromOneElectrode = fileReader.ReadAll_OneElectrodeAsInt(electrode);
                var rawSignal = currentExperiment.rawSignalFromOneElectrode;
                var plt = Model.DataPlot.Plot;
                plt.Clear();
                var myData = rawSignal.Select(x => (double)x).ToArray();
                plt.AddSignal(myData, currentExperiment.Descriptors.SamplingRate);
                var title = $"channel: {electrode.ChannelNumber} electrode: {electrode.ElectrodeNumber} (position : x={electrode.XCoordinate}, y={electrode.YCoordinate} µm)";
                plt.Title(title);

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
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
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
