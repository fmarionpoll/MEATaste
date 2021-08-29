using System.Windows.Controls;
using MEATaste.DataMEA.Models;
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
            var electrodesGrid = sender as DataGrid;
            if (electrodesGrid == null) return;

            Electrode electrode = (Electrode) electrodesGrid.SelectedItem;
            controller.SelectElectrode(electrode);
            electrodesGrid.ScrollIntoView(electrodesGrid.SelectedItem);
        }

    }
}
