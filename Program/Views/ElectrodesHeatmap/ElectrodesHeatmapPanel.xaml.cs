using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ScottPlot;

namespace MEATaste.Views.ElectrodesHeatmap
{
    public partial class ElectrodesHeatMapPanel
    {
        private readonly ElectrodeHeatmapPanelController controller;

        public ElectrodesHeatMapPanel()
        {
            controller = App.ServiceProvider.GetService<ElectrodeHeatmapPanelController>();
            DataContext = controller!.Model; 
            InitializeComponent();
        }

        private void PlotControl2_Loaded(object sender, RoutedEventArgs e)
        {
            var wpfControl = sender as WpfPlot;
            controller.AttachControlToModel(wpfControl);
        }
        
    }
}
