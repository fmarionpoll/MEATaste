using MEATaste.Infrastructure;

namespace MEATaste.Views.Grid3x3
{
    class Grid3X3Controller
    {
        public Grid3X3Model Model { get; }
        private readonly ApplicationState state;

        public Grid3X3Controller(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;
            Model = new Grid3X3Model();
        }
    }
}
