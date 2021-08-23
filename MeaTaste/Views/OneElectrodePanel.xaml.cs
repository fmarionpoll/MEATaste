using System;
using System.Linq;
using System.Windows.Input;
using TasteMEA.DataMEA.Models;
using TasteMEA.DataMEA.MaxWell;
using TasteMEA.DataMEA.Utilities;
using TasteMEA.Infrastructure;

namespace TasteMEA.Views
{
    /// <summary>
    /// Interaction logic for DisplayOneElectrodePanel.xaml
    /// </summary>
    public partial class OneElectrodePanel
    {
        private ushort[] rawSignalFromOneElectrode;
        private ScottPlot.WpfPlot[] formsPlots;

        public OneElectrodePanel()
        {
            InitializeComponent();
        }

        // Custom constructor to pass data
        private readonly ApplicationState state;
        private readonly MeaFileReader meaFileReader;

        public OneElectrodePanel(MeaFileReader meaFileReader, ApplicationState state) : this()
        {
            this.meaFileReader = meaFileReader;
            this.state = state;
        }

        private void UpdateSelectedElectrode(Electrode electrode)
        {
            // TODO find a way to update the other controls
            //UpdateSelectedElectrodeOnMap(electrode);
            UpdateSelectedChannel(electrode);
        }

        private void UpdateSelectedChannel(Electrode electrode)
        {
            var fileReader = new FileReader(); // delete this

            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                rawSignalFromOneElectrode = fileReader.ReadAll_OneElectrodeAsInt(electrode);

                var plt = RawSignal.Plot;
                plt.Clear();
                double[] myData = rawSignalFromOneElectrode.Select(x => (double)x).ToArray();
                plt.AddSignal(myData, state.CurrentMeaExperiment.Descriptors.SamplingRate);
                string title = $"channel: {electrode.ChannelNumber} electrode: {electrode.ElectrodeNumber} (position : x={electrode.XCoordinate}, y={electrode.YCoordinate} µm)";
                plt.Title(title);

                var plt2 = FilteredSignal.Plot;
                plt2.Clear();

                double[] medianRow = Filter.BMedian(myData, myData.Length, 20);
                plt2.AddSignal(medianRow, state.CurrentMeaExperiment.Descriptors.SamplingRate, System.Drawing.Color.Green);
                plt2.Title("derivRow + medianRow");

                double[] derivRow = Filter.BDeriv(myData, myData.Length);
                plt2.AddSignal(derivRow, state.CurrentMeaExperiment.Descriptors.SamplingRate, System.Drawing.Color.Orange);
                plt2.Title("derivRow");

                formsPlots = new[] { RawSignal, FilteredSignal };
                foreach (var fp in formsPlots)
                    fp.AxesChanged += OnAxesChanged;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void OnAxesChanged(object sender, EventArgs e)
        {
            ScottPlot.WpfPlot changedPlot = (ScottPlot.WpfPlot)sender;
            var newAxisLimits = changedPlot.Plot.GetAxisLimits();

            foreach (var fp in formsPlots)
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
