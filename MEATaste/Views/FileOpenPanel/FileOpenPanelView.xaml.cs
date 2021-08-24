using Microsoft.Extensions.DependencyInjection;

namespace MEATaste.Views.FileOpenPanel
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
