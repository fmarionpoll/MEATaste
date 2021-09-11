using System;
using System.Diagnostics;
using System.IO;
using MEATaste.Annotations;
using MEATaste.DataMEA.Models;

namespace MEATaste.DataMEA.dbWave
{
    public class DataFileWriter
    { 
        private const int DEVID				    = 4;	// device_id.........AD card id;
        private const int DEVFLAGS				= 6;	// device_flags......type of acquisition;
        private const int SCNCNT				= 8;	// scan_count .......number of acq channels;
        private const int CHANLST				= 10;	// channel_list[8] ..input channel list;
        private const int TRIGGER_MODE			= 26;	// trigger_mode......AD trigger mode [0..];
        private const int TRIGGER_CHAN			= 28;	// trigger_chan......AD trigger channel [0..7];
        private const int TRIGGER_THRESHOLD	    = 30;	// trigger_treshold..AD trig threshold value [binary];
        private const int TRIGGER_POLARITY		= 32;	// trigger_polarity;
        private const int GAINLST				= 42;	// gain_list[8]...... gain channel list;
        private const int CHANCOM				= 74;	// channel_comment[8][40];
        private const int ACQDATE				= 714;	// acq_date[10];
        private const int ACQTIME				= 724;	// acq_time[10];
        private const int TIMING				= 736;	// timing_source;
        private const int CLKPER				= 738;	// clock_period;
        private const int SAMCNT				= 742;  // sample_count;
        private const int RDCNT				    = 746;	// read_count;
        private const int ACQCOM				= 750;	// acq_comment[80];
        private const int VERSION				= 830;	// version;
        private const int CYBERA320			    = 832;	// CyberA320;
        private const int CYBER_1				= 840;	// cyber1
        private const int CYBER_2				= 868;  // cyber2
        private const int XGAIN				    = 914;  // xgain_list[8]

        private const int ACQCOM_LEN = 80;
        private const int DATA = 1024;

        public bool SaveCurrentElectrodeDataToAtlabFile(MeaExperiment experiment, ElectrodeProperties electrode,
            ElectrodeDataBuffer electrodeData)
        {

            var directoryName = CreateDirectoryFromExperimentFileName(experiment);
            var fileName = directoryName +
                              Path.DirectorySeparatorChar +
                              "data_electrode_" +
                              electrode.Electrode +
                              ".dat";

            bool flag;
            
            try
            {
                using (var binWriterToFile = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                {
                    WriteHeaderAtlab(binWriterToFile, experiment, electrode, electrodeData);
                    WriteDataAtlab(binWriterToFile, electrodeData);
                    binWriterToFile.Close();
                    Trace.WriteLine("dat file created and closed");
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

        private static void WriteHeaderAtlab(BinaryWriter binaryWriter, MeaExperiment experiment,
            ElectrodeProperties electrode,
            ElectrodeDataBuffer electrodeData)
        {
            binaryWriter.Seek(0, SeekOrigin.Begin);
            binaryWriter.Write(0xAAAA);
            binaryWriter.Seek(DEVID, SeekOrigin.Begin); 
            binaryWriter.Write((short)0); 
            binaryWriter.Seek(SCNCNT, SeekOrigin.Begin); 
            binaryWriter.Write((short)1);
            binaryWriter.Seek(CHANLST, SeekOrigin.Begin);
            binaryWriter.Write((short)electrode.Channel);
            binaryWriter.Seek(GAINLST, SeekOrigin.Begin);
            binaryWriter.Write((short)1);
            binaryWriter.Seek(CHANCOM, SeekOrigin.Begin);
            binaryWriter.Write(electrode.Electrode.ToString().ToCharArray());

            binaryWriter.Seek(ACQDATE, SeekOrigin.Begin);
            var timeStart = experiment.Descriptors.TimeStart;
            var acqDate = timeStart.ToString(@"MM'/'dd'/'yyyy HH:mm:ss");
            binaryWriter.Write(acqDate.ToCharArray());

            binaryWriter.Seek(CLKPER, SeekOrigin.Begin);
            var clockperiod = 4E6f / experiment.Descriptors.SamplingRate;
            binaryWriter.Write((Int32)clockperiod);

            Int32 length = (Int32) electrodeData.RawSignalUShort.LongLength;
            binaryWriter.Seek(SAMCNT, SeekOrigin.Begin); 
            binaryWriter.Write(length);

            binaryWriter.Seek(ACQCOM, SeekOrigin.Begin);
            binaryWriter.Write(electrode.Electrode.ToString().ToCharArray());

            binaryWriter.Seek(ACQCOM + 20, SeekOrigin.Begin);
            binaryWriter.Write(("X=" + electrode.XuM).ToCharArray());

            binaryWriter.Seek(ACQCOM + 30, SeekOrigin.Begin);
            binaryWriter.Write(("Y=" + electrode.YuM).ToCharArray());

            binaryWriter.Seek(XGAIN, SeekOrigin.Begin);
            binaryWriter.Write((float)experiment.Descriptors.Gain);

        }

        private static void WriteDataAtlab(BinaryWriter binaryWriter, [NotNull] ElectrodeDataBuffer electrodeData)
        {
            if (electrodeData == null) throw new ArgumentNullException(nameof(electrodeData));

            binaryWriter.Seek(DATA, SeekOrigin.Begin);
            foreach (var value in electrodeData.RawSignalUShort)
            {
                var dtvalue = (short) (value + 1024);
                binaryWriter.Write(dtvalue);
            }
        }

    }
}
