using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadOptimization.Models
{
    internal class Simplex
    {
        private readonly Point[] _points;

        // Количество точек в симплексе (для N измерений нужно N+1 точек)
        public int Size => _points.Length;

        // Размерность пространства
        public int Dimension => _points[0]?.Dimension ?? 0;

        // Индексатор
        public Point this[int index]
        {
            get => _points[index];
            set => _points[index] = value;
        }

        // Конструктор из массива точек
        public Simplex(Point[] points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            if (points.Length < 2)
                throw new ArgumentException("Симплекс должен содержать хотя бы 2 точки");

            // Проверяем, что все точки одной размерности
            int dim = points[0].Dimension;
            if (!points.All(p => p.Dimension == dim))
                throw new ArgumentException("Все точки должны иметь одинаковую размерность");

            // Создаем копии
            _points = points.Select(p => p.Clone()).ToArray();
        }

        // Получение точек по индексам (лучшей и худшей)
        public Point GetBest() => _points[0];
        public Point GetWorst() => _points[_points.Length - 1];

        // Получение всех точек кроме последней (все кроме худшей)
        public Point[] GetAllExceptWorst()
        {
            return _points.Take(_points.Length - 1).ToArray();
        }

        // Сортировка по значению (от лучшей к худшей)
        public void Sort()
        {
            Array.Sort(_points, (a, b) => a.Value.CompareTo(b.Value));
        }

        // Замена худшей точки новой
        public void ReplaceWorst(Point newPoint)
        {
            _points[_points.Length - 1] = newPoint.Clone();
        }

        // Проверка сходимости (все значения близки)
        public bool IsConverged(double tolerance)
        {
            double max = _points.Max(p => p.Value);
            double min = _points.Min(p => p.Value);
            return Math.Abs(max - min) < tolerance;
        }
    }
}
