using System.ComponentModel;
using MEATaste.Infrastructure;

namespace MEATaste.Views.MainView
{
    public partial class MainView
    {
        private FloatingViewHost mapsFloater;
        private FloatingViewHost tracesFloater;

        public MainView()
        {
            InitializeComponent();
            mapsFloater = new FloatingViewHost(MapsHost, MapsPanel, "MEATaste maps",
                () => MapsPanel.SetDocked(!mapsFloater.IsFloating));
            tracesFloater = new FloatingViewHost(TracesHost, TracesContent, "MEATaste traces",
                () => TracesPanel.SetDocked(!tracesFloater.IsFloating));
            MapsPanel.DockToggleRequested += (_, _) => ToggleMaps();
            TracesPanel.DockToggleRequested += (_, _) => ToggleTraces();
        }

        private void ToggleMaps()
        {
            mapsFloater.Toggle();
            MapsPanel.SetDocked(!mapsFloater.IsFloating);
        }

        private void ToggleTraces()
        {
            tracesFloater.Toggle();
            TracesPanel.SetDocked(!tracesFloater.IsFloating);
        }

        private void MainView_Closing(object sender, CancelEventArgs e)
        {
            mapsFloater.Shutdown();
            tracesFloater.Shutdown();
        }
    }
}
