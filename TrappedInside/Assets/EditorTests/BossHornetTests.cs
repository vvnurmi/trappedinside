using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class BossHornetDataTests
    {

        [Test]
        public void TestBossHornetDataCreation()
        {
            var bossHornetData = new BossHornetData(
                hornet: new GameObject("Test"),
                isFacingLeft: true,
                flyingPosition: 1);

            Assert.That(bossHornetData.IsFacingLeft, Is.True);
            Assert.That(bossHornetData.FlyingPosition, Is.EqualTo(1));
            Assert.That(bossHornetData.ReadyToTransition, Is.False);
        }

    }

    public class BossHornetStateTests
    {

    }

}


