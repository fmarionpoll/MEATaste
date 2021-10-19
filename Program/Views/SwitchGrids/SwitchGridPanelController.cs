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

        public void DoIt(Grid rootGrid)
        {
            Grid mainGrid = new Grid();
            rootGrid.Children.Clear();
            rootGrid.Children.Add(mainGrid);

            for (int icol = 0; icol < Model.NColumns; icol++)
            {
                Grid gridCol = new Grid();
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                mainGrid.Children.Add(gridCol);
                Grid.SetColumn(gridCol, icol);
                for (int irow = 0; irow < Model.NRows; irow++)
                {
                    Grid gridRow = new Grid();
                    PlotSignalPanel plotPanel = new PlotSignalPanel();
                    gridRow.Children.Add(plotPanel);

                    gridCol.RowDefinitions.Add(new RowDefinition());
                    gridCol.Children.Add(gridRow);
                    
                    Grid.SetRow(gridRow, irow);
                }
            }

        }
    }
}
