using Microsoft.VisualStudio.TestTools.UnitTesting;
using NelderMeadOptimization.Functions;
using NelderMeadOptimization.Models;
using NelderMeadOptimization.Optimizers;
using System;

namespace NelderMeadTests
{
    [TestClass]
    public class OptimizationResultTests
    {
        [TestMethod]
        public void OptimizationResultSphere2D()
        {
            var sphere = new SphereFunction(2);
            Point[] points = new Point[]
            {
                Point.Create(sphere.Evaluate, 2.0, 2.0),
                Point.Create(sphere.Evaluate, 3.0, 2.0),
                Point.Create(sphere.Evaluate, 2.0, 3.0)
            };
            var optimizer = new NelderMeadOptimizer(sphere);

            var result = optimizer.Optimize(points);

            Assert.AreEqual(sphere.Name, result.FunctionName);
            Assert.IsNotNull(result.OptimalPoint);
            Assert.IsTrue(result.Iterations == 27);
            Assert.IsTrue(result.Converged);
        }
    }
}
