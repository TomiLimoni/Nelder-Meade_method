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
    }
}
