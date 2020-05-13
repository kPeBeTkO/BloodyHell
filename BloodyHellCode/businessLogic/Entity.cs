using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayCasting;

namespace BloodyHell.Entities
{
    public abstract class Entity
    {
        public Vector Location { get; protected set; }
        public Vector Velocity { get; protected set; }
        public bool Alive = true;

    }
}
