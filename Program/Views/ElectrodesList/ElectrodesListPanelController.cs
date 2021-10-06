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

            eventSubscriber.Subscribe(EventType.CurrentExperimentChanged, LoadElectrodeListItems);
            eventSubscriber.Subscribe(EventType.SelectedChannelsChanged, SetSelectedChannels);
        }

        private void SetSelectedChannels()
        {
            var selectedChannels = state.ListSelectedChannels.Get();
            if (dataGrid == null || selectedChannels == null) return;

            var listSelectedElectrodes = GetSelectedChannelsFromDataGrid();
            if (IsListEqualToStateSelectedItems(listSelectedElectrodes)) return;

            foreach (var item in dataGrid.Items)
            {
                var row = (ElectrodePropertiesExtended) item;
                var channel = row.Channel;
                for (var j = 0; j < selectedChannels.Count; j++)
                {
                    if (j != channel) continue;
                    dataGrid.SelectedItems.Add(row);
                    break;
                }
            }

        }

        private void LoadElectrodeListItems()
        {
            var electrodeDatas = state.MeaExperiment.Get().Electrodes;
            if (electrodesExtendedPropertiesCollection != null &&
                electrodeDatas.Length == electrodesExtendedPropertiesCollection.Count)
            {
                if (IsDataGridChannelsContentEqualToStateElectrodeData())
                    return;
            }

            electrodesExtendedPropertiesCollection = new ObservableCollection<ElectrodePropertiesExtended>();
            foreach (var electrodeData in electrodeDatas)
            {
                var electrodePropertiesExtended = new ElectrodePropertiesExtended(electrodeData);
                electrodesExtendedPropertiesCollection.Add(electrodePropertiesExtended);
            }
            Model.ElectrodeListView = CollectionViewSource.GetDefaultView(electrodesExtendedPropertiesCollection);
        }

        private bool IsDataGridChannelsContentEqualToStateElectrodeData()
        {
            var list1 = new List<int>();
            foreach (ElectrodePropertiesExtended item in dataGrid.Items) list1.Add( item.Channel);
            
            var list2 = new List<int>();
            foreach (var electrodeData in state.MeaExperiment.Get().Electrodes) list2.Add(electrodeData.Electrode.Channel);

            var set = new HashSet<int>(list1);
            var equals = set.SetEquals(list2);
            return equals;
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
            if (listState == null)
            {
                return false;
            }
            var set = new HashSet<int>(listState);
            var equals = set.SetEquals(listSelectedElectrodes);
            return equals;
        }

    }
}