using System;
using System.Linq;
using System.Diagnostics;

namespace MeaTaste.Domain.DataMEA.Models
{
    // Models

    public class MeaExperiment
    {
        public string FileName { get; set; }
        public Descriptors Descriptors { get; set; }
        public MicroElectrodeArray MicroElectrodeArray { get; set; }
    }

    public class Descriptors
    {
        // time
        public DateTime TimeStart { get; set; }
        public DateTime TimeStop { get; set; }
        public double SamplingRate { get; set; } = 20000;
        // settings
        public double Gain { get; set; }
        public double Hpf { get; set; }
        public double Lsb { get; set; }
        // mapping
        public ElectrodeChannel[] RecordedChannels;
    }

    public class ElectrodeChannel
    {
        public int ChannelNumber;
        public int ElectrodeNumber;
        public double XCoordinates_um;
        public double YCoordinates_um;
    }

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
    public record RecordingPoint(int Index, int Amplication);

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
}
