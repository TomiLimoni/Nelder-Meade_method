using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace NelderMeadVisualizer.Visualization
{
    internal class SimplexLineRenderer
    {
        private readonly Canvas _canvas;
        private readonly DrawingContext _context;
        private readonly ZoomController _zoom;

        public SimplexLineRenderer(Canvas canvas, DrawingContext context, ZoomController zoom)
        {
            _canvas = canvas;
            _context = context;
            _zoom = zoom;
        }

        public void DrawLines(SimplexHistory history)
        {
            var snapshots = history.Snapshots;
            for (int i = 0; i < snapshots.Count; i++)
            {
                var snapshot = snapshots[i];
                double ageRatio = (double)i / snapshots.Count;

                DrawSimplexLines(snapshot.Points, snapshot.Iteration, ageRatio);
            }
        }

        private void DrawSimplexLines(double[][] points, int iteration, double ageRatio)
        {
            var brush = StyleHelper.GetLineBrush(iteration, ageRatio);
            double thickness = StyleHelper.GetLineThickness(ageRatio);

            for (int i = 0; i < points.Length; i++)
            {
                for (int j = i + 1; j < points.Length; j++)
                {
                    var p1 = _context.Transform(points[i][0], points[i][1], _zoom);
                    var p2 = _context.Transform(points[j][0], points[j][1], _zoom);

                    _canvas.Children.Add(new Line
                    {
                        X1 = p1.X,
                        Y1 = p1.Y,
                        X2 = p2.X,
                        Y2 = p2.Y,
                        Stroke = brush,
                        StrokeThickness = thickness
                    });
                }
            }
        }
    }
}
