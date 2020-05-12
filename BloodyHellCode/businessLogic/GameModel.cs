using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodyHell
{
    public enum GameState
    {
        Menu,
        InGame,
        Pause
    }

    class GameModel
    {
        private Stopwatch watch = new Stopwatch();
        private long lastFrame;
        private long frameTime;
        public readonly List<Level> Levels;
        public GameState curentState { get; private set; } = GameState.Menu;
        private int curentLevelID;
        public Level CurentLevel { get { return Levels[curentLevelID]; } }
        public GameModel(List<string> levelNames)
        {
            Levels = new List<Level>();
            foreach (var name in levelNames)
                Levels.Add(new Level(name));
        }

        public void Start()
        {
            if (curentState == GameState.Menu)
            {
                curentLevelID = 0;
                curentState = GameState.InGame;
                watch.Restart();
            }
            else
                throw new Exception("Попытка начать игру вне меню");
        }

        public void Pause()
        {
            switch (curentState)
            {
                case GameState.InGame:
                    curentState = GameState.Pause;
                    watch.Stop();
                    break;
                case GameState.Pause:
                    curentState = GameState.InGame;
                    watch.Start();
                    break;
                default:
                    throw new Exception("попытка зайти в паузу вне игры");
            }
        }

        public bool Update()
        {
            var curent = watch.ElapsedMilliseconds;
            frameTime = curent - lastFrame;
            lastFrame = curent;
            var goToNextLevel = CurentLevel.Update(frameTime);
            if (goToNextLevel)
            {
                watch.Restart();
                if (curentLevelID < Levels.Count - 1)
                {
                    var playerStats = CurentLevel.Player.Stats;
                    curentLevelID++;
                    CurentLevel.Player.Stats = playerStats;
                }
                else
                    curentState = GameState.Menu;

                CurentLevel.Update(frameTime);
            }
            return goToNextLevel;
        }
    }
}
