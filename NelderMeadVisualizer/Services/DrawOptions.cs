using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NelderMeadOptimization.Interface;
using NelderMeadVisualizer.Visualization;

namespace NelderMeadVisualizer.Services
{
    internal class DrawOptions
    {
        public SimplexHistory History { get; set; }
        public Bounds Bounds { get; set; }
        public ITestFunction Function { get; set; }
        public bool ShowContours { get; set; }

        public static DrawOptions Create(SimplexHistory history, Bounds bounds)
        {
            return new DrawOptions
            {
                History = history,
                Bounds = bounds,
                Function = null,
                ShowContours = false
            };
        }
    }
}
