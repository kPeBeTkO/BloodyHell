using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayCasting;

namespace BloodyHell.Entities
{
    public class PongBot : Enemy
    {
        public const float Speed = 3;
        public const float ViewDistance = 3;
        private Vector start;
        private Vector end;
        public bool IsTarget = false;
        

        public PongBot(Vector vector, Vector start, Vector end)
        {
            HitRange = 1;
            Reward = 100;
            Attackable = true;
            Location = vector;
            Velocity = new Vector(Speed, 0).Rotate(Math.PI / 4);
            this.start = start;
            this.end = end;
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

        public override void MakeTurn(long timeElapsed, Player player, List<Square> walls)
        {
            if (!Alive)
                return;

            var random = new Random();
            float randEndX = random.Next((int)start.X, (int)end.X) + (end.X - start.X) / 2;
            float randEndY = random.Next((int)start.Y, (int)end.Y) + (end.Y - start.Y) / 1.5f;

            IsTarget = !(Location.X >= randEndX || Location.X <= start.X || Location.Y >= randEndY || Location.Y <= start.Y);

            if (!IsTarget && (Location.X >= randEndX || Location.X <= start.X))
                Velocity = new Vector(-Velocity.X, Velocity.Y);

            if (!IsTarget && (Location.Y >= randEndY || Location.Y <= start.Y))
                Velocity = new Vector(Velocity.X, -Velocity.Y);

            Location += Velocity * (timeElapsed / 1000.0f);

            GoToPlayer(player, timeElapsed);
        }
    }
}
