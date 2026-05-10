using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NelderMeadOptimization.Interface;

namespace NelderMeadOptimization.Functions
{
    public class QuadraticFunction : ITestFunction
    {
        public string Name => "Quadratic: x^2 + xy + y^2 - 6x - 9y";
        public int Dimension => 2;
        public double Evaluate(double[] coordinates)
        {
            if (coordinates.Length != 2)
                throw new ArgumentException("Функция работает только в 2D");
            double x = coordinates[0];
            double y = coordinates[1];
            return x * x + x * y + y * y - 6 * x - 9 * y;
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
            // Минимум квадратичной функции в (1,4)
            return Math.Sqrt(Math.Pow(point[0] - 1, 2) + Math.Pow(point[1] - 4, 2));
        }
    }
}
