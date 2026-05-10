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
    internal class SimplexPointRenderer
    {
        private readonly Canvas _canvas;
        private readonly DrawingContext _context;
        private readonly ZoomController _zoom;

        public SimplexPointRenderer(Canvas canvas, DrawingContext context, ZoomController zoom)
        {
            _canvas = canvas;
            _context = context;
            _zoom = zoom;
        }

        public void DrawPoints(SimplexHistory.SimplexSnapshot snapshot)
        {
            if (snapshot == null) return;

            for (int i = 0; i < snapshot.Points.Length; i++)
            {
                var point = _context.Transform(snapshot.Points[i][0], snapshot.Points[i][1], _zoom);
                var style = StyleHelper.GetPointStyle(i, snapshot.Points.Length);

                AddEllipse(point, style.Brush, style.Size);

                if (i == 0 && snapshot.Iteration < 100)
                {
                    AddPointLabel(point, snapshot.Points[i]);
                }
            }
        }

        private void AddEllipse(Point position, Brush brush, double size)
        {
            var ellipse = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = brush,
                Stroke = Brushes.Black,
                StrokeThickness = 0.5
            };
            Canvas.SetLeft(ellipse, position.X - size / 2);
            Canvas.SetTop(ellipse, position.Y - size / 2);
            _canvas.Children.Add(ellipse);
        }

        private void AddPointLabel(Point screenPoint, double[] dataPoint)
        {
            var label = new TextBlock
            {
                Text = $"({dataPoint[0]:F2}, {dataPoint[1]:F2})",
                FontSize = 9,
                Foreground = Brushes.DarkGreen,
                Background = Brushes.White,
                Padding = new Thickness(2)
            };
            Canvas.SetLeft(label, screenPoint.X + 8);
            Canvas.SetTop(label, screenPoint.Y - 8);
            _canvas.Children.Add(label);
        }
    }
}
