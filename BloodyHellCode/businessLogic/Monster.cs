using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayCasting;

namespace BloodyHell.Entities
{
    public class Monster : IEntity
    {
        public Vector location;
        public Vector Velosity;

        public Monster(Vector vector)
        {
            location = vector;
            Velosity = new Vector(1f, 1f);
        }

        public void IsPlayer(Player player)
        {
            var linePlayer = location - player.Location;

            if (linePlayer.Length <= 5f)
            {
                Velosity = new Vector(10f, 10f);
            }
        }

        public void SetVelosityMonster()
        {
            location += Velosity;
        }

        public void IsWall(Map map)
        {
            if (location.X >= map.Width * map.ChunkSize)
                Velosity = new Vector(-Velosity.X, Velosity.Y);
            else if (location.X <= 0)
                Velosity = new Vector(-Velosity.X, Velosity.Y);
            else if (location.Y >= map.Heigth * map.ChunkSize)
                Velosity = new Vector(Velosity.X, -Velosity.Y);
            else if (location.Y <= 0)
                Velosity = new Vector(Velosity.X, -Velosity.Y);
        }

        public void MakeTurn(long timeElapsed)
        {
            throw new NotImplementedException();
        }
    }
}
