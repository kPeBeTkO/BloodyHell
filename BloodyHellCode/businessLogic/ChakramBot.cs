using RayCasting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodyHell.Entities
{
    class ChakramBot : Enemy
    {
        private const float speed = 2;
        private Vector start;
        private Vector end;

        public ChakramBot(Vector start, Vector end)
        {
            Attackable = false;
            HitRange = 1.5f;
            this.start = start + new Vector(0.5f, 0.5f);
            this.end = end + new Vector(0.5f, 0.5f);
            Location = this.start;
            Velocity = (this.end - this.start).Normalize() * speed;
        }

        public override void MakeTurn(long timeElapsed, Player player, List<Square> walls)
        {
            if (Location >= end)
            {
                Location = end;
                Velocity = (start - end).Normalize() * speed;
            }
            else if (Location <= start)
            {
                Location = start;
                Velocity = (end - start).Normalize() * speed;
            }
            Location += Velocity * (timeElapsed / 1000f);
        }
    }
}
