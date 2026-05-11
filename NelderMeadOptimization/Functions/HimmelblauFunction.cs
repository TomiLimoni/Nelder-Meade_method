using NelderMeadOptimization.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadOptimization.Functions
{
    // Функция Химмельблау – имеет 4 одинаковых глобальных минимума
    public class HimmelblauFunction : ITestFunction
    {
        public string Name => "Himmelblau: (x^2 + y - 11)^2 + (x + y^2 - 7)^2";
        public int Dimension => 2;

        public double Evaluate(double[] coordinates)
        {
            if (coordinates.Length != 2)
                throw new ArgumentException("Функция Химмельблау работает только в 2D");
            double x = coordinates[0];
            double y = coordinates[1];
            return Math.Pow(x * x + y - 11, 2) + Math.Pow(x + y * y - 7, 2);
        }

        public double[] GetRecommendedContourLevels(double visibleMin, double visibleMax)
        {
            double logMin = Math.Log10(Math.Max(0.1, visibleMin));
            double logMax = Math.Log10(visibleMax);
            double[] levels = new double[10];
            for (int i = 0; i < levels.Length; i++)
            {
                double t = (double)i / (levels.Length - 1);
                double logValue = logMin + t * (logMax - logMin);
                levels[i] = Math.Pow(10, logValue);
            }
            for (int i = 0; i < levels.Length; i++)
            {
                if (levels[i] < 1) levels[i] = Math.Round(levels[i], 2);
                else if (levels[i] < 10) levels[i] = Math.Round(levels[i], 1);
                else levels[i] = Math.Round(levels[i], 0);
            }
            return levels;
        }

        public double CalculateError(double[] point)
        {
            // Глобальные минимумы функции Химмельблау в точках со значением 0: 
            // (3,2), (-2.805118, 3.131312), (-3.779310, -3.283186), (3.584428, -1.848126)
            double[][] minima = new double[][]
            {
                new double[] { 3.0, 2.0 },
                new double[] { -2.805118, 3.131312 },
                new double[] { -3.779310, -3.283186 },
                new double[] { 3.584428, -1.848126 }
            };

            double minDistance = double.MaxValue;
            foreach (var minimum in minima)
            {
                double distance = Math.Sqrt(Math.Pow(point[0] - minimum[0], 2) + Math.Pow(point[1] - minimum[1], 2));
                if (distance < minDistance)
                    minDistance = distance;
            }
            return minDistance;
        }
    }
}
