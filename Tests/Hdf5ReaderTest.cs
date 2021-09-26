using MEATaste.DataMEA.MaxWell;
using Xunit;

namespace Tests
{
    public class Hdf5ReaderTest
    {
        [Fact]
        public void ShouldOpenFileAndGetContent()
        {
            var name = @"E:\2021 MaxWell\Trace_20210715_16_54_48_1mM(+++).raw.h5";
            var h5FileReader = new H5FileReader();
            h5FileReader.OpenReadMaxWellFile(name);
            h5FileReader.IsReadableAsMaxWellFile();
        }
    }
}
