using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadOptimization.Interface
{
    internal interface ITestFunction
    {
        string Name { get; }
        double Evaluate(double[] coordinates);
        int Dimension { get; }
    }
}
