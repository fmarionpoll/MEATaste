using Microsoft.Extensions.DependencyInjection;


namespace MEATaste.Views.ElectrodesMap
{

    public partial class ElectrodesMapPanel
    {
        private readonly ElectrodesMapPanelController controller;

        public ElectrodesMapPanel()
        {
            controller = App.ServiceProvider.GetService<ElectrodesMapPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }

        
    }
}
