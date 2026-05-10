using NelderMeadOptimization.Functions;
using NelderMeadOptimization.Interface;
using NelderMeadOptimization.Models;
using NelderMeadOptimization.Optimizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NelderMeadVisualizer.Services
{
    /// <summary>
    /// Форматирует результаты оптимизации для отображения в UI.
    /// Format() - для пошагового режима (есть полный симплекс)
    /// FormatResult() - для автоматической оптимизации (только результат)
    /// </summary>
    internal class ResultFormatter
    {
        public string Format(OptimizationData data)
        {
            double error = data.Function.CalculateError(data.Simplex.Best.Coordinates);

            var sb = new StringBuilder();
            sb.AppendLine("=== РЕЗУЛЬТАТ ОПТИМИЗАЦИИ ===\n");
            sb.AppendLine($"Функция: {data.Function.Name}");
            sb.AppendLine($"Найденная точка: {data.Simplex.Best}");
            sb.AppendLine($"Количество итераций: {data.Iteration}");
            sb.AppendLine($"Сходимость: {(data.Converged ? "ДА" : "НЕТ")}");
            sb.AppendLine($"Ошибка: {error:F8}");
            sb.AppendLine("\n--- Использованные параметры ---");
            sb.AppendLine($"Отражение: {data.Parameters.Alpha}");
            sb.AppendLine($"Растяжение: {data.Parameters.Beta}");
            sb.AppendLine($"Сжатие: {data.Parameters.Gamma}");
            sb.AppendLine($"Редукция: {data.Parameters.Sigma}");
            sb.AppendLine($"Точность: {data.Parameters.Tolerance}");
            sb.AppendLine($"Макс. итераций: {data.Parameters.MaxIterations}");

            if (!data.Converged)
                sb.AppendLine("\nВнимание: Достигнут лимит итераций!");

            return sb.ToString();
        }

        public string FormatResult(OptimizationResult result, Parameters parameters)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== РЕЗУЛЬТАТ ОПТИМИЗАЦИИ ===\n");
            sb.AppendLine($"Функция: {result.FunctionName}");
            sb.AppendLine($"Найденная точка: {result.OptimalPoint}");
            sb.AppendLine($"Количество итераций: {result.Iterations}");
            sb.AppendLine($"Сходимость: {(result.Converged ? "ДА" : "НЕТ")}");
            sb.AppendLine("\n--- Использованные параметры ---");
            sb.AppendLine($"Отражение: {parameters.Alpha}");
            sb.AppendLine($"Растяжение: {parameters.Beta}");
            sb.AppendLine($"Сжатие: {parameters.Gamma}");
            sb.AppendLine($"Редукция: {parameters.Sigma}");
            sb.AppendLine($"Точность: {parameters.Tolerance}");
            sb.AppendLine($"Макс. итераций: {parameters.MaxIterations}");

            if (!result.Converged)
                sb.AppendLine("\nВнимание: Достигнут лимит итераций!");

            return sb.ToString();
        }
        public string GetStatusText(bool converged) => converged ? "Успешно" : "Не сошлась";
    }
}
