using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;



namespace MEATaste.Views.ElectrodesList
{
    public partial class ElectrodesListPanel
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

        private void expand_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            controller.ExpandSelectionOneLevel();
        }

        private void restore_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            controller.ReduceSelectionOneLevel();
        }

        private void ElectrodesGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            controller.ElectrodesGridLoaded(sender, e);
        }
    }
}
