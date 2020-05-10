using System;
using System.Collections.Generic;
using System.Drawing;
using RayCasting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BloodyHell.Entities;

namespace BloodyHell
{
    public class GameForm : Form
    {
        private GameModel game;
        private Bitmap curentMapImage;
        private Vector mouse;
        private HashSet<Keys> keysPressed = new HashSet<Keys>();
        private Vector userInput
        {
            get
            {
                var result = Vector.Zero;
                foreach (var key in keysPressed)
                    result += directions[key];
                return result.Normalize();
            }
        }
        private readonly Dictionary<Keys, Vector> directions = new Dictionary<Keys, Vector>()
        {
            [Keys.W] = new Vector(0, 1),
            [Keys.S] = new Vector(0, -1),
            [Keys.A] = new Vector(1, 0),
            [Keys.D] = new Vector(-1, 0),
        };
        public GameForm(List<string> levelNames)
        {
            DoubleBuffered = true;
            
            Width = 1280;
            Height = 720;
            game = new GameModel(levelNames);
            var timer = new Timer() { Interval = 10 };
            timer.Start();
            timer.Tick += (sender, args) => 
            {
                if (game.curentState == GameState.InGame)
                {
                    game.CurentLevel.Player.SetVelosity(userInput, mouse);
                    if (game.Update())
                    {
                        curentMapImage = game.CurentLevel.Map.GetMapImage();
                    }
                }
                Invalidate();
            };
            KeyUp += (sender, args) =>
            {
                if (directions.ContainsKey(args.KeyCode))
                    keysPressed.Remove(args.KeyCode);
            };
            PreviewKeyDown += (sender, args) =>
            {
                if (directions.ContainsKey(args.KeyCode))
                    keysPressed.Add(args.KeyCode);
                if (args.KeyCode == Keys.Escape)
                {
                    game.Pause();
                    if (game.curentState == GameState.Pause)
                    {
                        CreateButton();
                    }
                    else
                    {
                        Controls.Clear();
                    }
                }
                if (args.KeyCode == Keys.ShiftKey)
                    game.CurentLevel.Player.Dash();
            };
            MouseDown += (sender, args) =>
            {
                game.CurentLevel.Player.Attack();
            };
            MouseMove += (sender, args) => mouse = new Vector(args.Location) / game.CurentLevel.Map.ChunkSize;
            Paint += (sender, args) =>
            {
                switch(game.curentState)
                {
                    case GameState.Menu:
                        DrawMenu(args.Graphics);
                        break;
                    case GameState.InGame:
                        DrawGame(args.Graphics);
                        break;
                    case GameState.Pause:
                        DrawPause(args.Graphics);
                        break;
                }
            };
        }

        private void CreateButton()
        {
            var speed = new Button()
            {
                Location = new Point(50, 100),
                Text = "Speed: " + game.CurentLevel.Player.State[Parameters.Speed].ToString(),
                BackColor = Color.White
            };
            Controls.Add(speed);

            var countDesh = new Button()
            {
                Location = new Point(150, 100),
                Text = "Dashes: " + game.CurentLevel.Player.State[Parameters.DashCount].ToString(),
                BackColor = Color.White
            };
            Controls.Add(countDesh);
            var restart = new Button()
            {
                Location = new Point(250, 100),
                Text = "Restart",
                BackColor = Color.White
            };
            Controls.Add(restart);
            restart.Click += (sender, args) =>
            {
                game.CurentLevel.Restart();
                Controls.Clear();
                game.Pause();
            };

            speed.Click += (sender, args) =>
            {
                game.CurentLevel.Player.DistributeSkills(Parameters.Speed);
                speed.Text = "Speed: " + game.CurentLevel.Player.State[Parameters.Speed].ToString();
                Controls.Clear();
                Controls.Add(speed);
                Controls.Add(countDesh);
                Controls.Add(restart);
            };

            countDesh.Click += (sender, args) =>
            {
                game.CurentLevel.Player.DistributeSkills(Parameters.DashCount);
                countDesh.Text = "Dashes: " + game.CurentLevel.Player.State[Parameters.DashCount].ToString();
                Controls.Clear();
                Controls.Add(countDesh);
                Controls.Add(speed);
                Controls.Add(restart);
            };
        }

