using Microsoft.VisualStudio.TestTools.UnitTesting;
using NelderMeadOptimization.Functions;
using NelderMeadOptimization.Models;
using NelderMeadOptimization.Optimizers;
using System;

namespace NelderMeadTests
{
    [TestClass]
    public class SimplexTests
    {
        [TestMethod]
        public void SortSimplexSphere2D()
        {
            var sphere = new SphereFunction(2);
            Point[] points = new Point[]
            {
                Point.Create(sphere.Evaluate, 2.0, 2.0),
                Point.Create(sphere.Evaluate, 3.0, 2.0),
                Point.Create(sphere.Evaluate, 2.0, 1.0)
            };
            var simplex = new Simplex(points);

            simplex.Sort();

            Assert.AreEqual(5, simplex.Best.Value);
            Assert.AreEqual(8, simplex[1].Value);
            Assert.AreEqual(13, simplex.Worst.Value);
        }
        [TestMethod]
        public void GetCentroidCoordinatesSphere2D()
        {
            var sphere = new SphereFunction(2);
            Point[] points = new Point[]
            {
                Point.Create(sphere.Evaluate, 1.0, 0.0),
                Point.Create(sphere.Evaluate, 3.0, 2.0),
                Point.Create(sphere.Evaluate, 5.0, 7.0)
            };
            var simplex = new Simplex(points);

            double[] centroid = simplex.GetCentroidCoordinates();

            Assert.AreEqual(2.0, centroid[0], 0.0001);
            Assert.AreEqual(1.0, centroid[1], 0.0001);
        }
        [TestMethod]
        public void ReflectSphere2D()
        {
            var sphere = new SphereFunction(2);
            Point[] points = new Point[]
            {
                Point.Create(sphere.Evaluate, 1.0, 1.0),
                Point.Create(sphere.Evaluate, 3.0, 3.0),
                Point.Create(sphere.Evaluate, 5.0, 5.0)
            };
            var simplex = new Simplex(points);
            double[] centroid = simplex.GetCentroidCoordinates();
            double alpha = 1.0;

            Point reflected = simplex.Reflect(centroid, sphere.Evaluate, alpha);

            Assert.AreEqual(-1.0, reflected[0], 0.0001);
            Assert.AreEqual(-1.0, reflected[1], 0.0001);
            Assert.AreEqual(2.0, reflected.Value, 0.0001);
        }
        [TestMethod]
        public void ExpandSphere2D()
        {
            var sphere = new SphereFunction(2);
            Point[] points = new Point[]
            {
                Point.Create(sphere.Evaluate, 1.0, 1.0),
                Point.Create(sphere.Evaluate, 3.0, 3.0),
                Point.Create(sphere.Evaluate, 5.0, 5.0)
            };
            double beta = 2.0;
            var simplex = new Simplex(points);
            double[] centroid = simplex.GetCentroidCoordinates();
            Point reflected = Point.Create(sphere.Evaluate, -1.0, -1.0);

            Point expanded = simplex.Expand(centroid, reflected, sphere.Evaluate, beta);

            Assert.AreEqual(-4.0, expanded[0], 0.0001);
            Assert.AreEqual(-4.0, expanded[1], 0.0001);
            Assert.AreEqual(32.0, expanded.Value, 0.0001);
        }
        [TestMethod]
        public void ContractInsideSphere2D()
        {
            var sphere = new SphereFunction(2);
            Point[] points = new Point[]
            {
                Point.Create(sphere.Evaluate, 1.0, 1.0),
                Point.Create(sphere.Evaluate, 3.0, 3.0),
                Point.Create(sphere.Evaluate, 5.0, 5.0)
            };
            double gamma = 0.5;
            var simplex = new Simplex(points);
            double[] centroid = simplex.GetCentroidCoordinates();

            Point contracted = simplex.ContractInside(centroid, sphere.Evaluate, gamma);

            Assert.AreEqual(3.5, contracted[0], 0.0001);
            Assert.AreEqual(3.5, contracted[1], 0.0001);
            Assert.AreEqual(24.5, contracted.Value, 0.0001);
        }
        [TestMethod]
        public void ContractOutsideSphere2D()
        {
            var sphere = new SphereFunction(2);
            Point[] points = new Point[]
            {
                Point.Create(sphere.Evaluate, 1.0, 1.0),
                Point.Create(sphere.Evaluate, 3.0, 3.0),
                Point.Create(sphere.Evaluate, 5.0, 5.0)
            };
            double gamma = 0.5;
            var simplex = new Simplex(points);
            double[] centroid = simplex.GetCentroidCoordinates();
            Point reflected = Point.Create(sphere.Evaluate, -1.0, -1.0);

            Point contracted = simplex.ContractOutside(centroid, reflected, sphere.Evaluate, gamma);

            Assert.AreEqual(0.5, contracted[0], 0.0001);
            Assert.AreEqual(0.5, contracted[1], 0.0001);
            Assert.AreEqual(0.5, contracted.Value, 0.0001);
        }
        [TestMethod]
        public void ReduceSphere2D()
        {
            var sphere = new SphereFunction(2);
            Point[] points = new Point[]
            {
                Point.Create(sphere.Evaluate, 1.0, 1.0),
                Point.Create(sphere.Evaluate, 3.0, 3.0),
                Point.Create(sphere.Evaluate, 5.0, 5.0)
            };
            double sigma = 0.5;
            var simplex = new Simplex(points);

            simplex.Reduce(sphere.Evaluate, sigma);

            Assert.AreEqual(1.0, simplex.Best[0]);
            Assert.AreEqual(1.0, simplex.Best[1]);
            Assert.AreEqual(2.0, simplex[1][0], 0.0001);
            Assert.AreEqual(2.0, simplex[1][1], 0.0001);
            Assert.AreEqual(3.0, simplex.Worst[0], 0.0001);
            Assert.AreEqual(3.0, simplex.Worst[1], 0.0001);
        }
        [TestMethod]
        public void ReplaceWorstSphere2D()
        {
            var sphere = new SphereFunction(2);
            Point[] points = new Point[]
            {
                Point.Create(sphere.Evaluate, 1.0, 1.0),
                Point.Create(sphere.Evaluate, 2.0, 2.0),
                Point.Create(sphere.Evaluate, 5.0, 5.0)
            };
            var simplex = new Simplex(points);
            Point newPoint = Point.Create(sphere.Evaluate, 3.0, 3.0);

            simplex.ReplaceWorst(newPoint);

            Assert.AreEqual(2, simplex.Best.Value);
            Assert.AreEqual(8, simplex[1].Value);
            Assert.AreEqual(18, simplex.Worst.Value);
        }
        [TestMethod]
        public void IsConvergedTrue()
        {
            var sphere = new SphereFunction(2);
            var points = new Point[]
            {
                Point.Create(sphere.Evaluate, 0, 0),
                Point.Create(sphere.Evaluate, 0.0001, 0),
                Point.Create(sphere.Evaluate, 0, 0.0001)
            };
            var simplex = new Simplex(points);
            double tolerance = 1e-6;

            bool converged = simplex.IsConverged(tolerance);

            Assert.IsTrue(converged);
        }
        [TestMethod]
        public void IsConvergedFalse()
        {
            var sphere = new SphereFunction(2);
            var points = new Point[]
            {
                Point.Create(sphere.Evaluate, 0, 0),
                Point.Create(sphere.Evaluate, 1, 0),
                Point.Create(sphere.Evaluate, 0, 1)
            };
            var simplex = new Simplex(points);
            double tolerance = 1e-6;

            bool converged = simplex.IsConverged(tolerance);

            Assert.IsFalse(converged);
        }
    }
}
