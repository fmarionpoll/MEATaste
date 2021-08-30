using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace MEATaste.Views.FileOpen
{
    public partial class FileOpenPanel
    {
        private readonly FileOpenPanelController controller;

        public FileOpenPanel()
        {
            controller = App.ServiceProvider.GetService<FileOpenPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }

        private void OpenDialogButton_Click(object sender, System.Windows.RoutedEventArgs e) => controller.OpenFile();

    }
}
