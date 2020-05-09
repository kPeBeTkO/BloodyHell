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
        public List<Monster> Monsters;
        public Level(string levelName)
        {
            LevelName = levelName;
            Monsters = new List<Monster>();
            LoadFromFile();
            Map = new Map(levelName);
        }
        public Level(Map map, Player player, List<Monster> monsters)
        {
            Map = map;
            Player = player;
            Monsters = monsters;
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
                        Monsters.Add(new Monster(new Vector(int.Parse(line[1]), int.Parse(line[2])),
                                                 new Vector(int.Parse(line[3]), int.Parse(line[4])),
                                                 new Vector(int.Parse(line[5]), int.Parse(line[6]))));
                        break;
                }
            }
        }

        public void KillMonsterOrPlayer()
        {
            foreach(var monster in Monsters.Where(x => x.Alive))
            {
                var distance = monster.Location - Player.Location;
                if (distance.Length <= Monster.HitRange)
                {
                    Player.Alive = false;
                }
                if (distance.Length <= Player.HitRange && Player.Attacing && Player.Direction.AngleBetwen(distance) < Math.PI / 4)
                {
                    monster.Alive = false;
                }
            }
        }

        public void Update(long timeElapsed)
        {
            Player.MakeTurn(timeElapsed, Map.Walls);
            foreach (var monster in Monsters)
            {
                monster.MakeTurn(timeElapsed, Player);
            }
            KillMonsterOrPlayer();
        }
    }
}
