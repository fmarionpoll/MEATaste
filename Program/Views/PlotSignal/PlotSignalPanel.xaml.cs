
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using ScottPlot;


namespace MEATaste.Views.PlotSignal
{
    public partial class PlotSignalPanel 
    {
        private readonly PlotSignalPanelController controller;

        public PlotSignalPanel()
        {
            controller = App.ServiceProvider.GetService<PlotSignalPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }

        private void PlotControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var wpfControl = sender as WpfPlot;
            controller.AttachControlToModel(wpfControl);
        }

        private void PlotControl_AxesChanged(object sender, System.EventArgs e)
        {
            controller.OnAxesChanged(sender, e);
        }


    }

}
