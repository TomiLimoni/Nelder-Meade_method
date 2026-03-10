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

            Console.WriteLine("Start simplex:");
            foreach (var point in simplex)
            {
                Console.WriteLine($"  {point}");
            }

            OptimizationResult res = optimizer.Optimize(simplex);

            Console.WriteLine("\nResult:");
            res.Print();

            // Тестируем на нескольких функциях
            Console.WriteLine("Test 1");
            Console.WriteLine("---------------------------------------------");
            TestSphereFunction();

            Console.WriteLine("Test 2");
            Console.WriteLine("---------------------------------------------");
            TestQuadraticFunction();

            Console.WriteLine("Test 3");
            Console.WriteLine("---------------------------------------------");
            TestRastriginFunction();

            Console.WriteLine("Test 4");
            Console.WriteLine("---------------------------------------------");
            TestRosenbrockFunction();

            Console.ReadLine();
        }

        static void TestSphereFunction()
        {
            // Создаем функцию
            ITestFunction sphere = new SphereFunction();

            // Создаем оптимизатор
            NelderMeadOptimizer optimizer = new NelderMeadOptimizer(sphere);

            // Создаем начальный симплекс (треугольник)
            Point[] initialSimplex = new Point[]
            {
                new Point(2.0, 2.0, sphere.Evaluate(2.0, 2.0)),   // точка A
                new Point(3.0, 2.0, sphere.Evaluate(3.0, 2.0)),   // точка B
                new Point(2.0, 3.0, sphere.Evaluate(2.0, 3.0))    // точка C
            };
            //Выводим начальный симплекс
            PrintStartSimplex(initialSimplex);

            // Запускаем оптимизацию
            OptimizationResult result = StartOptimization(optimizer, initialSimplex);

            // Выводим результат
            result.Print();

            // Проверяем, насколько близко к истинному минимуму
            Point pointTrueMin = new Point(0.0, 0.0, 0.0);
            CheckMin(result, pointTrueMin);
        }

        static void TestQuadraticFunction()
        {
            ITestFunction quadratic = new QuadraticFunction();

            NelderMeadOptimizer optimizer = new NelderMeadOptimizer(quadratic);

            Point[] initialSimplex = new Point[]
            {
                new Point(0.0, 0.0, quadratic.Evaluate(0.0, 0.0)),
                new Point(1.0, 0.0, quadratic.Evaluate(1.0, 0.0)),
                new Point(0.0, 1.0, quadratic.Evaluate(0.0, 1.0))
            };

            PrintStartSimplex(initialSimplex);

            // Запускаем оптимизацию
            OptimizationResult result = StartOptimization(optimizer, initialSimplex);

            // Выводим результат
            result.Print();

            // Проверяем, насколько близко к истинному минимуму
            Point pointTrueMin = new Point(1.0, 4.0, -21.0);
            CheckMin(result, pointTrueMin);
        }

        static void TestRastriginFunction()
        {
            ITestFunction rastrigin = new RastriginFunction();

            NelderMeadOptimizer optimizer = new NelderMeadOptimizer(rastrigin);

            Point[] initialSimplex = new Point[]
            {
                new Point(1.0, 1.0, rastrigin.Evaluate(1.0, 1.0)),
                new Point(2.0, 1.0, rastrigin.Evaluate(2.0, 1.0)),
                new Point(1.0, 2.0, rastrigin.Evaluate(1.0, 2.0))
            };

            PrintStartSimplex(initialSimplex);

            // Запускаем оптимизацию
            OptimizationResult result = StartOptimization(optimizer, initialSimplex);

            // Выводим результат
            result.Print();

            // Проверяем, насколько близко к истинному минимуму
            Point pointTrueMin = new Point(0.0, 0.0, 0.0);
            CheckMin(result, pointTrueMin);
        }

        static void TestRosenbrockFunction()
        {
            ITestFunction rosenbrock = new RosenbrockFunction();

            NelderMeadOptimizer optimizer = new NelderMeadOptimizer(rosenbrock);

            Point[] initialSimplex = new Point[]
            {
                new Point(-1.0, 1.0, rosenbrock.Evaluate(-1.0, 1.0)),
                new Point(0.0, 1.0, rosenbrock.Evaluate(0.0, 1.0)),
                new Point(-1.0, 2.0, rosenbrock.Evaluate(-1.0, 2.0))
            };

            PrintStartSimplex(initialSimplex);

            // Запускаем оптимизацию
            OptimizationResult result = StartOptimization(optimizer, initialSimplex);

            // Выводим результат
            result.Print();

            // Проверяем, насколько близко к истинному минимуму
            Point pointTrueMin = new Point(1.0, 1.0, 0.0);
            CheckMin(result, pointTrueMin);
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
        //функция запуска оптимизации
        static OptimizationResult StartOptimization(NelderMeadOptimizer optimizer, Point[] initialSimplex)
        {
            Console.WriteLine("\n Start optimization");
            OptimizationResult result = optimizer.Optimize(initialSimplex);
            return result;
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