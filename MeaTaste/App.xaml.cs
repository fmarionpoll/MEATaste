
using System;
using System.IO;
using System.Windows;
using MEATaste.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace MEATaste
{
    public partial class App : Application
    {
        private readonly ServiceProvider serviceProvider;
        public IServiceProvider ServiceProvider { get; private set; }
        //public IConfiguration Configuration { get; private set; }

        public App()
        {
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            //Configuration = builder.Build();

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
