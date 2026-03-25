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
    }
}
