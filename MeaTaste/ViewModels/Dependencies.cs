using MEATaste.DataMEA.MaxWell;
using Microsoft.Extensions.DependencyInjection;

namespace MEATaste.ViewModels
{
    public static class Dependencies
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            ConfigureHandlers();
            ConfigureViews();
            ConfigureViewModels();

            void ConfigureHandlers()
            {
                //var oldReader = new OldHdf5Reader();
                //var newReader = new NewHdf5Reader();

                //services.AddSingleton<GlobalHdf5Reader>();
                //services.AddSingleton<IHdf5Reader>(new GlobalHdf5Reader(oldReader, newReader));

                services.AddSingleton<ApplicationState>();
                services.AddSingleton<FileReader>();
                services.AddSingleton<MeaFileReader>();
            }

            void ConfigureViews()
            {
                services.AddSingleton<Views.FileOpenPanel>();
                services.AddSingleton<Views.ElectrodesMapPanel>();
                services.AddSingleton<Views.OneElectrodePanel>(); 
                services.AddSingleton<Views.MainWindow>();
            }
            
            void ConfigureViewModels()
            {
                services.AddSingleton<FileOpenPanelModel>();
                services.AddSingleton<ElectrodesMapPanelModel>();
                services.AddSingleton<OneElectrodePanelModel>();
                services.AddSingleton<MainWindowModel>();
            }
        }
    }
}
