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
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            var result1 = fileReader.ReadAll_OneElectrodeAsInt(electrodeProperties);
            //stopwatch.Stop();
            //Trace.WriteLine("Elapsed time -direct- is " +(stopwatch.ElapsedMilliseconds/1000).ToString("0.###") +" s" );

            //stopwatch.Start();
            //var result2 = fileReader.ReadAll_OneElectrodeAsIntParallel(electrodeProperties);
            //stopwatch.Stop();
            //Trace.WriteLine("Elapsed time -parallel- is " + (stopwatch.ElapsedMilliseconds / 1000).ToString("0.###") + " s");

            return result1;
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
            H5Group group = Root.Group("/");
            H5Dataset dataset = group.Dataset("version");
            string[] data = dataset.ReadString();
            FileVersion = data[0];
            
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
            var meaExp = new MeaExperiment(FileName, FileVersion, new Descriptors());
            ReadSettingsDescriptors(meaExp);
            ReadTimeDescriptors(meaExp);
            ReadMapElectrodes(meaExp);
            return meaExp;
        }

        public bool ReadTimeDescriptors(MeaExperiment meaExp)
        {
            H5Group group = Root.Group("/");
            H5Dataset dataset = group.Dataset("time");
            string[] data = dataset.ReadString();
            string lines = data[0];
            string[] strings = lines.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            meaExp.Descriptors.TimeStart = GetTimeFromString(strings[0], "start: ");
            meaExp.Descriptors.TimeStop = GetTimeFromString(strings[1], "stop: ");

            return true;
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
            H5Group group = Root.Group("/settings");

            double[] gainarray = ReadDoubleDataFromGroup(group, "gain");
            meaExp.Descriptors.Gain = gainarray[0];

            double[] hpfarray = ReadDoubleDataFromGroup(group, "hpf");
            meaExp.Descriptors.Hpf = hpfarray[0];

            double[] lsbarray = ReadDoubleDataFromGroup(group, "lsb");
            meaExp.Descriptors.Lsb = lsbarray[0];
            
            return true;
        }

        private double[] ReadDoubleDataFromGroup(H5Group group, string headerName)
        {
            H5Dataset datasetGain = group.Dataset(headerName);
            double[] results = datasetGain.Read<double>();
            return results;
        }

        public bool ReadMapElectrodes(MeaExperiment meaExp)
        {
            H5Group group = Root.Group("/");
            H5Dataset dataset = group.Dataset("mapping");
            Legacy.DatasetMembers[] compoundData = dataset.Read<Legacy.DatasetMembers>();

            meaExp.Descriptors.Electrodes = new ElectrodeProperties[compoundData.Length];
            for (int i = 0; i < compoundData.Length; i++)
            {
                ElectrodeProperties ec = new ElectrodeProperties(
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
            H5Group group = Root.Group("/");
            H5Dataset dataset = group.Dataset("sig");
            int ndimensions = dataset.Space.Rank;
            if (ndimensions != 2)
                return null; 
            var nbdatapoints = dataset.Space.Dimensions[1];      // any size*
            return Read_OneElectrodeDataAsInt(group, dataset, electrodeProperties.Channel, 0, nbdatapoints -1);
        }

        public ushort[] ReadAll_OneElectrodeAsIntParallel(ElectrodeProperties electrodeProperties)
        {
            H5Group group = Root.Group("/");
            H5Dataset dataset = group.Dataset("sig");
            var nbdatapoints = dataset.Space.Dimensions[1];      // any size*
            const ulong chunkSizePerChannel = 200;
            var result = new ushort[nbdatapoints];
            var nchunks = (long) (nbdatapoints / chunkSizePerChannel) ;

            int ndimensions = dataset.Space.Rank;
            if (ndimensions != 2)
                return null;

            Parallel.For (0, nchunks, i =>
            {
                var istart = (ulong) i * chunkSizePerChannel;
                var iend = istart + chunkSizePerChannel - 1;
                if (iend > nbdatapoints)
                    iend = nbdatapoints - 1;
                var chunkresult = Read_OneElectrodeDataAsInt(group, dataset, electrodeProperties.Channel, istart, iend);
                Array.Copy(chunkresult, 0, result, (int) istart, (int) (iend - istart + 1));
            }) ;

            return result;
        }

        public ushort[] Read_OneElectrodeDataAsInt(H5Group group, H5Dataset dataset, int channel, ulong startsAt, ulong endsAt)
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

