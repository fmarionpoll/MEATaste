using MeaTaste.Domain.MeaData.Models;

namespace TasteMEA.Domain.Hdf5.Cache
{
    public class Hdf5Cache : IHdf5CacheReader, IHdf5CacheWriter
    {
        private MeaExperiment meaExperiment;


        public void Save(MeaExperiment currentExperiment)
        {
            meaExperiment = currentExperiment;
        }

        public MeaExperiment Get() => meaExperiment;
    }

    public class FileSaver
    {
        private readonly IHdf5CacheWriter cache;

        public FileSaver(IHdf5CacheWriter cache)
        {
            this.cache = cache;
        }

        public void OpenFileAndSaveInCache() { }
    }

    public class GraphCreator
    {
        private readonly IHdf5CacheReader cache;

        public GraphCreator(IHdf5CacheReader cache)
        {
            this.cache = cache;
        }

        public byte[] CreateFromCurrentExperiment() { return new byte [] { 0, 1, 0, 1, 1, 1 }; }
    }

    public interface IHdf5CacheReader
    {
        MeaExperiment Get();
    }

    public interface IHdf5CacheWriter
    {
        void Save(MeaExperiment meaExperiment);
    }

}
