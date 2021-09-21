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
            Trace.WriteLine($"SelectElectrode({electrodeProperties.Electrode})");

            state.CurrentElectrode.Set(electrodeProperties);
            ChangeSelectedElectrode();
        }

        private void ChangeSelectedElectrode()
        {
            Trace.WriteLine("ChangeSelectedElectrode()");

            var stateElectrodeProperties = state.CurrentElectrode.Get();
            if (Model.SelectedElectrodeProperties == null)
            {
                Trace.WriteLine("ChangeSelectedElectrode() -- State.SelectedElectrode=NULL");
                return;
            }

            Model.SelectedElectrodeProperties = stateElectrodeProperties;
            Model.ElectrodeListView.MoveCurrentTo(Model.SelectedElectrodeProperties);
            Trace.WriteLine($"ChangeSelectedElectrode() -- dataGrid.SelectedItem={dataGrid.SelectedItem}");

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
           
            dataGrid = electrodesGrid;
            var electrodeProperties = (ElectrodeProperties)dataGrid.SelectedItem;

            var elState = state.CurrentElectrode.Get();
            if (elState != null)
            {
                Trace.WriteLine($"SelectionChanged to dataGrid electrode={electrodeProperties.Electrode}"
                                + $" with Current electrode={elState.Electrode}");
            }
            else
            {
                Trace.WriteLine($"SelectionChanged to dataGrid electrode={electrodeProperties.Electrode} state electrode=NULL");
            }
            SelectElectrode(electrodeProperties);
        }

        public void ElectrodesGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (sender is not DataGrid electrodesGrid) return;

            dataGrid = electrodesGrid;
            var electrodeProperties = (ElectrodeProperties)dataGrid.SelectedItem;
            var elState = state.CurrentElectrode.Get();
            Trace.WriteLine($"===> SelectedCellsChanged to dataGrid electrode={electrodeProperties.Electrode}"
                            + $" with Current electrode={elState.Electrode}");
        }

    }
}