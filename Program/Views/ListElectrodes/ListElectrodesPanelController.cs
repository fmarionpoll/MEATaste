﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;
using MEATaste.Views.Controls;

namespace MEATaste.Views.ListElectrodes
{
    public class ListElectrodesPanelController
    {
        public ListElectrodesPanelModel Model { get; }
        private ObservableCollection<ElectrodePropertiesExtended> electrodesExtendedPropertiesCollection;
        private DataGrid electrodeExtendedPropertiesGrid;
        private readonly ApplicationState state;


        public ListElectrodesPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;
            Model = new ListElectrodesPanelModel();

            eventSubscriber.Subscribe(EventType.MeaExperimentChanged, LoadElectrodeListItems);
            eventSubscriber.Subscribe(EventType.SelectedChannelsChanged, StateSelectedChannelsChanged);
        }

        private void StateSelectedChannelsChanged()
        {
            SilentSelectRows(state.DataSelected.Get().Channels.Keys.ToList());
        }

        private void SilentSelectRows(List<int> selectList)
        {
            if (electrodeExtendedPropertiesGrid == null || selectList.Count == 0) return;

            var gridSelectedChannels = GetSelectedChannelsFromDataGrid();
            var set = new HashSet<int>(selectList);
            if(set.SetEquals(gridSelectedChannels))
                return;
            
            if (gridSelectedChannels is { Count: > 0 })
            {
                var unselectList = gridSelectedChannels.Except(selectList).ToList();
                if (unselectList.Any())
                    SilentlyUnSelectRows(unselectList);
            }

            SilentlySelectRows(selectList);
            CollectionViewSource.GetDefaultView(electrodeExtendedPropertiesGrid.ItemsSource).Refresh();
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
        
        public void ElectrodesGridLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is not DataGrid electrodesGrid) return;
            electrodeExtendedPropertiesGrid = electrodesGrid;
        }

        private void SilentlySelectRows(List<int> channelsToAdd)
        {
            if (channelsToAdd == null) return;
            List<ElectrodePropertiesExtended> selectedRows = new();
            electrodeExtendedPropertiesGrid.Items
                .Cast<ElectrodePropertiesExtended>()
                .Where(item => channelsToAdd.Any(channel => channel == item.Channel))
                .Iter(item => selectedRows.Add(item));

            electrodeExtendedPropertiesGrid.SelectManyItems(selectedRows);
        }

        private void SilentlyUnSelectRows(List<int> channelsToRemove)
        {
            if (channelsToRemove == null) return;
            List<ElectrodePropertiesExtended> unSelectedRows = new();
            electrodeExtendedPropertiesGrid.Items
                .Cast<ElectrodePropertiesExtended>()
                .Where(item => channelsToRemove.Any(channel => channel == item.Channel))
                .Iter(item => unSelectedRows.Add(item));

            electrodeExtendedPropertiesGrid.UnSelectManyItems(unSelectedRows);
        }

    }
}