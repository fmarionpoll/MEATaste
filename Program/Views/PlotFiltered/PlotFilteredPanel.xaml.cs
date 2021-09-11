using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using ScottPlot;


namespace MEATaste.Views.PlotFiltered
{
    public partial class PlotFilteredPanel
    {
        private readonly PlotFilteredPanelController controller;

        public PlotFilteredPanel()
        {
            controller = App.ServiceProvider.GetService<PlotFilteredPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (controller != null && sender is ComboBox combo)
                controller.ChangeFilter(combo.SelectedIndex);
        }

        private void CheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var value = checkBox?.IsChecked == true;
            controller.AuthorizeReading(value);
        }
    }
}
