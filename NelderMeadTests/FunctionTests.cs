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
    }
}
