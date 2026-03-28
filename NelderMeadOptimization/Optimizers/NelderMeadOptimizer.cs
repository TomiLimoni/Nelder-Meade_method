using NelderMeadOptimization.Interface;
using NelderMeadOptimization.Models;
using System;

namespace NelderMeadOptimization.Optimizers
{
    internal class NelderMeadOptimizer
    {
        private readonly ITestFunction _function;
        private readonly Parameters _params;

        public NelderMeadOptimizer(ITestFunction function, Parameters parameters = null)
        {
            _function = function ?? throw new ArgumentNullException(nameof(function));
            _params = parameters ?? new Parameters();
        }

        public OptimizationResult Optimize(Point[] initialPoints)
        {
            if (initialPoints == null)
                throw new ArgumentNullException(nameof(initialPoints));

            int requiredPoints = _function.Dimension + 1;
            if (initialPoints.Length != requiredPoints)
                throw new ArgumentException($"Для размерности {_function.Dimension} нужно {requiredPoints} точек");

            var simplex = new Simplex(initialPoints);
            int iteration = 0;

            while (iteration < _params.MaxIterations && !simplex.IsConverged(_params.Tolerance))
            {
                iteration++;

                double[] centroid = simplex.GetCentroidCoordinates();
                Point reflected = simplex.Reflect(centroid, _function.Evaluate, _params.Alpha);

                if (reflected.Value < simplex.Best.Value)
                {
                    Point expanded = simplex.Expand(centroid, reflected, _function.Evaluate, _params.Beta);
                    simplex.ReplaceWorst(expanded.Value < reflected.Value ? expanded : reflected);
                }
                else if (reflected.Value < simplex.SecondWorst.Value)
                {
                    simplex.ReplaceWorst(reflected);
                }
                else
                {
                    if (reflected.Value < simplex.Worst.Value)
                    {
                        Point contracted = simplex.ContractOutside(centroid, reflected, _function.Evaluate, _params.Gamma);

                        if (contracted.Value < reflected.Value)
                            simplex.ReplaceWorst(contracted);
                        else
                            simplex.Reduce(_function.Evaluate, _params.Sigma);
                    }
                    else
                    {
                        Point contracted = simplex.ContractInside(centroid, _function.Evaluate, _params.Gamma);

                        if (contracted.Value < simplex.Worst.Value)
                            simplex.ReplaceWorst(contracted);
                        else
                            simplex.Reduce(_function.Evaluate, _params.Sigma);
                    }
                }
            }

            return new OptimizationResult
            {
                OptimalPoint = simplex.Best,
                Iterations = iteration,
                Converged = iteration < _params.MaxIterations,
                FunctionName = _function.Name
            };
        }
    }
}
