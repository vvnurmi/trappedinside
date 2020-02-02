using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class GeometryTests
    {
        public const double TestPrecision = 1e-6;

        [Test]
        public void ProjectPointOnLine()
        {
            var projection = Geometry.ProjectPointOnLine(
                linePoint: new Vector2(1, 2),
                lineVec: new Vector2(1, 1).normalized,
                point: new Vector2(1, 0));
            AssertEx.AreEqual(new Vector2(0, 1), projection, TestPrecision);
        }

        [Test]
        public void ProjectPointOnLineSegment()
        {
            var linePoint1 = new Vector2(1, 2);
            var linePoint2 = new Vector2(3, 4);
            Vector2 projection;

            // Projection to linePoint1.
            projection = Geometry.ProjectPointOnLineSegment(linePoint1, linePoint2, new Vector2(2, 0));
            AssertEx.AreEqual(new Vector2(1, 2), projection, TestPrecision);

            // Projection onto the line segment.
            projection = Geometry.ProjectPointOnLineSegment(linePoint1, linePoint2, new Vector2(5, 0));
            AssertEx.AreEqual(new Vector2(2, 3), projection, TestPrecision);

            // Projection to linePoint2.
            projection = Geometry.ProjectPointOnLineSegment(linePoint1, linePoint2, new Vector2(8, 0));
            AssertEx.AreEqual(new Vector2(3, 4), projection, TestPrecision);
        }

        [Test]
        public void PointOnWhichSideOfLineSegment()
        {
            var linePoint1 = new Vector2(1, 0);
            var linePoint2 = new Vector2(3, 0);
            int result;

            result = Geometry.PointOnWhichSideOfLineSegment(linePoint1, linePoint2, new Vector2(0, 0));
            Assert.AreEqual(1, result);

            result = Geometry.PointOnWhichSideOfLineSegment(linePoint1, linePoint2, new Vector2(2, 0));
            Assert.AreEqual(0, result);

            result = Geometry.PointOnWhichSideOfLineSegment(linePoint1, linePoint2, new Vector2(4, 0));
            Assert.AreEqual(2, result);
        }
    }
}
