using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace MEATaste.Views.SwitchGrids
{
    public partial class SwitchGridsPanel : UserControl
    {
        private readonly SwitchGridsPanelController controller;

        public SwitchGridsPanel()
        {
            controller = App.ServiceProvider.GetService<SwitchGridsPanelController>();
            DataContext = controller!.Model; InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            controller.DoIt(Root);
        }
    }
}
