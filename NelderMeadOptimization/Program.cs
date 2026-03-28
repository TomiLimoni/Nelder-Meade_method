using NelderMeadOptimization.Functions;
using NelderMeadOptimization.Models;
using NelderMeadOptimization.Optimizers;
using NelderMeadOptimization.Interface;
using System;

namespace NelderMeadOptimization
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ТЕСТИРОВАНИЕ МЕТОДА НЕЛДЕРА-МИДА");
            Console.WriteLine("================================\n");

            TestSphere2D();
            Console.WriteLine(new string('=', 60));

            TestSphere3D();
            Console.WriteLine(new string('=', 60));

            TestSphere5D();
            Console.WriteLine(new string('=', 60));

            TestRosenbrock();

            Console.WriteLine("\nНажмите любую клавишу для завершения...");
            Console.ReadKey();
        }

        static void TestSphere2D()
        {
            Console.WriteLine("\nТЕСТ 1: Сферическая функция в 2D");

            int dim = 2;
            var function = new SphereFunction(dim);

            // Создаем точки через фабричный метод
            Point[] points = new Point[]
            {
                Point.Create(new double[] { 2.0, 2.0 }, function.Evaluate),
                Point.Create(new double[] { 3.0, 2.0 }, function.Evaluate),
                Point.Create(new double[] { 2.0, 3.0 }, function.Evaluate)
            };

            Console.WriteLine($"Начальные точки (размерность {dim}):");
            foreach (var p in points)
            {
                Console.WriteLine($"  {p}");
            }

            var optimizer = new NelderMeadOptimizer(function);
            var result = optimizer.Optimize(points);

            // Используем ToString() вместо Print()
            Console.WriteLine(result.ToString());

            double error = Math.Abs(result.OptimalPoint[0]) + Math.Abs(result.OptimalPoint[1]);
            Console.WriteLine($"Суммарная ошибка от (0,0): {error:F6}");
        }

        static void TestSphere3D()
        {
            Console.WriteLine("\nТЕСТ 2: Сферическая функция в 3D");

            int dim = 3;
            var function = new SphereFunction(dim);

            Point[] points = new Point[]
            {
                Point.Create(new double[] { 2.0, 2.0, 2.0 }, function.Evaluate),
                Point.Create(new double[] { 3.0, 2.0, 2.0 }, function.Evaluate),
                Point.Create(new double[] { 2.0, 3.0, 2.0 }, function.Evaluate),
                Point.Create(new double[] { 2.0, 2.0, 3.0 }, function.Evaluate)
            };

            Console.WriteLine($"Начальные точки (размерность {dim}):");
            foreach (var p in points)
            {
                Console.WriteLine($"  {p}");
            }

            var optimizer = new NelderMeadOptimizer(function);
            var result = optimizer.Optimize(points);

            Console.WriteLine(result.ToString());

            double error = Math.Abs(result.OptimalPoint[0]) +
                          Math.Abs(result.OptimalPoint[1]) +
                          Math.Abs(result.OptimalPoint[2]);
            Console.WriteLine($"Суммарная ошибка от (0,0,0): {error:F6}");
        }

        static void TestSphere5D()
        {
            Console.WriteLine("\nТЕСТ 3: Сферическая функция в 5D");

            int dim = 5;
            var function = new SphereFunction(dim);

            Point[] points = new Point[]
            {
                Point.Create(new double[] { 2.0, 2.0, 2.0, 2.0, 2.0 }, function.Evaluate),
                Point.Create(new double[] { 3.0, 2.0, 2.0, 2.0, 2.0 }, function.Evaluate),
                Point.Create(new double[] { 2.0, 3.0, 2.0, 2.0, 2.0 }, function.Evaluate),
                Point.Create(new double[] { 2.0, 2.0, 3.0, 2.0, 2.0 }, function.Evaluate),
                Point.Create(new double[] { 2.0, 2.0, 2.0, 3.0, 2.0 }, function.Evaluate),
                Point.Create(new double[] { 2.0, 2.0, 2.0, 2.0, 3.0 }, function.Evaluate)
            };

            Console.WriteLine($"Начальные точки (размерность {dim}):");
            for (int i = 0; i < points.Length; i++)
            {
                Console.WriteLine($"  Точка {i + 1}: {points[i]}");
            }

            var optimizer = new NelderMeadOptimizer(function);
            var result = optimizer.Optimize(points);

            Console.WriteLine(result.ToString());

            double error = 0;
            for (int i = 0; i < dim; i++)
            {
                error += Math.Abs(result.OptimalPoint[i]);
            }
            Console.WriteLine($"Суммарная ошибка от (0,0,0,0,0): {error:F6}");
        }

        static void TestRosenbrock()
        {
            Console.WriteLine("\nТЕСТ 4: Функция Розенброка (2D)");

            var function = new RosenbrockFunction();

            Point[] points = new Point[]
            {
                Point.Create(new double[] { 0.0, 0.0 }, function.Evaluate),
                Point.Create(new double[] { 1.0, 0.0 }, function.Evaluate),
                Point.Create(new double[] { 0.0, 1.0 }, function.Evaluate)
            };

            Console.WriteLine("Начальные точки:");
            foreach (var p in points)
            {
                Console.WriteLine($"  {p}");
            }

            var optimizer = new NelderMeadOptimizer(function);
            var result = optimizer.Optimize(points);

            Console.WriteLine(result.ToString());

            double error = Math.Sqrt(
                Math.Pow(result.OptimalPoint[0] - 1.0, 2) +
                Math.Pow(result.OptimalPoint[1] - 1.0, 2));
            Console.WriteLine($"Ошибка от (1,1): {error:F6}");
        }
    }
}
