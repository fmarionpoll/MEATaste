using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;


namespace MEATaste.Views.PlotScrollBar
{
    
    public partial class PlotScrollBarPanel : UserControl
    {
        private readonly PlotScrollBarPanelController controller;


        public PlotScrollBarPanel()
        {
            controller = App.ServiceProvider.GetService<PlotScrollBarPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }
    }
}
