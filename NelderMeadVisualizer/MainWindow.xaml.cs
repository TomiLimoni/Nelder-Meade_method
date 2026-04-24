using NelderMeadOptimization.Functions;
using NelderMeadOptimization.Interface;
using NelderMeadOptimization.Models;
using NelderMeadOptimization.Optimizers;
using NelderMeadVisualizer.Optimization;
using NelderMeadVisualizer.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyPoint = NelderMeadOptimization.Models.Point;

namespace NelderMeadVisualizer
{
    public partial class MainWindow : Window
    {
        private SimplexHistory _history;
        private SimplexDrawer _drawer;
        private ZoomController _zoomController;
        private StepByStepOptimizer _stepOptimizer;

        private double _minX = -2, _maxX = 5, _minY = -2, _maxY = 5;

        public MainWindow()
        {
            InitializeComponent();
            InitializeComponents();

            btnRun.Click += BtnRun_Click;
            btnStep.Click += BtnStep_Click;
            btnReset.Click += BtnReset_Click;
            btnResetZoom.Click += ResetZoom_Click;
        }

        private void InitializeComponents()
        {
            _history = new SimplexHistory();
            _zoomController = new ZoomController(SimplexCanvas);
            _drawer = new SimplexDrawer(SimplexCanvas, _zoomController);
            _stepOptimizer = new StepByStepOptimizer();

            _stepOptimizer.IterationCompleted += (simplex, iter) =>
            {
                Dispatcher.Invoke(() =>
                {
                    _history.Add(simplex, iter);
                    UpdateBounds(simplex);
                    txtIterations.Text = $"Итераций: {iter}";

                    if (_stepOptimizer.IsCompleted)
                    {
                        ShowCompletionResult(simplex, iter);
                    }
                });
            };
        }

        private void UpdateBounds(Simplex simplex)
        {
            for (int i = 0; i < simplex.Size; i++)
            {
                _minX = Math.Min(_minX, simplex[i][0] - 1);
                _maxX = Math.Max(_maxX, simplex[i][0] + 1);
                _minY = Math.Min(_minY, simplex[i][1] - 1);
                _maxY = Math.Max(_maxY, simplex[i][1] + 1);
            }
            _drawer.DrawAll(_history, _minX, _maxX, _minY, _maxY);
        }

        private void ResetAll()
        {
            _history.Clear();
            _stepOptimizer.Reset();
            _zoomController.Reset();
            _minX = -2; _maxX = 5; _minY = -2; _maxY = 5;
            _drawer.Clear();

            btnRun.IsEnabled = true;
            btnStep.IsEnabled = true;
            txtResult.Text = "";
            txtIterations.Text = "Итераций: -";
            txtConverged.Text = "Сходимость: -";
            txtStatus.Text = "Готов";
            txtStatus.Foreground = new SolidColorBrush(Colors.Green);
            statusBarText.Text = "Готов к работе. Выберите функцию и нажмите 'Запустить оптимизацию'";

            ClearCanvas();
        }

        private async void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            ResetAll();
            await RunOptimizationAsync();
        }

        private void BtnStep_Click(object sender, RoutedEventArgs e)
        {
            if (_stepOptimizer.CurrentSimplex == null)
            {
                StartStepMode();
                return;
            }

            _stepOptimizer.PerformOneIteration();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetAll();
        }

        private void StartStepMode()
        {
            ITestFunction function = SelectFunction();
            var parameters = new Parameters
            {
                Tolerance = double.Parse(txtTolerance.Text, System.Globalization.CultureInfo.InvariantCulture),
                MaxIterations = int.Parse(txtMaxIterations.Text)
            };
            MyPoint[] initialPoints = CreateInitialPoints(function);

            _stepOptimizer.Initialize(function, parameters, initialPoints);
            _history.Clear();

            var initialSimplex = _stepOptimizer.CurrentSimplex;
            if (initialSimplex != null)
            {
                _history.Add(initialSimplex, 0);
                UpdateBounds(initialSimplex);
            }

            txtIterations.Text = "Итераций: 0";
            txtStatus.Text = "Пошаговый режим. Нажимайте 'ШАГ'";
            statusBarText.Text = "Пошаговый режим";
            btnRun.IsEnabled = false;
        }

