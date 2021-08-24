﻿using MEATaste.DataMEA.MaxWell;
using MEATaste.ViewModels;
using MEATaste.Views.FileOpenPanel;
using Microsoft.Extensions.DependencyInjection;

namespace MEATaste.Infrastructure
{
    public static class Dependencies
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Views.MainWindow>();
            ConfigureHandlers();
            ConfigureControllers();

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

            void ConfigureControllers()
            {
                services.AddSingleton<FileOpenPanelController>();
                services.AddSingleton<ElectrodesMapPanelModel>();
                services.AddSingleton<OneElectrodePanelModel>();
                services.AddSingleton<MainWindowModel>();
            }
        }
    }
}
