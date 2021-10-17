using MEATaste.Infrastructure;

namespace MEATaste.Views.MapsSwitch
{
    class MapsSwitchPanelController
    {
        public MapsSwitchPanelModel Model { get; }
        private readonly ApplicationState state;
        
        public MapsSwitchPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;
            Model = new MapsSwitchPanelModel();
        }

    }
}
