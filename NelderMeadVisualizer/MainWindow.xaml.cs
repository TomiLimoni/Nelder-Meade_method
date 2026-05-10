using NelderMeadOptimization.Functions;
using NelderMeadOptimization.Interface;
using NelderMeadOptimization.Models;
using NelderMeadOptimization.Optimizers;
using NelderMeadVisualizer.Optimization;
using NelderMeadVisualizer.Services;
using NelderMeadVisualizer.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private readonly ZoomController _zoomController;
        private readonly SimplexStateManager _simplexManager;
        private readonly UIManager _uiManager;
        private readonly ResultFormatter _resultFormatter;
        private readonly OptimizationRunner _optimizationRunner;
        private readonly StepByStepOptimizer _stepOptimizer;

        public MainWindow()
        {
            InitializeComponent();

            var controls = UIControls.FromMainWindow(this);

            _zoomController = new ZoomController(SimplexCanvas);
            _simplexManager = new SimplexStateManager(SimplexCanvas, _zoomController);
            _uiManager = new UIManager(controls);
            _resultFormatter = new ResultFormatter();
            _stepOptimizer = new StepByStepOptimizer();

            _optimizationRunner = new OptimizationRunner(
                onIterationComplete: OnIterationComplete,
                onComplete: OnOptimizationComplete
            );

            InitializeStepOptimizer();
            AttachEventHandlers();
        }

        private void OnIterationComplete(Simplex simplex, int iteration)
        {
            Dispatcher.Invoke(() =>
            {
                var function = SelectFunction();
                var drawOptions = DrawOptions.Create(_simplexManager.History, _simplexManager.Bounds);
                drawOptions.Function = function;
                drawOptions.ShowContours = chkShowContours.IsChecked == true;
                _simplexManager.AddAndDraw(simplex, iteration, drawOptions);
                UpdateIterationDisplay(iteration);
            });
        }

        private void OnOptimizationComplete(OptimizationResult result)
        {
            Dispatcher.Invoke(() =>
            {
                if (result != null)
                    ShowCompletionResult(result);
                else
                    ShowCancelledResult();
                _uiManager.UnlockAfterOptimization();
            });
        }

        private void InitializeStepOptimizer()
        {
            _stepOptimizer.IterationCompleted += (simplex, iter) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var function = SelectFunction();
                    var options = DrawOptions.Create(_simplexManager.History, _simplexManager.Bounds);
                    options.Function = function;
                    options.ShowContours = chkShowContours.IsChecked == true;
                    _simplexManager.AddAndDraw(simplex, iter, options);
                    UpdateIterationDisplay(iter);

                    if (_stepOptimizer.IsCompleted)
                    {
                        var data = new OptimizationData
                        {
                            Function = SelectFunction(),
                            Simplex = simplex,
                            Iteration = iter,
                            Parameters = GetParameters(),
                            Converged = simplex.IsConverged(GetParameters().Tolerance)
                        };

                        txtResult.Text = _resultFormatter.Format(data);
                        double error = data.Function.CalculateError(data.Simplex.Best.Coordinates);
                        txtFinalError.Text = $"{error:F8}";
                        txtConverged.Text = data.Converged ? "Да" : "Нет";

                        _uiManager.UpdateStatus(_resultFormatter.GetStatusText(data.Converged));
                        _uiManager.UpdateStatusBar("Пошаговый режим завершён");
                        btnStep.IsEnabled = false;
                    }
                });
            };
        }

        private void AttachEventHandlers()
        {
            btnRun.Click += BtnRun_Click;
            btnStep.Click += BtnStep_Click;
            btnReset.Click += BtnReset_Click;
            btnResetZoom.Click += (s, e) => _zoomController.Reset();
            btnResetPoints.Click += (s, e) => _uiManager.ResetInitialPoints();
            chkShowContours.Checked += (s, e) => RedrawWithContours();
            chkShowContours.Unchecked += (s, e) => RedrawWithContours();
        }

        private Parameters GetParameters()
        {
            try
            {
                var culture = System.Globalization.CultureInfo.InvariantCulture;
                return new Parameters
                {
                    Alpha = double.Parse(txtAlpha.Text, culture),
                    Beta = double.Parse(txtBeta.Text, culture),
                    Gamma = double.Parse(txtGamma.Text, culture),
                    Sigma = double.Parse(txtSigma.Text, culture),
                    Tolerance = double.Parse(txtTolerance.Text, culture),
                    MaxIterations = int.Parse(txtMaxIterations.Text)
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в параметрах: {ex.Message}\nИспользуются стандартные значения",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return new Parameters();
            }
        }

        private void ResetAll()
        {
            _simplexManager.Clear();
            _stepOptimizer.Reset();
            _zoomController.Reset();
            _optimizationRunner.Cancel();

            txtResult.Text = "";
            UpdateIterationDisplay(-1);
            txtConverged.Text = "-";
            txtFinalError.Text = "-";

            _uiManager.UpdateStatus("Готов");
            _uiManager.UpdateStatusBar("Готов к работе. Выберите функцию и нажмите 'Запустить'");
        }

        private void RedrawWithContours()
        {
            if (_simplexManager.History.Count > 0)
            {
                var function = SelectFunction();
                var drawOptions = DrawOptions.Create(_simplexManager.History, _simplexManager.Bounds);
                drawOptions.Function = function;
                drawOptions.ShowContours = chkShowContours.IsChecked == true;
                _simplexManager.Redraw(drawOptions);
            }
        }

        private async void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            ResetAll();
            _uiManager.LockForOptimization();

            var function = SelectFunction();
            var parameters = GetParameters();
            var initialPoints = CreateInitialPoints(function);

            await _optimizationRunner.RunAsync(function, parameters, initialPoints);
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
            _uiManager.FullUnlock();
        }

        private void StartStepMode()
        {
            var function = SelectFunction();
            var parameters = GetParameters();
            var initialPoints = CreateInitialPoints(function);

            _stepOptimizer.Initialize(function, parameters, initialPoints);
            _simplexManager.Clear();
            _uiManager.LockForStepMode();

            var initialSimplex = _stepOptimizer.CurrentSimplex;
            if (initialSimplex != null)
            {
                var options = DrawOptions.Create(_simplexManager.History, _simplexManager.Bounds);
                options.Function = function;
                options.ShowContours = chkShowContours.IsChecked == true;

                _simplexManager.AddAndDraw(initialSimplex, 0, options);
            }

            UpdateIterationDisplay(0);
            _uiManager.UpdateStatus("Пошаговый режим. Нажимайте 'ШАГ'", isError: false);
            _uiManager.UpdateStatusBar("Пошаговый режим. Для выхода нажмите 'Сбросить'");
        }

        private void ShowCompletionResult(OptimizationResult result)
        {
            var function = SelectFunction();
            var parameters = GetParameters();

            txtResult.Text = _resultFormatter.FormatResult(result, parameters);
            UpdateIterationDisplay(result.Iterations);
            txtConverged.Text = result.Converged ? "Да" : "Нет";

            double error = function.CalculateError(result.OptimalPoint.Coordinates);
            txtFinalError.Text = $"{error:F8}";

            _uiManager.UpdateStatus(_resultFormatter.GetStatusText(result.Converged));
            _uiManager.UpdateStatusBar("Готово");
        }

        private void ShowCancelledResult()
        {
            _uiManager.UpdateStatus("Прервано", isError: true);
            _uiManager.UpdateStatusBar("Оптимизация прервана");
        }

        private void UpdateIterationDisplay(int iteration)
        {
            txtIterations.Text = iteration >= 0 ? $"{iteration}" : "-";
        }

        private ITestFunction SelectFunction()
        {
            string selected = (cmbFunction.SelectedItem as ComboBoxItem)?.Content.ToString();

            switch (selected)
            {
                case "Сферическая": return new SphereFunction(2);
                case "Розенброка": return new RosenbrockFunction();
                case "Квадратичная": return new QuadraticFunction();
                case "Растригина": return new RastriginFunction();
                default: return new SphereFunction(2);
            }
        }
        private MyPoint[] CreateInitialPoints(ITestFunction function)
        {
            try
            {
                var culture = System.Globalization.CultureInfo.InvariantCulture;
                double x1 = double.Parse(txtX1.Text, culture);
                double y1 = double.Parse(txtY1.Text, culture);
                double x2 = double.Parse(txtX2.Text, culture);
                double y2 = double.Parse(txtY2.Text, culture);
                double x3 = double.Parse(txtX3.Text, culture);
                double y3 = double.Parse(txtY3.Text, culture);

                return new MyPoint[]
                {
                    MyPoint.Create(function.Evaluate, x1, y1),
                    MyPoint.Create(function.Evaluate, x2, y2),
                    MyPoint.Create(function.Evaluate, x3, y3)
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в начальных точках: {ex.Message}\nИспользуются стандартные значения",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);

                return new MyPoint[]
                {
                    MyPoint.Create(function.Evaluate, 2.0, 2.0),
                    MyPoint.Create(function.Evaluate, 3.0, 2.0),
                    MyPoint.Create(function.Evaluate, 2.0, 3.0)
                };
            }
        }
    }
}
