using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MEATaste.DataMEA.MaxWell;
using Xunit;

namespace Tests
{
    public class Hdf5ReaderTest
    {
        //[Fact]
        public void ShouldOpenFileAndGetContent(FileReader fileReader, string fileName)
        {
            if (fileName == null)
                fileName = @"E:\2021 MaxWell\Trace_20210715_16_54_48_1mM(+++).raw.h5";
            bool Open_OK = fileReader.OpenReadMaxWellFile(fileName);
            bool v0_OK = fileReader.IsFileReadableAsMaxWellFile();
        }
    }
}
