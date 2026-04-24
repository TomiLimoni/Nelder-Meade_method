using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using NelderMeadOptimization.Models;

namespace NelderMeadVisualizer.Visualization
{
    internal class SimplexDrawer
    {
        private readonly Canvas _canvas;
        private readonly ZoomController _zoomController;

        public SimplexDrawer(Canvas canvas, ZoomController zoomController)
        {
            _canvas = canvas;
            _zoomController = zoomController;
            _zoomController.ZoomChanged += () => RedrawLast();
        }

        private SimplexHistory _lastHistory;
        private double _lastMinX, _lastMaxX, _lastMinY, _lastMaxY;

        public void DrawAll(SimplexHistory history, double minX, double maxX, double minY, double maxY)
        {
            _lastHistory = history;
            _lastMinX = minX;
            _lastMaxX = maxX;
            _lastMinY = minY;
            _lastMaxY = maxY;

            _canvas.Children.Clear();

            if (history.Count == 0) return;

            double canvasWidth = _canvas.Width;
            double canvasHeight = _canvas.Height;

            double baseScaleX = canvasWidth / (maxX - minX);
            double baseScaleY = canvasHeight / (maxY - minY);
            double baseScale = Math.Min(baseScaleX, baseScaleY) * 0.85;

            double baseOffsetX = canvasWidth / 2 - (minX + maxX) / 2 * baseScale;
            double baseOffsetY = canvasHeight / 2 - (minY + maxY) / 2 * baseScale;

            Func<double, double, System.Windows.Point> transform = (x, y) =>
                new System.Windows.Point(
                    _zoomController.TransformX(x, baseScale, baseOffsetX),
                    _zoomController.TransformY(y, baseScale, baseOffsetY)
                );

            var snapshots = history.Snapshots;

            for (int h = 0; h < snapshots.Count; h++)
            {
                var snapshot = snapshots[h];
                double ageRatio = (double)h / snapshots.Count;

                byte alpha = (byte)(60 + ageRatio * 150);
                Color baseColor = ColorHelper.GetColorForIteration(snapshot.Iteration);
                Color fadedColor = Color.FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);
                SolidColorBrush brush = new SolidColorBrush(fadedColor);
                double thickness = 1.5 + ageRatio * 2.5;

                DrawSimplexLines(snapshot.Points, brush, thickness, baseScale, baseOffsetX, baseOffsetY);
            }

            DrawPoints(history.Last, baseScale, baseOffsetX, baseOffsetY);

            DrawFixedAxes(transform, minX, maxX, minY, maxY);

            DrawInfo(history, baseScale, baseOffsetX, baseOffsetY, canvasWidth, canvasHeight, minX, maxX, minY, maxY);
        }

        private void RedrawLast()
        {
            if (_lastHistory != null)
                DrawAll(_lastHistory, _lastMinX, _lastMaxX, _lastMinY, _lastMaxY);
        }

        private void DrawSimplexLines(double[][] points, Brush brush, double thickness,
                                      double baseScale, double baseOffsetX, double baseOffsetY)
        {
            for (int i = 0; i < points.Length; i++)
            {
                for (int j = i + 1; j < points.Length; j++)
                {
                    double x1 = _zoomController.TransformX(points[i][0], baseScale, baseOffsetX);
                    double y1 = _zoomController.TransformY(points[i][1], baseScale, baseOffsetY);
                    double x2 = _zoomController.TransformX(points[j][0], baseScale, baseOffsetX);
                    double y2 = _zoomController.TransformY(points[j][1], baseScale, baseOffsetY);

                    Line line = new Line
                    {
                        X1 = x1,
                        Y1 = y1,
                        X2 = x2,
                        Y2 = y2,
                        Stroke = brush,
                        StrokeThickness = thickness
                    };
                    _canvas.Children.Add(line);
                }
            }
        }

        private void DrawPoints(SimplexHistory.SimplexSnapshot snapshot,
                                double baseScale, double baseOffsetX, double baseOffsetY)
        {
            if (snapshot == null) return;

            for (int i = 0; i < snapshot.Points.Length; i++)
            {
                double x = _zoomController.TransformX(snapshot.Points[i][0], baseScale, baseOffsetX);
                double y = _zoomController.TransformY(snapshot.Points[i][1], baseScale, baseOffsetY);

                Brush pointBrush;
                double size;
                if (i == 0) { pointBrush = Brushes.Green; size = 12; }
                else if (i == snapshot.Points.Length - 1) { pointBrush = Brushes.Red; size = 9; }
                else { pointBrush = Brushes.DarkBlue; size = 8; }

                Ellipse ellipse = new Ellipse
                {
                    Width = size,
                    Height = size,
                    Fill = pointBrush,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5
                };
                Canvas.SetLeft(ellipse, x - size / 2);
                Canvas.SetTop(ellipse, y - size / 2);
                _canvas.Children.Add(ellipse);
                
                if (i == 0 && snapshot.Iteration < 100)
                {
                    TextBlock label = new TextBlock
                    {
                        Text = $"({snapshot.Points[i][0]:F2}, {snapshot.Points[i][1]:F2})",
                        FontSize = 9,
                        Foreground = Brushes.DarkGreen,
                        Background = Brushes.White,
                        Padding = new Thickness(2)
                    };
                    Canvas.SetLeft(label, x + 8);
                    Canvas.SetTop(label, y - 8);
                    _canvas.Children.Add(label);
                }
            }
        }

        private void DrawInfo(SimplexHistory history, double baseScale, double baseOffsetX, double baseOffsetY,
                      double canvasWidth, double canvasHeight, double minX, double maxX, double minY, double maxY)
        {
            var last = history.Last;
            int iteration = last?.Iteration ?? 0;
            double bestValue = last?.BestValue ?? 0;

            TextBlock infoLabel = new TextBlock
            {
                Text = $"Симплексов: {history.Count} | Итерация: {iteration} | Значение: {bestValue:F6}",
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255)),
                Padding = new Thickness(6)
            };
            Canvas.SetLeft(infoLabel, 10);
            Canvas.SetTop(infoLabel, 10);
            _canvas.Children.Add(infoLabel);
        }

        private void DrawFixedAxes(Func<double, double, System.Windows.Point> transform, double minX, double maxX, double minY, double maxY)
        {
            System.Windows.Point axisXStart = transform(minX, 0);
            System.Windows.Point axisXEnd = transform(maxX, 0);

            if (axisXStart.Y >= 0 && axisXStart.Y <= _canvas.Height)
            {
                Line xAxis = new Line
                {
                    X1 = axisXStart.X,
                    Y1 = axisXStart.Y,
                    X2 = axisXEnd.X,
                    Y2 = axisXEnd.Y,
                    Stroke = new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection { 5, 3 }
                };
                _canvas.Children.Add(xAxis);
            }

            System.Windows.Point axisYStart = transform(0, minY);
            System.Windows.Point axisYEnd = transform(0, maxY);

            if (axisYStart.X >= 0 && axisYStart.X <= _canvas.Width)
            {
                Line yAxis = new Line
                {
                    X1 = axisYStart.X,
                    Y1 = axisYStart.Y,
                    X2 = axisYEnd.X,
                    Y2 = axisYEnd.Y,
                    Stroke = new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection { 5, 3 }
                };
                _canvas.Children.Add(yAxis);
            }
        }

        public void Clear()
        {
            _canvas.Children.Clear();
        }
    }
}