        private async Task RunOptimizationAsync()
        {
            ITestFunction function = SelectFunction();
            double tolerance = double.Parse(txtTolerance.Text, System.Globalization.CultureInfo.InvariantCulture);
            int maxIterations = int.Parse(txtMaxIterations.Text);
            MyPoint[] initialPoints = CreateInitialPoints(function);

            var simplex = new Simplex(initialPoints);
            int iteration = 0;

            _history.Clear();
            _history.Add(simplex, 0);
            UpdateBounds(simplex);

            while (iteration < maxIterations && !simplex.IsConverged(tolerance))
            {
                iteration++;

                double[] centroid = simplex.GetCentroidCoordinates();
                MyPoint reflected = simplex.Reflect(centroid, function.Evaluate, 1.0);

                if (reflected.Value < simplex.Best.Value)
                {
                    MyPoint expanded = simplex.Expand(centroid, reflected, function.Evaluate, 2.0);
                    simplex.ReplaceWorst(expanded.Value < reflected.Value ? expanded : reflected);
                }
                else if (reflected.Value < simplex.SecondWorst.Value)
                {
                    simplex.ReplaceWorst(reflected);
                }
                else
                {
                    if (reflected.Value < simplex.Worst.Value)
                    {
                        MyPoint contracted = simplex.ContractOutside(centroid, reflected, function.Evaluate, 0.5);
                        if (contracted.Value < reflected.Value)
                            simplex.ReplaceWorst(contracted);
                        else
                            simplex.Reduce(function.Evaluate, 0.5);
                    }
                    else
                    {
                        MyPoint contracted = simplex.ContractInside(centroid, function.Evaluate, 0.5);
                        if (contracted.Value < simplex.Worst.Value)
                            simplex.ReplaceWorst(contracted);
                        else
                            simplex.Reduce(function.Evaluate, 0.5);
                    }
                }

                _history.Add(simplex, iteration);
                UpdateBounds(simplex);
                txtIterations.Text = $"Итераций: {iteration}";

                await Task.Delay(150);
            }

            ShowCompletionResult(simplex, iteration);
            btnRun.IsEnabled = true;
        }

        private void ShowCompletionResult(Simplex simplex, int iteration)
        {
            var result = new OptimizationResult
            {
                OptimalPoint = simplex.Best,
                Iterations = iteration,
                Converged = true,
                FunctionName = _stepOptimizer.CurrentSimplex != null ?
                    SelectFunction().Name : SelectFunction().Name
            };
            ShowResult(result);

            if (_stepOptimizer.IsCompleted || iteration >= int.Parse(txtMaxIterations.Text))
            {
                txtStatus.Text = "Оптимизация завершена! Нажмите 'Сбросить'";
                btnStep.IsEnabled = false;
            }
            else
            {
                txtStatus.Text = "Оптимизация завершена успешно!";
                txtStatus.Foreground = new SolidColorBrush(Colors.Green);
            }
        }

        private ITestFunction SelectFunction()
        {
            if (rbSphere.IsChecked == true)
                return new SphereFunction(2);
            if (rbRosenbrock.IsChecked == true)
                return new RosenbrockFunction();
            if (rbQuadratic.IsChecked == true)
                return new QuadraticFunction();
            if (rbRastrigin.IsChecked == true)
                return new RastriginFunction();
            return new SphereFunction(2);
        }

        private MyPoint[] CreateInitialPoints(ITestFunction function)
        {
            return new MyPoint[]
            {
                MyPoint.Create(function.Evaluate, 2.0, 2.0),
                MyPoint.Create(function.Evaluate, 3.0, 2.0),
                MyPoint.Create(function.Evaluate, 2.0, 3.0)
            };
        }

        private void ShowResult(OptimizationResult result)
        {
            string output = $"=== РЕЗУЛЬТАТ ОПТИМИЗАЦИИ ===\n\n";
            output += $"Функция: {result.FunctionName}\n";
            output += $"Найденная точка: {result.OptimalPoint}\n";
            output += $"Количество итераций: {result.Iterations}\n";
            output += $"Сходимость: {(result.Converged ? "ДА " : "НЕТ ")}\n";

            if (!result.Converged)
                output += "\n Внимание: Достигнут лимит итераций!";

            txtResult.Text = output;
            txtIterations.Text = $"Итераций: {result.Iterations}";
            txtConverged.Text = $"Сходимость: {(result.Converged ? "Да" : "Нет")}";

            if (result.Converged)
            {
                txtStatus.Text = "Оптимизация завершена успешно!";
                txtStatus.Foreground = new SolidColorBrush(Colors.Green);
                statusBarText.Text = "Готово";
            }
            else
            {
                txtStatus.Text = "Оптимизация не сошлась (лимит итераций)";
                txtStatus.Foreground = new SolidColorBrush(Colors.Red);
                statusBarText.Text = "Не сошлась";
            }
        }

        private void ClearCanvas()
        {
            SimplexCanvas.Children.Clear();
            _history?.Clear();
        }
        private void ResetZoom_Click(object sender, RoutedEventArgs e)
        {
            _zoomController.Reset();
        }
    }
}
