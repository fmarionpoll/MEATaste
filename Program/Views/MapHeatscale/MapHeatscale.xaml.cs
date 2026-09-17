using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ScottPlot.WPF;

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

        private void HeatMapPanel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            controller.SetActive(IsVisible);
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e) => controller.ZoomIn();

        private void ZoomOut_Click(object sender, RoutedEventArgs e) => controller.ZoomOut();

        private void Fit_Click(object sender, RoutedEventArgs e) => controller.Fit();
    }
}
