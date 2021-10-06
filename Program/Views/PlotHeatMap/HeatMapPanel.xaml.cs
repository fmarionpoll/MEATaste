using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ScottPlot;

namespace MEATaste.Views.PlotHeatMap
{
    public partial class HeatMapPanel
    {
        private readonly HeatMapPanelController controller;

        public HeatMapPanel()
        {
            controller = App.ServiceProvider.GetService<HeatMapPanelController>();
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
