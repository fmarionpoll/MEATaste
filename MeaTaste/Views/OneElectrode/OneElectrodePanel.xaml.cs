using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;


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
    }

}
