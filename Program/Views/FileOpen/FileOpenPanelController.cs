using MEATaste.DataMEA.MaxWell;
using MEATaste.Infrastructure;
using Microsoft.Win32;

namespace MEATaste.Views.FileOpen
{
    public class FileOpenPanelController
    {
        public FileOpenPanelModel Model { get; }

        private readonly MeaFileReader meaFileReader;
        private readonly ApplicationState state;

        public FileOpenPanelController(MeaFileReader meaFileReader, ApplicationState state)
        {
            this.meaFileReader = meaFileReader;
            this.state = state;

            Model = new FileOpenPanelModel();
        }

        public void OpenFile()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != true) return;

            var fileName = openFileDialog.FileName;
            state.CurrentMeaExperiment.Set(meaFileReader.ReadFile(fileName));

            var currentExperiment = state.CurrentMeaExperiment.Get();
            Model.FileNameLabel = currentExperiment.FileName + " version="+ currentExperiment.FileVersion;
        }

    }
}
