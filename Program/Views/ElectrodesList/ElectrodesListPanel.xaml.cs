using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;



namespace MEATaste.Views.ElectrodesList
{
    public partial class ElectrodesListPanel : UserControl
    {
        private readonly ElectrodesListPanelController controller;
        
        public ElectrodesListPanel()
        {
            controller = App.ServiceProvider.GetService<ElectrodesListPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }
        
        private void ElectrodesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           controller.ElectrodesGrid_SelectionChanged(sender, e);
        }

    }
}
