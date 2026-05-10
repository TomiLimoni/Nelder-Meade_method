using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NelderMeadVisualizer.Visualization
{
    internal class AxisRenderer
    {
        private readonly Canvas _canvas;
        private readonly DrawingContext _context;
        private readonly ZoomController _zoom;

        public AxisRenderer(Canvas canvas, DrawingContext context, ZoomController zoom)
        {
            _canvas = canvas;
            _context = context;
            _zoom = zoom;
        }

        public void DrawAxes()
        {
            DrawXAxis();
            DrawYAxis();
        }

        private void DrawXAxis()
        {
            var start = _context.Transform(_context.MinX, 0, _zoom);
            var end = _context.Transform(_context.MaxX, 0, _zoom);

            if (IsYVisible(start.Y))
                _canvas.Children.Add(CreateAxisLine(start, end));
        }

        private void DrawYAxis()
        {
            var start = _context.Transform(0, _context.MinY, _zoom);
            var end = _context.Transform(0, _context.MaxY, _zoom);

            if (IsXVisible(start.X))
                _canvas.Children.Add(CreateAxisLine(start, end));
        }

        private bool IsYVisible(double y) => y >= 0 && y <= _context.CanvasHeight;
        private bool IsXVisible(double x) => x >= 0 && x <= _context.CanvasWidth;

        private Line CreateAxisLine(Point start, Point end)
        {
            return new Line
            {
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y,
                Stroke = new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection { 5, 3 }
            };
        }
    }
}
