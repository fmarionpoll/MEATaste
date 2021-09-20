using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Data;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;
using System.Windows.Controls;
using System.Windows;



namespace MEATaste.Views.ElectrodesList
{
    public class ElectrodesListPanelController
    {
        public ElectrodesListPanelModel Model { get; }
        private ObservableCollection<ElectrodeProperties> electrodesList;
        private DataGrid dataGrid;
        private readonly ApplicationState state;

        public ElectrodesListPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;
            Model = new ElectrodesListPanelModel();

            eventSubscriber.Subscribe(EventType.CurrentExperimentChanged, LoadElectrodeListItems);
            eventSubscriber.Subscribe(EventType.SelectedElectrodeChanged, ChangeSelectedElectrode);
        }
        
        public void SelectElectrode(ElectrodeProperties electrodeProperties)
        {
            state.CurrentElectrode.Set(electrodeProperties);
            ChangeSelectedElectrode();
        }

        private void ChangeSelectedElectrode()
        {
            Model.SelectedElectrodeProperties = state.CurrentElectrode.Get();
            Model.ElectrodeListView.MoveCurrentTo(Model.SelectedElectrodeProperties);
            dataGrid?.ScrollIntoView(dataGrid.SelectedItem);
        }

        private void LoadElectrodeListItems()
        {
            var array = state.CurrentExperiment.Get().Descriptors.Electrodes;
            electrodesList = new ObservableCollection<ElectrodeProperties>(array);
            Model.ElectrodeListView = CollectionViewSource.GetDefaultView(electrodesList);
        }

        public void ElectrodesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not DataGrid electrodesGrid) return;
            Trace.WriteLine("SelectionChanged");

            dataGrid = electrodesGrid;
            var electrodeProperties = (ElectrodeProperties)dataGrid.SelectedItem;
            SelectElectrode(electrodeProperties);
            ChangeSelectedElectrode();
        }

        public void ElectrodesGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            Trace.WriteLine("SelectedCellsChanged");
        }

    }
}