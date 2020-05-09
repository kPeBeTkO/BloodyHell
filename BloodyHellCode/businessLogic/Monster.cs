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
        public const float HitRange = 1;
        public const float Speed = 3;
        public const float ViewDistance = 3;
        public Vector Location;
        public Vector Velosity;
        public Vector Start;
        public Vector End;
        public bool IsTarget = false;
        public bool Alive = true;
        

        public Monster(Vector vector, Vector start, Vector end)
        {
            Location = vector;
            Velosity = new Vector(Speed, 0).Rotate(Math.PI / 4);
            Start = start;
            End = end;
        }

        public void GoToPlayer(Player player, long timeElapsed)
        {
            if (!player.Alive)
                return;
            var linePlayer = player.Location - Location;

            if (linePlayer.Length <= ViewDistance && IsTarget)
            {
                Velosity = linePlayer.Normalize() * Speed;
            }
        }
        public void CheckInsideBounds()
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
            if (!Alive)
                return;
            var random = new Random();

            CheckInsideBounds();
            if (!IsTarget && (Location.X > End.X || Location.X < Start.X))
                Velosity = new Vector(-Velosity.X, Velosity.Y);

            if (!IsTarget && (Location.Y > End.Y || Location.Y < Start.Y))
                Velosity = new Vector(Velosity.X, -Velosity.Y);

            Location += Velosity * (timeElapsed / 1000.0f);

            GoToPlayer(player, timeElapsed);
        }
    }
}
