using System.Windows.Controls;
using System.Windows.Input;
using TasteMEA.DataMEA.Models;
using TasteMEA.DataMEA.MaxWell;

namespace TasteMEA.Views
{
    /// <summary>
    /// Interaction logic for ElectrodesMap.xaml
    /// </summary>
    public partial class ElectrodesMapPanel : UserControl
    {
        private ScottPlot.Plottable.ScatterPlot HighlightedPoint;
        private int LastHighlightedIndex = -1;
        private readonly ApplicationState state;
        private readonly MeaFileReader meaFileReader;

        public ElectrodesMapPanel()
        {
            // TODO
            //this.meaFileReader = meaFileReader;
            //this.state = state; 
            InitializeComponent();
        }

        private void ElectrodesMap_MouseMove(object sender, MouseEventArgs e)
        {
            MoveCursorNearPoint(sender);
        }

        private void ElectrodesMap_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int pointIndex = MoveCursorNearPoint(sender);
            Electrode electrode = state.CurrentMeaExperiment.Descriptors.Electrodes[pointIndex];
            // TODO: not sure this is right (search item first then select?)
            //UpdateSelectedElectrode(electrode);
            //ListViewChannels.SelectedIndex = pointIndex;
        }

        private int MoveCursorNearPoint(object sender)
        {
            ScottPlot.WpfPlot changedPlot = (ScottPlot.WpfPlot)sender;
            (double mouseCoordX, double mouseCoordY) = changedPlot.GetMouseCoordinates();
            double xyRatio = changedPlot.Plot.XAxis.Dims.PxPerUnit / changedPlot.Plot.YAxis.Dims.PxPerUnit;

            int index = 0;
            ScottPlot.Plottable.IPlottable[] table = ElectrodesMap.Plot.GetPlottables();
            if (table.Length > 0)
            {
                ScottPlot.Plottable.ScatterPlot myScatterPlot = (ScottPlot.Plottable.ScatterPlot)table[0];
                (double pointX, double pointY, int pointIndex) = myScatterPlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
                HighLightMapOfElectrodeAt(pointX, pointY, pointIndex);
                index = pointIndex;
            }
            return index;
        }

        private void HighLightMapOfElectrodeAt(double xs, double ys, int pointIndex)
        {
            // place the highlight over the point of interest
            HighlightedPoint.Xs[0] = xs;
            HighlightedPoint.Ys[0] = ys;
            HighlightedPoint.IsVisible = true;

            // render if the highlighted point chnaged
            if (LastHighlightedIndex != pointIndex)
            {
                LastHighlightedIndex = pointIndex;
                ElectrodesMap.Plot.Render();
            }
        }

        private void UpdateSelectedElectrodeOnMap(Electrode electrode)
        {
            HighLightMapOfElectrodeAt(electrode.XCoordinate, electrode.YCoordinate, electrode.ChannelNumber);
        }

        private void InitElectrodesMap()
        {
            double[] xs = state.CurrentMeaExperiment.Descriptors.GetElectrodesXCoordinate();
            double[] ys = state.CurrentMeaExperiment.Descriptors.GetElectrodesYCoordinate();
            var plt = ElectrodesMap.Plot;
            plt.AddScatterPoints(xs, ys);
            plt.Title("Electrodes Map");
            plt.XLabel("Horizontal position µm");
            plt.YLabel("Vertical position µm");

            // Add a red circle we can move around later as a highlighted point indicator
            HighlightedPoint = plt.AddPoint(0, 0);
            HighlightedPoint.Color = System.Drawing.Color.Red;
            HighlightedPoint.MarkerSize = 10;
            HighlightedPoint.MarkerShape = ScottPlot.MarkerShape.openCircle;
            HighlightedPoint.IsVisible = false;
        }

    }
}
