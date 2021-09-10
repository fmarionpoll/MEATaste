using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using MEATaste.Annotations;
using MEATaste.DataMEA.Models;

namespace MEATaste.DataMEA.dbWave
{
    public class DataFileWriter
    {
         const int DEVID				= 4;	// device_id.........AD card id;
         const int DEVFLAGS				= 6;	// device_flags......type of acquisition;
         const int SCNCNT				= 8;	// scan_count .......number of acq channels;
         const int CHANLST				= 10;	// channel_list[8] ..input channel list;
         const int TRIGGER_MODE			= 26;	// trigger_mode......AD trigger mode [0..];
         const int TRIGGER_CHAN			= 28;	// trigger_chan......AD trigger channel [0..7];
         const int TRIGGER_THRESHOLD	= 30;	// trigger_treshold..AD trig threshold value [binary];
         const int TRIGGER_POLARITY		= 32;	// trigger_polarity;
         const int GAINLST				= 42;	// gain_list[8]...... gain channel list;
         const int CHANCOM				= 74;	// channel_comment[8][40];
         const int ACQDATE				= 714;	// acq_date[10];
         const int ACQTIME				= 724;	// acq_time[10];
         const int TIMING				= 736;	// timing_source;
         const int CLKPER				= 738;	// clock_period;
         const int SAMCNT				= 742;	// sample_count;
         const int RDCNT				= 746;	// read_count;
         const int ACQCOM				= 750;	// acq_comment[80];
         const int VERSION				= 830;	// version;
         const int CYBERA320			= 832;	// CyberA320;
         const int CYBER_1				= 840;	// cyber1
         const int CYBER_2				= 868;	// cyber2
         const int XGAIN				= 914;	// xgain_list[8]

         const int ACQCOM_LEN = 80;
         private const int DATA = 1024;

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
                    WriteDataATLAB(binWriterToFile, electrodeData);
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

        private void WriteHeaderATLAB(BinaryWriter binWriterToFile, MeaExperiment experiment,
            ElectrodeProperties electrode,
            ElectrodeDataBuffer electrodeData)
        {
            //  datafile_Atlab.h & datafile_Atlab.cpp
            const int headerLength = 1024;
            var bufferBytes = new byte[headerLength];
            var stream = new MemoryStream(bufferBytes);
            var utf8 = new UTF8Encoding();
            var bw = new BinaryWriter(stream, utf8);
            bw.Seek(0, SeekOrigin.Begin);       // header version
            bw.Write(0xAAAA);
            bw.Seek(DEVID, SeekOrigin.Begin);   // device ID
            bw.Write(0); 
            bw.Seek(SCNCNT, SeekOrigin.Begin);  // number of data channels
            bw.Write(1);
            bw.Seek(CHANLST, SeekOrigin.Begin);
            bw.Write((short)electrode.Channel);
            bw.Seek(GAINLST, SeekOrigin.Begin);
            bw.Write((short)1);
            bw.Seek(CHANCOM, SeekOrigin.Begin);
            bw.Write(electrode.Electrode.ToString());

            bw.Seek(ACQDATE, SeekOrigin.Begin);
            bw.Seek(ACQTIME, SeekOrigin.Begin);

            bw.Seek(ACQCOM, SeekOrigin.Begin);
            bw.Write(electrode.Electrode.ToString()+ "  XuM="+ electrode.XuM+" YuM="+electrode.YuM);

            bw.Seek(XGAIN, SeekOrigin.Begin);
            bw.Write((float)experiment.Descriptors.Gain);

            var length = electrodeData.RawSignalUShort.LongLength;
            bw.Seek(SAMCNT, SeekOrigin.Begin);  // data length
            bw.Write((short) length);  

            binWriterToFile.Write(bufferBytes);
        }

        private static void WriteDataATLAB(BinaryWriter binWriterToFile, [NotNull] ElectrodeDataBuffer electrodeData)
        {
            if (electrodeData == null) throw new ArgumentNullException(nameof(electrodeData));

            binWriterToFile.Seek(DATA, SeekOrigin.Begin);       // header version
            foreach (var value in electrodeData.RawSignalUShort)
            {
                var dtvalue = (short) ((short) value + (short) 1024);
                binWriterToFile.Write(dtvalue);
            }
        }

        /*
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

            // TODO
            binWriter.Write(bufferBytes, 0, headerLength);
        }
        */
    }
}
