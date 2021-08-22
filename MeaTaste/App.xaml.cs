
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TasteMEA.Infrastructure;


namespace TasteMEA
{
    public partial class App : Application
    {
        private readonly ServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            Dependencies.ConfigureServices(services);
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = serviceProvider.GetService<Views.MainWindow>();
            mainWindow.Show();
        }
    }
}
