using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayCasting;

namespace BloodyHell.Entities
{
    public class Monster : Entity
    {
        public const float HitRange = 1;
        public const float Speed = 3;
        public const float ViewDistance = 3;
        public Vector Start;
        public Vector End;
        public bool IsTarget = false;
        

        public Monster(Vector vector, Vector start, Vector end)
        {
            Location = vector;
            Velocity = new Vector(Speed, 0).Rotate(Math.PI / 4);
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
                Velocity = linePlayer.Normalize() * Speed;
            }
        }

        public void MakeTurn(long timeElapsed, Player player)
        {
            if (!Alive)
                return;

            var random = new Random();
            float randEndX = random.Next((int)Start.X, (int)End.X) + (End.X - Start.X) / 2;
            float randEndY = random.Next((int)Start.Y, (int)End.Y) + (End.Y - Start.Y) / 1.5f;

            IsTarget = !(Location.X >= randEndX || Location.X <= Start.X || Location.Y >= randEndY || Location.Y <= Start.Y);

            if (!IsTarget && (Location.X >= randEndX || Location.X <= Start.X))
                Velocity = new Vector(-Velocity.X, Velocity.Y);

            if (!IsTarget && (Location.Y >= randEndY || Location.Y <= Start.Y))
                Velocity = new Vector(Velocity.X, -Velocity.Y);

            Location += Velocity * (timeElapsed / 1000.0f);

            GoToPlayer(player, timeElapsed);
        }
    }
}
