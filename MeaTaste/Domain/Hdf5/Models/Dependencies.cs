using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeaTaste.Domain.Hdf5.Extracting
{
    
    class MainService
    {
        private IDataReader<string> _stringDataReader;

        public MainService(IDataReader<string> stringDataReader) =>
            _stringDataReader = stringDataReader;

        public void ExecuteTask()
        {
            var result = _stringDataReader.Read("test");

        }
    }

    public class Data
    {
        public string Content { get; set; }
    }


    public interface IDataReader<T>
    {
        Data Read(T input);
    }

    public class StringDataReader : IDataReader<string>
    {
        public Data Read(string input) => new Data { Content = "toto" };
    }

    public class IntegerDataReader : IDataReader<int>
    {
        public Data Read(int input)
        {
            throw new NotImplementedException();
        }
    }

    public class ByteDataReader : IDataReader<byte>
    {
        public Data Read(byte input)
        {
            throw new NotImplementedException();
        }
    }
}
