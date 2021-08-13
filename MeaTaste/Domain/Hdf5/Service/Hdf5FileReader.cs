using MeaTaste.Domain.MeaData.Models;

namespace MeaTaste.Domain.Hdf5.Service
{
    public static class Hdf5FileReader
    {
        public static MeaExperiment Read(string fileName)
        {
            var rawFileData = HDF5CSharp.Hdf5.ReadFlatFileStructure(fileName);

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
    }
}
