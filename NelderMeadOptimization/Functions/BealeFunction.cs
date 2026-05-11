using NelderMeadOptimization.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadOptimization.Functions
{
    // Функция Била – невыпуклая, с узким оврагом
    public class BealeFunction : ITestFunction
    {
        public string Name => "Beale: (1.5 - x + xy)^2 + (2.25 - x + xy^2)^2 + (2.625 - x + xy^3)^2";
        public int Dimension => 2;

        public double Evaluate(double[] coordinates)
        {
            if (coordinates.Length != 2)
                throw new ArgumentException("Функция Бил работает только в 2D");

            double x = coordinates[0];
            double y = coordinates[1];

            double term1 = Math.Pow(1.5 - x + x * y, 2);
            double term2 = Math.Pow(2.25 - x + x * y * y, 2);
            double term3 = Math.Pow(2.625 - x + x * y * y * y, 2);

            return term1 + term2 + term3;
        }

        public double[] GetRecommendedContourLevels(double visibleMin, double visibleMax)
        {
            double logMin = Math.Log10(Math.Max(0.1, visibleMin));
            double logMax = Math.Log10(Math.Max(visibleMax, 1));
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
            // Глобальный минимум функции Бил в (3, 0.5) со значением 0
            return Math.Sqrt(Math.Pow(point[0] - 3, 2) + Math.Pow(point[1] - 0.5, 2));
        }
    }
}
