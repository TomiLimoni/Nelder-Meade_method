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
            // ТЕСТ 1: 2D сферическая функция
            TestSphere2D();

            Console.WriteLine("\n" + new string('=', 60));

            // ТЕСТ 2: 3D сферическая функция
            TestSphere3D();

            Console.WriteLine("\n" + new string('=', 60));

            // ТЕСТ 3: 5D сферическая функция
            TestSphere5D();

            Console.WriteLine("\n" + new string('=', 60));

            // ТЕСТ 4: Функция Розенброка (только 2D)
            TestRosenbrock();

            Console.WriteLine("\nНажмите любую клавишу для завершения...");
            Console.ReadKey();
        }
        static void TestSphere2D()
        {
            Console.WriteLine("\nТЕСТ 1: Сферическая функция в 2D");

            int dim = 2;
            var function = new SphereFunction(dim);
            var optimizer = new NelderMeadOptimizer(function);

            // Создаем начальный симплекс (3 точки для 2D)
            Point[] points = new Point[]
            {
                new Point(new double[] { 2.0, 2.0 }),
                new Point(new double[] { 3.0, 2.0 }),
                new Point(new double[] { 2.0, 3.0 })
            };

            // ВАЖНО: Вычисляем и сохраняем значения для вывода
            Console.WriteLine($"Начальные точки (размерность {dim}):");
            foreach (var p in points)
            {
                double val = function.Evaluate(p.Coordinates);
                p.UpdateValue(val); // Сохраняем значение в точку
                Console.WriteLine($"  {p}");
            }

            // Запускаем оптимизацию
            var result = optimizer.Optimize(points);
            result.Print();

            // Проверка: насколько близко к (0,0)
            double error = 0;
            for (int i = 0; i < dim; i++)
            {
                error += Math.Abs(result.OptimalPoint[i]);
            }
            Console.WriteLine($"Суммарная ошибка от (0,0): {error:F6}");
        }

        static void TestSphere3D()
        {
            Console.WriteLine("\nТЕСТ 2: Сферическая функция в 3D");

            int dim = 3;
            var function = new SphereFunction(dim);
            var optimizer = new NelderMeadOptimizer(function);

            // Для 3D нужно 4 точки
            Point[] points = new Point[]
            {
                new Point(new double[] { 2.0, 2.0, 2.0 }),
                new Point(new double[] { 3.0, 2.0, 2.0 }),
                new Point(new double[] { 2.0, 3.0, 2.0 }),
                new Point(new double[] { 2.0, 2.0, 3.0 })
            };

            // ВАЖНО: Вычисляем и сохраняем значения для вывода
            Console.WriteLine($"Начальные точки (размерность {dim}):");
            foreach (var p in points)
            {
                double val = function.Evaluate(p.Coordinates);
                p.UpdateValue(val); // Сохраняем значение в точку
                Console.WriteLine($"  {p}");
            }

            // Запускаем оптимизацию
            var result = optimizer.Optimize(points);
            result.Print();

            // Проверка: насколько близко к (0,0,0)
            double error = 0;
            for (int i = 0; i < dim; i++)
            {
                error += Math.Abs(result.OptimalPoint[i]);
            }
            Console.WriteLine($"Суммарная ошибка от (0,0,0): {error:F6}");

            // Дополнительная проверка
        }

        static void TestSphere5D()
        {
            Console.WriteLine("\nТЕСТ 3: Сферическая функция в 5D");

            int dim = 5;
            var function = new SphereFunction(dim);
            var optimizer = new NelderMeadOptimizer(function);

            // Для 5D нужно 6 точек
            Point[] points = new Point[]
            {
                new Point(new double[] { 2.0, 2.0, 2.0, 2.0, 2.0 }),
                new Point(new double[] { 3.0, 2.0, 2.0, 2.0, 2.0 }),
                new Point(new double[] { 2.0, 3.0, 2.0, 2.0, 2.0 }),
                new Point(new double[] { 2.0, 2.0, 3.0, 2.0, 2.0 }),
                new Point(new double[] { 2.0, 2.0, 2.0, 3.0, 2.0 }),
                new Point(new double[] { 2.0, 2.0, 2.0, 2.0, 3.0 })
            };

            // ВАЖНО: Вычисляем и сохраняем значения для вывода
            Console.WriteLine($"Начальные точки (размерность {dim}):");
            for (int i = 0; i < points.Length; i++)
            {
                double val = function.Evaluate(points[i].Coordinates);
                points[i].UpdateValue(val); // Сохраняем значение в точку
                Console.WriteLine($"  Точка {i + 1}: {points[i]}");
            }

            // Запускаем оптимизацию
            var result = optimizer.Optimize(points);
            result.Print();

            // Проверка: насколько близко к (0,0,0,0,0)
            double error = 0;
            for (int i = 0; i < dim; i++)
            {
                error += Math.Abs(result.OptimalPoint[i]);
            }
            Console.WriteLine($"Суммарная ошибка от (0,0,0,0,0): {error:F6}");

            // Дополнительная проверка
        }

        static void TestRosenbrock()
        {
            Console.WriteLine("\nТЕСТ 4: Функция Розенброка (только 2D)");

            var function = new RosenbrockFunction();
            var optimizer = new NelderMeadOptimizer(function);

            // Для 2D нужно 3 точки
            Point[] points = new Point[]
            {
                new Point(new double[] { 0.0, 0.0 }),
                new Point(new double[] { 1.0, 0.0 }),
                new Point(new double[] { 0.0, 1.0 })
            };

            // ВАЖНО: Вычисляем и сохраняем значения для вывода
            Console.WriteLine("Начальные точки:");
            foreach (var p in points)
            {
                double val = function.Evaluate(p.Coordinates);
                p.UpdateValue(val); // Сохраняем значение в точку
                Console.WriteLine($"  {p}");
            }

            // Запускаем оптимизацию
            var result = optimizer.Optimize(points);
            result.Print();

            // Проверка: насколько близко к (1,1)
            double error = Math.Sqrt(
                Math.Pow(result.OptimalPoint[0] - 1.0, 2) +
                Math.Pow(result.OptimalPoint[1] - 1.0, 2));
            Console.WriteLine($"Ошибка от (1,1): {error:F6}");

        }
    }
}