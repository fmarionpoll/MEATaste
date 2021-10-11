using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;


namespace MEATaste.Views.SwitchMaps
{
    public partial class SwitchMapsPanel : UserControl
    {
        private readonly SwitchMapsPanelController controller;
        
        public SwitchMapsPanel()
        {
            controller = App.ServiceProvider.GetService<SwitchMapsPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }
    }
}
