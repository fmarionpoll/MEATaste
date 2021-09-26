using System;
using System.Diagnostics;
using System.Threading.Tasks;
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

        public ushort[] ReadDataForOneElectrode(ElectrodeProperties electrodeProperties)
        {
            var stopwatch = new Stopwatch();
            //stopwatch.Start();
            //var result1 = fileReader.ReadAll_OneElectrodeAsInt(electrodeProperties);
            //stopwatch.Stop();
            //Trace.WriteLine("Elapsed time -direct- is " + (stopwatch.ElapsedMilliseconds / 1000).ToString("0.###") + " s");
            //return result1;

            stopwatch.Start();
            var result2 = fileReader.ReadAll_OneElectrodeAsIntParallel(electrodeProperties);
            stopwatch.Stop();
            Trace.WriteLine("Elapsed time -parallel- is " + (stopwatch.ElapsedMilliseconds / 1000).ToString("0.###") + " s");
            return result2;
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
            var h5Group = Root.Group("/");
            var h5Dataset = h5Group.Dataset("version");
            var data = h5Dataset.ReadString();
            FileVersion = data[0];
            
            switch (FileVersion)
            {
                case "20160704":
                    Trace.WriteLine($"MaxWell file version: legacy ({FileVersion}) or v0");
                    return true;
                case "20190530":
                    Trace.WriteLine($"MaxWell file version: 2021 on ({FileVersion}) or v1");
                    return false;
                default:
                    Trace.WriteLine("unknown file structure");
                    break;
            }
            return false;
        }

        public MeaExperiment GetExperimentInfos()
        {
            var meaExp = new MeaExperiment(FileName, FileVersion, new Descriptors());
            ReadSettingsDescriptors(meaExp);
            ReadTimeDescriptors(meaExp);
            ReadMapElectrodes(meaExp);
            return meaExp;
        }

        public bool ReadTimeDescriptors(MeaExperiment meaExp)
        {
            var h5Group = Root.Group("/");
            var h5Dataset = h5Group.Dataset("time");
            var data = h5Dataset.ReadString();
            var lines = data[0];
            var strings = lines.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            meaExp.Descriptors.TimeStart = GetTimeFromString(strings[0], "start: ");
            meaExp.Descriptors.TimeStop = GetTimeFromString(strings[1], "stop: ");

            return true;
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

        public bool ReadSettingsDescriptors(MeaExperiment meaExp)
        {
            var h5Group = Root.Group("/settings");

            var gainarray = ReadDoubleDataFromGroup(h5Group, "gain");
            meaExp.Descriptors.Gain = gainarray[0];

            var hpfarray = ReadDoubleDataFromGroup(h5Group, "hpf");
            meaExp.Descriptors.Hpf = hpfarray[0];

            var lsbarray = ReadDoubleDataFromGroup(h5Group, "lsb");
            meaExp.Descriptors.Lsb = lsbarray[0];
            
            return true;
        }

        private double[] ReadDoubleDataFromGroup(H5Group h5Group, string headerName)
        {
            var datasetGain = h5Group.Dataset(headerName);
            return datasetGain.Read<double>();
        }

        public bool ReadMapElectrodes(MeaExperiment meaExp)
        {
            var h5Group = Root.Group("/");
            var h5Dataset = h5Group.Dataset("mapping");
            Legacy.DatasetMembers[] compoundData = h5Dataset.Read<Legacy.DatasetMembers>();

            meaExp.Descriptors.Electrodes = new ElectrodeProperties[compoundData.Length];
            for (var i = 0; i < compoundData.Length; i++)
            {
                var ec = new ElectrodeProperties(
                    compoundData[i].electrode,
                    compoundData[i].channel,
                    compoundData[i].x,
                    compoundData[i].y);
                meaExp.Descriptors.Electrodes[i] = ec;
            }
            return true;
        }

        public ushort[] ReadAll_OneElectrodeAsInt(ElectrodeProperties electrodeProperties)
        {
            var h5Group = Root.Group("/");
            var h5Dataset = h5Group.Dataset("sig");
            int ndimensions = h5Dataset.Space.Rank;
            if (ndimensions != 2)
                return null; 
            var nbdatapoints = h5Dataset.Space.Dimensions[1]; 
            return Read_OneElectrodeDataAsInt(h5Dataset, electrodeProperties.Channel, 0, nbdatapoints -1);
        }

        public ushort[] ReadAll_OneElectrodeAsIntParallel(ElectrodeProperties electrodeProperties)
        {
            var h5Group = Root.Group("/");
            var h5Dataset = h5Group.Dataset("sig");
            var nbdatapoints = h5Dataset.Space.Dimensions[1];
            const ulong chunkSizePerChannel = 200;
            var result = new ushort[nbdatapoints];
            var nchunks = (long)(nbdatapoints / chunkSizePerChannel);

            int ndimensions = h5Dataset.Space.Rank;
            if (ndimensions != 2)
                return null;

            Parallel.For(0, nchunks, i =>
            {
                var fileName = FileName;
                var lRoot = H5File.OpenRead(fileName);
                var lgroup = lRoot.Group("/");
                var ldataset = lgroup.Dataset("sig");

                var istart = (ulong)i * chunkSizePerChannel;
                var iend = istart + chunkSizePerChannel - 1;
                if (iend > nbdatapoints)
                    iend = nbdatapoints - 1;
                var chunkresult = Read_OneElectrodeDataAsInt(ldataset, electrodeProperties.Channel, istart, iend);
                Array.Copy(chunkresult, 0, result, (int)istart, (int)(iend - istart + 1));
                lRoot.Dispose();
            });

            return result;
        }

        public ushort[] Read_OneElectrodeDataAsInt(H5Dataset dataset, int channel, ulong startsAt, ulong endsAt)
        {
            var nbPointsRequested = endsAt - startsAt + 1;

            //Trace.WriteLine($"startsAt: {startsAt} endsAt: {endsAt} nbPointsRequested={nbPointsRequested}");
            
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



    }

}

