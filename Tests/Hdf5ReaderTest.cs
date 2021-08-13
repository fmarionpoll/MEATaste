using MeaTaste.Domain.Hdf5.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class Hdf5ReaderTest
    {
        //[Fact]
        public void ShouldOpenFileAndGetContent()
        {
            var content = Hdf5FileReader.Read(@"E:\2021 MaxWell\Trace_20210715_16_54_48_1mM(+++).raw.h5");
        }
    }
}
