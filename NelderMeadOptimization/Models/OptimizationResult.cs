using System.Text;

namespace NelderMeadOptimization.Models
{
    internal class OptimizationResult
    {
        public Point OptimalPoint { get; set; }
        public int Iterations { get; set; }
        public bool Converged { get; set; }
        public string FunctionName { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"\n Result for: {FunctionName}");
            sb.AppendLine($"Optimal Point: {OptimalPoint}");
            sb.AppendLine($"Number of iterations: {Iterations}");
            sb.AppendLine($"Converged? {(Converged ? "Yes" : "No")}");
            return sb.ToString();
        }
    }
}
