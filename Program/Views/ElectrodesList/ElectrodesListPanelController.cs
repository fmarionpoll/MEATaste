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
            if (dataGrid == null) return;

            dataGrid.SelectedItems.Clear();
            
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
            var electrodePropertiesExtended = (IList<ElectrodePropertiesExtended>)dataGrid.SelectedItems;
            var listSelectedElectrodes = state.ListSelectedChannels.Get();
            listSelectedElectrodes.Clear();
            listSelectedElectrodes.AddRange(electrodePropertiesExtended.Select(item => item.Channel));
            state.ListSelectedChannels.Set(listSelectedElectrodes);
        }

    }
}