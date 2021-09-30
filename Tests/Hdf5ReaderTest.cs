using System;
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
        private readonly H5FileReader h5FileReader = new();

        private string GetFileNameHdd(bool optionHdd)
        {
            return optionHdd ? @"E:\2021 MaxWell\Trace_20210715_16_54_48_1mM(+++).raw.h5"
                             : @"C:\Users\fred\Downloads\Trace_20210715_16_54_48_1mM(+++).raw.h5";
        }

        public bool OpenReadMaxWellFile(string localFileName)
        {
            Root = H5File.OpenRead(localFileName);
            return Root != null;
        }

        private void RegisterIntelFilter()
        {
            H5Filter.Register(
                identifier: H5FilterID.Deflate,
                name: "deflate",
                filterFunc: DeflateHelper_Intel_ISA_L.FilterFunc);
        }

        private void OpenTestFile()
        {
            fileName = GetFileNameHdd(false);
            if (!OpenReadMaxWellFile(fileName))
                throw new Exception();
        }

        [Fact]
        public void ShouldOpenFileAndGetContent()
        {
            var name = GetFileNameHdd(false);
            h5FileReader.OpenReadMaxWellFile(name);
            h5FileReader.IsReadableAsMaxWellFile();
        }
        
        [Fact]
        public void OpenAndReadH5MaxwellFileTest()
        {
            OpenTestFile();
            var unused = ReadAll_OneElectrodeAsInt(863);
        }

        [Fact]
        public void OpenAndReadH5MaxwellFileAndIntelFilterTest()
        {
            OpenTestFile();
            RegisterIntelFilter();
            var unused = ReadAll_OneElectrodeAsInt(863);
        }

        public ushort[] ReadAll_OneElectrodeAsInt(int channel)
        {
            var h5Group = Root.Group("/");
            var h5Dataset = h5Group.Dataset("sig");
            int ndimensions = h5Dataset.Space.Rank;
            if (ndimensions != 2)
                return null;
            var nbdatapoints = h5Dataset.Space.Dimensions[1];
            return h5FileReader.ReadIntervalForOneChannelAsInt(h5Dataset, channel, 0, nbdatapoints - 1);
        }

        [Fact]
        public void OpenAndReadH5MaxwellFileTestWithMmf()
        {
            OpenTestFile();
            var unused = ReadAll_OneElectrodeAsIntWithMmf(863);
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
            return h5FileReader.ReadIntervalForOneChannelAsInt(h5Dataset, channel, 0, nbdatapoints - 1);
        }

        [Fact]
        public void OpenAndReadH5MaxwellFileWithThreadsTest()
        {
            OpenTestFile();
            RegisterIntelFilter();
            var unused = ReadAll_OneElectrodeAsIntParallel(863);
        }

        public ushort[] ReadAll_OneElectrodeAsIntParallel(int channel)
        {
            var h5Group = Root.Group("/");
            var h5Dataset = h5Group.Dataset("sig");

            var nbdatapoints = h5Dataset.Space.Dimensions[1];
            const ulong chunkSizePerChannel = 200 * 100;
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
                var chunkresult = h5FileReader.ReadIntervalForOneChannelAsInt(dataset, channel, istart, iend);
                Array.Copy(chunkresult, 0, result, (int)istart, (int)(iend - istart + 1));
                h5File.Dispose();
            });

            return result;
        }



    }


}
