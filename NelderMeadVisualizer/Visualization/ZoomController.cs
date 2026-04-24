using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NelderMeadVisualizer.Visualization
{
    internal class ZoomController
    {
        private readonly Canvas _canvas;
        private double _userScale = 1.0;
        private double _userOffsetX = 0;
        private double _userOffsetY = 0;
        private Point _lastMousePosition;
        private bool _isPanning = false;

        public event Action ZoomChanged;

        public ZoomController(Canvas canvas)
        {
            _canvas = canvas;
            AttachEvents();
        }

        private void AttachEvents()
        {
            _canvas.MouseWheel += OnMouseWheel;
            _canvas.MouseLeftButtonDown += OnMouseDown;
            _canvas.MouseLeftButtonUp += OnMouseUp;
            _canvas.MouseMove += OnMouseMove;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
            _userScale *= zoomFactor;
            _userScale = Math.Max(0.1, Math.Min(10, _userScale));
            ZoomChanged?.Invoke();
            e.Handled = true;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isPanning = true;
            _lastMousePosition = e.GetPosition(_canvas);
            _canvas.CaptureMouse();
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isPanning = false;
            _canvas.ReleaseMouseCapture();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning)
            {
                Point currentPos = e.GetPosition(_canvas);
                _userOffsetX += currentPos.X - _lastMousePosition.X;
                _userOffsetY += currentPos.Y - _lastMousePosition.Y;
                _lastMousePosition = currentPos;
                ZoomChanged?.Invoke();
            }
        }

        public void Reset()
        {
            _userScale = 1.0;
            _userOffsetX = 0;
            _userOffsetY = 0;
            ZoomChanged?.Invoke();
        }

        public double TransformX(double x, double baseScale, double baseOffsetX)
        {
            return x * (baseScale * _userScale) + (baseOffsetX + _userOffsetX);
        }

        public double TransformY(double y, double baseScale, double baseOffsetY)
        {
            return y * (baseScale * _userScale) + (baseOffsetY + _userOffsetY);
        }
    }
}
