using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NelderMeadOptimization.Models;

namespace NelderMeadVisualizer.Visualization
{
    internal class Bounds
    {
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }

        public Bounds(double minX, double maxX, double minY, double maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }

        public void Update(Simplex simplex)
        {
            for (int i = 0; i < simplex.Size; i++)
            {
                MinX = Math.Min(MinX, simplex[i][0] - 1);
                MaxX = Math.Max(MaxX, simplex[i][0] + 1);
                MinY = Math.Min(MinY, simplex[i][1] - 1);
                MaxY = Math.Max(MaxY, simplex[i][1] + 1);
            }
        }

        public void Reset()
        {
            MinX = -5;
            MaxX = 8;
            MinY = -5;
            MaxY = 8;
        }
    }
}
