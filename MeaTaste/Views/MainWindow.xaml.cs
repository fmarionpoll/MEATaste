using TasteMEA.DataMEA.MaxWell;
using TasteMEA.ViewModels;

namespace TasteMEA.Views
{
    public partial class MainWindow
    {
        private readonly ApplicationState state;
        private readonly MeaFileReader meaFileReader;

        public MainWindow(MeaFileReader meaFileReader, ApplicationState state)
        {
            this.meaFileReader = meaFileReader;
            this.state = state;
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


