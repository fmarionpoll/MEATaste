using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;


namespace MEATaste.Views.PlotSignal
{

    public partial class PlotSignalPanel
    {
        private readonly PlotSignalPanelController controller;
 
        public PlotSignalPanel()
        {
            controller = App.ServiceProvider.GetService<PlotSignalPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }

        public void UpdateChannelList(List<int> channelList)
        {
            controller.UpdateChannelList(channelList);
        }

    }

}

