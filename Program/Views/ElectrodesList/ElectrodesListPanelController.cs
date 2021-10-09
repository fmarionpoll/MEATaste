using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;
using System.Windows.Controls;

namespace MEATaste.Views.ElectrodesList
{
    public class ElectrodesListPanelController
    {
        public ElectrodesListPanelModel Model { get; }
        private ObservableCollection<ElectrodePropertiesExtended> electrodesExtendedPropertiesCollection;
        private DataGrid dataGrid;
        private readonly ApplicationState state;

        public ElectrodesListPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;
            Model = new ElectrodesListPanelModel();

            eventSubscriber.Subscribe(EventType.MeaExperimentChanged, LoadElectrodeListItems);
            eventSubscriber.Subscribe(EventType.SelectedChannelsChanged, SetSelectedChannels);
        }

        private void SetSelectedChannels()
        {
            var selectedChannels = state.ListSelectedChannels.Get();
            if (dataGrid == null || selectedChannels == null) return;

            var listSelectedElectrodes = GetSelectedChannelsFromDataGrid();
            if (IsListEqualToStateSelectedItems(listSelectedElectrodes)) return;

            dataGrid.Items
                .Cast<ElectrodePropertiesExtended>()
                .Where(item => selectedChannels.Any(channel => channel == item.Channel))
                .Iter(item => dataGrid.SelectedItems.Add(item));
        }

        private void LoadElectrodeListItems()
        {
            var electrodeDatas = state.MeaExperiment.Get().Electrodes;
            electrodesExtendedPropertiesCollection = new ObservableCollection<ElectrodePropertiesExtended>();
            foreach (var electrodeData in electrodeDatas)
            {
                var electrodePropertiesExtended = new ElectrodePropertiesExtended(electrodeData);
                electrodesExtendedPropertiesCollection.Add(electrodePropertiesExtended);
            }
            Model.ElectrodeListView = CollectionViewSource.GetDefaultView(electrodesExtendedPropertiesCollection);
        }

        public void ElectrodesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not DataGrid electrodesGrid) return;

            var addedRows = e.AddedItems;
            var removedRows = e.RemovedItems;
            if (addedRows.Count == 0 && removedRows.Count == 0)
                return;
            dataGrid = electrodesGrid;
            var listSelectedElectrodes = GetSelectedChannelsFromDataGrid(); 

            if (!IsListEqualToStateSelectedItems(listSelectedElectrodes))
            {
                state.ListSelectedChannels.Set(listSelectedElectrodes);
            }
            
        }

        private List<int> GetSelectedChannelsFromDataGrid()
        {
            var electrodePropertiesExtended = dataGrid.SelectedItems.Cast<ElectrodePropertiesExtended>().ToList();
            var listSelectedElectrodes = new List<int>();
            listSelectedElectrodes.AddRange(electrodePropertiesExtended.Select(item => item.Channel));
            return listSelectedElectrodes;
        }

        private bool IsListEqualToStateSelectedItems(List<int> listSelectedElectrodes)
        {
            var listState = state.ListSelectedChannels.Get();
            if (listState == null) return false;
            
            var set = new HashSet<int>(listState);
            var equals = set.SetEquals(listSelectedElectrodes);
            return equals;
        }

    }
}