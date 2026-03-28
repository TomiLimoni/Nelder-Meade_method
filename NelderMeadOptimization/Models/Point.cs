using System;
using System.Linq;

namespace NelderMeadOptimization.Models
{
    internal class Point
    {
        public double[] Coordinates { get; }
        public int Dimension => Coordinates.Length;
        public double Value { get; }

        private Point(double[] coordinates, double value)
        {
            Coordinates = coordinates ?? throw new ArgumentNullException(nameof(coordinates));
            Value = value;
        }

        public static Point Create(double[] coordinates, Func<double[], double> evaluate)
        {
            if (evaluate == null)
                throw new ArgumentNullException(nameof(evaluate));

            double value = evaluate(coordinates);
            return new Point(coordinates, value);
        }

        public double this[int index] => Coordinates[index];

        public override string ToString()
        {
            string coords = string.Join("; ", Coordinates.Select(c => c.ToString("F6")));
            return $"({coords}) = {Value:F6}";
        }
    }
}
