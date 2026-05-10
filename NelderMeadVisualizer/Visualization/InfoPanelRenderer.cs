using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NelderMeadVisualizer.Visualization
{
    internal class InfoPanelRenderer
    {
        private readonly Canvas _canvas;

        public InfoPanelRenderer(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void DrawInfo(SimplexHistory history)
        {
            var last = history.Last;
            int iteration = last?.Iteration ?? 0;
            double bestValue = last?.BestValue ?? 0;

            var info = new TextBlock
            {
                Text = $"Симплексов: {history.Count} | Итерация: {iteration} | Значение: {bestValue:F6}",
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255)),
                Padding = new Thickness(6)
            };
            Canvas.SetLeft(info, 10);
            Canvas.SetTop(info, 10);
            _canvas.Children.Add(info);
        }
    }
}
