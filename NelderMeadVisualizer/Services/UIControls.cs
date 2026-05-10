using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NelderMeadVisualizer.Services
{
    internal class UIControls
    {
        // Параметры метода
        public TextBox Alpha { get; set; }
        public TextBox Beta { get; set; }
        public TextBox Gamma { get; set; }
        public TextBox Sigma { get; set; }
        public TextBox Tolerance { get; set; }
        public TextBox MaxIterations { get; set; }

        // Начальные точки
        public TextBox X1 { get; set; }
        public TextBox Y1 { get; set; }
        public TextBox X2 { get; set; }
        public TextBox Y2 { get; set; }
        public TextBox X3 { get; set; }
        public TextBox Y3 { get; set; }

        // Выбор функции
        public ComboBox FunctionSelector { get; set; }

        // Кнопки
        public Button Run { get; set; }
        public Button Reset { get; set; }
        public Button ResetPoints { get; set; }
        public Button ResetZoom { get; set; }
        public Button Step { get; set; }

        // Статусы
        public TextBlock Status { get; set; }
        public TextBlock StatusBar { get; set; }

        // Конструктор для создания из MainWindow
        public static UIControls FromMainWindow(MainWindow window)
        {
            return new UIControls
            {
                Alpha = window.txtAlpha,
                Beta = window.txtBeta,
                Gamma = window.txtGamma,
                Sigma = window.txtSigma,
                Tolerance = window.txtTolerance,
                MaxIterations = window.txtMaxIterations,
                X1 = window.txtX1,
                Y1 = window.txtY1,
                X2 = window.txtX2,
                Y2 = window.txtY2,
                X3 = window.txtX3,
                Y3 = window.txtY3,
                FunctionSelector = window.cmbFunction,
                Run = window.btnRun,
                Reset = window.btnReset,
                ResetPoints = window.btnResetPoints,
                ResetZoom = window.btnResetZoom,
                Step = window.btnStep,
                Status = window.txtStatus,
                StatusBar = window.statusBarText
            };
        }
    }
}
