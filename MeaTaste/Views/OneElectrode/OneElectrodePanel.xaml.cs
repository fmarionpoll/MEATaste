using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using ScottPlot;


namespace MEATaste.Views.OneElectrode
{
  
    public partial class OneElectrodePanel
    {
        private readonly OneElectrodePanelController controller;

        public OneElectrodePanel()
        {
            controller = App.ServiceProvider.GetService<OneElectrodePanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            bool value = checkBox.IsChecked == true;
            controller.AuthorizeReading(value);
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
