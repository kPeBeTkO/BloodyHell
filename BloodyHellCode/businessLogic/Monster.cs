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
        public Vector Location;
        public Vector Velosity;
        //public float Width;
        //public float Heigth;
        public Vector Start;
        public Vector End;
        public bool IsTarget;
        public bool IsKill = false;

        public Monster(Vector vector, Vector start, Vector end)
        {
            Location = vector;
            Velosity = new Vector(1f, 1f);
            Start = start;
            End = end;
        }

        public void IsPlayer(Player player, long timeElapsed)
        {
            var linePlayer = Location - player.Location;

            if (linePlayer.Length <= 3f && IsTarget)
            {
                Velosity = linePlayer.Normalize() * -2f;
            }
        }
        public void IsWall()
        {
            if (Location.X > End.Y || Location.X < Start.X || Location.Y > End.X || Location.Y < Start.Y)
            {
                IsTarget = false;
            }
            else
            {
                IsTarget = true;
            }
        }

        public void MakeTurn(long timeElapsed, Player player)
        {
            var random = new Random();

            if (!IsTarget && Location.X > End.X)
                Velosity = new Vector(-Velosity.X, Velosity.Y);
            else if (!IsTarget && Location.X < Start.X)
                Velosity = new Vector(-Velosity.X, Velosity.Y);

            if (!IsTarget && Location.Y > End.Y)
                Velosity = new Vector(Velosity.X, -Velosity.Y);
            else if (!IsTarget && Location.Y < Start.Y)
                Velosity = new Vector(Velosity.X, -Velosity.Y);

            Location += Velosity * (timeElapsed / 1000.0f);

            IsWall();
            IsPlayer(player, timeElapsed);
        }
    }
}
