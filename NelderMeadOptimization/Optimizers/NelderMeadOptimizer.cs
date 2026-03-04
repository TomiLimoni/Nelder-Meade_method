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
        private readonly ITestFunction _function;

        private readonly double _alpha;  // коэффициент отражения
        private readonly double _beta;   // коэффициент растяжени
        private readonly double _gamma;  // коэффициент сжатия
        private readonly double _sigma;  // коэффициент редукции
        private readonly double _tolerance;  // точность
        private readonly int _maxIterations; // максимум итераций

        public NelderMeadOptimizer(
            ITestFunction function,
            double alpha = 1.0,
            double beta = 2.0,
            double gamma = 0.5,
            double sigma = 0.5,
            double tolerance = 1e-6,
            int maxIterations = 1000)
        {
            _function = function ?? throw new ArgumentNullException(nameof(function));
            _alpha = alpha;
            _beta = beta;
            _gamma = gamma;
            _sigma = sigma;
            _tolerance = tolerance;
            _maxIterations = maxIterations;
        }

        public NelderMeadOptimizer(ITestFunction function)
                : this(function, 1.0, 2.0, 0.5, 0.5, 1e-6, 1000)
        {
        }


        /*public OptimizationResult Optimize(Point[] initialSimplex)
        {
            return new OptimizationResult
            {
                FunctoinName = _function.Name,
                OptimalPoint = initialSimplex[0],
                Iterations = 0,
                Converged = false
            };

        }*/


        public OptimizationResult Optimize(Point[] initialSimplex)
        {
            if (initialSimplex == null)
                throw new ArgumentNullException(nameof(initialSimplex));

            if (initialSimplex.Length != 3)
                throw new ArgumentException("There should be 3 points!");

            // Копируем начальный симплекс, чтобы не изменять исходный массив
            Point[] simplex = new Point[3];
            for (int i = 0; i < 3; i++)
            {
                simplex[i] = new Point(
                    initialSimplex[i].X,
                    initialSimplex[i].Y,
                    initialSimplex[i].Value);
            }

            SortPoints(simplex);

            int iteration = 0;

            // Основной цикл оптимизации
            while (iteration < _maxIterations && !IsConverged(simplex))
            {
                iteration++;

                // 1. Лучшая, средняя и худшая точки
                Point best = simplex[0];   // x^l
                Point good = simplex[1];   // x^g
                Point worst = simplex[2];  // x^n

                // 2. Вычисляем центр тяжести (без худшей точки)
                Point centroid = ComputeCentroid(best, good);

                // 3. Отражение
                Point reflected = Reflection(centroid, worst);
                reflected.Value = _function.Evaluate(reflected.X, reflected.Y);

                // 4. Анализ результата отражения
                if (reflected.Value < best.Value)
                {
                    // Очень хороший результат - пробуем растяжение
                    Point expanded = Expansion(centroid, reflected);
                    expanded.Value = _function.Evaluate(expanded.X, expanded.Y);

                    if (expanded.Value < reflected.Value)
                        simplex[2] = expanded;  // берем растянутую
                    else
                        simplex[2] = reflected;  // берем отраженную
                }
                else if (reflected.Value < good.Value)
                {
                    // Хороший результат - принимаем отражение
                    simplex[2] = reflected;
                }
                else
                {
                    // Плохой результат - пробуем сжатие
                    if (reflected.Value < worst.Value)
                    {
                        // Внешнее сжатие (отраженная точка лучше худшей)
                        Point contracted = OuterContraction(centroid, reflected);
                        contracted.Value = _function.Evaluate(contracted.X, contracted.Y);

                        if (contracted.Value < reflected.Value)
                            simplex[2] = contracted;
                        else
                            Reduction(simplex, best);
                    }
                    else
                    {
                        // Внутреннее сжатие (отраженная точка хуже худшей)
                        Point contracted = InnerContraction(centroid, worst);
                        contracted.Value = _function.Evaluate(contracted.X, contracted.Y);

                        if (contracted.Value < worst.Value)
                            simplex[2] = contracted;
                        else
                            Reduction(simplex, best);
                    }
                }

                // Пересортировываем точки
                SortPoints(simplex);
            }

            // Формируем результат
            return new OptimizationResult
            {
                OptimalPoint = simplex[0],
                Iterations = iteration,
                Converged = iteration < _maxIterations,
                FunctoinName = _function.Name
            };
        }

        private Point ComputeCentroid(Point best, Point good)
        {
            double centerX = (best.X + good.X) / 2.0;
            double centerY = (best.Y + good.Y) / 2.0;

            return new Point(centerX, centerY, 0);
        }

        private bool IsConverged(Point[] simplex)
        {
            double maxValue = simplex.Max(p => p.Value);
            double minValue = simplex.Min(p => p.Value);

            return Math.Abs(maxValue - minValue) < _tolerance;
        }

       // Отражение: U(r) = c(r) + α[c(r) - x ^ n(r)]
        private Point Reflection(Point centroid, Point worst)
        {
            double x = centroid.X + _alpha * (centroid.X - worst.X);
            double y = centroid.Y + _alpha * (centroid.Y - worst.Y);
            return new Point(x, y, 0);
        }

        // Растяжение: V(r) = c(r) + β[U(r) - c(r)]
        private Point Expansion(Point centroid, Point reflected)
        {
            double x = centroid.X + _beta * (reflected.X - centroid.X);
            double y = centroid.Y + _beta * (reflected.Y - centroid.Y);
            return new Point(x, y, 0);
        }

        // Внутреннее сжатие: W(r) = c(r) + γ[x^n(r) - c(r)]
        private Point InnerContraction(Point centroid, Point worst)
        {
            double x = centroid.X + _gamma * (worst.X - centroid.X);
            double y = centroid.Y + _gamma * (worst.Y - centroid.Y);
            return new Point(x, y, 0);
        }

        // Внешнее сжатие: W(r) = c(r) + γ[U(r) - c(r)]
        private Point OuterContraction(Point centroid, Point reflected)
        {
            double x = centroid.X + _gamma * (reflected.X - centroid.X);
            double y = centroid.Y + _gamma * (reflected.Y - centroid.Y);
            return new Point(x, y, 0);
        }




        // Редукция (сжатие всего симплекса к лучшей точке)
        private void Reduction(Point[] simplex, Point best)
        {
            for (int i = 1; i < simplex.Length; i++)
            {
                double newX = best.X + _sigma * (simplex[i].X - best.X);
                double newY = best.Y + _sigma * (simplex[i].Y - best.Y);
                simplex[i].X = newX;
                simplex[i].Y = newY;
                simplex[i].Value = _function.Evaluate(newX, newY);
            }
        }

        private void SortPoints(Point[] points)
        {
            Array.Sort(points, (a, b) => a.Value.CompareTo(b.Value));
        }






    }
}
