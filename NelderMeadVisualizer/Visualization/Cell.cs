using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadVisualizer.Visualization
{
    public struct Cell
    {
        public readonly double X1, Y1, X2, Y2;
        public readonly double F11, F12, F21, F22;

        public Cell(double x1, double y1, double x2, double y2,
                    double f11, double f12, double f21, double f22)
        {
            X1 = x1; Y1 = y1; X2 = x2; Y2 = y2;
            F11 = f11; F12 = f12; F21 = f21; F22 = f22;
        }
    }
}
