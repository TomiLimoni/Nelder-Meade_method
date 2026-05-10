using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NelderMeadOptimization.Interface;
using NelderMeadOptimization.Models;
using NelderMeadOptimization.Optimizers;

namespace NelderMeadVisualizer.Services
{
    internal class OptimizationData
    {
        public ITestFunction Function { get; set; }
        public Simplex Simplex { get; set; }
        public int Iteration { get; set; }
        public Parameters Parameters { get; set; }
        public bool Converged { get; set; }
    }
}
