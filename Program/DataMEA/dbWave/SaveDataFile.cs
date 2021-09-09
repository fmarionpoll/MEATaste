﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using MEATaste.DataMEA.Models;

namespace MEATaste.DataMEA.dbWave
{
    public class DataFileWriter
    {
        public bool SaveCurrentElectrodeDataToFile(MeaExperiment experiment, ElectrodeProperties electrode,
            ElectrodeDataBuffer electrodeData)
        {

            var directoryName = CreateDirectoryFromExperimentFileName(experiment);
            var fileName = directoryName +
                              Path.DirectorySeparatorChar +
                              "data_electrode_" +
                              electrode.Electrode +
                              ".dat";

            bool flag = false;
            
            try
            {
                using (var binWriterToFile = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                {
                    WriteHeaderATLAB(binWriterToFile, experiment, electrode, electrodeData);
                    WriteDataATLAB(binWriterToFile, experiment, electrodeData);
                    
                    binWriterToFile.Close();
                }

                flag = true;
            }
            catch (IOException ioException)
            {
                Trace.WriteLine($"Error: {ioException.Message}");
                flag = false;
            }

            return flag;
        }

        private string  CreateDirectoryFromExperimentFileName(MeaExperiment experiment)
        {
            var directoryName = experiment.FileName.Substring(0, experiment.FileName.Length-3);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            return directoryName;
        }

        private byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        private void WriteHeaderATLAB(BinaryWriter binWriterToFile, MeaExperiment experiment, ElectrodeProperties electrode,
            ElectrodeDataBuffer electrodeData)
        {
            //  datafile_Atlab.h & datafile_Atlab.cpp
            const int headerLength = 1024;
            var bufferBytes = new byte[headerLength];
            var stream = new MemoryStream(bufferBytes);
            UTF8Encoding utf8 = new UTF8Encoding();
            BinaryWriter bw = new BinaryWriter(stream, utf8);
            bw.Seek(0, SeekOrigin.Begin);
            bw.Write(0xAAAA);
            bw.Seek(4, SeekOrigin.Begin);
            bw.Write(0);        // "DT_OTHER"
            bw.Seek(8, SeekOrigin.Begin);
            bw.Write(1);        // scan_count SCNCNT

            long length = electrodeData.RawSignalUShort.LongLength;
            bw.Seek(742, SeekOrigin.Begin);
            bw.Write(length);   // SAMCNT  length of the data acquisition

            /*
             * 
             * ACQCOM / ACQCOM_LEN acquisition comment
             * CS_STIM (20 chars), CS_CONC (10), CS_SENSILLUM (10)
             * ACQ_DATE
             * ACQ_TIME
             * CLKPER
             * CHANLST
             * GAINKST
             * CHANCOM
             * XGAIN
             * VERSION
             * CYBER320
             * version=0: TIMING
             * version=1: TRIGGER_MODE, TRIGGER_ChAN, TRIGGER_THRESHOLD
             */

            
            binWriterToFile.Write(bufferBytes, 0, headerLength);
        }

        private void WriteDataATLAB(BinaryWriter binWriterToFile, MeaExperiment experiment, ElectrodeDataBuffer electrodeData)
        {
           // here transform 10 bits data into 12 bits and short (or char?)
            //binWriterToFile.Write(ObjectToByteArray(electrodeData.RawSignalUShort));
        }

        private void WriteHeaderAwave(BinaryWriter binWriter, MeaExperiment experiment, ElectrodeProperties electrode,
            ElectrodeDataBuffer electrodeData)
        {
            const int headerLength = 256;
            var bufferBytes = new byte[headerLength];
            var stream = new MemoryStream(bufferBytes);
            UTF8Encoding utf8 = new UTF8Encoding();
            BinaryWriter bw = new BinaryWriter(stream, utf8);

            bw.Seek(0, SeekOrigin.Begin);
            bw.Write(utf8.GetBytes("AWAVE"));


            binWriter.Write(bufferBytes, 0, headerLength);
        }
    }
}
