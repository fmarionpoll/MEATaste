using MEATaste.DataMEA.MaxWell;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;
using MEATaste.Views.FileOpen;


namespace MEATaste.Views.ElectrodesMap
{
    public class ElectrodeMapPanelController
    {
        public ElectrodesMapPanelViewModel ViewModel { get; }

        private readonly MeaFileReader meaFileReader;
        private readonly ApplicationState state;

        private ScottPlot.Plottable.ScatterPlot highlightedPoint;
        private int lastHighlightedIndex = -1;


        public ElectrodeMapPanelController(MeaFileReader meaFileReader, ApplicationState state)
        {
            this.meaFileReader = meaFileReader;
            this.state = state;

            ViewModel = new ElectrodesMapPanelViewModel();
            FileOpenPanelViewModel.NewFileIsLoadedAction += DisplayMap;
        }

        
        public void SelectElectrode(int index)
        {

        }

        public void OutlineElectrode(int index)
        {

        }

        private void DisplayMap()
        {
            double[] xs = state.CurrentMeaExperiment.Descriptors.GetElectrodesXCoordinate();
            double[] ys = state.CurrentMeaExperiment.Descriptors.GetElectrodesYCoordinate();
            var plt = ScottPlotElectrodesMap.Plot;
            plt.AddScatterPoints(xs, ys);
            plt.Title("Electrodes Map");
            plt.XLabel("Horizontal position µm");
            plt.YLabel("Vertical position µm");

            // Add a red circle we can move around later as a highlighted point indicator
            highlightedPoint = plt.AddPoint(0, 0);
            highlightedPoint.Color = System.Drawing.Color.Red;
            highlightedPoint.MarkerSize = 10;
            highlightedPoint.MarkerShape = ScottPlot.MarkerShape.openCircle;
            highlightedPoint.IsVisible = false;
        }

        private void HighLightMapOfElectrodeAt(double xs, double ys, int pointIndex)
        {
            // place the highlight over the point of interest
            highlightedPoint.Xs[0] = xs;
            highlightedPoint.Ys[0] = ys;
            highlightedPoint.IsVisible = true;

            // render if the highlighted point chnaged
            if (lastHighlightedIndex != pointIndex)
            {
                lastHighlightedIndex = pointIndex;
                ScottPlotElectrodesMap.Plot.Render();
            }
        }

        private int MoveCursorNearPoint(object sender)
        {
            ScottPlot.WpfPlot changedPlot = (ScottPlot.WpfPlot)sender;
            (double mouseCoordX, double mouseCoordY) = changedPlot.GetMouseCoordinates();
            double xyRatio = changedPlot.Plot.XAxis.Dims.PxPerUnit / changedPlot.Plot.YAxis.Dims.PxPerUnit;

            int index = 0;
            ScottPlot.Plottable.IPlottable[] table = ScottPlotElectrodesMap.Plot.GetPlottables();
            if (table.Length > 0)
            {
                ScottPlot.Plottable.ScatterPlot myScatterPlot = (ScottPlot.Plottable.ScatterPlot)table[0];
                (double pointX, double pointY, int pointIndex) = myScatterPlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
                HighLightMapOfElectrodeAt(pointX, pointY, pointIndex);
                index = pointIndex;
            }
            return index;
        }
        
        private void UpdateSelectedElectrodeOnMap(Electrode electrode)
        {
            HighLightMapOfElectrodeAt(electrode.XCoordinate, electrode.YCoordinate, electrode.ChannelNumber);
        }

    }
}