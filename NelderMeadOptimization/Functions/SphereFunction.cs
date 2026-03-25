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
        public string Name => $"Spherical function ({Dimension}D)";
        public int Dimension { get; }
        public SphereFunction(int dimension = 2)
        {
            Dimension = dimension;
        }
        public double Evaluate(double[] coordinates)
        {
            if (coordinates.Length != Dimension)
                throw new ArgumentException($"Ожидалось {Dimension} координат");

            // f(x) = sum(x_i²)
            double sum = 0;
            for (int i = 0; i < Dimension; i++)
            {
                sum += coordinates[i] * coordinates[i];
            }
            return sum;
        }
    }
}