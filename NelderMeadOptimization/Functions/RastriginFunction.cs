using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NelderMeadOptimization.Interface;

namespace NelderMeadOptimization.Functions
{
    //функция Растригина - сложная, много локальных минимумов
    public class RastriginFunction : ITestFunction
    {
        public string Name => "Rastrigin: 20 + x^2 - 10cos(2*pi*x) + y^2 - 10cos(2*pi*y)";
        public int Dimension => 2; // фиксированная размерность
        public double Evaluate(double[] coordinates)
        {
            if (coordinates.Length != 2)
                throw new ArgumentException("Функция Растригина работает только в 2D");
            double x = coordinates[0];
            double y = coordinates[1];
            return 20 + (x * x - 10 * Math.Cos(2 * Math.PI * x)) + (y * y - 10 * Math.Cos(2 * Math.PI * y));
        }

        public double[] GetRecommendedContourLevels(double visibleMin, double visibleMax)
        {
            double[] levels = new double[6];
            for (int i = 0; i < levels.Length; i++)
            {
                levels[i] = 2 + i * 5;
            }
            return levels;
        }
        public double CalculateError(double[] point)
        {
            // Глобальный минимум функции Растригина в (0,0)
            return Math.Sqrt(point[0] * point[0] + point[1] * point[1]);
        }
    }
}
