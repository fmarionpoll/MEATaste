using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;


namespace MEATaste.Views.FileOpen
{
    public partial class FileOpenPanel : UserControl
    {
        private readonly FileOpenPanelController controller;

        public FileOpenPanel()
        {
            controller = App.ServiceProvider.GetService<FileOpenPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }

        private void OpenDialogButton_Click(object sender, System.Windows.RoutedEventArgs e) => controller.OpenFile();

        private void SaveCurrentClick(object sender, System.Windows.RoutedEventArgs e)
        {
            controller.SaveCurrentElectrodeDataClick();
        }

        private void SaveAllClick(object sender, System.Windows.RoutedEventArgs e)
        {
            controller.SaveAllElectrodesDataClick();
        }
    }
}
