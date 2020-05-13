using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayCasting;

namespace BloodyHell.Entities
{
    public abstract class Enemy : Entity
    {
        public float HitRange { get; protected set; }
        public bool Attackable { get; protected set; }
        public int Reward { get; protected set; }
        public abstract void MakeTurn(long timeElapsed, Player player, List<Square> walls);
    }
}
