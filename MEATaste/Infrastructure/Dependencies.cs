using MEATaste.DataMEA.MaxWell;
using MEATaste.Views.ElectrodesList;
using MEATaste.Views.ElectrodesMap;
using MEATaste.Views.FileOpen;
using MEATaste.Views.MainView;
using MEATaste.Views.OneElectrode;
using Microsoft.Extensions.DependencyInjection;

namespace MEATaste.Infrastructure
{
    public static class Dependencies
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainView>();
            ConfigureHandlers();
            ConfigureControllers();

            void ConfigureHandlers()
            {
                var eventBus = new EventBus();
                services.AddSingleton<IEventRaiser>(eventBus);
                services.AddSingleton<IEventSubscriber>(eventBus);

                services.AddSingleton<StatePropertyFactory>();
                services.AddSingleton<ApplicationState>();
                services.AddSingleton<FileReader>();
                services.AddSingleton<MeaFileReader>();
            }

            void ConfigureControllers()
            {
                services.AddSingleton<FileOpenPanelController>();
                services.AddSingleton<ElectrodesListPanelController>();
                services.AddSingleton<ElectrodesMapPanelController>(); 
                services.AddSingleton<OneElectrodePanelController>();
                services.AddSingleton<MainViewModel>();
            }
        }
    }
}
