using NUnit.Framework;
using System;
using RayCasting;

namespace BloodyHellTests
{
    [TestFixture]
    public class RayCastTests
    {
        [TestCase(15, 5, Math.PI, 0, 0, 10, 10, 5)]
        [TestCase(5, 15, -Math.PI / 2, 0, 0, 10, 5, 10)]
        [TestCase(-5, 5, 0, 0, 0, 10, 0, 5)]
        public static void TestSquareRayCollision(int rayX, int rayY, double alpha,
                                                  int sqrX, int sqrY, int sqrSize,
                                                  float expX, float expY)
        {
            var ray = new Ray(new Vector(rayX, rayY), alpha);
            var square = new Square(new Vector(sqrX, sqrY), sqrSize);
            Assert.AreEqual(new Vector(expX, expY), ray.GetIntersectionPoint(square));
        }
    }
}
