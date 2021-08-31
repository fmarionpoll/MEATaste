using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using ScottPlot;


namespace MEATaste.Views.PlotFiltered
{
    public partial class PlotFilteredPanel : UserControl
    {
        private readonly PlotFilteredPanelController controller;
        public PlotFilteredPanel()
        {
            controller = App.ServiceProvider.GetService<PlotFilteredPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }

        private void CheckBox_Checked_1(object sender, System.Windows.RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            bool value = checkBox.IsChecked == true;
            controller.AuthorizeReading(value);
        }

        private void PlotControl_Loaded_1(object sender, System.Windows.RoutedEventArgs e)
        {
            var wpfControl = sender as WpfPlot;
            controller.AttachControlToModel(wpfControl);
        }

        private void PlotControl_AxesChanged_1(object sender, System.EventArgs e)
        {
            controller.OnAxesChanged(sender, e);
        }
    }
}
