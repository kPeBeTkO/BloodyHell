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
        

        public PongBot( Vector start, Vector end)
        {
            var random = new Random();

            HitRange = 1;
            Reward = 100;
            Attackable = true;
            Location = new Vector(random.Next((int)start.X, (int)end.X - 2) + 0.6f, random.Next((int)start.Y, (int)end.Y - 2) + 0.6f);
            Velocity = new Vector(Speed, 0).Rotate(Math.PI * 2 * random.Next(0, 100) / 100);
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
            var delta = 0.5f;

            var locationTillEnd = new Vector(Location.X + delta, Location.Y + delta);
            var locationTillStart = new Vector(Location.X - delta, Location.Y - delta);
            //какая то залупа получилась если честно
            float randVelocityX = 0 < Velocity.X ? random.Next(0, (int)Velocity.X) : random.Next((int)Velocity.X, 0);
            float randVelocityY = 0 < Velocity.Y ? random.Next(0, (int)Velocity.Y) : random.Next((int)Velocity.Y, 0);

            if (randVelocityY == 0)
                randVelocityY += random.Next(-3, 2) + 0.5f;
            else if (randVelocityX == 0)
                randVelocityX += random.Next(-3, 2) + 0.5f;

            IsTarget = !(locationTillEnd.X >= end.X || locationTillStart.X <= start.X 
                || locationTillEnd.Y >= end.Y || locationTillStart.Y <= start.Y);

            if (!IsTarget && (locationTillEnd.X >= end.X || locationTillStart.X <= start.X))
                Velocity = new Vector(-Velocity.X, randVelocityY);

            if (!IsTarget && (locationTillEnd.Y >= end.Y || locationTillStart.Y <= start.Y))
                Velocity = new Vector(randVelocityX, -Velocity.Y);

            Location += Velocity * (timeElapsed / 1000.0f);

            GoToPlayer(player, timeElapsed);
        }
    }
}
