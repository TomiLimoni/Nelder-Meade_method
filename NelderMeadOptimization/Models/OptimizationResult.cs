using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadOptimization.Models
{
    internal class OptimizationResult
    {
        public Point OptimalPoint { get; set; }
        public int  Iterations { get; set; }
        public bool Converged { get; set; }
        public string FunctoinName { get; set; }
        public void Print()
        {
            Console.WriteLine($"\n Result for: {FunctoinName}");
            Console.WriteLine($"Optimal Point: {OptimalPoint}");
            Console.WriteLine($"Number of iterations: {Iterations}");
            Console.WriteLine($"Converged? {(Converged ? "Yes" : "No")}");
        }
    }
}
