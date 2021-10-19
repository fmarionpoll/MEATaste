
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using ScottPlot;


namespace MEATaste.Views.PlotSignal
{
    public partial class PlotSignalPanel 
    {
        private readonly PlotSignalPanelController controller;
        public int Id { get; set; }

        public PlotSignalPanel()
        {
            controller = App.ServiceProvider.GetService<PlotSignalPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }

        public PlotSignalPanel(int id)
        {
            controller = App.ServiceProvider.GetService<PlotSignalPanelController>();
            DataContext = controller!.Model;
            controller.Id = Id;
            this.Id = id;
            InitializeComponent();
        }

        //private void PlotControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    var wpfControl = sender as WpfPlot;
        //    controller.AttachControlToModel(Id, wpfControl);
        //}

        private void PlotControl_AxesChanged(object sender, System.EventArgs e)
        {
            controller.OnAxesChanged(sender, e);
        }

        public void UpdateChannelList(List<int> channelList)
        {
            controller.UpdateChannelList(Id, channelList);
        }

    }

}
