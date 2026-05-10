using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NelderMeadOptimization.Interface;

namespace NelderMeadOptimization.Functions
{
    //Функция Розенброка - с оврагом
    public class RosenbrockFunction : ITestFunction
    {
        public string Name => "Rosenbrock: (1-x)^2 + 100(y-x^2)^2";
        public int Dimension => 2; // фиксированная размерность
        public double Evaluate(double[] coordinates)
        {
            if (coordinates.Length != 2)
                throw new ArgumentException("Функция Розенброка работает только в 2D");
            double x = coordinates[0];
            double y = coordinates[1];
            return Math.Pow(1 - x, 2) + 100 * Math.Pow(y - x * x, 2);
        }

        public double[] GetRecommendedContourLevels(double visibleMin, double visibleMax)
        {
            double logMin = Math.Log10(Math.Max(0.1, visibleMin));
            double logMax = Math.Log10(Math.Max(visibleMax, 1));

            double[] levels = new double[12];
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
            // Минимум функции Розенброка в (1,1)
            return Math.Sqrt(Math.Pow(point[0] - 1, 2) + Math.Pow(point[1] - 1, 2));
        }
    }
}
