using NUnit.Framework;
using System;
using System.Collections.Generic;
using BloodyHell.Entities;
using RayCasting;

namespace BloodyHellTests
{
    public class MonsterTests
    {
        [Test]
        public void OutsideBounce()
        {
            var monster = new Monster(new Vector(100, 1001), new Vector(3,1000), new Vector(120,0));
            monster.IsTarget = true;
            monster.CheckInsideBounds();
            Assert.IsFalse(monster.IsTarget);
        }

        [Test]
        public void InsideBounce()
        {
            var monster = new Monster(new Vector(4,95), new Vector(3,90), new Vector(5,100));
            monster.IsTarget = true;
            monster.CheckInsideBounds();
            Assert.IsTrue(monster.IsTarget);
        }

        [Test]
        public void ToBigX()
        {
            var Monster = new Monster(new Vector(100, 3), new Vector(50, 3), new Vector(60, 3));
            var result = new Vector(-Monster.Velosity.X, Monster.Velosity.Y);
            var player = new Player(new Vector(0, 0));
            Monster.MakeTurn(10, player);
            Assert.AreEqual(Monster.Velosity, result);
        }

        [Test]
        public void ToSmallX()
        {
            var Monster = new Monster(new Vector(10, 3), new Vector(50, 3), new Vector(60, 3));
            var result = new Vector(-Monster.Velosity.X, Monster.Velosity.Y);
            var player = new Player(new Vector(0, 0));
            Monster.MakeTurn(10, player);
            Assert.AreEqual(Monster.Velosity, result);
        }

        [Test]
        public void ToBigY()
        {
            var Monster = new Monster(new Vector(3,100), new Vector(3,50), new Vector(3,60));
            var result = new Vector(Monster.Velosity.X,-Monster.Velosity.Y);
            var player = new Player(new Vector(0, 0));
            Monster.MakeTurn(10, player);
            Assert.AreEqual(Monster.Velosity, result);
        }

        [Test]
        public void ToSmallY()
        {
            var Monster = new Monster(new Vector(3,10), new Vector(3,50), new Vector(3,60));
            var result = new Vector(Monster.Velosity.X,-Monster.Velosity.Y);
            var player = new Player(new Vector(0, 0));
            Monster.MakeTurn(10, player);
            Assert.AreEqual(Monster.Velosity, result);
        }
    }
}
