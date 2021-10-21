using MEATaste.DataMEA.dbWave;
using MEATaste.DataMEA.MaxWell;
using MEATaste.Views.MapHeatscale;
using MEATaste.Views.MapElectrodes;
using MEATaste.Views.FileOpen;
using MEATaste.Views.ListElectrodes;
using MEATaste.Views.MainView;
using MEATaste.Views.PlotSignal;
using MEATaste.Views.PlotScrollBar;
using MEATaste.Views.SwitchGrids;
using MEATaste.Views.SwitchMaps;
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
                services.AddSingleton<H5FileReader>();
                services.AddSingleton<DataFileWriter>();
            }

            void ConfigureControllers()
            {
                services.AddSingleton<FileOpenPanelController>();
                services.AddSingleton<ListElectrodesPanelController>();
                services.AddSingleton<MapElectrodesController>();
                services.AddSingleton<MapHeatscalelController>();
                services.AddSingleton<PlotSignalPanelController>();
                services.AddTransient<PlotScrollBarPanelController>();
                services.AddSingleton<SwitchMapsPanelController>(); 
                services.AddSingleton<SwitchGridsPanelController>();
                services.AddSingleton<MainViewModel>();
            }
        }
    }
}
