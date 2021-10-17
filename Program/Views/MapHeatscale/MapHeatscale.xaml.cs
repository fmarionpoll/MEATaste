using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ScottPlot;

namespace MEATaste.Views.MapHeatscale
{
    public partial class ElectrodesHeatMapPanel
    {
        private readonly MapHeatscalelController controller;

        public ElectrodesHeatMapPanel()
        {
            controller = App.ServiceProvider.GetService<MapHeatscalelController>();
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
