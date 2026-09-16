using System;
using ScottPlot;

namespace MEATaste.Views.MapVoltage
{
    public class SignedVoltageColormap : IColormap
    {
        public string Name => "SignedVoltage";

        private static readonly Color DarkRed = new(140, 0, 0);
        private static readonly Color LightGrey = new(210, 210, 210);
        private static readonly Color DarkGreen = new(0, 110, 0);

        public Color GetColor(double position)
        {
            if (double.IsNaN(position))
                return Colors.Transparent;

            position = Math.Clamp(position, 0, 1);
            if (position < 0.5)
                return Lerp(DarkRed, LightGrey, position * 2);
            return Lerp(LightGrey, DarkGreen, (position - 0.5) * 2);
        }

        private static Color Lerp(Color a, Color b, double t)
        {
            t = Math.Clamp(t, 0, 1);
            return new Color(
                (byte)(a.R + (b.R - a.R) * t),
                (byte)(a.G + (b.G - a.G) * t),
                (byte)(a.B + (b.B - a.B) * t));
        }
    }
}
