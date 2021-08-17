using MeaTaste.DataMEA.Models;
using HDF.PInvoke;
using HDF5.NET;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MeaTaste.DataMEA.MaxWell
{
    public static class FileReader
    {
        public static string FileName { get; set; }
        public static string FileVersion { get; set; } = "unknown";
        public static H5File Root { get; set; }


        public static bool OpenReadMaxWellFile(string fileName)
        {
            H5.open();
            uint majnum = 0, minnum = 0, relnum = 0;
            if (H5.get_libversion(ref majnum, ref minnum, ref relnum) >= 0)
                Trace.WriteLine($"HDF5 {majnum}.{minnum}.{relnum}");

            Root = H5File.OpenRead(fileName);
            FileName = fileName;
            return Root != null;
        }

        public static bool IsFileReadableAsMaxWellFile()
        {
            try
            {
                H5Group group = Root.Group("/");
                H5Dataset dataset = group.Dataset("version");
                string[] data = dataset.ReadString();
                FileVersion = data[0];
            }
            finally
            {
            }

            if (FileVersion == "20160704")
            {
                Trace.WriteLine($"MaxWell file version: legacy ({FileVersion}) or v0");
                return true;
            }
            else if (FileVersion == "20190530")
            {
                Trace.WriteLine($"MaxWell file version: 2021 on ({FileVersion}) or v1");
                return false;
            }
            else
                Trace.WriteLine("unknown file structure");
            return false;
        }

        public static MeaExperiment GetExperimentInfos()
        {
            MeaExperiment MeaExp = new MeaExperiment
            {

                Descriptors = new Descriptors
                {

                },
                MicroElectrodeArray = new MicroElectrodeArray
                {
                    Pixels = new Pixel[1024]
                }
            };
            MeaExp.FileName = FileName;
            ReadSettingsDescriptors(MeaExp);
            ReadTimeDescriptors(MeaExp);
            ReadMapElectrodes(MeaExp);
            return MeaExp;
        }

        public static bool ReadTimeDescriptors(MeaExperiment MeaExp)
        {
            bool flag = false;
            try
            {
                H5Group group = Root.Group("/");
                H5Dataset dataset = group.Dataset("time");
                string[] data = dataset.ReadString();
                string lines = data[0];
                // "start: 2021-07-15 16:54:58;\nstop: 2021-07-15 16:57:54\n"
                string[] strings = lines.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                MeaExp.Descriptors.TimeStart = GetTimeFromString(strings[0], "start: ");
                MeaExp.Descriptors.TimeStop = GetTimeFromString(strings[1], "stop: ");
                flag = true;
            }
            finally
            {
            }
            return flag;
        }

        private static DateTime GetTimeFromString(string inputstring, string pattern)
        {
            int pos1 = pattern.Length;
            int pos2 = inputstring.IndexOf(';');
            if (pos2 < 0)
                pos2 = inputstring.Length;
            string dateInput = inputstring.Substring(pos1, pos2 - pos1);
            DateTime parsedDate = DateTime.Parse(dateInput);
            return parsedDate;
        }

        public static bool ReadSettingsDescriptors(MeaExperiment MeaExp)
        {
            bool flag = false;
            try
            {
                H5Group group = Root.Group("/settings");

                double[] gainarray = ReadDoubleDataFromGroup(group, "gain");
                MeaExp.Descriptors.Gain = gainarray[0];

                double[] hpfarray = ReadDoubleDataFromGroup(group, "hpf");
                MeaExp.Descriptors.Hpf = hpfarray[0];

                double[] lsbarray = ReadDoubleDataFromGroup(group, "lsb");
                MeaExp.Descriptors.Lsb = lsbarray[0];
            }
            finally
            {
            }
            return flag;
        }

        private static double[] ReadDoubleDataFromGroup(H5Group group, string headerName)
        {
            H5Dataset datasetGain = group.Dataset(headerName);
            double[] results = datasetGain.Read<double>();
            return results;
        }

        [StructLayout(LayoutKind.Explicit, Size = 24)]
        internal struct DatasetMembers
        {
            [FieldOffset(0)]
            public int channel;

            [FieldOffset(4)]
            public int electrode;

            [FieldOffset(8)]
            public double x;

            [FieldOffset(16)]
            public double y;
        };

        public static bool ReadMapElectrodes(MeaExperiment MeaExp)
        {
            bool flag = false;
            try
            {
                H5Group group = Root.Group("/");
                H5Dataset dataset = group.Dataset("mapping");
                DatasetMembers[] compoundData = dataset.Read<DatasetMembers>();

                MeaExp.Descriptors.RecordedChannels = new ElectrodeChannel[compoundData.Length];
                for (int i=0; i< compoundData.Length; i++)
                {
                    ElectrodeChannel ec = new ElectrodeChannel(
                        compoundData[i].channel, 
                        compoundData[i].electrode, 
                        compoundData[i].x,
                        compoundData[i].y);
                    MeaExp.Descriptors.RecordedChannels[i] = ec;
                }
            }
            finally
            {
            }
            return flag;
        }
       
    
    }
}
