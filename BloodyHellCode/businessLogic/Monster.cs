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
        public Vector Start;
        public Vector End;
        public bool IsTarget = false;
        public bool Alive = false;

        public Monster(Vector vector, Vector start, Vector end)
        {
            Location = vector;
            Velosity = new Vector(3, 0).Rotate(Math.PI / 3.92);
            Start = start;
            End = end;
        }

        public void IsPlayer(Player player, long timeElapsed)
        {
            var linePlayer = Location - player.Location;

            if (linePlayer.Length <= 3 && IsTarget)
            {
                Velosity = linePlayer.Normalize() * -3;
            }
        }
        public void IsWall()
        {
            if (Location.X > End.X || Location.X < Start.X || Location.Y > End.Y || Location.Y < Start.Y)
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

            IsWall();
            if (!IsTarget && (Location.X > End.X || Location.X < Start.X))
                Velosity = new Vector(-Velosity.X, Velosity.Y);

            if (!IsTarget && (Location.Y > End.Y || Location.Y < Start.Y))
                Velosity = new Vector(Velosity.X, -Velosity.Y);

            Location += Velosity * (timeElapsed / 1000.0f);

            IsPlayer(player, timeElapsed);
        }
    }
}
