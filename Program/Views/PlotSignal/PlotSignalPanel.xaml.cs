using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;


namespace MEATaste.Views.PlotSignal
{

    public partial class PlotSignalPanel : UserControl
    {
        private readonly PlotSignalPanelController controller;
 
        public PlotSignalPanel()
        {
            controller = App.ServiceProvider.GetService<PlotSignalPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }

        private void PlotControl_AxesChanged(object sender, System.EventArgs e)
        {
            controller.OnAxesChanged(sender, e);
        }

        public void UpdateChannelList(List<int> channelList)
        {
            controller.UpdateChannelList(channelList);
        }

    }

}

