
// adapted from Illya Reznykov https://github.com/IReznykov/Blog

namespace MEATaste.DataMEA.Models
{
    public class DynamicGrid
    {
        public int NColumns { get; set; }
        public int NRows { get; set; }

        public DynamicGrid(int ncols, int nrows)
        {
            NColumns = ncols;
            NRows = nrows;
        }
    }
}
