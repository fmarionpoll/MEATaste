using System.Collections.ObjectModel;
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

        //private void SetCurrentElectrodeIndexFromChannelNumber(int selectedChannel)
        //{
        //    var currentExperiment = state.CurrentMeaExperiment.Get();

        //    var selectedElectrode = currentExperiment.Descriptors.Electrodes.FirstOrDefault(x => x.ChannelNumber == selectedChannel);
        //    if (selectedElectrode == null) return;

        //    state.SelectedElectrode.Set(selectedElectrode);
        //    Model.SelectedElectrodeChannelNumber = selectedChannel;
        //}

        public void SelectElectrode(Electrode electrode) => state.SelectedElectrode.Set(electrode);

        private void ChangeSelectedElectrode() => Model.SelectedElectrode = state.SelectedElectrode.Get();


        private void LoadElectrodeListItems() => 
            Model.Electrodes = new ObservableCollection<Electrode>(state.CurrentMeaExperiment.Get().Descriptors.Electrodes);
    }
}