        private void DrawPause(Graphics graphics)
        {
            graphics.DrawString("Вы в павузе)", new Font("arial", 40), Brushes.White, 200, 20);
        }
        private void DrawMenu(Graphics graphics)
        {
            var start = new Button()
            {
                Location = new Point(100, 100),
                Text = "Start"
            };
            Controls.Add(start);
            start.Click += (sender, args) => 
            { 
                game.Start();  
                Controls.Clear(); 
                curentMapImage = game.CurentLevel.Map.GetMapImage(); 
                BackColor = Color.Black; 
            };
        }

        private void DrawGame(Graphics graphics)
        {
            DrawRayCast(graphics, game.CurentLevel, 500);
            DrawExit(graphics, game.CurentLevel.Exit, game.CurentLevel.Map.ChunkSize);
            DrawPlayer(graphics, game.CurentLevel.Player, game.CurentLevel.Map.ChunkSize);
            DrawMonster(graphics, game.CurentLevel.Monsters, game.CurentLevel.Map.ChunkSize);
        }

        private void DrawRayCast(Graphics graphics, Level level, int rayCount)
        {
            var camera = level.Player.Location;
            var walls = level.Map.Walls;
            var chunkSize = level.Map.ChunkSize;
            var ray = new Ray(camera, 0);
            var brush = new TextureBrush(curentMapImage);
            var pen = new Pen(brush, width: 10);
            var HittedWalls = new HashSet<Square>();

            for (var i = 0; i < rayCount; i++)
            {
                var a = ray.FirstIntersectionOfRay(walls);
                HittedWalls.Add(a.Item2);
                if (a.Item1 != null)
                    graphics.DrawLine(pen, camera * chunkSize, a.Item1 * chunkSize);
                else
                    graphics.DrawLine(pen, camera * chunkSize, (camera + ray.Direction * 10) * chunkSize);
                ray.Rotate(Math.PI * 2 / rayCount);
            }
            foreach (var square in HittedWalls)
            {
                if (square != null)
                    graphics.FillRectangle(brush, square.Location.X * chunkSize, square.Location.Y * chunkSize, chunkSize, chunkSize);
            }
        }

        private void DrawExit(Graphics graphics, Vector exit, int chunkSize)
        {
            graphics.FillRectangle(Brushes.GreenYellow, new Square(exit * chunkSize, chunkSize));
        }

        private void DrawPlayer(Graphics graphics, Player player, int chunkSize)
        {
            var camera = player.Location;
            if (player.Attacing)
            {
                //graphics.DrawPie(Pens.Azure, (camera.X - 1) * chunkSize, (camera.Y - 1) * chunkSize, chunkSize * 2, chunkSize * 2, (float)player.Direction.Angle - (float)Math.PI / 4, (float)Math.PI / 2);
                graphics.FillPie(Brushes.Red, (camera.X - 2) * chunkSize, (camera.Y - 2) * chunkSize, chunkSize * 4, chunkSize * 4, (float)(player.Direction.Angle * 180 / Math.PI - 45), 90);
            }
            graphics.FillEllipse(Brushes.Blue, (camera.X - 0.3f) * chunkSize, (camera.Y - 0.3f) * chunkSize, chunkSize * 0.6f, chunkSize * 0.6f);
            graphics.DrawLine(Pens.Silver, camera * chunkSize, (camera + player.Direction) * chunkSize);
        }

        private void DrawMonster(Graphics graphics, List<Monster> monsters, int chunkSize)
        {
            foreach (var monster in monsters)
            {
                var location = monster.Location;
                graphics.FillRectangle(Brushes.Gray, new RectangleF(location.X * chunkSize, location.Y * chunkSize, 0.5f * chunkSize, 0.5f * chunkSize));
            }
        }
    }
}
