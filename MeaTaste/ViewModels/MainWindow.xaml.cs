using Microsoft.Win32;
using System.Windows;
using System.Windows.Navigation;
using TasteMEA.DataMEA.MaxWell;



namespace TasteMEA
{
    public partial class MainWindow
    {
        private readonly ApplicationState state;
        private readonly MeaFileReader meaFileReader;

        //public MainWindow(ApplicationState state, MeaFileReader meaFileReader)
        //{
        //    this.state = state;
        //    this.meaFileReader = meaFileReader;
        //    InitializeComponent();
        //}

        public MainWindow()
        {
            state = new ApplicationState();
            meaFileReader = new MeaFileReader();
            InitializeComponent();
        }

        private void OpenDialogButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;
                state.CurrentMeaExperiment = meaFileReader.ReadFile(fileName);
                UpdateLabelsWhenNewFileIsOpened();
            }
        }

        private void UpdateLabelsWhenNewFileIsOpened()
        {
            OpenedFileLabel.Content = "File: " + state.CurrentMeaExperiment.FileName;
            OpenedFileVersion.Content = "Version: " + state.CurrentMeaExperiment.FileVersion;
            var channelsList = state.CurrentMeaExperiment.Descriptors.GetElectrodesChannelNumber();
            ListViewChannels.ItemsSource = channelsList;
            InitElectrodesMap();
        }
    }
}


