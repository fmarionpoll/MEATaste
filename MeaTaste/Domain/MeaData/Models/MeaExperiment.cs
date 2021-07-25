using System.Linq;

namespace MeaTaste.Domain.MeaData.Models
{
    public class MicroElectrodeArrayAnalyzor
    {
        private readonly MicroElectrodeArray _microElectrodeArray;

        public MicroElectrodeArrayAnalyzor(MicroElectrodeArray microElectrodeArray) =>
            _microElectrodeArray = microElectrodeArray;

        public void Analyze() { }

        public bool PixelExists(int x, int y) =>
            _microElectrodeArray.Pixels.Any(p => p.X == x && p.Y == y);

        public Pixel GetPixel(int X, int Y) =>
            _microElectrodeArray.Pixels.First(p => p.X == X && p.Y == Y);
    }

    // Models

    public class MeaExperiment
    {
        public Descriptors Descriptors { get; set; }
        public MicroElectrodeArray MicroElectrodeArray { get; set; }
    }

    public class Descriptors
    {
        // props
    }

    public class MicroElectrodeArray
    {
        public Pixel[] Pixels { get; set; } // 1024
    }

    public class Pixel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double Value => CalculateMedian();
        public Recording[] Recordings { get; set; }

        public double CalculateMedian()
        {
            // calculate median from Recordings
            return 0;
        }
    }

    public record Recording(double Gain, double Period, RecordingPoint[] Points);
    public record RecordingPoint(int Index, int Amplication);
}
