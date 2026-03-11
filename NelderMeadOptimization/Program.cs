using NelderMeadOptimization.Functions;
using NelderMeadOptimization.Interface;
using NelderMeadOptimization.Models;
using NelderMeadOptimization.Optimizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadOptimization
{
    internal class Program
    {
        static void Main(string[] args)
        {
             Console.WriteLine("Test");

             Point p = new Point(1.5, 2.3, 42.0);
             Console.WriteLine(p.ToString());

            ITestFunction sphere = new SphereFunction();
            double x = 2.0;
            double y = 3.0;
            double value = sphere.Evaluate(x, y);

            Console.WriteLine($"\nFunction: {sphere.Name}");
            Console.WriteLine($"f({x}, {y}) = {value}");

            x = 0; y = 0;
            value = sphere.Evaluate(x, y);
            Console.WriteLine($"f({x}, {y}) = {value}");

            OptimizationResult result = new OptimizationResult
            {
                OptimalPoint = p,
                Iterations = 42,
                Converged = true,
                FunctoinName = sphere.Name
            };
            result.Print();

            //тест оптимиз
            NelderMeadOptimizer optimizer = new NelderMeadOptimizer(sphere);
            Point[] simplex = new Point[]
            {
                new Point(0, 0, sphere.Evaluate(0, 0)),
                new Point(1, 0, sphere.Evaluate(1, 0)),
                new Point(0, 1, sphere.Evaluate(0, 1))
            };

            PrintStartSimplex(simplex);

            OptimizationResult res = optimizer.Optimize(simplex);

            res.Print();

            //Словарь тестируемых функций
            var testFunction = new Dictionary<ITestFunction, Point>()
            {
                { new SphereFunction(), new Point(0.0, 0.0, 0.0) },
                { new QuadraticFunction(), new Point(1.0, 4.0, -21.0) },
                { new RastriginFunction(), new Point(0.0, 0.0, 0.0) },
                { new RosenbrockFunction(), new Point(1.0, 1.0, 0.0) }
            };

            // Тестируем на нескольких функциях
            int count = 0;
            foreach(var test in testFunction)
            {
                count++;
                Console.WriteLine($"\nTest {count}");
                Console.WriteLine("---------------------------------------------");
                StartTestFunction(test.Key, test.Value);
            }
            Console.ReadLine();
        }
        static void StartTestFunction(ITestFunction function, Point pointTrueMin)
        {
            //Создаём начальный симплекс (треугольник)
            Point[] initialSimplex = GetInitialSimplex(function);

            //Выводим начальный симплекс
            PrintStartSimplex(initialSimplex);

            //Создаём оптимизатор
            NelderMeadOptimizer optimizer = new NelderMeadOptimizer(function);

            // Запускаем оптимизацию
            Console.WriteLine("\n Start optimization");
            OptimizationResult result = optimizer.Optimize(initialSimplex);

            // Выводим результат
            result.Print();

            // Проверяем, насколько близко к истинному минимуму
            CheckMin(result, pointTrueMin);
        }
        static Point[] GetInitialSimplex(ITestFunction function)
        {
            switch (function.Name)
            {
                case "Sphere func: x^2 + y^2":
                    return new Point[]
                    {
                        new Point(2.0, 2.0, function.Evaluate(2.0, 2.0)),   // точка A
                        new Point(3.0, 2.0, function.Evaluate(3.0, 2.0)),   // точка B
                        new Point(2.0, 3.0, function.Evaluate(2.0, 3.0))    // точка C
                    };
                case "Quadratic: x^2 + xy + y^2 - 6x - 9y":
                    return new Point[]
                    {
                        new Point(0.0, 0.0, function.Evaluate(0.0, 0.0)),
                        new Point(1.0, 0.0, function.Evaluate(1.0, 0.0)),
                        new Point(0.0, 1.0, function.Evaluate(0.0, 1.0))
                    };
                case "Rastrigin: 20 + x^2 - 10cos(2*pi*x) + y^2 - 10cos(2*pi*y)":
                    return new Point[]
                    {
                        new Point(1.0, 1.0, function.Evaluate(1.0, 1.0)),
                        new Point(2.0, 1.0, function.Evaluate(2.0, 1.0)),
                        new Point(1.0, 2.0, function.Evaluate(1.0, 2.0))
                    };
                case "Rosenbrock: (1-x)^2 + 100(y-x^2)^2":
                    return new Point[]
                    {
                        new Point(-1.0, 1.0, function.Evaluate(-1.0, 1.0)),
                        new Point(0.0, 1.0, function.Evaluate(0.0, 1.0)),
                        new Point(-1.0, 2.0, function.Evaluate(-1.0, 2.0))
                    };
                default:
                    throw new ArgumentException("Unknown function");
            }
        }
        //функция для вывода начального симплекса
        static void PrintStartSimplex(Point[] simplex)
        {
            Console.WriteLine("Start simplex:");
            foreach (var p in simplex)
            {
                Console.WriteLine($"  {p}");
            }
        }
        //функция для проверки близости к истинному минимуму
        static void CheckMin(OptimizationResult result, Point pointTrueMin)
        {
            double error = Math.Sqrt(
                Math.Pow(result.OptimalPoint.X - pointTrueMin.X, 2) +
                Math.Pow(result.OptimalPoint.Y - pointTrueMin.Y, 2));
            Console.WriteLine($"\nError: {error:F6}");
            Console.WriteLine(error < 1e-4 ? "Yes" : "No");
        }
    }
}