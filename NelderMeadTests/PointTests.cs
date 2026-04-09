using Microsoft.VisualStudio.TestTools.UnitTesting;
using NelderMeadOptimization.Functions;
using NelderMeadOptimization.Models;
using System;

namespace NelderMeadTests
{
    [TestClass]
    public class PointTests
    {
        [TestMethod]
        public void Create_Point()
        {
            var sphere = new SphereFunction(2);
            double[] coords = { 2.0, 3.0 };

            var point = Point.Create(coords, sphere.Evaluate);

            Assert.AreEqual(2.0, point[0]);
            Assert.AreEqual(3.0, point[1]);
            Assert.AreEqual(13.0, point.Value);
        }
        [TestMethod]
        public void ToString_for_point()
        {
            var sphere = new SphereFunction(2);
            double[] coords = { 2.5, 3.5 };
            var point = Point.Create(coords, sphere.Evaluate);

            string str = point.ToString();

            Assert.AreEqual("(2,500000; 3,500000) = 18,500000", str);
        }
    }
}
