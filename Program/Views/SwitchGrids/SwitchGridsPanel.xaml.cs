using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace MEATaste.Views.SwitchGrids
{
    public partial class SwitchGridsPanel : UserControl
    {
        private readonly SwitchGridsPanelController controller;

        public event EventHandler DockToggleRequested;

        public SwitchGridsPanel()
        {
            controller = App.ServiceProvider.GetService<SwitchGridsPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }

        public void SetDocked(bool docked)
        {
            DockToggleButton.Content = docked ? "Undock" : "Dock";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            controller.DoIt(Root);
        }

        private void DockToggleButton_Click(object sender, RoutedEventArgs e)
        {
            DockToggleRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
