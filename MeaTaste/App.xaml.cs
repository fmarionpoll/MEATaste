using System.Windows;
using MEATaste.Infrastructure;
using MEATaste.Views.MainWindow;
using Microsoft.Extensions.DependencyInjection;


namespace MEATaste
{
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get ; private set; }
        private readonly ServiceProvider serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            Dependencies.ConfigureServices(services);
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            ServiceProvider = serviceProvider;
            var mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow!.Show();
        }
    }
}
