using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace MEATaste.Views.ElectrodesList
{
    public partial class ElectrodesListPanelView : UserControl
    {
        private readonly ElectrodesListPanelController controller;

        public ElectrodesListPanelView()
        {
            controller = App.ServiceProvider.GetService<ElectrodesListPanelController>();
            DataContext = controller!.ViewModel;
            InitializeComponent();
        }
    }
}
