using TasteMEA.DataMEA.Models;

namespace TasteMEA
{
    public class ApplicationState
    {
        public MeaExperiment CurrentMeaExperiment;
        public ushort[] OneIntRow;
        public ScottPlot.WpfPlot[] FormsPlots;
    }
}
