using MEATaste.DataMEA.MaxWell;
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
    }
}