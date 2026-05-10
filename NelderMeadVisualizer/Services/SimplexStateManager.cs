using NelderMeadOptimization.Interface;
using NelderMeadOptimization.Models;
using NelderMeadVisualizer.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NelderMeadVisualizer.Services
{
    /// <summary>
    /// Управляет состоянием симплекса: хранит историю, координирует отрисовку,
    /// обновляет границы области видимости.
    /// </summary>
    internal class SimplexStateManager
    {
        private readonly SimplexHistory _history;
        private readonly SimplexDrawer _drawer;
        private readonly Bounds _bounds;

        public SimplexHistory History => _history;
        public Bounds Bounds => _bounds;

        public SimplexStateManager(Canvas canvas, ZoomController zoomController)
        {
            _history = new SimplexHistory();
            _bounds = new Bounds(-5, 8, -5, 8);
            _drawer = new SimplexDrawer(canvas, zoomController);
        }

        public void AddAndDraw(Simplex simplex, int iteration, DrawOptions options)
        {
            _history.Add(simplex, iteration);
            UpdateBounds(simplex);

            var drawOptions = DrawOptions.Create(_history, _bounds);
            drawOptions.Function = options.Function;
            drawOptions.ShowContours = options.ShowContours;

            _drawer.Draw(drawOptions);
        }

        public void Redraw(DrawOptions options)
        {
            if (_history.Count > 0)
            {
                var drawOptions = DrawOptions.Create(_history, _bounds);
                drawOptions.Function = options.Function;
                drawOptions.ShowContours = options.ShowContours;
                _drawer.Draw(drawOptions);
            }
            else
            {
                _drawer.Clear();
            }
        }

        private void UpdateBounds(Simplex simplex)
        {
            _bounds.Update(simplex);
        }

        public void Clear()
        {
            _history.Clear();
            _bounds.Reset();
            _drawer.Clear();
        }
    }
}
