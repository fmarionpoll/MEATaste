using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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

 
        public void SelectElectrode(Electrode electrode) => state.SelectedElectrode.Set(electrode);

        private void ChangeSelectedElectrode()
        {
            Model.SelectedElectrode = state.SelectedElectrode.Get();
            Trace.WriteLine($"selected electrode is ={Model.SelectedElectrode}");
            // center row and select in red?
            Model.ElectedElectrodeIndex =
        }

        private void LoadElectrodeListItems() => 
            Model.Electrodes = new ObservableCollection<Electrode>(state.CurrentMeaExperiment.Get().Descriptors.Electrodes);
    }
}