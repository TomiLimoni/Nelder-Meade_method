using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NelderMeadOptimization.Models;

namespace NelderMeadVisualizer.Visualization
{
    internal class SimplexHistory
    {
        private List<SimplexSnapshot> _history = new List<SimplexSnapshot>();
        private const int MaxHistory = 50;

        public IReadOnlyList<SimplexSnapshot> Snapshots => _history;
        public int Count => _history.Count;
        public SimplexSnapshot Last => _history.Count > 0 ? _history[_history.Count - 1] : null;

        public void Add(Simplex simplex, int iteration)
        {
            var snapshot = new SimplexSnapshot(simplex, iteration);
            _history.Add(snapshot);

            while (_history.Count > MaxHistory)
                _history.RemoveAt(0);
        }

        public void Clear()
        {
            _history.Clear();
        }

        public class SimplexSnapshot
        {
            public double[][] Points { get; }
            public int Iteration { get; }
            public double BestValue { get; }

            public SimplexSnapshot(Simplex simplex, int iteration)
            {
                Points = new double[simplex.Size][];
                for (int i = 0; i < simplex.Size; i++)
                {
                    Points[i] = new double[2];
                    Points[i][0] = simplex[i][0];
                    Points[i][1] = simplex[i][1];
                }
                Iteration = iteration;
                BestValue = simplex.Best.Value;
            }
        }
    }
}
