using MEATaste.DataMEA.MaxWell;
using MEATaste.ViewModels;

namespace MEATaste.Views
{
    public partial class MainWindow
    {
        private readonly ApplicationState _applicationState;
        private readonly MeaFileReader _meaFileReader;

        public MainWindow(MeaFileReader meaFileReader, ApplicationState applicationState)
        {
            this._meaFileReader = meaFileReader;
            this._applicationState = applicationState;
            DataContext = _applicationState;
            InitializeComponent();
        }

        // TODO
        //private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    int selected = ListViewChannels.SelectedIndex;
        //    Electrode electrode = state.CurrentMeaExperiment.Descriptors.Electrodes[selected];
        //    UpdateSelectedElectrode(electrode);
        //}

    }
}


