using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;
using HDF5.NET;
using MEATaste.DataMEA.MaxWell;
using Xunit;

namespace Tests
{
    public class Hdf5ReaderTest
    {

        private static H5File Root { get; set; }
        private string fileName;

        private string GetFileNameHdd(bool optionHdd)
        {
            return optionHdd ? "E:\\2021 MaxWell\\Trace_20210715_16_54_48_1mM(+++).raw.h5"
                : "C:\\Users\\fred\\Downloads\\Trace_20210715_16_54_48_1mM(+++).raw.h5";
        }

        public bool OpenReadMaxWellFile(string localFileName)
        {
            Root = H5File.OpenRead(localFileName);
            return Root != null;
        }

        private void OpenTestFile()
        {
            fileName = GetFileNameHdd(true);
            if (!OpenReadMaxWellFile(fileName))
                throw new Exception();
        }

        [Fact]
        public void ShouldOpenFileAndGetContent()
        {
            var name = @"E:\2021 MaxWell\Trace_20210715_16_54_48_1mM(+++).raw.h5";
            var h5FileReader = new H5FileReader();
            h5FileReader.OpenReadMaxWellFile(name);
            h5FileReader.IsReadableAsMaxWellFile();
        }
        
        [Fact]
        public void OpenAndReadH5MaxwellFileTest()
        {
            var sw = Stopwatch.StartNew();
            OpenTestFile();
            var unused = ReadAll_OneElectrodeAsInt(863);
            Trace.WriteLine($"read channel in: {sw.Elapsed.TotalSeconds:F3} s");
        }

        public ushort[] ReadAll_OneElectrodeAsInt(int channel)
        {
            var h5Group = Root.Group("/");
            var h5Dataset = h5Group.Dataset("sig");
            int ndimensions = h5Dataset.Space.Rank;
            if (ndimensions != 2)
                return null;
            var nbdatapoints = h5Dataset.Space.Dimensions[1];
            return Read_OneElectrodeDataAsInt(h5Dataset, channel, 0, nbdatapoints - 1);
        }

        [Fact]
        public void OpenAndReadH5MaxwellFileTestWithMmf()
        {
            var sw = Stopwatch.StartNew();
            OpenTestFile();
            var unused = ReadAll_OneElectrodeAsIntWithMmf(863);
            Trace.WriteLine($"read channel in: {sw.Elapsed.TotalSeconds:F3} s");
        }

        public ushort[] ReadAll_OneElectrodeAsIntWithMmf(int channel)
        {
            var fileStream = File.Open(
                fileName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            var mmf = MemoryMappedFile.CreateFromFile(
                fileStream,
                mapName: "MemMap",
                capacity: 0,
                MemoryMappedFileAccess.Read,
                HandleInheritability.None,
                leaveOpen: true);

            var mmfStream = mmf.CreateViewStream(
                offset: 0,
                size: 0,
                MemoryMappedFileAccess.Read);

            var h5File = H5File.Open(mmfStream);

            var h5Group = h5File.Group("/");
            var h5Dataset = h5Group.Dataset("sig");
            int ndimensions = h5Dataset.Space.Rank;
            if (ndimensions != 2)
                return null;
            var nbdatapoints = h5Dataset.Space.Dimensions[1];
            return Read_OneElectrodeDataAsInt(h5Dataset, channel, 0, nbdatapoints - 1);
        }

        [Fact]
        public void OpenAndReadH5MaxwellFileWithThreadsTest()
        {
            var sw = Stopwatch.StartNew();
            OpenTestFile();
            var unused = ReadAll_OneElectrodeAsIntParallel(863);
            Trace.WriteLine($"read channel in: {sw.Elapsed.TotalSeconds:F3} s");
        }

        public ushort[] ReadAll_OneElectrodeAsIntParallel(int channel)
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
                var h5File = H5File.OpenRead(fileName);
                var group = h5File.Group("/");
                var dataset = group.Dataset("sig");

                var istart = (ulong)i * chunkSizePerChannel;
                var iend = istart + chunkSizePerChannel - 1;
                if (iend > nbdatapoints)
                    iend = nbdatapoints - 1;
                var chunkresult = Read_OneElectrodeDataAsInt(dataset, channel, istart, iend);
                Array.Copy(chunkresult, 0, result, (int)istart, (int)(iend - istart + 1));
                h5File.Dispose();
            });

            return result;
        }

        [Fact]
        public void OpenAndReadH5MaxwellFileWithThreadsTestmmf()
        {
            var sw = Stopwatch.StartNew();
            OpenTestFile();
            var unused = ReadAll_OneElectrodeAsIntParallelmmf(863);
            Trace.WriteLine($"read channel in: {sw.Elapsed.TotalSeconds:F3} s");
        }

        public ushort[] ReadAll_OneElectrodeAsIntParallelmmf(int channel)
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

            var fileStream = File.Open(
                fileName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            var mmf = MemoryMappedFile.CreateFromFile(
                fileStream,
                mapName: "MemMap",
                capacity: 0,
                MemoryMappedFileAccess.Read,
                HandleInheritability.None,
                leaveOpen: true);

            Parallel.For(0, nchunks, i =>
            {
                var mmfStream = mmf.CreateViewStream(
                    offset: 0,
                    size: 0,
                    MemoryMappedFileAccess.Read);

                var h5MmFile = H5File.Open(mmfStream);
                var h5MmGroup = h5MmFile.Group("/");
                var h5MmDataset = h5MmGroup.Dataset("sig");

                var istart = (ulong)i * chunkSizePerChannel;
                var iend = istart + chunkSizePerChannel - 1;
                if (iend > nbdatapoints)
                    iend = nbdatapoints - 1;
                var chunkresult = Read_OneElectrodeDataAsInt(h5MmDataset, channel, istart, iend);
                Array.Copy(chunkresult, 0, result, (int)istart, (int)(iend - istart + 1));
                
            });

            return result;
        }

        public ushort[] Read_OneElectrodeDataAsInt(H5Dataset dataset, int channel, ulong startsAt, ulong endsAt)
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

    }


}
