using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using MEATaste.DataMEA.MaxWell;
using MEATaste.Infrastructure;
using MEATaste.Views.PlotSignal;

namespace MEATaste.Views.SwitchGrids
{
    public class SwitchGridsPanelController
    {
        public SwitchGridsPanelModel Model { get; }
        private readonly ApplicationState state;
        private List<PlotSignalPanel> plotSignalList;

        public SwitchGridsPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;
            Model = new SwitchGridsPanelModel();
            eventSubscriber.Subscribe(EventType.SelectedChannelsChanged, ChangeSelectedElectrode);
        }

        // https://stackoverflow.com/questions/34009584/wpf-how-to-dynamically-create-a-grid-with-x-rows-and-y-columns-with-consecutive

        private void ChangeSelectedElectrode()
        {
            List<int> listSelectedChannels = new(state.DataSelected.Get().Channels.Keys.ToList());
            //UpdateSelectedElectrodeData(listSelectedChannels);
            if (plotSignalList == null || listSelectedChannels.Count == 0) return;

            if (state.DataSelected.Get().IsLoadingDataFromFileNeeded())
                LoadDataFromFilev2();

            var index = 0;
            foreach (var plotSignalPanel in plotSignalList)
            {
                if (listSelectedChannels.Count > plotSignalList.Count)
                {
                    plotSignalPanel.UpdateChannelList(listSelectedChannels);
                }
                else
                {
                    var dummyList = listSelectedChannels.GetRange(index, 1);
                    plotSignalPanel.UpdateChannelList(dummyList);
                }
                
                index++;
                if (index >= listSelectedChannels.Count)
                    break;
                
            }
        }

        private void LoadDataFromFilev2()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            H5FileReader.A13ReadAllDataFromChannelsParallel(state.DataSelected.Get());
            Mouse.OverrideCursor = null;
        }

        public void DoIt(Grid rootGrid)
        {
            if (plotSignalList == null || Model.NColumns * Model.NRows != plotSignalList.Count)
            {
                Grid mainGrid = new();
                rootGrid.Children.Clear();
                rootGrid.Children.Add(mainGrid);
                plotSignalList = new();
                int index = 0;

                for (var icol = 0; icol < Model.NColumns; icol++)
                {
                    Grid gridCol = new();
                    mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    mainGrid.Children.Add(gridCol);
                    Grid.SetColumn(gridCol, icol);

                    for (var irow = 0; irow < Model.NRows; irow++)
                    {
                        Grid gridRow = new();
                        PlotSignalPanel plotPanel = new(index);
                        plotSignalList.Add(plotPanel);
                        gridRow.Children.Add(plotPanel);
                        index++;

                        gridCol.RowDefinitions.Add(new RowDefinition());
                        gridCol.Children.Add(gridRow);

                        Grid.SetRow(gridRow, irow);
                    }
                }
            }
            ChangeSelectedElectrode();
        }
    }
}
