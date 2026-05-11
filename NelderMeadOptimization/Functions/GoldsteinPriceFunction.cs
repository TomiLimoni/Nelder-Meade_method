using NelderMeadOptimization.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadOptimization.Functions
{
    // Функция Гольдштейна-Прайса - невыпуклая, несколько локальных минимумов
    public class GoldsteinPriceFunction : ITestFunction
    {
        public string Name => "Goldstein-Price: [1 + (x+y+1)^2*(19 - 14x + 3x^2 - 14y + 6xy + 3y^2)] * [30 + (2x-3y)^2*(18 - 32x + 12x^2 + 48y - 36xy + 27y^2)]";
        public int Dimension => 2;

        public double Evaluate(double[] coordinates)
        {
            if (coordinates.Length != 2)
                throw new ArgumentException("Функция Гольдштейна-Прайса работает только в 2D");

            double x = coordinates[0];
            double y = coordinates[1];

            double term1 = 1 + Math.Pow(x + y + 1, 2) * (19 - 14 * x + 3 * x * x - 14 * y + 6 * x * y + 3 * y * y);
            double term2 = 30 + Math.Pow(2 * x - 3 * y, 2) * (18 - 32 * x + 12 * x * x + 48 * y - 36 * x * y + 27 * y * y);

            return term1 * term2;
        }

        public double[] GetRecommendedContourLevels(double visibleMin, double visibleMax)
        {
            double minVal = Math.Max(visibleMin, 3.0);
            double logMin = Math.Log10(minVal);
            double logMax = Math.Log10(visibleMax);
            double[] levels = new double[12];
            for (int i = 0; i < levels.Length; i++)
            {
                double t = (double)i / (levels.Length - 1);
                double logValue = logMin + t * (logMax - logMin);
                levels[i] = Math.Pow(10, logValue);
            }

            for (int i = 0; i < levels.Length; i++)
            {
                if (levels[i] < 10) levels[i] = Math.Round(levels[i], 1);
                else levels[i] = Math.Round(levels[i], 0);
            }
            return levels;
        }

        public double CalculateError(double[] point)
        {
            // Глобальный минимум в (0, -1) со значением 3
            return Math.Sqrt(Math.Pow(point[0] - 0, 2) + Math.Pow(point[1] - (-1), 2));
        }
    }
}
