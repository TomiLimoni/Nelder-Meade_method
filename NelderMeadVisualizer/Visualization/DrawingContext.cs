using NelderMeadOptimization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace NelderMeadVisualizer.Visualization
{
    internal class DrawingContext
    {
        public double BaseScale { get; set; }
        public double BaseOffsetX { get; set; }
        public double BaseOffsetY { get; set; }
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }
        public double CanvasWidth { get; set; }
        public double CanvasHeight { get; set; }

        public System.Windows.Point Transform(double x, double y, ZoomController zoom)
        {
            return new System.Windows.Point(
                zoom.TransformX(x, BaseScale, BaseOffsetX),
                zoom.TransformY(y, BaseScale, BaseOffsetY)
            );
        }

        public static DrawingContext Create(Canvas canvas, Bounds bounds)
        {
            double canvasWidth = canvas.Width;
            double canvasHeight = canvas.Height;

            double baseScaleX = canvasWidth / (bounds.MaxX - bounds.MinX);
            double baseScaleY = canvasHeight / (bounds.MaxY - bounds.MinY);
            double baseScale = Math.Min(baseScaleX, baseScaleY) * 0.85;

            double baseOffsetX = canvasWidth / 2 - (bounds.MinX + bounds.MaxX) / 2 * baseScale;
            double baseOffsetY = canvasHeight / 2 - (bounds.MinY + bounds.MaxY) / 2 * baseScale;

            return new DrawingContext
            {
                BaseScale = baseScale,
                BaseOffsetX = baseOffsetX,
                BaseOffsetY = baseOffsetY,
                MinX = bounds.MinX,
                MaxX = bounds.MaxX,
                MinY = bounds.MinY,
                MaxY = bounds.MaxY,
                CanvasWidth = canvasWidth,
                CanvasHeight = canvasHeight
            };
        }
    }
}
