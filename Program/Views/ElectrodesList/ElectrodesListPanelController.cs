﻿using System.Collections.ObjectModel;
using System.Windows.Data;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;


namespace MEATaste.Views.ElectrodesList
{
    public class ElectrodesListPanelController
    {
        public ElectrodesListPanelModel Model { get; }

        private readonly ApplicationState state;

        public ElectrodesListPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;
            Model = new ElectrodesListPanelModel();

            eventSubscriber.Subscribe(EventType.CurrentExperimentChanged, LoadElectrodeListItems);
            eventSubscriber.Subscribe(EventType.SelectedElectrodeChanged, ChangeSelectedElectrode);
        }

 
        public void SelectElectrode(ElectrodeRecord electrodeRecord)
        {
            state.SelectedElectrode.Set(electrodeRecord);
        }

        private void ChangeSelectedElectrode()
        {
           var selectedElectrodeRecord = state.SelectedElectrode.Get();
           Model.ElectrodeListView.MoveCurrentTo(Model.SelectedElectrodeRecord);
        }

        private void LoadElectrodeListItems()
        {
            Model.Electrodes = new ObservableCollection<ElectrodeRecord>(state.CurrentMeaExperiment.Get().Descriptors.Electrodes);
            Model.ElectrodeListView = CollectionViewSource.GetDefaultView(Model.Electrodes);
        }
    }
}