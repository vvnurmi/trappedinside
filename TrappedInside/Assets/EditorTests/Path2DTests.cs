﻿using NUnit.Framework;
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
    }
}