using NelderMeadOptimization.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadOptimization.Functions
{
    // Функция Аккли – экспоненциальная c множеством локальных минимумов
    public class AckleyFunction : ITestFunction
    {
        public string Name => "Ackley: -20*exp(-0.2*sqrt(0.5*(x^2+y^2))) - exp(0.5*(cos(2πx)+cos(2πy))) + e + 20";
        public int Dimension => 2;

        public double Evaluate(double[] coordinates)
        {
            if (coordinates.Length != 2)
                throw new ArgumentException("Функция Аккли работает только в 2D");

            double x = coordinates[0];
            double y = coordinates[1];
            double sumSq = x * x + y * y;
            double cosSum = Math.Cos(2 * Math.PI * x) + Math.Cos(2 * Math.PI * y);
            double term1 = -20.0 * Math.Exp(-0.2 * Math.Sqrt(0.5 * sumSq));
            double term2 = -Math.Exp(0.5 * cosSum);
            return term1 + term2 + Math.E + 20.0;
        }

        public double[] GetRecommendedContourLevels(double visibleMin, double visibleMax)
        {
            double min = Math.Max(0, visibleMin);
            double max = Math.Min(visibleMax, 25);
            double[] levels = new double[8];
            for (int i = 0; i < levels.Length; i++)
            {
                levels[i] = min + (max - min) * (i + 1) / (levels.Length + 1);
            }
            return levels;
        }

        public double CalculateError(double[] point)
        {
            // Глобальный минимум функции Аккли в (0,0) со значением 0
            return Math.Sqrt(point[0] * point[0] + point[1] * point[1]);
        }
    }
}
