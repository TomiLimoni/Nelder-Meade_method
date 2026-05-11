using NelderMeadOptimization.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace NelderMeadVisualizer.Visualization
{
    internal class ContourRenderer
    {
        private readonly Canvas _canvas;
        private readonly DrawingContext _context;
        private readonly ZoomController _zoom;
        private readonly ITestFunction _function;

        private Dictionary<string, double> _valueCache;
        private int _lastResolution;
        private double _lastMinX, _lastMaxX, _lastMinY, _lastMaxY;

        public ContourRenderer(Canvas canvas, DrawingContext context, ZoomController zoom, ITestFunction function)
        {
            _canvas = canvas;
            _context = context;
            _zoom = zoom;
            _function = function;
            _valueCache = new Dictionary<string, double>();
        }

        private int GetDynamicResolution()
        {
            double scale = _zoom.GetCurrentScale();

            // Для Розенброка нужно более высокое разрешение из-за оврага
            string functionName = _function.Name;
            bool isRosenbrock = functionName.Contains("Rosenbrock");

            if (isRosenbrock)
            {
                // Розенброку нужно больше деталей
                if (scale > 2.0) return 120;
                if (scale > 1.0) return 90;
                return 70;
            }

            // Остальные функции
            if (scale > 2.0) return 60;
            if (scale > 1.0) return 50;
            return 40;
        }

        public void DrawContours()
        {
            int resolution = GetDynamicResolution();

            if (resolution != _lastResolution ||
                Math.Abs(_context.MinX - _lastMinX) > 0.1 ||
                Math.Abs(_context.MaxX - _lastMaxX) > 0.1 ||
                Math.Abs(_context.MinY - _lastMinY) > 0.1 ||
                Math.Abs(_context.MaxY - _lastMaxY) > 0.1)
            {
                _valueCache.Clear();
                _lastResolution = resolution;
                _lastMinX = _context.MinX;
                _lastMaxX = _context.MaxX;
                _lastMinY = _context.MinY;
                _lastMaxY = _context.MaxY;
            }

            double[] levels = GetDynamicContourLevels();

            foreach (double level in levels)
            {
                DrawContourLine(level, resolution);
            }
        }

        private double[] GetDynamicContourLevels()
        {
            double x1 = _context.MinX;
            double y1 = _context.MinY;
            double x2 = _context.MaxX;
            double y2 = _context.MaxY;

            double fMin = double.MaxValue;
            double fMax = double.MinValue;

            double[] testPoints = {
                GetCachedValue(x1, y1),
                GetCachedValue(x1, y2),
                GetCachedValue(x2, y1),
                GetCachedValue(x2, y2),
                GetCachedValue((x1 + x2) / 2, (y1 + y2) / 2)
            };

            foreach (double f in testPoints)
            {
                if (!double.IsInfinity(f) && !double.IsNaN(f))
                {
                    fMin = Math.Min(fMin, f);
                    fMax = Math.Max(fMax, f);
                }
            }

            fMin = Math.Max(0, fMin - 0.5);
            fMax = fMax + 0.5;

            return _function.GetRecommendedContourLevels(fMin, fMax);
        }

        private string GetCacheKey(double x, double y)
        {
            int ix = (int)Math.Round(x * 10);
            int iy = (int)Math.Round(y * 10);
            return ix + "_" + iy;
        }

        private double GetCachedValue(double x, double y)
        {
            string key = GetCacheKey(x, y);

            if (_valueCache.ContainsKey(key))
                return _valueCache[key];

            double value = _function.Evaluate(new double[] { x, y });
            _valueCache[key] = value;
            return value;
        }

        private void DrawContourLine(double level, int resolution)
        {
            double stepX = (_context.MaxX - _context.MinX) / resolution;
            double stepY = (_context.MaxY - _context.MinY) / resolution;

            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    double x1 = _context.MinX + i * stepX;
                    double y1 = _context.MinY + j * stepY;
                    double x2 = x1 + stepX;
                    double y2 = y1 + stepY;

                    double f11 = GetCachedValue(x1, y1);
                    double f12 = GetCachedValue(x1, y2);
                    double f21 = GetCachedValue(x2, y1);
                    double f22 = GetCachedValue(x2, y2);

                    var cell = new Cell(x1, y1, x2, y2, f11, f12, f21, f22);
                    DrawLinesInCell(cell, level);
                }
            }
        }

        private void DrawLinesInCell(Cell cell, double level)
        {
            bool top = (cell.F11 - level) * (cell.F12 - level) < 0;
            bool bottom = (cell.F21 - level) * (cell.F22 - level) < 0;
            bool left = (cell.F11 - level) * (cell.F21 - level) < 0;
            bool right = (cell.F12 - level) * (cell.F22 - level) < 0;

            if (!top && !bottom && !left && !right) return;

            if (top && bottom)
            {
                double t = GetInterpolationT(cell.F11, cell.F12, level);
                double x_top = cell.X1 + t * (cell.X2 - cell.X1);
                double y_top = cell.Y1;
                t = GetInterpolationT(cell.F21, cell.F22, level);
                double x_bottom = cell.X1 + t * (cell.X2 - cell.X1);
                double y_bottom = cell.Y2;
                Point p_top = new Point(x_top, y_top);
                Point p_bottom = new Point(x_bottom, y_bottom);
                DrawSegment(p_top, p_bottom);
            }
            else if (left && right)
            {
                double t = GetInterpolationT(cell.F11, cell.F21, level);
                double x_left = cell.X1;
                double y_left = cell.Y1 + t * (cell.Y2 - cell.Y1);
                t = GetInterpolationT(cell.F12, cell.F22, level);
                double x_right = cell.X2;
                double y_right = cell.Y1 + t * (cell.Y2 - cell.Y1);
                Point p_left = new Point(x_left, y_left);
                Point p_right = new Point(x_right, y_right);
                DrawSegment(p_left, p_right);
            }
            else if (top && left)
            {
                double t = GetInterpolationT(cell.F11, cell.F12, level);
                double x_top = cell.X1 + t * (cell.X2 - cell.X1);
                double y_top = cell.Y1;
                t = GetInterpolationT(cell.F11, cell.F21, level);
                double x_left = cell.X1;
                double y_left = cell.Y1 + t * (cell.Y2 - cell.Y1);
                Point p_top = new Point(x_top, y_top);
                Point p_left = new Point(x_left, y_left);
                DrawSegment(p_top, p_left);
            }
            else if (top && right)
            {
                double t = GetInterpolationT(cell.F11, cell.F12, level);
                double x_top = cell.X1 + t * (cell.X2 - cell.X1);
                double y_top = cell.Y1;
                t = GetInterpolationT(cell.F12, cell.F22, level);
                double x_right = cell.X2;
                double y_right = cell.Y1 + t * (cell.Y2 - cell.Y1);
                Point p_top = new Point(x_top, y_top);
                Point p_right = new Point(x_right, y_right);
                DrawSegment(p_top, p_right);
            }
            else if (bottom && left)
            {
                double t = GetInterpolationT(cell.F21, cell.F22, level);
                double x_bottom = cell.X1 + t * (cell.X2 - cell.X1);
                double y_bottom = cell.Y2;
                t = GetInterpolationT(cell.F11, cell.F21, level);
                double x_left = cell.X1;
                double y_left = cell.Y1 + t * (cell.Y2 - cell.Y1);
                Point p_bottom = new Point(x_bottom, y_bottom);
                Point p_left = new Point(x_left, y_left);
                DrawSegment(p_bottom, p_left);
            }
            else if (bottom && right)
            {
                double t = GetInterpolationT(cell.F21, cell.F22, level);
                double x_bottom = cell.X1 + t * (cell.X2 - cell.X1);
                double y_bottom = cell.Y2;
                t = GetInterpolationT(cell.F12, cell.F22, level);
                double x_right = cell.X2;
                double y_right = cell.Y1 + t * (cell.Y2 - cell.Y1);
                Point p_bottom = new Point(x_bottom, y_bottom);
                Point p_right = new Point(x_right, y_right);
                DrawSegment(p_bottom, p_right);
            }
        }

        private double GetInterpolationT(double f1, double f2, double level)
        {
            if (Math.Abs(f2 - f1) < 1e-10) return 0.5;
            return (level - f1) / (f2 - f1);
        }

        private void DrawSegment(Point p1, Point p2)
        {
            var screenP1 = _context.Transform(p1.X, p1.Y, _zoom);
            var screenP2 = _context.Transform(p2.X, p2.Y, _zoom);

            if (screenP1.X < -1000 && screenP2.X < -1000) return;
            if (screenP1.X > _context.CanvasWidth + 1000 && screenP2.X > _context.CanvasWidth + 1000) return;
            if (screenP1.Y < -1000 && screenP2.Y < -1000) return;
            if (screenP1.Y > _context.CanvasHeight + 1000 && screenP2.Y > _context.CanvasHeight + 1000) return;

            double thickness = _zoom.GetCurrentScale() > 1.5 ? 1.0 : 0.7;

            _canvas.Children.Add(new Line
            {
                X1 = screenP1.X,
                Y1 = screenP1.Y,
                X2 = screenP2.X,
                Y2 = screenP2.Y,
                Stroke = new SolidColorBrush(Color.FromRgb(140, 140, 140)),
                StrokeThickness = thickness,
                StrokeDashArray = new DoubleCollection { 4, 3 }
            });
        }
    }
}
