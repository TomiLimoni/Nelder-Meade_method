using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NelderMeadOptimization.Interface;


namespace NelderMeadOptimization.Functions
{
    public class SphereFunction : ITestFunction
    {
        public string Name => $"Spherical function ({Dimension}D)";
        public int Dimension { get; }
        public SphereFunction(int dimension = 2)
        {
            Dimension = dimension;
        }
        public double Evaluate(double[] coordinates)
        {
            if (coordinates.Length != Dimension)
                throw new ArgumentException($"Ожидалось {Dimension} координат");

            // f(x) = sum(x_i²)
            double sum = 0;
            for (int i = 0; i < Dimension; i++)
            {
                sum += coordinates[i] * coordinates[i];
            }
            return sum;
        }

        public double[] GetRecommendedContourLevels(double visibleMin, double visibleMax)
        {
            double min = Math.Max(0, visibleMin);
            double max = visibleMax;
            double[] levels = new double[8];
            for (int i = 0; i < levels.Length; i++)
            {
                levels[i] = min + (max - min) * (i + 1) / (levels.Length + 1);
            }
            return levels;
        }
        public double CalculateError(double[] point)
        {
            // Для сферической функции минимум в (0,0,...)
            double sum = 0;
            for (int i = 0; i < point.Length; i++)
                sum += point[i] * point[i];
            return Math.Sqrt(sum);
        }
    }
}