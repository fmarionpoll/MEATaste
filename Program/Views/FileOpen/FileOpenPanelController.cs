using MEATaste.DataMEA.dbWave;
using MEATaste.DataMEA.MaxWell;
using MEATaste.Infrastructure;
using Microsoft.Win32;


namespace MEATaste.Views.FileOpen
{
    public class FileOpenPanelController
    {
        public FileOpenPanelModel Model { get; }

        private readonly H5FileReader h5FileReader;
        private readonly DataFileWriter dataFileWriter; 
        private readonly ApplicationState state;
       

        public FileOpenPanelController(ApplicationState state,
            H5FileReader h5FileReader, 
            DataFileWriter dataFileWriter)
        {
            this.h5FileReader = h5FileReader;
            this.dataFileWriter = dataFileWriter;
            this.state = state;

            Model = new FileOpenPanelModel();
        }

        public void OpenFile()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != true) return;

            var fileName = openFileDialog.FileName;
            state.MeaExperiment.Set(h5FileReader.OpenFileAndReadExperiment(fileName));

            var currentExperiment = state.MeaExperiment.Get();
            Model.FileNameLabel = currentExperiment.FileName + " version="+ currentExperiment.FileVersion;
        }

        public void SaveCurrentElectrodeDataClick()
        {
            var meaExp = state.MeaExperiment.Get();
            var channel = state.CurrentElectrode.Get().Channel;
            var electrodeData = meaExp.Electrodes[channel];
            dataFileWriter.SaveCurrentElectrodeDataToAtlabFile(meaExp, electrodeData);
        }

        public void SaveAllElectrodesDataClick()
        {
            var meaExp = state.MeaExperiment.Get();
            foreach (var electrodeData in meaExp.Electrodes.Values)
            {
                electrodeData.RawSignalUShort = h5FileReader.ReadChannelDataAll(electrodeData.Electrode.Channel);
                dataFileWriter.SaveCurrentElectrodeDataToAtlabFile(meaExp, electrodeData);
            }
        }

    }
}
