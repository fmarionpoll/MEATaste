using System.Windows;
using MeaTaste.DataMEA.Models;
using System.Linq;
using System;
using System.Windows.Input;
using TasteMEA.DataMEA.Utilies;
using TasteMEA.DataMEA.MaxWell;

namespace MeaTaste
{
    public partial class MainWindow : Window
    {

        public ushort[,] allData = null;
        public ushort[] oneIntRow;
        public ScottPlot.WpfPlot[] FormsPlots;

        private void UpdateSelectedChannel(Electrode electrode)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                oneIntRow = FileReader.ReadAll_OneElectrodeAsInt(electrode);

                var plt = wpfPlot1.Plot;
                plt.Clear();
                double[] myData = oneIntRow.Select(x => (double)x).ToArray();
                plt.AddSignal(myData, MeaExpInfos.Descriptors.SamplingRate);
                string title = $"channel: {electrode.ChannelNumber} electrode: {electrode.ElectrodeNumber} (position : x={electrode.XCoordinates_um}, y={electrode.YCoordinates_um} µm)";
                plt.Title(title);

                var plt2 = wpfPlot2.Plot;
                plt2.Clear();

                double[] medianRow = Filter.BMedian(myData, myData.Length, 20);
                plt2.AddSignal(medianRow, MeaExpInfos.Descriptors.SamplingRate, System.Drawing.Color.Green);
                plt2.Title("derivRow + medianRow");

                double[] derivRow = Filter.BDeriv(myData, myData.Length);
                plt2.AddSignal(derivRow, MeaExpInfos.Descriptors.SamplingRate, System.Drawing.Color.Orange);
                plt2.Title("derivRow");

                FormsPlots = new ScottPlot.WpfPlot[] { wpfPlot1, wpfPlot2 };
                foreach (var fp in FormsPlots)
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
            Electrode electrode = MeaExpInfos.Descriptors.Electrodes[selected];
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

            foreach (var fp in FormsPlots)
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