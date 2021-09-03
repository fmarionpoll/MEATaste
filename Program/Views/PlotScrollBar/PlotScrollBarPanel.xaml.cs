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

        private void Slider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            controller.Slider_ValueChanged(sender, e);
        }

        private void ScrollBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            controller.SliderBar_Scroll(sender, e);
        }
    }
}
