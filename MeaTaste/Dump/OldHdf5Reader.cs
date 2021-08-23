namespace MEATaste.DataMEA
{
    public class OldHdf5Reader : IHdf5Reader
    {
        public string Read(string fileName) => fileName;
    }

    public class NewHdf5Reader : IHdf5Reader
    {
        public string Read(string fileName) => fileName + "toto";
    }

    public class GlobalHdf5Reader : IHdf5Reader
    {
        private readonly OldHdf5Reader oldReader;
        private readonly NewHdf5Reader newReader;

        public GlobalHdf5Reader(OldHdf5Reader oldReader, NewHdf5Reader newReader)
        {
            this.oldReader = oldReader;
            this.newReader = newReader;
        }

        public string Read(string fileName) => 
            fileName.StartsWith("new") 
                ? newReader.Read(fileName) 
                : oldReader.Read(fileName);
    }

    public interface IHdf5Reader
    {
        string Read(string fileName);
    }
}
