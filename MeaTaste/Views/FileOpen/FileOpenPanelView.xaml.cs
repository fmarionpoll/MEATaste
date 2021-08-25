using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace MEATaste.Views.FileOpen
{
    public partial class FileOpenPanelView
    {
        private readonly FileOpenPanelController controller;

        public FileOpenPanelView()
        {
            controller = App.ServiceProvider.GetService<FileOpenPanelController>();
            DataContext = controller!.ViewModel;
            InitializeComponent();
        }

        private void OpenDialogButton_Click(object sender, System.Windows.RoutedEventArgs e) => controller.OpenFile();

    }
}
