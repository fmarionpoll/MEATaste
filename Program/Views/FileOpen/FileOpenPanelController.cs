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

        private readonly MeaFileReader meaFileReader;
        private readonly DataFileWriter dataFileWriter; 
        private readonly ApplicationState state;
        private readonly IEventRaiser eventRaiser;

        public FileOpenPanelController(ApplicationState state, 
            MeaFileReader meaFileReader, 
            DataFileWriter dataFileWriter, IEventRaiser eventRaiser)
        {
            this.meaFileReader = meaFileReader;
            this.dataFileWriter = dataFileWriter;
            this.state = state;
            this.eventRaiser = eventRaiser;

            Model = new FileOpenPanelModel();
        }

        public void OpenFile()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != true) return;

            var fileName = openFileDialog.FileName;
            state.CurrentExperiment.Set(meaFileReader.ReadFile(fileName));

            var currentExperiment = state.CurrentExperiment.Get();
            Model.FileNameLabel = currentExperiment.FileName + " version="+ currentExperiment.FileVersion;
            Model.AcquisitionSettingsLabel = 
                "Gain=" + currentExperiment.Descriptors.Gain
                + " High-pass filter(Hz)=" + currentExperiment.Descriptors.Hpf
                + " Sampling rate(Hz)=" + currentExperiment.Descriptors.SamplingRate;
        }

        public void SaveCurrentElectrodeDataClick()
        {
            var experiment = state.CurrentExperiment.Get();
            var electrode = state.CurrentElectrode.Get();
            var electrodeData = state.ElectrodeBuffer.Get();
            dataFileWriter.SaveCurrentElectrodeDataToAtlabFile(experiment, electrode, electrodeData);
        }

        public void SaveAllElectrodesDataClick()
        {
            var experiment = state.CurrentExperiment.Get();
            var array = state.CurrentExperiment.Get().Descriptors.Electrodes;
            var electrodeBuffer = state.ElectrodeBuffer.Get();
            if (electrodeBuffer == null)
            {
                state.ElectrodeBuffer.Set(new ElectrodeDataBuffer());
                electrodeBuffer = state.ElectrodeBuffer.Get();
            }

            foreach (var electrode in array)
            {
                state.CurrentElectrode.Set(electrode);
                eventRaiser.Raise(EventType.SelectedElectrodeChanged);
               
                electrodeBuffer.RawSignalUShort = meaFileReader.ReadDataForOneElectrode(electrode);
                eventRaiser.Raise(EventType.ElectrodeRecordLoaded);

                var electrodeDataBuffer = state.ElectrodeBuffer.Get() ??
                                          throw new ArgumentNullException("state.ElectrodeBuffer.Get()");
                dataFileWriter.SaveCurrentElectrodeDataToAtlabFile(experiment, electrode, electrodeDataBuffer);
            }
        }

    }
}
