using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasteMEA.DataMEA.MaxWell;
using Xunit;

namespace Tests
{
    public class Hdf5ReaderTest
    {
        //[Fact]
        public void ShouldOpenFileAndGetContent()
        {
            bool Open_OK = FileReader.OpenReadMaxWellFile(@"E:\2021 MaxWell\Trace_20210715_16_54_48_1mM(+++).raw.h5");
            bool v0_OK = FileReader.IsFileReadableAsMaxWellFile();
        }
    }
}
