using System;
using MEATaste.DataMEA.dbWave;
using MEATaste.DataMEA.MaxWell;
using MEATaste.DataMEA.Models;
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
            state.CurrentExperiment.Set(h5FileReader.OpenFileAndReadExperiment(fileName));

            var currentExperiment = state.CurrentExperiment.Get();
            Model.FileNameLabel = currentExperiment.FileName + " version="+ currentExperiment.FileVersion;
            
        }

        public void SaveCurrentElectrodeDataClick()
        {
            var experiment = state.CurrentExperiment.Get();
            var electrode = state.CurrentElectrode.Get();
            var electrodeData = state.ElectrodeData.Get();
            dataFileWriter.SaveCurrentElectrodeDataToAtlabFile(experiment, electrode, electrodeData);
        }

        public void SaveAllElectrodesDataClick()
        {
            var experiment = state.CurrentExperiment.Get();
            var array = state.CurrentExperiment.Get().Electrodes;
            var electrodeBuffer = state.ElectrodeData.Get();
            if (electrodeBuffer == null)
            {
                state.ElectrodeData.Set(new ElectrodeData());
                electrodeBuffer = state.ElectrodeData.Get();
            }

            foreach (var electrode in array)
            {
                state.CurrentElectrode.Set(electrode);
                electrodeBuffer.RawSignalUShort = h5FileReader.ReadChannelDataAll(electrode.Channel);
                var electrodeDataBuffer = state.ElectrodeData.Get() ??
                                          throw new ArgumentNullException("state.ElectrodeBuffer.Get()");
                dataFileWriter.SaveCurrentElectrodeDataToAtlabFile(experiment, electrode, electrodeDataBuffer);
            }
        }

    }
}
