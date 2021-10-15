using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;
using System.Windows.Controls;
using MEATaste.Views.Controls;

namespace MEATaste.Views.ElectrodesList
{
    public class ElectrodesListPanelController
    {
        public ElectrodesListPanelModel Model { get; }
        private ObservableCollection<ElectrodePropertiesExtended> electrodesExtendedPropertiesCollection;
        private DataGrid electrodeExtendedPropertiesGrid;
        private readonly ApplicationState state;
        private List<int> initialSelectedChannelsList;

        public ElectrodesListPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;
            Model = new ElectrodesListPanelModel();

            eventSubscriber.Subscribe(EventType.MeaExperimentChanged, LoadElectrodeListItems);
            eventSubscriber.Subscribe(EventType.SelectedChannelsChanged, SetSelectedChannels);
        }

        private void SetSelectedChannels()
        {
            var dictionary = state.DataSelected.Get(); 
            var listSelectedChannels = dictionary.Channels.Keys.ToList();
            if (electrodeExtendedPropertiesGrid == null || listSelectedChannels.Count == 0) return;

            var listSelectedElectrodes = GetSelectedChannelsFromDataGrid();
            if (dictionary.IsListEqualToStateSelectedItems(listSelectedElectrodes)) return;

            electrodeExtendedPropertiesGrid.Items
                .Cast<ElectrodePropertiesExtended>()
                .Where(item => listSelectedChannels.Any(channel => channel == item.Channel))
                .Iter(item => electrodeExtendedPropertiesGrid.SelectedItems.Add(item));
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
            if (sender is not DataGrid) return;

            var addedRows = e.AddedItems;
            var removedRows = e.RemovedItems;
            if (addedRows.Count == 0 && removedRows.Count == 0)
                return;
            var channelsFromDataGrid = GetSelectedChannelsFromDataGrid();

            var dictionary = state.DataSelected.Get();
            if (dictionary.IsListEqualToStateSelectedItems(channelsFromDataGrid)) return;

            dictionary.TrimDictionaryToList(channelsFromDataGrid);
            state.DataSelected.Set(dictionary);
        }

        private List<int> GetSelectedChannelsFromDataGrid()
        {
            if (electrodeExtendedPropertiesGrid == null)
                return null;
            var electrodePropertiesExtended = electrodeExtendedPropertiesGrid.SelectedItems.Cast<ElectrodePropertiesExtended>().ToList();
            var listSelectedElectrodes = new List<int>();
            listSelectedElectrodes.AddRange(electrodePropertiesExtended.Select(item => item.Channel));
            return listSelectedElectrodes;
        }

        public void ExpandSelection()
        {
            StoreSelectionIfEmpty();
            var listChannels = state.DataSelected.Get().Channels.Keys.ToList();
            var expandedSelectedChannelsList = GetAllElectrodesAroundCurrentSelection(listChannels, 20);

            List<ElectrodePropertiesExtended> selectedIElectrodePropertiesExtendeds = new();
            electrodeExtendedPropertiesGrid.Items
                .Cast<ElectrodePropertiesExtended>()
                .Where(item => expandedSelectedChannelsList.Any(channel => channel == item.Channel))
                .Iter(item => selectedIElectrodePropertiesExtendeds.Add(item));

            electrodeExtendedPropertiesGrid.UnselectAll();
            electrodeExtendedPropertiesGrid.SelectManyItems(selectedIElectrodePropertiesExtendeds);
        }

        private List<int> GetAllElectrodesAroundCurrentSelection(List<int> currentChannelsList, double delta)
        {
            List<int> expandedSelectedChannelsList = new();
            var meaExp = state.MeaExperiment.Get();
            foreach (var channel in currentChannelsList)
            {
                var electrode = meaExp.Electrodes.Single(x => x.Electrode.Channel == channel).Electrode;
                var xMax = electrode.XuM + delta;
                var xMin = electrode.XuM - delta;
                var yMax = electrode.YuM + delta;
                var yMin = electrode.YuM - delta;
                expandedSelectedChannelsList.Add(channel);
                expandedSelectedChannelsList.AddRange(from electrodeData in meaExp.Electrodes
                    where electrodeData.Electrode.Channel != channel
                    where !(electrodeData.Electrode.XuM > xMax)
                    where !(electrodeData.Electrode.XuM < xMin)
                    where !(electrodeData.Electrode.YuM > yMax)
                    where !(electrodeData.Electrode.YuM < yMin)
                    select electrodeData.Electrode.Channel);
            }

            return expandedSelectedChannelsList;
        }

        public void RestoreSelection()
        {
            List<ElectrodePropertiesExtended> selectedIElectrodePropertiesExtendeds = new();
            electrodeExtendedPropertiesGrid.Items
                .Cast<ElectrodePropertiesExtended>()
                .Where(item => initialSelectedChannelsList.Any(channel => channel == item.Channel))
                .Iter(item => selectedIElectrodePropertiesExtendeds.Add(item));
            electrodeExtendedPropertiesGrid.UnselectAll();
            electrodeExtendedPropertiesGrid.SelectManyItems(selectedIElectrodePropertiesExtendeds);

            initialSelectedChannelsList.Clear();
        }

        private void StoreSelectionIfEmpty()
        {
            if (initialSelectedChannelsList != null && initialSelectedChannelsList.Count > 0)
                return;
            initialSelectedChannelsList = GetSelectedChannelsFromDataGrid();
        }

        public void ElectrodesGridLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is not DataGrid electrodesGrid) return;
            electrodeExtendedPropertiesGrid = electrodesGrid;
        }

    }
}