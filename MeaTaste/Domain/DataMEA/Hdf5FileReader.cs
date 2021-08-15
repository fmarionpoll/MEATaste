using MeaTaste.Domain.MeaData.Models;
using HDF5CSharp;

namespace MeaTaste.Domain.Hdf5.Service
{
    public static class Hdf5FileReader
    {
        public static MeaExperiment Read(string fileName)
        {
            //var rawFileData = HDF5CSharp.Hdf5.ReadFlatFileStructure(fileName);
            var fileId2 = H5F.open(fileName, H5F.OpenMode.ACC_RDONLY);
            //Open group
            H5GroupId groupId2 = H5G.open(fileId2, "/3BData/");


            return new MeaExperiment
            {
                Descriptors = new Descriptors
                {

                },
                MicroElectrodeArray = new MicroElectrodeArray
                {
                    Pixels = new Pixel[1024]
                }
            };
        }

        public static string ReadVersion(string fileName)
        {
            long fileId = -1;
            string version = "none";
            try
            {
                fileId = Hdf5.OpenFile(fileName, true);
                var result = Hdf5.ReadObject<Coordinate>(fileId, "/version");
            }
            finally
            {
                if (fileId > 0)
                {
                    Hdf5.CloseFile(fileId);
                }
            }
            return version;
        }


    }
}
