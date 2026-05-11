using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NelderMeadOptimization.Functions;

namespace NelderMeadTests
{
    [TestClass]
    public class FunctionTests
    {
        [TestMethod]
        public void SphereFunction_result_in_point()
        {
            var sphere = new SphereFunction(2);
            double[] point = { 0.0, 0.0 };

            double result = sphere.Evaluate(point);

            Assert.AreEqual(0.0, result, 1e-10);
        }
        [TestMethod]
        public void RosenbrockFunction_result_in_point()
        {
            var rosenbrock = new RosenbrockFunction();
            double[] point = { 1.0, 1.0 };

            double result = rosenbrock.Evaluate(point);

            Assert.AreEqual(0.0, result, 1e-10);
        }
        [TestMethod]
        public void QuadraticFunction_result_in_point()
        {
            var quadratic = new QuadraticFunction();
            double[] point = { 0, 0 };

            double result = quadratic.Evaluate(point);

            Assert.AreEqual(0.0, result, 1e-10);
        }

        [TestMethod]
        public void RastriginFunction_result_in_point()
        {
            var rastrigin = new RastriginFunction();
            double[] point = { 0, 0 };

            double result = rastrigin.Evaluate(point);

            Assert.AreEqual(0.0, result, 1e-10);
        }
        [TestMethod]
        public void AckleyFunction_result_in_point()
        {
            var ackley = new AckleyFunction();
            double[] point = { 0.0, 0.0 };

            double result = ackley.Evaluate(point);

            Assert.AreEqual(0.0, result, 1e-10);
        }

        [TestMethod]
        public void BealeFunction_result_in_point()
        {
            var beale = new BealeFunction();
            double[] point = { 3.0, 0.5 };

            double result = beale.Evaluate(point);

            Assert.AreEqual(0.0, result, 1e-10);
        }

        [TestMethod]
        public void BoothFunction_result_in_point()
        {
            var booth = new BoothFunction();
            double[] point = { 1.0, 3.0 };

            double result = booth.Evaluate(point);

            Assert.AreEqual(0.0, result, 1e-10);
        }

        [TestMethod]
        public void GoldsteinPriceFunction_result_in_point()
        {
            var goldsteinPrice = new GoldsteinPriceFunction();
            double[] point = { 0.0, -1.0 };

            double result = goldsteinPrice.Evaluate(point);

            Assert.AreEqual(3.0, result, 1e-10);
        }

        [TestMethod]
        public void HimmelblauFunction_result_in_one_of_minimums()
        {
            var himmelblau = new HimmelblauFunction();
            // Один из четырёх глобальных минимумов: (3.0, 2.0)
            double[] point = { 3.0, 2.0 };

            double result = himmelblau.Evaluate(point);

            Assert.AreEqual(0.0, result, 1e-10);
        }
    }
}
