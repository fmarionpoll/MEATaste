using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            eventSubscriber.Subscribe(EventType.SelectedElectrodeChanged, ChangeSelectedElectrode);
        }
        
        public void SelectElectrode(int channel)
        {
            var electrodeDatas = state.MeaExperiment.Get().Electrodes;
            var electrodeData = electrodeDatas.Single(x => x.Electrode.Channel == channel);
            state.CurrentElectrode.Set(electrodeData.Electrode);
            ChangeSelectedElectrode();
        }

        private void ChangeSelectedElectrode()
        {
            var currentElectrode = state.CurrentElectrode.Get();
            ElectrodePropertiesExtended item = null;
            if (currentElectrode != null)
            {
                var channel = currentElectrode.Channel;
                item = electrodesExtendedPropertiesCollection.Single(x => x.Channel == channel);
            }
            Model.SelectedElectrodeExtendedProperties = item;
            dataGrid?.ScrollIntoView(item); 
           
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
           
            dataGrid = electrodesGrid;
            var electrodePropertiesExtended = (ElectrodePropertiesExtended)dataGrid.SelectedItem;
            SelectElectrode(electrodePropertiesExtended.Channel);
        }

        public void ElectrodesGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (sender is not DataGrid electrodesGrid) return;

            dataGrid = electrodesGrid;
            var electrodeProperties = (ElectrodePropertiesExtended)dataGrid.SelectedItem;
            var elState = state.CurrentElectrode.Get();
        }

    }
}