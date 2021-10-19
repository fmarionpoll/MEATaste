using System.Windows.Controls;
using MEATaste.Infrastructure;
using MEATaste.Views.PlotSignal;

namespace MEATaste.Views.SwitchGrids
{
    class SwitchGridsPanelController
    {
        public SwitchGridsPanelModel Model { get; }
        private readonly ApplicationState state;

        public SwitchGridsPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;
            Model = new SwitchGridsPanelModel();
        }

        // https://stackoverflow.com/questions/34009584/wpf-how-to-dynamically-create-a-grid-with-x-rows-and-y-columns-with-consecutive

        public void DoIt(Grid rootGrid)
        {
            Grid mainGrid = new ();
            rootGrid.Children.Clear();
            rootGrid.Children.Add(mainGrid);

            for (var icol = 0; icol < Model.NColumns; icol++)
            {
                Grid gridCol = new ();
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                mainGrid.Children.Add(gridCol);
                Grid.SetColumn(gridCol, icol);
                for (var irow = 0; irow < Model.NRows; irow++)
                {
                    Grid gridRow = new ();
                    PlotSignalPanel plotPanel = new ();
                    gridRow.Children.Add(plotPanel);

                    gridCol.RowDefinitions.Add(new RowDefinition());
                    gridCol.Children.Add(gridRow);
                    
                    Grid.SetRow(gridRow, irow);
                }
            }

        }
    }
}
