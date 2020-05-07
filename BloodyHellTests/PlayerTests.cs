using NUnit.Framework;
using BloodyHell.Entities;
using System.Collections.Generic;
using System;

namespace BloodyHellTests
{
    public class PlayerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestDistributeSkills()
        {
            var playerState = new Dictionary<Parameters, int>() 
            { { Parameters.skillPoints, 2 }, { Parameters.Speed, 5 } };
            var player = new Player(playerState);

            player.DistributeSkills(Parameters.Speed);

            Assert.AreEqual(playerState[Parameters.Speed], 10);
            Assert.AreEqual(playerState[Parameters.skillPoints], 1);
        }

        [Test]
        public void TestAddExperience()
        {
            var playerState = new Dictionary<Parameters, int>()
            { { Parameters.Level, 0 }, { Parameters.Experience, 90 }, { Parameters.skillPoints, 0 } };
            var player = new Player(playerState);

            player.AddExperience(30);

            Assert.AreEqual(playerState[Parameters.Level], 1);
            Assert.AreEqual(playerState[Parameters.Experience], 20);
            Assert.AreEqual(playerState[Parameters.skillPoints], 1);
        }
    }
}