using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NelderMeadVisualizer.Visualization
{
    internal static class ColorHelper
    {
        private const int MaxIterationsForColor = 30;

        public static Color GetColorForIteration(int iteration)
        {
            double ratio = System.Math.Min(1.0, (double)iteration / MaxIterationsForColor);

            byte r = (byte)(System.Math.Min(255, ratio * 255 * 1.5));
            byte g = (byte)(System.Math.Min(255, ratio * 255));
            byte b = (byte)(System.Math.Max(0, 200 - ratio * 200));

            return Color.FromRgb(r, g, b);
        }
    }
}
