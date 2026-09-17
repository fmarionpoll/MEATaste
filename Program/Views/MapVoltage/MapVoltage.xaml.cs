using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using ScottPlot.WPF;

namespace MEATaste.Views.MapVoltage
{
    public partial class VoltageMapPanel
    {
        private static readonly double[] PlaySpeeds = { 0.5, 1, 2, 5, 10 };
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

        private void RunStop_Click(object sender, RoutedEventArgs e) => controller.ToggleRun();

        private void ZoomIn_Click(object sender, RoutedEventArgs e) => controller.ZoomIn();

        private void ZoomOut_Click(object sender, RoutedEventArgs e) => controller.ZoomOut();

        private void Fit_Click(object sender, RoutedEventArgs e) => controller.Fit();

        private void Speed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (controller == null) return;
            var index = ((ComboBox)sender).SelectedIndex;
            if (index < 0 || index >= PlaySpeeds.Length) return;
            controller.SetPlaySpeed(PlaySpeeds[index]);
        }

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
