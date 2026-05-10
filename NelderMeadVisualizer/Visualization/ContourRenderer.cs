using NelderMeadOptimization.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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

                    DrawLinesInCell(x1, y1, x2, y2, f11, f12, f21, f22, level);
                }
            }
        }

        private void DrawLinesInCell(double x1, double y1, double x2, double y2,
                                     double f11, double f12, double f21, double f22,
                                     double level)
        {
            bool top = (f11 - level) * (f12 - level) < 0;
            bool bottom = (f21 - level) * (f22 - level) < 0;
            bool left = (f11 - level) * (f21 - level) < 0;
            bool right = (f12 - level) * (f22 - level) < 0;

            if (!top && !bottom && !left && !right) return;

            if (top && bottom)
            {
                double t = GetInterpolationT(f11, f12, level);
                double x_top = x1 + t * (x2 - x1);
                double y_top = y1;
                t = GetInterpolationT(f21, f22, level);
                double x_bottom = x1 + t * (x2 - x1);
                double y_bottom = y2;
                DrawSegment(x_top, y_top, x_bottom, y_bottom);
            }
            else if (left && right)
            {
                double t = GetInterpolationT(f11, f21, level);
                double x_left = x1;
                double y_left = y1 + t * (y2 - y1);
                t = GetInterpolationT(f12, f22, level);
                double x_right = x2;
                double y_right = y1 + t * (y2 - y1);
                DrawSegment(x_left, y_left, x_right, y_right);
            }
            else if (top && left)
            {
                double t = GetInterpolationT(f11, f12, level);
                double x_top = x1 + t * (x2 - x1);
                double y_top = y1;
                t = GetInterpolationT(f11, f21, level);
                double x_left = x1;
                double y_left = y1 + t * (y2 - y1);
                DrawSegment(x_top, y_top, x_left, y_left);
            }
            else if (top && right)
            {
                double t = GetInterpolationT(f11, f12, level);
                double x_top = x1 + t * (x2 - x1);
                double y_top = y1;
                t = GetInterpolationT(f12, f22, level);
                double x_right = x2;
                double y_right = y1 + t * (y2 - y1);
                DrawSegment(x_top, y_top, x_right, y_right);
            }
            else if (bottom && left)
            {
                double t = GetInterpolationT(f21, f22, level);
                double x_bottom = x1 + t * (x2 - x1);
                double y_bottom = y2;
                t = GetInterpolationT(f11, f21, level);
                double x_left = x1;
                double y_left = y1 + t * (y2 - y1);
                DrawSegment(x_bottom, y_bottom, x_left, y_left);
            }
            else if (bottom && right)
            {
                double t = GetInterpolationT(f21, f22, level);
                double x_bottom = x1 + t * (x2 - x1);
                double y_bottom = y2;
                t = GetInterpolationT(f12, f22, level);
                double x_right = x2;
                double y_right = y1 + t * (y2 - y1);
                DrawSegment(x_bottom, y_bottom, x_right, y_right);
            }
        }

        private double GetInterpolationT(double f1, double f2, double level)
        {
            if (Math.Abs(f2 - f1) < 1e-10) return 0.5;
            return (level - f1) / (f2 - f1);
        }

        private void DrawSegment(double x1, double y1, double x2, double y2)
        {
            var p1 = _context.Transform(x1, y1, _zoom);
            var p2 = _context.Transform(x2, y2, _zoom);

            if (p1.X < -1000 && p2.X < -1000) return;
            if (p1.X > _context.CanvasWidth + 1000 && p2.X > _context.CanvasWidth + 1000) return;
            if (p1.Y < -1000 && p2.Y < -1000) return;
            if (p1.Y > _context.CanvasHeight + 1000 && p2.Y > _context.CanvasHeight + 1000) return;

            double thickness = _zoom.GetCurrentScale() > 1.5 ? 1.0 : 0.7;

            _canvas.Children.Add(new Line
            {
                X1 = p1.X,
                Y1 = p1.Y,
                X2 = p2.X,
                Y2 = p2.Y,
                Stroke = new SolidColorBrush(Color.FromRgb(140, 140, 140)),
                StrokeThickness = thickness,
                StrokeDashArray = new DoubleCollection { 4, 3 }
            });
        }
    }
}
