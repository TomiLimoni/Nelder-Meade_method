using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NelderMeadVisualizer.Visualization
{
    internal static class StyleHelper
    {
        private const int MaxIterationsForColor = 30;

        public class PointStyle
        {
            public Brush Brush { get; set; }
            public double Size { get; set; }
        }

        public static Color GetColorForIteration(int iteration)
        {
            double ratio = System.Math.Min(1.0, (double)iteration / MaxIterationsForColor);
            byte r = (byte)(System.Math.Min(255, ratio * 255 * 1.5));
            byte g = (byte)(System.Math.Min(255, ratio * 255));
            byte b = (byte)(System.Math.Max(0, 200 - ratio * 200));
            return Color.FromRgb(r, g, b);
        }

        public static Brush GetLineBrush(int iteration, double ageRatio)
        {
            byte alpha = (byte)(60 + ageRatio * 150);
            Color baseColor = GetColorForIteration(iteration);
            Color fadedColor = Color.FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);
            return new SolidColorBrush(fadedColor);
        }

        public static double GetLineThickness(double ageRatio)
        {
            return 1.5 + ageRatio * 2.5;
        }

        public static PointStyle GetPointStyle(int index, int totalPoints)
        {
            if (index == 0)
                return new PointStyle { Brush = Brushes.Green, Size = 12 };
            if (index == totalPoints - 1)
                return new PointStyle { Brush = Brushes.Red, Size = 9 };
            return new PointStyle { Brush = Brushes.DarkBlue, Size = 8 };
        }
    }
}
