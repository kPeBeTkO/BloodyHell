using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BloodyHell.Entities;
using RayCasting;

namespace BloodyHell
{
    class Level
    {
        public readonly string LevelName;
        public Map Map { get; private set; }
        public List<IEntity> Entities;
        public Player Player { get; private set; }
        public Vector Exit { get; private set; }
        public Level(string levelName)
        {
            LevelName = levelName;
            LoadFromFile();
        }
        public Level(Map map, Player player)
        {
            Map = map;
            Player = player;
        }

        public void LoadFromFile()
        {
            var file = new StreamReader("LevelData/" + LevelName + ".txt");
            var n = int.Parse(file.ReadLine());
            for (var i = 0; i < n; i++)
            {
                var line = file.ReadLine().Split();
                switch(line[0])
                {
                    case "Player":
                        Player = new Player(new Vector(int.Parse(line[1]), int.Parse(line[2])));
                        break;
                    case "Exit":
                        Exit = new Vector(int.Parse(line[1]), int.Parse(line[2]));
                        break;
                    case "Monster":
                        //надо дописать
                        break;
                }
            }
        }

        public void Update(long timeElapsed)
        {
            Player.MakeTurn(timeElapsed, Map.Walls);
        }
    }
}
