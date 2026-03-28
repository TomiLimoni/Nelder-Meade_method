using System;
using System.Linq;

namespace NelderMeadOptimization.Models
{
    internal class Simplex
    {
        private Point[] _points;

        public int Size => _points.Length;
        public int Dimension => _points[0]?.Dimension ?? 0;

        public Point Best => _points[0];
        public Point Worst => _points[_points.Length - 1];
        public Point SecondWorst => _points[_points.Length - 2];

        public Point this[int index]
        {
            get => _points[index];
            set => _points[index] = value;
        }

        public Simplex(Point[] points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            if (points.Length < 2)
                throw new ArgumentException("Симплекс должен содержать хотя бы 2 точки");

            int dim = points[0].Dimension;
            if (!points.All(p => p.Dimension == dim))
                throw new ArgumentException("Все точки должны иметь одинаковую размерность");

            _points = points.ToArray();
            Sort();
        }

        public void Sort()
        {
            Array.Sort(_points, (a, b) => a.Value.CompareTo(b.Value));
        }

        public double[] GetCentroidCoordinates()
        {
            int dimension = Dimension;
            double[] center = new double[dimension];

            for (int i = 0; i < dimension; i++)
            {
                center[i] = _points.Take(_points.Length - 1).Average(p => p[i]);
            }

            return center;
        }

        public Point Reflect(double[] centroid, Func<double[], double> evaluate, double alpha)
        {
            int dimension = Dimension;
            double[] reflected = new double[dimension];

            for (int i = 0; i < dimension; i++)
            {
                reflected[i] = centroid[i] + alpha * (centroid[i] - Worst[i]);
            }

            return Point.Create(reflected, evaluate);
        }

        public Point Expand(double[] centroid, Point reflected, Func<double[], double> evaluate, double beta)
        {
            int dimension = Dimension;
            double[] expanded = new double[dimension];

            for (int i = 0; i < dimension; i++)
            {
                expanded[i] = centroid[i] + beta * (reflected[i] - centroid[i]);
            }

            return Point.Create(expanded, evaluate);
        }

        public Point ContractInside(double[] centroid, Func<double[], double> evaluate, double gamma)
        {
            int dimension = Dimension;
            double[] contracted = new double[dimension];

            for (int i = 0; i < dimension; i++)
            {
                contracted[i] = centroid[i] + gamma * (Worst[i] - centroid[i]);
            }

            return Point.Create(contracted, evaluate);
        }

        public Point ContractOutside(double[] centroid, Point reflected, Func<double[], double> evaluate, double gamma)
        {
            int dimension = Dimension;
            double[] contracted = new double[dimension];

            for (int i = 0; i < dimension; i++)
            {
                contracted[i] = centroid[i] + gamma * (reflected[i] - centroid[i]);
            }

            return Point.Create(contracted, evaluate);
        }

        public void Reduce(Func<double[], double> evaluate, double sigma)
        {
            for (int i = 1; i < _points.Length; i++)
            {
                double[] newCoords = new double[Dimension];
                for (int j = 0; j < Dimension; j++)
                {
                    newCoords[j] = Best[j] + sigma * (_points[i][j] - Best[j]);
                }
                _points[i] = Point.Create(newCoords, evaluate);
            }
            Sort();
        }

        public void ReplaceWorst(Point newPoint)
        {
            _points[_points.Length - 1] = newPoint;
            Sort();
        }

        public bool IsConverged(double tolerance)
        {
            double max = _points.Max(p => p.Value);
            double min = _points.Min(p => p.Value);
            return Math.Abs(max - min) < tolerance;
        }
    }
}
