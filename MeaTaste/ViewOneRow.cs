using System;
using System.Linq;
using System.Windows.Input;
using TasteMEA.DataMEA.MaxWell;
using TasteMEA.DataMEA.Models;
using TasteMEA.DataMEA.Utilities;

namespace TasteMEA
{
    public partial class MainWindow
    {
        

        private void UpdateSelectedChannel(Electrode electrode)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                state.OneIntRow = FileReader.ReadAll_OneElectrodeAsInt(electrode);

                var plt = WpfPlot1.Plot;
                plt.Clear();
                double[] myData = state.OneIntRow.Select(x => (double)x).ToArray();
                plt.AddSignal(myData, state.CurrentMeaExperiment.Descriptors.SamplingRate);
                string title = $"channel: {electrode.ChannelNumber} electrode: {electrode.ElectrodeNumber} (position : x={electrode.XCoordinate}, y={electrode.YCoordinate} µm)";
                plt.Title(title);

                var plt2 = WpfPlot2.Plot;
                plt2.Clear();

                double[] medianRow = Filter.BMedian(myData, myData.Length, 20);
                plt2.AddSignal(medianRow, state.CurrentMeaExperiment.Descriptors.SamplingRate, System.Drawing.Color.Green);
                plt2.Title("derivRow + medianRow");

                double[] derivRow = Filter.BDeriv(myData, myData.Length);
                plt2.AddSignal(derivRow, state.CurrentMeaExperiment.Descriptors.SamplingRate, System.Drawing.Color.Orange);
                plt2.Title("derivRow");

                state.FormsPlots = new[] { WpfPlot1, WpfPlot2 };
                foreach (var fp in state.FormsPlots)
                    fp.AxesChanged += OnAxesChanged;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int selected = ListViewChannels.SelectedIndex;
            Electrode electrode = state.CurrentMeaExperiment.Descriptors.Electrodes[selected];
            UpdateSelectedElectrode(electrode);
        }

        private void UpdateSelectedElectrode(Electrode electrode)
        {
            UpdateSelectedElectrodeOnMap(electrode);
            UpdateSelectedChannel(electrode);
        }

        private void OnAxesChanged(object sender, EventArgs e)
        {
            ScottPlot.WpfPlot changedPlot = (ScottPlot.WpfPlot)sender;
            var newAxisLimits = changedPlot.Plot.GetAxisLimits();

            foreach (var fp in state.FormsPlots)
            {
                if (fp == changedPlot)
                    continue;

                // disable events briefly to avoid an infinite loop
                fp.Configuration.AxesChangedEventEnabled = false;
                fp.Plot.SetAxisLimitsX(newAxisLimits.XMin, newAxisLimits.XMax);
                fp.Render();
                fp.Configuration.AxesChangedEventEnabled = true;
            }
        }



    }
}