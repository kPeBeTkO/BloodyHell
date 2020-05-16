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
        public const float Size = 0.5f;
        public const float Speed = 3;
        public const float ViewDistance = 3;
        private Vector start;
        private Vector end;
        public bool PlayerInBounds = false;
        private Random random = new Random();

        public PongBot( Vector start, Vector end)
        {
            HitRange = 1;
            Reward = 33;
            Attackable = true;
            Location = new Vector(random.Next((int)start.X, (int)end.X) + 0.5f, random.Next((int)start.Y, (int)end.Y) + 0.5f);
            Velocity = new Vector(Speed, 0).Rotate(Math.PI / 2 * random.Next(0, 4) + Math.PI / 4);
            this.start = start;
            this.end = end;
        }

        public void GoToPlayer(Player player, long timeElapsed)
        {
            if (!player.Alive)
                return;

            var linePlayer = player.Location - Location;

            if (linePlayer.Length <= ViewDistance && PlayerInBounds)
            {
                Velocity = linePlayer.Normalize() * Speed;
            }
            else if (Math.Abs(Math.Abs(Velocity.X) - Math.Abs(Velocity.Y)) > 0.01)
            {

                Velocity = new Vector(Speed, 0).Rotate(Math.PI / 2 * random.Next(0, 4) + Math.PI / 4);
            }
        }

        public override void MakeTurn(long timeElapsed, Player player, List<Square> walls)
        {
            if (!Alive)
                return;
            PlayerInBounds = player.Location >= start && player.Location <= end;
            GoToPlayer(player, timeElapsed);
            var futureLocation = Location + Velocity * (timeElapsed / 1000.0f);
            if ((futureLocation.X >= end.X - Size || futureLocation.X <= start.X + Size))
                Velocity = new Vector(-Velocity.X, Velocity.Y);
            if ((futureLocation.Y >= end.Y - Size || futureLocation.Y <= start.Y + Size))
                Velocity = new Vector(Velocity.X, -Velocity.Y);
            Location += Velocity * (timeElapsed / 1000.0f);
        }
    }
}
