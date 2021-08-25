using System.Collections.ObjectModel;
using MEATaste.DataMEA.MaxWell;
using MEATaste.DataMEA.Models;
using MEATaste.Infrastructure;

namespace MEATaste.Views.ElectrodesList
{
    public class ElectrodesListPanelController
    {
        public ElectrodesListPanelViewModel ViewModel { get; }

        private readonly MeaFileReader meaFileReader;
        private readonly ApplicationState state;

        public ElectrodesListPanelController(MeaFileReader meaFileReader, ApplicationState state)
        {
            this.meaFileReader = meaFileReader;
            this.state = state;

            ViewModel = new ElectrodesListPanelViewModel();
        }

        public void FillTable()
        {
            ViewModel.ElectrodesTable = new ObservableCollection<Electrode>();
            foreach (Electrode electrode in state.CurrentMeaExperiment.Descriptors.Electrodes)
            {
                ViewModel.ElectrodesTable.Add(electrode);
            }
        }

        public void SelectedRow(int selectedRowIndex)
        {
            ViewModel.SelectedElectrodesTableIndex = selectedRowIndex;
        }
    }
}