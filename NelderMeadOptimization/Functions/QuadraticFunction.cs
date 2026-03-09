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
        public double Evaluate(double x, double y)
        {
            return x * x + x * y + y * y - 6 * x - 9 * y;
        }
    }
}
