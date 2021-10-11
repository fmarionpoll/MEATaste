using MEATaste.Infrastructure;

namespace MEATaste.Views.SwitchMaps
{
    class SwitchMapsPanelController
    {
        public SwitchMapsPanelModel Model { get; }
        private readonly ApplicationState state;
        
        public SwitchMapsPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;
            Model = new SwitchMapsPanelModel();
        }

    }
}
