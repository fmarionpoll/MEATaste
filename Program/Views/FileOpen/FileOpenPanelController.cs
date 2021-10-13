using System.Linq;
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
            state.MeaExperiment.Set(H5FileReader.OpenFileAndReadExperiment(fileName));

            var currentExperiment = state.MeaExperiment.Get();
            Model.FileNameLabel = currentExperiment.FileName + " version="+ currentExperiment.FileVersion;
        }

        public void SaveCurrentElectrodeDataClick()
        {
            var meaExp = state.MeaExperiment.Get();
            var listSelectedChannels = state.DataSelected.Get().Channels.Keys.ToList();

            foreach (var channel in listSelectedChannels)
            {
                var electrodeData = meaExp.Electrodes.Single(x => x.Electrode.Channel == channel);
                var data = state.DataSelected.Get().Channels[channel];
                dataFileWriter.SaveCurrentElectrodeDataToAtlabFile(meaExp, electrodeData, data);
            }
        }

        public void SaveAllElectrodesDataClick()
        {
            var meaExp = state.MeaExperiment.Get();
            foreach (var electrodeData in meaExp.Electrodes)
            {
                var data = H5FileReader.ReadAllDataFromSingleChannel(electrodeData.Electrode.Channel);
                dataFileWriter.SaveCurrentElectrodeDataToAtlabFile(meaExp, electrodeData, data);
            }
        }

    }
}
