using NUnit.Framework;
using UnityEngine;
using NSubstitute;

namespace Tests
{
    public class BossHornetDataTests
    {
        [Test]
        public void TestBossHornetDataCreation()
        {
            var bossHornetData = new BossHornet(
                hornet: new GameObject("Test"),
                isFacingLeft: true,
                flyingPosition: 1);

            Assert.That(bossHornetData.IsFacingLeft, Is.True);
            Assert.That(bossHornetData.FlyingPosition, Is.EqualTo(1));
            Assert.That(bossHornetData.ReadyToTransition, Is.False);
        }

    }

    public class BossHornetStartWaitTests
    {
        private IBossHornetMovements _context;
        private ITime _time;
        private BossHornetStartWait _bossHornetStartWait;

        [SetUp]
        public void SetUp()
        {
            _context = Substitute.For<IBossHornetMovements>();
            _time = Substitute.For<ITime>();
            _time.RealtimeSinceStartup.Returns(32.0f);
            _bossHornetStartWait = new BossHornetStartWait(time: _time, waitTime: 1.0f, attackType: AttackType.FlyInCircle);
            _bossHornetStartWait.SetContext(_context);
        }

        [Test]
        public void TestThatTransitionIsNotCalledWhenWaitTimeIsNotExceeded()
        {
            _bossHornetStartWait.Handle();
            _context.DidNotReceive().TransitionTo(Arg.Any<BossHornetState>(), Arg.Any<string>());
        }

        [Test]
        public void TestThatTransitionIsCalledWhenWaitTimeisExceeded()
        {
            _time.RealtimeSinceStartup.Returns(34.0f);
            _bossHornetStartWait.Handle();
            _context.Received().TransitionTo(Arg.Any<BossHornetFlyInCircle>(), "IsFlying");

        }
    }

    public class BossHornetFlyInCircleTests
    {
        private IBossHornetMovements _context;
        private ITime _time;
        private BossHornetFlyInCircle _state;

        [SetUp]
        public void SetUp()
        {
            _context = Substitute.For<IBossHornetMovements>();
            _context.CircleRadius.Returns(2.0f);
            _context.CurrentAngularVelocity.Returns(1.0f);
            _time = Substitute.For<ITime>();
            _time.RealtimeSinceStartup.Returns(0.0f);
            _state = new BossHornetFlyInCircle(time: _time, hornetPositionUpdater: new CirclePositionUpdater(), startAngle: 0.0f);
            _state.SetContext(_context);
        }

        [Test]
        public void TestThatStateIsTransitionedCorrectly()
        {
            _context.ActiveBossHornets.Returns(new BossHornet[0]);
            _context.ReadyToStateTransition.Returns(true);
            _state.Handle();
            _context.Received().TransitionTo(Arg.Any<BossHornetAttack>(), "IsAttacking");
        }

        [Test]
        public void TestThatHornetPositionIsNotUpdatedIfMovementStartTimeNotReached()
        {
            _time.RealtimeSinceStartup.Returns(0.0f);
            _state.UpdateHornetPosition(Substitute.For<IBossHornet>());
            var temp = _context.DidNotReceive().CurrentAngularVelocity;
        }


        [Test]
        public void TestThatReadyToTransitionFlagIsSetWhenMaxAngleIsReached()
        {
            _context.CurrentCircleAngle.Returns(Mathf.PI / 2.0f);
            _time.RealtimeSinceStartup.Returns(10.0f);
            var bossHornet = Substitute.For<IBossHornet>();
            _state.UpdateHornetPosition(bossHornet);
            Assert.That(bossHornet.ReadyToTransition, Is.True);
        }

        [Test]
        public void TestThatBossHornetPositionIsUpdatedCorrectly()
        {
            _context.CurrentCircleAngle.Returns(Mathf.PI);
            _time.RealtimeSinceStartup.Returns(2.0f);
            var bossHornet = Substitute.For<IBossHornet>();
            _state.UpdateHornetPosition(bossHornet);
            Assert.That(bossHornet.ReadyToTransition, Is.False);
            Assert.That(bossHornet.Position.x, Is.EqualTo(-0.83).Within(0.01));
            Assert.That(bossHornet.Position.y, Is.EqualTo(1.81).Within(0.01));
        }

        [Test]
        public void TestFlipRequiredReturnsFalseWhenHornetIsInCorrectPosition()
        {
            var bossHornet = Substitute.For<IBossHornet>();
            bossHornet.IsFacingLeft.Returns(false);
            _context.Position.Returns(new Vector3(0f, 0f));
            bossHornet.Position.Returns(new Vector3(-1.0f, 0f));
            Assert.That(_state.FlipRequired(bossHornet), Is.False);
        }

        [Test]
        public void TestFlipRequiredReturnsTrueWhenHornetIsInCorrectPosition()
        {
            var bossHornet = Substitute.For<IBossHornet>();
            bossHornet.IsFacingLeft.Returns(true);
            _context.Position.Returns(new Vector3(0f, 0f));
            bossHornet.Position.Returns(new Vector3(-1.0f, 0f));
            Assert.That(_state.FlipRequired(bossHornet), Is.True);
        }

    }

}


