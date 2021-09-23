using System;
using System.Diagnostics;
using System.IO;
using MEATaste.Annotations;
using MEATaste.DataMEA.Models;

namespace MEATaste.DataMEA.dbWave
{
    public class DataFileWriter
    { 
        private const int Devid				    = 4;	// device_id.........AD card id;
        private const int Devflags				= 6;	// device_flags......type of acquisition;
        private const int Scncnt				= 8;	// scan_count .......number of acq channels;
        private const int Chanlst				= 10;	// channel_list[8] ..input channel list;
        private const int TriggerMode			= 26;	// trigger_mode......AD trigger mode [0..];
        private const int TriggerChan			= 28;	// trigger_chan......AD trigger channel [0..7];
        private const int TriggerThreshold	    = 30;	// trigger_treshold..AD trig threshold value [binary];
        private const int TriggerPolarity		= 32;	// trigger_polarity;
        private const int Gainlst				= 42;	// gain_list[8]...... gain channel list;
        private const int Chancom				= 74;	// channel_comment[8][40];
        private const int Acqdate				= 714;	// acq_date[10];
        private const int Acqtime				= 724;	// acq_time[10];
        private const int Timing				= 736;	// timing_source;
        private const int Clkper				= 738;	// clock_period;
        private const int Samcnt				= 742;  // sample_count;
        private const int Rdcnt				    = 746;	// read_count;
        private const int Acqcom				= 750;	// acq_comment[80];
        private const int Version				= 830;	// version;
        private const int Cybera320			    = 832;	// CyberA320;
        private const int Cyber1				= 840;	// cyber1
        private const int Cyber2				= 868;  // cyber2
        private const int Xgain				    = 914;  // xgain_list[8]

        private const int AcqcomLen = 80;
        private const int Data = 1024;

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
            binaryWriter.Seek(Devid, SeekOrigin.Begin); 
            binaryWriter.Write((short)0); 
            binaryWriter.Seek(Scncnt, SeekOrigin.Begin); 
            binaryWriter.Write((short)1);
            binaryWriter.Seek(Chanlst, SeekOrigin.Begin);
            binaryWriter.Write((short)electrode.Channel);
            binaryWriter.Seek(Gainlst, SeekOrigin.Begin);
            binaryWriter.Write((short)1);
            binaryWriter.Seek(Chancom, SeekOrigin.Begin);
            binaryWriter.Write(electrode.Electrode.ToString().ToCharArray());

            binaryWriter.Seek(Acqdate, SeekOrigin.Begin);
            var timeStart = experiment.Descriptors.TimeStart;
            var acqDate = timeStart.ToString(@"MM'/'dd'/'yyyy HH:mm:ss");
            binaryWriter.Write(acqDate.ToCharArray());

            binaryWriter.Seek(Clkper, SeekOrigin.Begin);
            var clockperiod = 4E6f / experiment.Descriptors.SamplingRate;
            binaryWriter.Write((Int32)clockperiod);

            var length = electrodeData.RawSignalUShort.LongLength;
            binaryWriter.Seek(Samcnt, SeekOrigin.Begin); 
            binaryWriter.Write((Int32)length);

            binaryWriter.Seek(Acqcom, SeekOrigin.Begin);
            binaryWriter.Write(electrode.Electrode.ToString().ToCharArray());

            binaryWriter.Seek(Acqcom + 20, SeekOrigin.Begin);
            binaryWriter.Write(("X=" + electrode.XuM).ToCharArray());

            binaryWriter.Seek(Acqcom + 30, SeekOrigin.Begin);
            binaryWriter.Write(("Y=" + electrode.YuM).ToCharArray());

            binaryWriter.Seek(Xgain, SeekOrigin.Begin);
            double xgain = 20/(4096 * experiment.Descriptors.Lsb);
            binaryWriter.Write((float)xgain);
        }

        private static void WriteDataAtlab(BinaryWriter binaryWriter, [NotNull] ElectrodeDataBuffer electrodeData)
        {
            if (electrodeData == null) throw new ArgumentNullException(nameof(electrodeData));

            binaryWriter.Seek(Data, SeekOrigin.Begin);
            const short delta = 2048 - 512;
            
            foreach (var value in electrodeData.RawSignalUShort)
            {
                var dtvalue = (short) (value + delta);
                binaryWriter.Write(dtvalue);
            }
 
        }

    }
}
