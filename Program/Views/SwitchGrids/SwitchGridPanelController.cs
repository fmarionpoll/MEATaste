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
        private List<PlotSignalPanel> plotSignalList = new();

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
            if (plotSignalList.Count == 0 || listSelectedChannels.Count == 0) return;

            if (state.DataSelected.Get().IsLoadingDataFromFileNeeded())
                LoadDataFromFilev2();

            var index = 0;
            var range = listSelectedChannels.Count / plotSignalList.Count;
            if (range == 0) range = 1;
            foreach (var plotSignalPanel in plotSignalList)
            {
                var dummyList = listSelectedChannels.GetRange(index, range);
                plotSignalPanel.UpdateChannelList(dummyList);
                index++;
                if (index >= listSelectedChannels.Count)
                    break;
            }
        }

        private void LoadDataFromFilev2()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            H5FileReader.A13ReadAllDataFromChannelsParallel(state.DataSelected.Get());
            sw.Stop();
            Trace.WriteLine($"--> nchannels= {state.DataSelected.Get().Channels.Count} read in t(s)= {sw.Elapsed.Seconds}");
            Mouse.OverrideCursor = null;
        }

        public void DoIt(Grid rootGrid)
        {
            if (Model.NColumns * Model.NRows != plotSignalList.Count)
            {
                rootGrid.Children.Clear();
                plotSignalList.Clear();
                Grid mainGrid = new();
                rootGrid.Children.Add(mainGrid);
               
                for (var column = 0; column < Model.NColumns; column++)
                {
                    Grid gridCol = new();
                    mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    mainGrid.Children.Add(gridCol);
                    Grid.SetColumn(gridCol, column);

                    for (var row = 0; row < Model.NRows; row++)
                    {
                        PlotSignalPanel plotPanel = new();
                        //TODO : stpre row & col
                        plotSignalList.Add(plotPanel);
                        
                        Grid gridRow = new();
                        gridRow.Children.Add(plotPanel);

                        gridCol.RowDefinitions.Add(new RowDefinition());
                        gridCol.Children.Add(gridRow);

                        Grid.SetRow(gridRow, row);
                    }
                }
            }
            ChangeSelectedElectrode();
        }
    }
}
