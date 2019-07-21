using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class Path2DTests
    {
        public const double TestPrecision = 1e-6f;

        public static Path2D CreatePath() => new Path2D
        {
            points = new[]
            {
                new Vector2(0, 0),
                new Vector2(2, 0),
                new Vector2(2, 2),
                new Vector2(0, 2),
            }
        };

        public static Path2D CreateDegeneratePath() => new Path2D
        {
            points = new[]
            {
                new Vector2(0, 0),
                new Vector2(0, 0),
                new Vector2(0, 0),
            }
        };

        public static Path2D CreateSemidegeneratePath() => new Path2D
        {
            points = new[]
            {
                new Vector2(0, 0),
                new Vector2(0, 0),
                new Vector2(2, 0),
            }
        };

        [Test]
        public void NextPoint()
        {
            var path = CreatePath();
            var p = new Path2DParam();

            p = path.NextPoint(p);
            Assert.AreEqual(1.0f, p.t);

            p = path.NextPoint(p);
            Assert.AreEqual(2.0f, p.t);

            p = path.NextPoint(p);
            Assert.AreEqual(3.0f, p.t);

            p = path.NextPoint(p);
            Assert.AreEqual(0.0f, p.t);
        }

        [Test]
        public void Add()
        {
            var path = CreatePath();
            var p = new Path2DParam();

            p = path.Add(p, 0.0f);
            Assert.AreEqual(0.0f, p.t, TestPrecision);

            p = path.Add(p, 0.5f);
            Assert.AreEqual(0.5f, p.t, TestPrecision);

            p = path.Add(p, 3.0f);
            Assert.AreEqual(3.5f, p.t, TestPrecision);

            p = path.Add(p, 0.6f);
            Assert.AreEqual(0.1f, p.t, TestPrecision);
        }

        [Test]
        public void At()
        {
            var path = CreatePath();

            Assert.AreEqual(new Vector2(0, 0), path.At((Path2DParam)0.0f));
            Assert.AreEqual(new Vector2(2, 0), path.At((Path2DParam)1.0f));
            Assert.AreEqual(new Vector2(2, 1), path.At((Path2DParam)1.5f));
        }

        [Test]
        public void NormalAt()
        {
            var path = CreatePath();

            Assert.AreEqual(Vector2.down, path.NormalAt((Path2DParam)0.5f));
            Assert.AreEqual(Vector2.right, path.NormalAt((Path2DParam)1.5f));
            Assert.AreEqual(Vector2.up, path.NormalAt((Path2DParam)2.5f));
            Assert.AreEqual(Vector2.left, path.NormalAt((Path2DParam)3.5f));
        }

        [Test]
        public void Walk()
        {
            var path = CreatePath();
            var p = new Path2DParam();

            p = path.Walk(p, 1);
            Assert.AreEqual(0.5f, p.t, TestPrecision);

            p = path.Walk(p, 1);
            Assert.AreEqual(1.0f, p.t, TestPrecision);

            p = path.Walk(p, 3);
            Assert.AreEqual(2.5f, p.t, TestPrecision);
        }

        [Test]
        public void FindNearest()
        {
            var path = CreatePath();
            Path2DParam p;

            // Position on path point.
            p = path.FindNearest(new Vector2(0, 0));
            Assert.AreEqual(0.0f, p.t, TestPrecision);

            // Position between path points.
            p = path.FindNearest(new Vector2(1, 0));
            Assert.AreEqual(0.5f, p.t, TestPrecision);

            // Position between last and first path points.
            p = path.FindNearest(new Vector2(0, 1));
            Assert.AreEqual(3.5f, p.t, TestPrecision);

            // Point not directly on the path.
            p = path.FindNearest(new Vector2(1, 2.1f));
            Assert.AreEqual(2.5f, p.t, TestPrecision);
        }

        /// <summary>
        /// Reproduce a bug where <see cref="Path2D.FindNearest(Vector2)"/> returned
        /// a path parameter that referred to a path point after the last one.
        /// </summary>
        [Test]
        public void FindNearestReturnsValidPathParam()
        {
            var path = CreatePath();
            var p = path.FindNearest(new Vector2(-1e-7f, 1e-7f));
            var testPosition = path.At(p);
        }

        /// <summary>
        /// Reproduce a bug where <see cref="Path2D.Walk(Path2DParam, float)"/> skipped
        /// a point due to a floating point precision issue.
        /// </summary>
        [Test]
        public void WalkDoesntSkipPoints()
        {
            // Note: float can represent 1.9999999f but will round 2.9999999f to 3.0f.
            var path = CreatePath();
            var p = (Path2DParam)1.9999999f;
            p = path.Walk(p, 0.2f);
            Assert.AreEqual(2.1f, p.t, delta: 1e-6);
        }

        /// <summary>
        /// Reproduce a crash when handling degenerate paths.
        /// </summary>
        [Test]
        public void DegeneratePath()
        {
            // Find point on a degenerate path.
            var path = CreateDegeneratePath();
            var p = path.FindNearest(new Vector2(0, 0));
            Assert.AreEqual(0, p.t);

            // Walk a degenerate path.
            p = path.Walk(p, 1);
            Assert.AreEqual(0, p.t);
        }

        /// <summary>
        /// Paths with duplicate points are handled without problems.
        /// </summary>
        [Test]
        public void SemidegeneratePath()
        {
            var path = CreateSemidegeneratePath();
            Path2DParam p;

            p = path.FindNearest(new Vector2(0, 0));
            Assert.AreEqual(1, p.t);

            p = path.FindNearest(new Vector2(2, 0));
            Assert.AreEqual(2, p.t);

            p = path.Walk(p, 1);
            Assert.AreEqual(2.5f, p.t, TestPrecision);

            p = path.Walk(p, 2);
            Assert.AreEqual(1.5f, p.t, TestPrecision);
        }
    }
}
