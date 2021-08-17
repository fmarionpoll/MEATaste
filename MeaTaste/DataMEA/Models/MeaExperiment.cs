using System;
using System.Linq;
using System.Diagnostics;

namespace MeaTaste.DataMEA.Models
{
    // Models

    public record MeaExperiment
    {
        public string FileName;
        public Descriptors Descriptors;
        public MicroElectrodeArray MicroElectrodeArray;
    }

    public record Descriptors
    {
        // time
        public DateTime TimeStart;
        public DateTime TimeStop;
        public double SamplingRate = 20000;
        // settings
        public double Gain;
        public double Hpf;
        public double Lsb;
        // mapping
        public ElectrodeChannel[] RecordedChannels;
    }

    public record ElectrodeChannel(
        int ChannelNumber,
        int ElectrodeNumber,
        double XCoordinates_um,
        double YCoordinates_um);
 

    // -------------------------------------

    public class MicroElectrodeArray
    {
        public Pixel[] Pixels { get; set; } // 1024/65000
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
    public record RecordingPoint(int Index, int Amplification);

    public class MicroElectrodeArrayAnalyzer
    {
        private readonly MicroElectrodeArray _microElectrodeArray;

        public MicroElectrodeArrayAnalyzer(MicroElectrodeArray microElectrodeArray) =>
            _microElectrodeArray = microElectrodeArray;

        public void Analyze() { }

        public bool PixelExists(int x, int y) =>
            _microElectrodeArray.Pixels.Any(p => p.X == x && p.Y == y);

        public Pixel GetPixel(int X, int Y) =>
            _microElectrodeArray.Pixels.First(p => p.X == X && p.Y == Y);
    }
}
