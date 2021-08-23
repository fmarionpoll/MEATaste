﻿using System;
using System.Diagnostics;
using HDF.PInvoke;
using HDF5.NET;
using MEATaste.DataMEA.Models;

namespace MEATaste.DataMEA.MaxWell
{
    public class MeaFileReader
    {
        private readonly FileReader fileReader;

        public MeaFileReader(FileReader fileReader)
        {
            this.fileReader = fileReader;
        }

        public MeaExperiment ReadFile(string fileName)
        {
            if (fileReader.OpenReadMaxWellFile(fileName)
                && fileReader.IsFileReadableAsMaxWellFile())
            {
                return fileReader.GetExperimentInfos();
            }
            return null;
        }
    }

    public class FileReader
    {
        public static string FileName { get; set; }
        public static string FileVersion { get; set; } = "unknown";
        public static H5File Root { get; set; }

        
        public bool OpenReadMaxWellFile(string fileName)
        {
            H5.open();
            uint majnum = 0, minnum = 0, relnum = 0;
            if (H5.get_libversion(ref majnum, ref minnum, ref relnum) >= 0)
                Trace.WriteLine($"HDF5 {majnum}.{minnum}.{relnum}");

            Root = H5File.OpenRead(fileName);
            FileName = fileName;
            return Root != null;
        }

        public bool IsFileReadableAsMaxWellFile()
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

        public MeaExperiment GetExperimentInfos()
        {
            MeaExperiment MeaExp = new MeaExperiment(FileName, new Descriptors());
            MeaExp.FileVersion = FileVersion;
            ReadSettingsDescriptors(MeaExp);
            ReadTimeDescriptors(MeaExp);
            ReadMapElectrodes(MeaExp);
            return MeaExp;
        }

        public bool ReadTimeDescriptors(MeaExperiment meaExp)
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
                meaExp.Descriptors.TimeStart = GetTimeFromString(strings[0], "start: ");
                meaExp.Descriptors.TimeStop = GetTimeFromString(strings[1], "stop: ");
                flag = true;
            }
            finally
            {
            }
            return flag;
        }

        private DateTime GetTimeFromString(string inputString, string pattern)
        {
            int pos1 = pattern.Length;
            int pos2 = inputString.IndexOf(';');
            if (pos2 < 0)
                pos2 = inputString.Length;
            string dateInput = inputString.Substring(pos1, pos2 - pos1);
            DateTime parsedDate = DateTime.Parse(dateInput);
            return parsedDate;
        }

        public bool ReadSettingsDescriptors(MeaExperiment meaExp)
        {
            bool flag = false;
            try
            {
                H5Group group = Root.Group("/settings");

                double[] gainarray = ReadDoubleDataFromGroup(group, "gain");
                meaExp.Descriptors.Gain = gainarray[0];

                double[] hpfarray = ReadDoubleDataFromGroup(group, "hpf");
                meaExp.Descriptors.Hpf = hpfarray[0];

                double[] lsbarray = ReadDoubleDataFromGroup(group, "lsb");
                meaExp.Descriptors.Lsb = lsbarray[0];
            }
            finally
            {
            }
            return flag;
        }

        private double[] ReadDoubleDataFromGroup(H5Group group, string headerName)
        {
            H5Dataset datasetGain = group.Dataset(headerName);
            double[] results = datasetGain.Read<double>();
            return results;
        }

        public bool ReadMapElectrodes(MeaExperiment MeaExp)
        {
            bool flag = false;
            try
            {
                H5Group group = Root.Group("/");
                H5Dataset dataset = group.Dataset("mapping");
                Legacy.DatasetMembers[] compoundData = dataset.Read<Legacy.DatasetMembers>();

                MeaExp.Descriptors.Electrodes = new Electrode[compoundData.Length];
                for (int i = 0; i < compoundData.Length; i++)
                {
                    Electrode ec = new Electrode(
                        compoundData[i].channel,
                        compoundData[i].electrode,
                        compoundData[i].x,
                        compoundData[i].y);
                    MeaExp.Descriptors.Electrodes[i] = ec;
                }

                flag = true;
            }
            finally
            {
            }
            return flag;
        }

        public ushort[] ReadAll_OneElectrodeAsInt(Electrode electrode)
        {
            H5Group group = Root.Group("/");
            H5Dataset dataset = group.Dataset("sig");
            ulong nbchannels = dataset.Space.Dimensions[0]; // 1028 expected
            ulong nbdatapoints = dataset.Space.Dimensions[1]; // any size
            return Read_OneElectrodeDataAsInt(electrode.ChannelNumber, 0, nbdatapoints -1);
        }

        public ushort[] Read_OneElectrodeDataAsInt(int Channel, ulong startsAt, ulong endsAt)
        {
            ushort[] result;
            try
            {
                H5Group group = Root.Group("/");
                H5Dataset dataset = group.Dataset("sig");

                int ndimensions = dataset.Space.Rank;
                if (ndimensions != 2)
                    return null;
                //ulong nbchannels = dataset.Space.Dimensions[0];     // 1028 expected
                //ulong nbdatapoints = dataset.Space.Dimensions[1];   // any size
                //var dataType = dataset.Type;

                ulong nbpoints = endsAt - startsAt + 1;

                var memoryDims = new ulong[] { nbpoints };

                var datasetSelection = new HyperslabSelection(
                    rank: 2,
                    starts: new ulong[] { (ulong)Channel, startsAt },   // start at row ElectrodeNumber, column 0
                    strides: new ulong[] { 1, 1 },                              // don't skip anything
                    counts: new ulong[] { 1, nbpoints },                        // read 1 row, ndatapoints columns
                    blocks: new ulong[] { 1, 1 }                                // blocks are single elements
                );

                var memorySelection = new HyperslabSelection(
                    rank: 1,
                    starts: new ulong[] { 0 },
                    strides: new ulong[] { 1 },
                    counts: new ulong[] { nbpoints },
                    blocks: new ulong[] { 1 }
                );

                result = dataset
                    .Read<ushort>(
                        fileSelection: datasetSelection,
                        memorySelection: memorySelection,
                        memoryDims: memoryDims
                    );  
            }
            finally
            {
            }
            return result;
        }


    }

}

