using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NelderMeadOptimization.Interface;


namespace NelderMeadOptimization.Functions
{
    public class SphereFunction : ITestFunction
    {
        public string Name => "Sphere func";

        public double Evaluate(double x, double y)
        {
            return x * x + y * y;
        }
    }
}