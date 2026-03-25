using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadOptimization.Optimizers
{
    internal class Parameters
    {
        public double Alpha { get; set; } = 1.0;   // отражение
        public double Beta { get; set; } = 2.0;    // растяжение
        public double Gamma { get; set; } = 0.5;   // сжатие
        public double Sigma { get; set; } = 0.5;   // редукция
        public double Tolerance { get; set; } = 1e-6; // точность
        public int MaxIterations { get; set; } = 1000; // максимум итераций
        public int Dimension { get; set; } // Размерность пространства

        public Parameters() { }

        public Parameters(double alpha, double beta, double gamma,
                         double sigma, double tolerance, int maxIterations)
        {
            Alpha = alpha;
            Beta = beta;
            Gamma = gamma;
            Sigma = sigma;
            Tolerance = tolerance;
            MaxIterations = maxIterations;
        }
    }
}
