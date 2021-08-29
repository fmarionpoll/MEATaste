using System;
using System.Linq;
using System.Windows.Input;
using MEATaste.DataMEA.MaxWell;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;
using MEATaste.Views.ElectrodesList;
using MEATaste.Views.FileOpen;

namespace MEATaste.Views.OneElectrode
{
    public class OneElectrodePanelController
    {
        public OneElectrodePanelModel Model { get; }

        private readonly MeaFileReader meaFileReader;
        private readonly ApplicationState state;

        public OneElectrodePanelController(MeaFileReader meaFileReader, ApplicationState state)
        {
            this.meaFileReader = meaFileReader;
            this.state = state;

            Model = new OneElectrodePanelModel();

            FileOpenPanelModel.NewHdf5FileIsLoadedAction += ClearCurrentData;
            ElectrodesListPanelModel.NewCurrentElectrodeChannelAction += CurrentDataHasChanged();
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
            var fileReader = new FileReader(); // delete this

            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                state.CurrentMeaExperiment.rawSignalFromOneElectrode = fileReader.ReadAll_OneElectrodeAsInt(electrode);
                ushort[] rawSignal = state.CurrentMeaExperiment.rawSignalFromOneElectrode;
                var plt = Model.DataPlot.Plot;
                plt.Clear();
                double[] myData = rawSignal.Select(x => (double)x).ToArray();
                plt.AddSignal(myData, state.CurrentMeaExperiment.Descriptors.SamplingRate);
                string title = $"channel: {electrode.ChannelNumber} electrode: {electrode.ElectrodeNumber} (position : x={electrode.XCoordinate}, y={electrode.YCoordinate} µm)";
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
