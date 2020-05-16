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
        public Player Player { get; private set; }
        public Vector Exit { get; private set; }
        public List<Enemy> Enemies;
        private Random random = new Random();
        public Dictionary<Parameters, int> InitialStats = new Dictionary<Parameters, int>();
        public Level(string levelName)
        {
            LevelName = levelName;
            Enemies = new List<Enemy>();
            LoadFromFile();
            Map = new Map(levelName);
        }
        public Level(Map map, Player player, List<Enemy> monsters)
        {
            Map = map;
            Player = player;
            Enemies = monsters;
        }

        public void Restart()
        {
            Enemies = new List<Enemy>();
            LoadFromFile();
            foreach (var parametr in InitialStats.Keys)
            {
                Player.Stats[parametr] = InitialStats[parametr];
            }
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
                    case "PongBot":
                        Enemies.Add(new PongBot(new Vector(int.Parse(line[1]), int.Parse(line[2])),
                                                 new Vector(int.Parse(line[3]), int.Parse(line[4]))));
                        break;
                    case "ChakramBot":
                        Enemies.Add(new ChakramBot(new Vector(float.Parse(line[1]), float.Parse(line[2])),
                                                 new Vector(float.Parse(line[3]), float.Parse(line[4]))));
                        break;
                }
            }
        }

        public void KillMonsterOrPlayer()
        {
            foreach(var enemy in Enemies.Where(x => x.Alive))
            {
                var distance = enemy.Location - Player.Location;
                if (distance.Length <= enemy.HitRange && !Player.InDash)
                {
                    Player.Alive = false;
                }
                if (enemy.Attackable && distance.Length <= Player.HitRange && 
                    ((Player.Attacing && Player.Direction.AngleBetwen(distance) < Math.PI / 4) || (Player.InDash && distance.Length < Player.DashHitRange)))
                {
                    enemy.Alive = false;
                    Player.AddExperience((int)Math.Round(enemy.Reward * (1 + (random.NextDouble() - 0.5) / 4)));
                }
            }
        }

        public bool Update(long timeElapsed)
        {
            Player.MakeTurn(timeElapsed, Map.Walls);
            foreach (var enemy in Enemies)
            {
                enemy.MakeTurn(timeElapsed, Player, Map.Walls);
            }
            KillMonsterOrPlayer();
            return Player.Location.DistanceTo(Exit + new Vector(0.5f, 0.5f)) < 0.5f;
        }
    }
}
