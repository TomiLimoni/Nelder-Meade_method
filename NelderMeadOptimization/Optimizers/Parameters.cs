namespace NelderMeadOptimization.Optimizers
{
    internal class Parameters
    {
        public double Alpha { get; set; } = 1.0;
        public double Beta { get; set; } = 2.0;
        public double Gamma { get; set; } = 0.5;
        public double Sigma { get; set; } = 0.5;
        public double Tolerance { get; set; } = 1e-6;
        public int MaxIterations { get; set; } = 1000;
        public int Dimension { get; set; } = 0;
    }
}
