using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using HDF5.NET;
using MEATaste.DataMEA.Models;

namespace MEATaste.DataMEA.MaxWell
{
    public class H5FileReader
    {
        private static string H5FileName { get; set; }
        private static string H5FileVersion { get; set; } = "unknown";
        private static H5File H5FileRoot { get; set; }
        
        public bool OpenReadMaxWellFile(string fileName)
        {
            H5FileRoot = H5File.OpenRead(fileName);
            H5FileName = fileName;
            RegisterIntelFilter();
            return H5FileRoot != null;
        }

        public bool IsReadableAsMaxWellFile()
        {
            var h5Group = H5FileRoot.Group("/");
            var h5Dataset = h5Group.Dataset("version");
            var data = h5Dataset.ReadString();
            H5FileVersion = data[0];
            
            switch (H5FileVersion)
            {
                case "20160704":
                    Trace.WriteLine($"MaxWell file version: legacy ({H5FileVersion}) or v0");
                    return true;
                case "20190530":
                    Trace.WriteLine($"MaxWell file version: 2021 on ({H5FileVersion}) or v1");
                    return false;
                default:
                    Trace.WriteLine("unknown file structure");
                    break;
            }
            return false;
        }

        public MeaExperiment GetExperimentInfos()
        {
            var meaExp = new MeaExperiment(H5FileName, H5FileVersion, new DataAcquisitionSettings());
            ReadSettings(meaExp);
            ReadAcquisitionTimeIntervals(meaExp);
            ReadMapping(meaExp);
            ReadSpikeTimes(meaExp);
            return meaExp;
        }

        public MeaExperiment OpenFileAndReadExperiment(string fileName)
        {
            if (OpenReadMaxWellFile(fileName)
                && IsReadableAsMaxWellFile())
            {
                return GetExperimentInfos();
            }
            return null;
        }

        private void ReadAcquisitionTimeIntervals(MeaExperiment meaExp)
        {
            var h5Group = H5FileRoot.Group("/");
            var h5Dataset = h5Group.Dataset("time");
            var stringArray = h5Dataset.ReadString();
            var lines = stringArray[0];
            var strings = lines.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            meaExp.DataAcquisitionSettings.TimeStart = GetTimeFromString(strings[0], "start: ");
            meaExp.DataAcquisitionSettings.TimeStop = GetTimeFromString(strings[1], "stop: ");

            h5Dataset = h5Group.Dataset("sig");
            meaExp.DataAcquisitionSettings.nDataAcquisitionPoints = h5Dataset.Space.Dimensions[1];
        }

        private DateTime GetTimeFromString(string inputString, string pattern)
        {
            var pos1 = pattern.Length;
            var pos2 = inputString.IndexOf(';');
            if (pos2 < 0)
                pos2 = inputString.Length;
            var dateInput = inputString.Substring(pos1, pos2 - pos1);
            var parsedDate = DateTime.Parse(dateInput);
            return parsedDate;
        }

        private void ReadSettings(MeaExperiment meaExp)
        {
            var h5Group = H5FileRoot.Group("/settings");

            var gainarray = h5Group.Dataset("gain").Read<double>();
            meaExp.DataAcquisitionSettings.Gain = gainarray[0];

            var hpfarray = h5Group.Dataset("hpf").Read<double>(); 
            meaExp.DataAcquisitionSettings.Hpf = hpfarray[0];

            var lsbarray = h5Group.Dataset("lsb").Read<double>(); 
            meaExp.DataAcquisitionSettings.Lsb = lsbarray[0];
        }

        private void ReadMapping(MeaExperiment meaExp)
        {
            var h5Group = H5FileRoot.Group("/");
            var h5Dataset = h5Group.Dataset("mapping");
            var compoundData = h5Dataset.Read<FileMapElectrodeProperties.DatasetMembers>();

            meaExp.Electrodes = new ElectrodeData[compoundData.Length];

            for (var i = 0; i < compoundData.Length; i++)
            {
                var ec = new ElectrodeProperties(
                    compoundData[i].channel,
                    compoundData[i].electrode,
                    compoundData[i].x,
                    compoundData[i].y);
                meaExp.Electrodes[i] = new ElectrodeData(ec);
            }
        }

        private void ReadSpikeTimes(MeaExperiment meaExp)
        {
            var h5Group = H5FileRoot.Group("/proc0");
            var h5Dataset = h5Group.Dataset("spikeTimes");
            var compoundData = h5Dataset.Read<FileMapSpikeTime.DatasetMembers>();

            for (var i = 0; i < compoundData.Length; i++)
            {
                var ec = new SpikeDetected(
                    compoundData[i].frameno,
                    compoundData[i].channel,
                    compoundData[i].amplitude);

                for (var index = 0; index < meaExp.Electrodes.Length; index++)
                {
                    var t = meaExp.Electrodes[index];
                    if (t.Electrode.Channel != ec.Channel)
                        continue;

                    List<SpikeDetected> spikeTimes = t.SpikeTimes;
                    if (spikeTimes == null)
                    {
                        spikeTimes = new List<SpikeDetected>();
                        t.SpikeTimes = spikeTimes;
                    }

                    spikeTimes.Add(ec);
                    break;
                }
            }
        }
        
        public ushort[] ReadAllDataFromSingleChannel(int channel)
        {
            var h5Dataset = H5FileRoot.Group("/").Dataset("sig");
            var nbdatapoints = h5Dataset.Space.Dimensions[1];
            const ulong chunkSizePerChannel = 200 * 100;
            var result = new ushort[nbdatapoints];
            var nchunks = (long)(1 +nbdatapoints / chunkSizePerChannel);

            int ndimensions = h5Dataset.Space.Rank;
            if (ndimensions != 2)
                return null;

            Parallel.For(0, nchunks, i =>
            {
                var h5File = H5File.OpenRead(H5FileName);
                var dataset = h5File.Group("/").Dataset("sig");

                var istart = (ulong)i * chunkSizePerChannel;
                var iend = istart + chunkSizePerChannel - 1;
                if (iend > nbdatapoints)
                    iend = nbdatapoints - 1;
                var chunkresult = ReadDataSingleChannel(dataset, channel, istart, iend);
                Array.Copy(chunkresult, 0, result, (int)istart, (int)(iend - istart + 1));
                h5File.Dispose();
            });

            return result;
        }

        public ushort[] ReadDataSingleChannel(H5Dataset dataset, int channel, ulong startsAt, ulong endsAt)
        {
            var nbPointsRequested = endsAt - startsAt + 1;

            var datasetSelection = new HyperslabSelection(
                rank: 2,
                starts: new[] { (ulong)channel, startsAt },         // start at row ElectrodeNumber, column 0
                strides: new ulong[] { 1, 1 },                      // don't skip anything
                counts: new ulong[] { 1, nbPointsRequested },       // read 1 row, ndatapoints columns
                blocks: new ulong[] { 1, 1 }                        // blocks are single elements
            );

            var memorySelection = new HyperslabSelection(
                rank: 1,
                starts: new ulong[] { 0 },
                strides: new ulong[] { 1 },
                counts: new[] { nbPointsRequested },
                blocks: new ulong[] { 1 }
            );

            var memoryDims = new[] { nbPointsRequested };
            var result = dataset
                .Read<ushort>(
                    fileSelection: datasetSelection,
                    memorySelection: memorySelection,
                    memoryDims: memoryDims
                );

            return result;
        }

        public void RegisterIntelFilter()
        {
            H5Filter.Register(
                identifier: H5FilterID.Deflate,
                name: "deflate",
                filterFunc: DeflateHelperIntelISA.FilterFunc);
        }
    }

}

