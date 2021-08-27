using System.Collections.ObjectModel;
using MEATaste.DataMEA.MaxWell;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;
using MEATaste.Views.FileOpen;



namespace MEATaste.Views.ElectrodesList
{
    public class ElectrodesListPanelController
    {
        public ElectrodesListPanelModel Model { get; }

        private readonly MeaFileReader meaFileReader;
        private readonly ApplicationState state;

        public ElectrodesListPanelController(MeaFileReader meaFileReader, ApplicationState state)
        {
            this.meaFileReader = meaFileReader;
            this.state = state;

            Model = new ElectrodesListPanelModel();
            FileOpenPanelModel.NewFileIsLoadedAction += TableLoadData;
        }

        public void TableLoadData()
        {
            Model.ElectrodesTableModel = new ObservableCollection<Electrode>();
            foreach (Electrode electrode in state.CurrentMeaExperiment.Descriptors.Electrodes)
            {
                Model.ElectrodesTableModel.Add(electrode);
            }
        }


        public void SelectedChannel(int selectedChannel)
        {
            
            foreach (Electrode electrode in state.CurrentMeaExperiment.Descriptors.Electrodes)
            {
                if (electrode.ChannelNumber != selectedChannel) continue;
                Model.SelectedElectrodeChannelNumber = selectedChannel;
                break;
            }
        }

        public int GetElectrodeChannel(int index) =>
            state.CurrentMeaExperiment.Descriptors.Electrodes[index].ChannelNumber;

    }
}