using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace MEATaste.Views.SwitchMaps
{
    public partial class SwitchMapsPanel
    {
        private readonly SwitchMapsPanelController controller;

        public event EventHandler DockToggleRequested;

        public SwitchMapsPanel()
        {
            controller = App.ServiceProvider.GetService<SwitchMapsPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }

        public void SetDocked(bool docked)
        {
            DockToggleButton.Content = docked ? "Undock" : "Dock";
        }

        private void DockToggleButton_Click(object sender, RoutedEventArgs e)
        {
            DockToggleRequested?.Invoke(this, EventArgs.Empty);
        }

        private void PlayheadScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            controller.PlayheadScroll(e.NewValue);
        }
    }
}
