using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NelderMeadVisualizer.Services
{
    /// <summary>
    /// Управляет состоянием UI-элементов (блокировка/разблокировка кнопок и полей).
    /// Позволяет изолировать логику UI от MainWindow.
    /// </summary>
    internal class UIManager
    {
        private readonly UIControls _ui;

        public UIManager(UIControls uiControls)
        {
            _ui = uiControls;
        }

        public void LockForOptimization() => SetControlsEnabled(false);
        public void UnlockAfterOptimization()
        {
            SetControlsEnabled(true);
            _ui.Step.IsEnabled = true;
        }

        public void LockForStepMode()
        {
            _ui.FunctionSelector.IsEnabled = false;
            _ui.X1.IsEnabled = _ui.Y1.IsEnabled = _ui.X2.IsEnabled = false;
            _ui.Y2.IsEnabled = _ui.X3.IsEnabled = _ui.Y3.IsEnabled = false;
            _ui.ResetPoints.IsEnabled = false;
            _ui.Run.IsEnabled = false;

            _ui.Alpha.IsEnabled = _ui.Beta.IsEnabled = true;
            _ui.Gamma.IsEnabled = _ui.Sigma.IsEnabled = true;
            _ui.Tolerance.IsEnabled = _ui.MaxIterations.IsEnabled = true;
            _ui.Reset.IsEnabled = _ui.ResetZoom.IsEnabled = true;
            _ui.Step.IsEnabled = true;
        }

        public void FullUnlock() => SetControlsEnabled(true);

        private void SetControlsEnabled(bool enabled)
        {
            _ui.FunctionSelector.IsEnabled = enabled;
            _ui.Alpha.IsEnabled = _ui.Beta.IsEnabled = enabled;
            _ui.Gamma.IsEnabled = _ui.Sigma.IsEnabled = enabled;
            _ui.Tolerance.IsEnabled = _ui.MaxIterations.IsEnabled = enabled;
            _ui.X1.IsEnabled = _ui.Y1.IsEnabled = enabled;
            _ui.X2.IsEnabled = _ui.Y2.IsEnabled = enabled;
            _ui.X3.IsEnabled = _ui.Y3.IsEnabled = enabled;
            _ui.Run.IsEnabled = enabled;
            _ui.Reset.IsEnabled = enabled;
            _ui.ResetPoints.IsEnabled = enabled;
            _ui.ResetZoom.IsEnabled = enabled;
            _ui.Step.IsEnabled = enabled;
        }

        public void UpdateStatus(string text, bool isError = false)
        {
            _ui.Status.Text = text;
            _ui.Status.Foreground = isError ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);
        }

        public void UpdateStatusBar(string text) => _ui.StatusBar.Text = text;
        public void ResetInitialPoints()
        {
            _ui.X1.Text = _ui.Y1.Text = "2.0";
            _ui.X2.Text = "3.0";
            _ui.Y2.Text = "2.0";
            _ui.X3.Text = "2.0";
            _ui.Y3.Text = "3.0";
        }
    }
}
