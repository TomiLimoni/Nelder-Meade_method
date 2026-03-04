using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadOptimization.Models
{
    internal class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double Value { get; set; }

        public Point(double x, double y, double value)
        {
            X = x;
            Y = y;
            Value = value;
        }
        public override string ToString()
        {
            return $" ({X}; {Y})={Value}";
        }
    }
}
