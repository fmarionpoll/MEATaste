using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using ScottPlot.WPF;

namespace MEATaste.Views.MapVoltage
{
    public partial class VoltageMapPanel
    {
        private readonly MapVoltageController controller;

        public VoltageMapPanel()
        {
            controller = App.ServiceProvider.GetService<MapVoltageController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }

        private void PlotControl_Loaded(object sender, RoutedEventArgs e)
        {
            controller.AttachControlToModel(sender as WpfPlot);
        }

        private void VoltageMapPanel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            controller.SetActive(IsVisible);
        }

        private void SkipBack_Click(object sender, RoutedEventArgs e) => controller.StepLarge(-1);

        private void StepBack_Click(object sender, RoutedEventArgs e) => controller.StepSmall(-1);

        private void StepForward_Click(object sender, RoutedEventArgs e) => controller.StepSmall(1);

        private void SkipForward_Click(object sender, RoutedEventArgs e) => controller.StepLarge(1);

        private void RunStop_Click(object sender, RoutedEventArgs e) => controller.ToggleRun();

        private void TimeText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                controller.CommitTimeText();
        }

        private void ScaleText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                controller.CommitScaleAmplitude();
        }

        private void AutoScale_Changed(object sender, RoutedEventArgs e)
        {
            if (controller != null)
                controller.AutoScaleChanged();
        }
    }
}
