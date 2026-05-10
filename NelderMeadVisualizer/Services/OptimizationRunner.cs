using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NelderMeadOptimization.Functions;
using NelderMeadOptimization.Interface;
using NelderMeadOptimization.Models;
using NelderMeadOptimization.Optimizers;
using System.Threading;
using System.Windows;
using MyPoint = NelderMeadOptimization.Models.Point;

namespace NelderMeadVisualizer.Services
{
    internal class OptimizationRunner
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly Action<Simplex, int> _onIterationComplete;
        private readonly Action<OptimizationResult> _onComplete;

        public OptimizationRunner(
            Action<Simplex, int> onIterationComplete,
            Action<OptimizationResult> onComplete)
        {
            _onIterationComplete = onIterationComplete;
            _onComplete = onComplete;
        }

        public async Task RunAsync(
            ITestFunction function,
            Parameters parameters,
            MyPoint[] initialPoints)
        {
            var optimizer = new NelderMeadOptimizer(function, parameters);

            optimizer.IterationCompleted += (simplex, iteration) =>
            {
                Thread.Sleep(150);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _onIterationComplete?.Invoke(simplex, iteration);
                });
            };

            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var result = await Task.Run(
                    () => optimizer.Optimize(initialPoints),
                    _cancellationTokenSource.Token);

                _onComplete?.Invoke(result);
            }
            catch (OperationCanceledException)
            {
                _onComplete?.Invoke(null);
            }
        }

        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }
}
