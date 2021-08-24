using MEATaste.DataMEA.MaxWell;
using MEATaste.Infrastructure;
using Microsoft.Win32;

namespace MEATaste.Views.FileOpen
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
            }
        }

    }
}
