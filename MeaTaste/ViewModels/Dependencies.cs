using Microsoft.Extensions.DependencyInjection;
using TasteMEA.DataMEA.MaxWell;

namespace TasteMEA.ViewModels
{
    public static class Dependencies
    {
        public static void ConfigureServices(ServiceCollection services)
        {
            ConfigureHandlers();
            ConfigureWindows();

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

            void ConfigureWindows()
            {
                services.AddSingleton<Views.FileOpenedPanel>();
                services.AddSingleton<Views.ElectrodesMapPanel>();
                services.AddSingleton<Views.OneElectrodePanel>(); 
                services.AddSingleton<Views.MainWindow>();
            }
        }
    }
}
