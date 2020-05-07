using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BloodyHell.Entities;

namespace BloodyHell
{
    class Level
    {
        public Map Map;
        public List<IEntity> Entities;
        public Player Player;
        public Level(Map map, Player player)
        {
            Map = map;
            Player = player;
        }

        public void Update(long timeElapsed)
        {
            Player.MakeTurn(timeElapsed, Map.Walls);
        }
    }
}
