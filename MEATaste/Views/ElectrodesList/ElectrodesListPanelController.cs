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
            FileOpenPanelModel.NewFileIsLoadedAction += FillTable;
        }

        public void FillTable()
        {
            Model.ElectrodesTable = new ObservableCollection<Electrode>();
            foreach (Electrode electrode in state.CurrentMeaExperiment.Descriptors.Electrodes)
            {
                Model.ElectrodesTable.Add(electrode);
            }
        }

        public void SelectedRow(int selectedRowIndex)
        {
            Model.SelectedElectrodeIndex = selectedRowIndex;
        }

        public int GetElectrodeChannel(int index) =>
            state.CurrentMeaExperiment.Descriptors.Electrodes[index].ChannelNumber;
    }
}