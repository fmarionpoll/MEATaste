using MEATaste.DataMEA.MaxWell;
using MEATaste.Infrastructure;
using Microsoft.Win32;

namespace MEATaste.Views.FileOpenPanel
{
    public class FileOpenPanelController
    {
        public FileOpenPanelViewModel ViewModel { get; }

        private readonly MeaFileReader meaFileReader;
        private readonly ApplicationState state;

        public FileOpenPanelController(MeaFileReader meaFileReader, ApplicationState state)
        {
            this.meaFileReader = meaFileReader;
            this.state = state;

            ViewModel = new FileOpenPanelViewModel
            {
                FileNameLabel = "file name",
                FileVersionLabel = "file version"
            };
        }

        public void OpenFile()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;
                state.CurrentMeaExperiment = meaFileReader.ReadFile(fileName);

                ViewModel.FileNameLabel = state.CurrentMeaExperiment.FileName;
                ViewModel.FileVersionLabel = state.CurrentMeaExperiment.FileVersion;
                //UpdateLabelsWhenNewFileIsOpened();
            }
        }

        private void UpdateLabelsWhenNewFileIsOpened()
        {
            //FileNameLabel.Content = "File: " + state.CurrentMeaExperiment.FileName;
            //FileVersionLabel.Content = "Version: " + state.CurrentMeaExperiment.FileVersion;
            //var channelsList = state.CurrentMeaExperiment.Descriptors.GetElectrodesChannelNumber();
            //ListViewChannels.ItemsSource = channelsList;
            //InitElectrodesMap();
        }
    }
}
