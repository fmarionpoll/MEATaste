using System.Windows.Controls;
using MEATaste.DataMEA.Models;
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
            if (sender is not DataGrid electrodesGrid) return;

            ElectrodeRecord electrodeRecord = (ElectrodeRecord) electrodesGrid.SelectedItem;
            controller.SelectElectrode(electrodeRecord);
            electrodesGrid.ScrollIntoView(electrodesGrid.SelectedItem);
        }

    }
}
