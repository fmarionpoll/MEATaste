using Microsoft.Win32;
using TasteMEA.DataMEA.MaxWell;
using TasteMEA.Infrastructure;


namespace TasteMEA.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FileOpenedPanel
    {
        public FileOpenedPanel()
        {
            InitializeComponent();
        }

        private readonly ApplicationState state;
        private readonly MeaFileReader meaFileReader;

        // Custom constructor to pass expense report data
        public FileOpenedPanel(MeaFileReader meaFileReader, ApplicationState state): this()
        {
            this.meaFileReader = meaFileReader;
            this.state = state;
        }


        private void OpenDialogButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;
                state.CurrentMeaExperiment = meaFileReader.ReadFile(fileName);
                //UpdateLabelsWhenNewFileIsOpened();
            }
        }

        private void UpdateLabelsWhenNewFileIsOpened()
        {
            FileNameLabel.Content = "File: " + state.CurrentMeaExperiment.FileName;
            FileVersionLabel.Content = "Version: " + state.CurrentMeaExperiment.FileVersion;
            //var channelsList = state.CurrentMeaExperiment.Descriptors.GetElectrodesChannelNumber();
            //ListViewChannels.ItemsSource = channelsList;
            //InitElectrodesMap();
        }
    }


   
}
