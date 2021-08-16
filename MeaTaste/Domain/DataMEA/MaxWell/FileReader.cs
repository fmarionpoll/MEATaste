using MeaTaste.Domain.DataMEA.Models;
using HDF.PInvoke;
using HDF5.NET;
using System;
using System.Diagnostics;

namespace MeaTaste.Domain.DataMEA.MaxWell
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
                string TimeStartString = data[0];
                // "start: 2021-07-15 16:54:58;\nstop: 2021-07-15 16:57:54\n"
            }
            finally
            {
            }
            return flag;
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
    }

   
}
