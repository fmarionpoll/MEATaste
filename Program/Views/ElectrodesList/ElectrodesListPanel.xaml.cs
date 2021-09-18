using System.Diagnostics;
using System.Windows;
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
            Trace.WriteLine("SelectionChanged");
            controller.ElectrodesGrid_SelectionChanged(sender, e);
        }

        private void ElectrodesGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            Trace.WriteLine("SelectedCellsChanged");
        }
    }
}
