using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;


namespace MEATaste.Views.ElectrodesMap
{

    public partial class ElectrodesMapPanel : UserControl
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
