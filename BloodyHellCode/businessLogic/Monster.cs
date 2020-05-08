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
        public float Width;
        public float Heigth;
        public bool IsTarget;
        public bool IsKill = false;

        public Monster(Vector vector, float Width, float Heigth)
        {
            location = vector;
            Velosity = new Vector(1f, 1f);
            this.Width = Width;
            this.Heigth = Heigth;
        }

        public void IsPlayer(Player player, long timeElapsed)
        {
            var linePlayer = location - player.Location;

            if (linePlayer.Length <= 3f && IsTarget)
            {
                Velosity = linePlayer.Normalize() * -0.03f * (timeElapsed / 15.0f);
            }

            if (linePlayer.Length <= 1f && IsKill)
            {
                player.State[Parameters.Experience] += 50;
            }

            if (linePlayer.Length <= 0.5f && !IsKill)
            {
                player.IsKill = true;
            }
        }

        public void SetVelosityMonster()
        {
            location += Velosity;
        }

        public void IsWall()
        {
            if (location.X > Width || location.X < 0 || location.Y > Heigth || location.Y < 0)
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

            if (!IsTarget && location.X > random.Next(1, (int)Width))
                Velosity = new Vector(-Velosity.X, Velosity.Y); // * (timeElapsed / 15.0f);
            else if (!IsTarget && location.X < 0)
                Velosity = new Vector(-Velosity.X, Velosity.Y); // * (timeElapsed / 15.0f);

            if (!IsTarget && location.Y > random.Next(1, (int)Heigth))
                Velosity = new Vector(Velosity.X, -Velosity.Y); // * (timeElapsed / 15.0f);
            else if (!IsTarget && location.Y < 0)
                Velosity = new Vector(Velosity.X, -Velosity.Y); // * (timeElapsed / 15.0f);

            IsPlayer(player, timeElapsed);
        }
    }
}
