using Microsoft.VisualStudio.TestTools.UnitTesting;
using NelderMeadOptimization.Optimizers;
using System;

namespace NelderMeadTests
{
    [TestClass]
    public class ParametersTests
    {
        [TestMethod]
        public void ParametersUnchanged()
        {
            var p = new Parameters();

            Assert.AreEqual(1.0, p.Alpha);
            Assert.AreEqual(2.0, p.Beta);
            Assert.AreEqual(0.5, p.Gamma);
            Assert.AreEqual(0.5, p.Sigma);
            Assert.AreEqual(1e-6, p.Tolerance);
            Assert.AreEqual(1000, p.MaxIterations);
        }
        [TestMethod]
        public void ParametersChanged()
        {
            var p = new Parameters
            {
                Alpha = 1.5,
                Beta = 3.0,
                Gamma = 0.7,
                Sigma = 0.3,
                Tolerance = 1e-5,
                MaxIterations = 500
            };

            Assert.AreEqual(1.5, p.Alpha);
            Assert.AreEqual(3.0, p.Beta);
            Assert.AreEqual(0.7, p.Gamma);
            Assert.AreEqual(0.3, p.Sigma);
            Assert.AreEqual(1e-5, p.Tolerance);
            Assert.AreEqual(500, p.MaxIterations);
        }
    }
}
