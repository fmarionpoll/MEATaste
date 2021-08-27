using Microsoft.Extensions.DependencyInjection;
using OxyPlot.Wpf;
using System.Windows.Controls;


namespace MEATaste.Views.ElectrodesMap
{


    public partial class ElectrodesMapPanel : UserControl
    {
        private readonly ElectrodesMapPanelController controller;
        private static readonly PlotView PlotView = new PlotView();

        public ElectrodesMapPanel()
        {
            controller = App.ServiceProvider.GetService<ElectrodesMapPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }


        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
