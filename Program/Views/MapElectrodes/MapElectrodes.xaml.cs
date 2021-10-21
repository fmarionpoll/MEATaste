using Microsoft.Extensions.DependencyInjection;


namespace MEATaste.Views.MapElectrodes
{
    public partial class MapElectrodes
    {
        private readonly MapElectrodesController controller;

        public MapElectrodes()
        {
            controller = App.ServiceProvider.GetService<MapElectrodesController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }
    }
}
