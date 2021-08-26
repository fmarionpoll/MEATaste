using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;


namespace MEATaste.Views.ElectrodesMap
{
    public partial class ElectrodesMapPanelView : UserControl
    {
        private readonly ElectrodeMapPanelController controller;

        public ElectrodesMapPanelView()
        {
            controller = App.ServiceProvider.GetService<ElectrodeMapPanelController>();
            DataContext = controller!.ViewModel;
            InitializeComponent();
        }

        //private void ElectrodesMap_MouseMove(object sender, MouseEventArgs e)
        //{
        //    MoveCursorNearPoint(sender);
        //}

        //private void ElectrodesMap_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    int pointIndex = MoveCursorNearPoint(sender);
        //    Electrode electrode = state.CurrentMeaExperiment.Descriptors.Electrodes[pointIndex];
        //    // TODO: not sure this is right (search item first then select?)
        //    //UpdateSelectedElectrode(electrode);
        //    //ListViewChannels.SelectedIndex = pointIndex;
        //}
    }
}
