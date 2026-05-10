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
    /// <summary>
    /// Пошаговый оптимизатор для визуализации алгоритма Нелдера-Мида.
    /// Выполняет одну итерацию за раз, позволяя пользователю наблюдать за процессом.
    /// ВНИМАНИЕ: Логика дублирует NelderMeadOptimizer, но это необходимо для 
    /// пошагового режима визуализации.
    /// </summary>
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
            MyPoint reflected = _simplex.Reflect(centroid, _function.Evaluate, _params.Alpha);

            if (reflected.Value < _simplex.Best.Value)
            {
                MyPoint expanded = _simplex.Expand(centroid, reflected, _function.Evaluate, _params.Beta);
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
                    MyPoint contracted = _simplex.ContractOutside(centroid, reflected, _function.Evaluate, _params.Gamma);
                    if (contracted.Value < reflected.Value)
                        _simplex.ReplaceWorst(contracted);
                    else
                        _simplex.Reduce(_function.Evaluate, _params.Sigma);
                }
                else
                {
                    MyPoint contracted = _simplex.ContractInside(centroid, _function.Evaluate, _params.Gamma);
                    if (contracted.Value < _simplex.Worst.Value)
                        _simplex.ReplaceWorst(contracted);
                    else
                        _simplex.Reduce(_function.Evaluate, _params.Sigma);
                }
            }

            if (_simplex.IsConverged(_params.Tolerance) || _iteration >= _params.MaxIterations)
            {
                IsCompleted = true;
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
