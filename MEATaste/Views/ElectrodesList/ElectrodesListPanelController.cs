using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public void SetCurrentElectrodeIndexFromChannelNumber(int selectedChannel)
        {
            foreach (Electrode electrode in state.CurrentMeaExperiment.Descriptors.Electrodes)
            {
                if (electrode.ChannelNumber != selectedChannel) continue;

                state.CurrentMeaExperiment.CurrentElectrodesIndex =
                    GetIndexOfElectrodeFromChannelNumber(selectedChannel);

                //int indexCurrent = state.CurrentMeaExperiment.CurrentElectrodesIndex;
                //Electrode electrodeCurrent = state.CurrentMeaExperiment.Descriptors.Electrodes[indexCurrent];
                //Trace.WriteLine(
                //    $"List: electrode = {electrodeCurrent}");
                Model.SelectedElectrodeChannelNumber = selectedChannel;
                break;
            }
        }

        private int GetIndexOfElectrodeFromChannelNumber(int selectedChannel)
        {
            int indexElectrode = -1;
            for (int i = 0; i < state.CurrentMeaExperiment.Descriptors.Electrodes.Length; i++)
            {
                var electrode = state.CurrentMeaExperiment.Descriptors.Electrodes[i];
                if (electrode.ChannelNumber != selectedChannel) continue;
                indexElectrode = i;
                break;
            }

            return indexElectrode;
        }


    }
}