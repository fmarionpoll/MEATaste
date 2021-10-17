
namespace MEATaste.Infrastructure
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
