using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using OxyPlot.Wpf;


namespace MEATaste.Views.ElectrodesList
{
    public partial class ElectrodesListPanelView : UserControl
    {
        private readonly ElectrodesListPanelController controller;
        private static readonly PlotView PlotView = new PlotView();

        public ElectrodesListPanelView()
        {
            controller = App.ServiceProvider.GetService<ElectrodesListPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }

        private void ElectrodesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg == null) return;
            var index = dg.SelectedIndex;
            controller.SelectedRow(index);
        }

    }
}
