using NelderMeadOptimization.Interface;
using NelderMeadOptimization.Models;
using NelderMeadOptimization.Optimizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPoint = NelderMeadOptimization.Models.Point;

namespace NelderMeadVisualizer.Optimization
{
    internal class StepByStepOptimizer
    {
        private ITestFunction _function;
        private Parameters _params;
        private Simplex _simplex;
        private int _iteration;

        public Simplex CurrentSimplex => _simplex;
        public bool IsCompleted { get; private set; }

        public event Action<Simplex, int> IterationCompleted;

        public void Initialize(ITestFunction function, Parameters parameters, MyPoint[] initialPoints)
        {
            _function = function;
            _params = parameters;
            _simplex = new Simplex(initialPoints);
            _iteration = 0;
            IsCompleted = false;

            IterationCompleted?.Invoke(_simplex, _iteration);
        }

        public bool PerformOneIteration()
        {
            if (IsCompleted) return false;

            if (_simplex.IsConverged(_params.Tolerance) || _iteration >= _params.MaxIterations)
            {
                IsCompleted = true;
                return false;
            }

            _iteration++;

            double[] centroid = _simplex.GetCentroidCoordinates();
            MyPoint reflected = _simplex.Reflect(centroid, _function.Evaluate, 1.0);

            if (reflected.Value < _simplex.Best.Value)
            {
                MyPoint expanded = _simplex.Expand(centroid, reflected, _function.Evaluate, 2.0);
                _simplex.ReplaceWorst(expanded.Value < reflected.Value ? expanded : reflected);
            }
            else if (reflected.Value < _simplex.SecondWorst.Value)
            {
                _simplex.ReplaceWorst(reflected);
            }
            else
            {
                if (reflected.Value < _simplex.Worst.Value)
                {
                    MyPoint contracted = _simplex.ContractOutside(centroid, reflected, _function.Evaluate, 0.5);
                    if (contracted.Value < reflected.Value)
                        _simplex.ReplaceWorst(contracted);
                    else
                        _simplex.Reduce(_function.Evaluate, 0.5);
                }
                else
                {
                    MyPoint contracted = _simplex.ContractInside(centroid, _function.Evaluate, 0.5);
                    if (contracted.Value < _simplex.Worst.Value)
                        _simplex.ReplaceWorst(contracted);
                    else
                        _simplex.Reduce(_function.Evaluate, 0.5);
                }
            }

            IterationCompleted?.Invoke(_simplex, _iteration);
            return true;
        }

        public void Reset()
        {
            _simplex = null;
            _iteration = 0;
            IsCompleted = false;
        }
    }
}
