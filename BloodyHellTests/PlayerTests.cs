using NUnit.Framework;
using BloodyHell.Entities;
using System.Collections.Generic;
using System;
using RayCasting;

namespace BloodyHellTests
{
    public class PlayerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PlayerDoesntMoveWithZeroInput()
        {
            var player = new Player(new Vector(0, 0));
            player.MakeTurn(1000, new List<Square>());
            Assert.AreEqual(player.Location, new Vector(0, 0));
        }

        [Test]
        public void PlayerMoveWithDefaultSpeed()
        {
            var player = new Player(new Vector(0, 0));
            player.SetVelosity(new Vector(0, 1), new Vector(1, 0));
            player.MakeTurn(1000, new List<Square>());
            Assert.AreEqual(Player.DefaultSpeed, player.Location.X, 0.001);
            Assert.AreEqual(0, player.Location.Y, 0.001);
        }

        [Test]
        public void PlayerDoesnMoveThrowWall()
        {
            var player = new Player(new Vector(0, 0));
            player.SetVelosity(new Vector(0, 1), new Vector(1, 0));
            player.MakeTurn(1000, new List<Square>() {  new Square(new Vector(1, 0), 1)});
            Assert.IsTrue(player.Location.X < 1);
        }

        [Test]
        public void PlayerPopsFromWall()
        {
            var player = new Player(new Vector(0.5f, 0.5f));
            player.MakeTurn(1000, new List<Square>() { new Square(new Vector(0, 0), 1) });
            Assert.IsTrue((int)Math.Floor(player.Location.X) != 0 || (int)Math.Floor(player.Location.Y) != 0);
        }
        [Test]
        public void PlayerDoesnMoveThrowCornerOfWall()
        {
            var player = new Player(new Vector(0, 0));
            player.SetVelosity(new Vector(0, 1), new Vector(1, 1));
            player.MakeTurn(1000, new List<Square>() { new Square(new Vector(1, 1), 1) });
            Assert.IsTrue(player.Location.X < 1 || player.Location.Y < 1);
        }

        [Test]
        public void PlayerDoesnMoveThrowWallCorner()
        {
            var player = new Player(new Vector(0, 0));
            player.SetVelosity(new Vector(0, 1), new Vector(1, 1));
            player.MakeTurn(1000, new List<Square>() { new Square(new Vector(1, 1), 1), new Square(new Vector(0, 1), 1), new Square(new Vector(1, 0), 1) });
            Assert.IsTrue(player.Location.X < 1 && player.Location.Y < 1);
        }

        [Test]
        public void PlayerSlideAlongWall()
        {
            var player = new Player(new Vector(0, 0));
            player.SetVelosity(new Vector(0, 1), new Vector(1, 1));
            player.MakeTurn(1000, new List<Square>() { new Square(new Vector(0, 1), 10) });
            Assert.IsTrue(player.Location.X > 1 && player.Location.Y < 1);
        }

        [Test]
        public void PlayerStopsBeforeWall()
        {
            var player = new Player(new Vector(0, 0.5f));
            player.SetVelosity(new Vector(0, 1), new Vector(1, 0.5f));
            player.MakeTurn(1000, new List<Square>() { new Square(new Vector(1, 0), 1) });
            Assert.AreEqual(1 - Player.Size, player.Location.X, 0.001);
        }

        [Test]
        public void TestDistributeSkills()
        {
            var playerState = new Dictionary<Parameters, int>() 
            { { Parameters.SkillPoints, 2 }, { Parameters.Speed, 5 } };
            var player = new Player(playerState);

            player.DistributeSkills(Parameters.Speed);

            Assert.AreEqual(player.Stats[Parameters.Speed], 6);
            Assert.AreEqual(player.Stats[Parameters.SkillPoints], 1);
        }

        [Test]
        public void TestAddExperience()
        {
            var playerState = new Dictionary<Parameters, int>()
            { { Parameters.Level, 0 }, { Parameters.Experience, 90 }, { Parameters.SkillPoints, 0 } };
            var player = new Player(playerState);

            player.AddExperience(30);

            Assert.AreEqual(player.Stats[Parameters.Level], 1);
            Assert.AreEqual(player.Stats[Parameters.Experience], 20);
            Assert.AreEqual(player.Stats[Parameters.SkillPoints], 1);
        }
    }
}