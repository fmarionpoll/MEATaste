using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;


namespace MEATaste.Views.MapsSwitch
{
    public partial class MapsSwitchPanel
    {
        private readonly MapsSwitchPanelController controller;
        
        public MapsSwitchPanel()
        {
            controller = App.ServiceProvider.GetService<MapsSwitchPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }
    }
}
