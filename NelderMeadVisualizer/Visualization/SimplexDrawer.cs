using NelderMeadOptimization.Interface;
using NelderMeadOptimization.Models;
using NelderMeadVisualizer.Services;
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
    internal class SimplexDrawer
    {
        private readonly Canvas _canvas;
        private readonly ZoomController _zoom;
        private DrawOptions _lastOptions;

        public SimplexDrawer(Canvas canvas, ZoomController zoomController)
        {
            _canvas = canvas;
            _zoom = zoomController;
            _zoom.ZoomChanged += () => RedrawLast();
        }

        public void Draw(DrawOptions options)
        {
            _lastOptions = options;
            var context = DrawingContext.Create(_canvas, options.Bounds);
            _canvas.Children.Clear();

            if (options.ShowContours && options.Function != null && options.History.Count > 0)
            {
                var contourRenderer = new ContourRenderer(_canvas, context, _zoom, options.Function);
                contourRenderer.DrawContours();
            }

            if (options.History.Count == 0) return;

            var lineRenderer = new SimplexLineRenderer(_canvas, context, _zoom);
            var pointRenderer = new SimplexPointRenderer(_canvas, context, _zoom);
            var axisRenderer = new AxisRenderer(_canvas, context, _zoom);
            var infoRenderer = new InfoPanelRenderer(_canvas);

            lineRenderer.DrawLines(options.History);
            pointRenderer.DrawPoints(options.History.Last);
            axisRenderer.DrawAxes();
            infoRenderer.DrawInfo(options.History);
        }

        private void RedrawLast()
        {
            if (_lastOptions != null)
            {
                var bounds = new Bounds(_lastOptions.Bounds.MinX, _lastOptions.Bounds.MaxX,
                                        _lastOptions.Bounds.MinY, _lastOptions.Bounds.MaxY);
                _lastOptions.Bounds = bounds;
                Draw(_lastOptions);
            }
        }

        public void Clear() => _canvas.Children.Clear();
    }
}
