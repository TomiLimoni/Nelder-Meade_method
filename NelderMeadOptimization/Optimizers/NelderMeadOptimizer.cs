using NelderMeadOptimization.Interface;
using NelderMeadOptimization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadOptimization.Optimizers
{
    internal class NelderMeadOptimizer
    {
        private readonly ITestFunction function;
        private readonly Parameters parameters;

        public NelderMeadOptimizer(ITestFunction function, Parameters parameters = null)
        {
            this.function = function ?? throw new ArgumentNullException(nameof(function));
            this.parameters = parameters ?? new Parameters();
            this.parameters.Dimension = function.Dimension; // запоминаем размерность
        }

        public OptimizationResult Optimize(Point[] initialPoints)
        {
            if (initialPoints == null)
                throw new ArgumentNullException(nameof(initialPoints));

            // Для N измерений нужно N+1 точек
            int requiredPoints = function.Dimension + 1;
            if (initialPoints.Length != requiredPoints)
                throw new ArgumentException($"Для размерности {function.Dimension} нужно {requiredPoints} точек");

            // Создаём и сортируем симплекс
            var simplex = new Simplex(initialPoints);
            simplex.Sort();

            int iteration = 0;

            // Основной цикл оптимизации
            while (iteration < parameters.MaxIterations && !simplex.IsConverged(parameters.Tolerance))
            {
                iteration++;

                Point best = simplex.GetBest();     // лучшая
                Point worst = simplex.GetWorst();   // худшая
                Point[] others = simplex.GetAllExceptWorst(); // все кроме худшей

                // Центр тяжести без худшей точки
                Point centroid = ComputeCentroid(others);

                // Отражение
                Point reflected = Reflection(centroid, worst);
                reflected.UpdateValue(function.Evaluate(reflected.Coordinates));

                // Анализ результата отражения
                if (reflected.Value < best.Value)
                {
                    // Очень хороший результат - пробуем растяжение
                    Point expanded = Expansion(centroid, reflected);
                    expanded.UpdateValue(function.Evaluate(expanded.Coordinates));

                    simplex.ReplaceWorst(expanded.Value < reflected.Value ? expanded : reflected);
                }
                else if (reflected.Value < simplex[function.Dimension - 1].Value) // лучше предпоследней
                {
                    // Хороший результат - принимаем отражение
                    simplex.ReplaceWorst(reflected);
                }
                else
                {
                    // Плохой результат - пробуем сжатие
                    if (reflected.Value < worst.Value)
                    {
                        // Внешнее сжатие (отраженная точка лучше худшей)
                        Point contracted = OuterContraction(centroid, reflected);
                        contracted.UpdateValue(function.Evaluate(contracted.Coordinates));

                        if (contracted.Value < reflected.Value)
                            simplex.ReplaceWorst(contracted);
                        else
                            Reduction(simplex, best);
                    }
                    else
                    {
                        // Внутреннее сжатие (отраженная точка хуже худшей)
                        Point contracted = InnerContraction(centroid, worst);
                        contracted.UpdateValue(function.Evaluate(contracted.Coordinates));

                        if (contracted.Value < worst.Value)
                            simplex.ReplaceWorst(contracted);
                        else
                            Reduction(simplex, best);
                    }
                }

                simplex.Sort();
            }

            return new OptimizationResult
            {
                OptimalPoint = simplex[0],
                Iterations = iteration,
                Converged = iteration < parameters.MaxIterations,
                FunctoinName = function.Name
            };
        }

        private Point ComputeCentroid(Point[] points)
        {
            int dimension = points[0].Dimension;
            double[] center = new double[dimension];

            // Суммируем все координаты
            for (int i = 0; i < dimension; i++)
            {
                center[i] = points.Average(p => p[i]);
            }

            return new Point(center);
        }

       // Отражение: U(r) = c(r) + α[c(r) - x ^ n(r)]
        private Point Reflection(Point centroid, Point worst)
        {
            int dimension = centroid.Dimension;
            double[] reflected = new double[dimension];

            for (int i = 0; i < dimension; i++)
            {
                reflected[i] = centroid[i] + parameters.Alpha * (centroid[i] - worst[i]);
            }

            return new Point(reflected);

        }

        // Растяжение: V(r) = c(r) + β[U(r) - c(r)]
        private Point Expansion(Point centroid, Point reflected)
        {
            int dimension = centroid.Dimension;
            double[] expanded = new double[dimension];

            for (int i = 0; i < dimension; i++)
            {
                expanded[i] = centroid[i] + parameters.Beta * (reflected[i] - centroid[i]);
            }

            return new Point(expanded);
        }

        // Внутреннее сжатие: W(r) = c(r) + γ[x^n(r) - c(r)]
        private Point InnerContraction(Point centroid, Point worst)
        {
            int dimension = centroid.Dimension;
            double[] contracted = new double[dimension];

            for (int i = 0; i < dimension; i++)
            {
                contracted[i] = centroid[i] + parameters.Gamma * (worst[i] - centroid[i]);
            }

            return new Point(contracted);
        }

        // Внешнее сжатие: W(r) = c(r) + γ[U(r) - c(r)]
        private Point OuterContraction(Point centroid, Point reflected)
        {
            int dimension = centroid.Dimension;
            double[] contracted = new double[dimension];

            for (int i = 0; i < dimension; i++)
            {
                contracted[i] = centroid[i] + parameters.Gamma * (reflected[i] - centroid[i]);
            }

            return new Point(contracted);
        }

        // Редукция (сжатие всего симплекса к лучшей точке)
        private void Reduction(Simplex simplex, Point best)
        {
            // Для всех точек кроме лучшей
            for (int i = 1; i < simplex.Size; i++)
            {
                Point p = simplex[i];
                double[] newCoords = new double[p.Dimension];

                for (int j = 0; j < p.Dimension; j++)
                {
                    // new = best + σ(p - best)
                    newCoords[j] = best[j] + parameters.Sigma * (p[j] - best[j]);
                }

                Point newPoint = new Point(newCoords);
                newPoint.UpdateValue(function.Evaluate(newCoords));
                simplex[i] = newPoint;
            }
        }
    }
}
