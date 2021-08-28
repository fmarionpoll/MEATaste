using MEATaste.DataMEA.MaxWell;
using MEATaste.Infrastructure;

namespace MEATaste.Views.OneElectrode
{
    class OneElectrodePanelController
    {
        public OneElectrodePanelModel Model { get; }

        private readonly MeaFileReader meaFileReader;
        private readonly ApplicationState state;

        public OneElectrodePanelController(MeaFileReader meaFileReader, ApplicationState state)
        {
            this.meaFileReader = meaFileReader;
            this.state = state;

            Model = new OneElectrodePanelModel();
            
        }
    }
}
