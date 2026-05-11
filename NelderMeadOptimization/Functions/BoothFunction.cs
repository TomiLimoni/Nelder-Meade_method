using NelderMeadOptimization.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadOptimization.Functions
{
    // Функция Бута – квадратичный овраг
    public class BoothFunction : ITestFunction
    {
        public string Name => "Booth: (x + 2y - 7)^2 + (2x + y - 5)^2";
        public int Dimension => 2;

        public double Evaluate(double[] coordinates)
        {
            if (coordinates.Length != 2)
                throw new ArgumentException("Функция Бута работает только в 2D");
            double x = coordinates[0];
            double y = coordinates[1];
            return Math.Pow(x + 2 * y - 7, 2) + Math.Pow(2 * x + y - 5, 2);
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
            // Глобальный минимум функции Бута в (1, 3) со значением 0
            return Math.Sqrt(Math.Pow(point[0] - 1, 2) + Math.Pow(point[1] - 3, 2));
        }
    }
}
