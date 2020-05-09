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
            var timer = new Timer() { Interval = 20 };
            timer.Start();
            timer.Tick += (sender, args) => 
            {
                if (game.curentState == GameState.InGame)
                {
                    game.CurentLevel.Player.SetVelosity(userInput, mouse);
                    game.Update();
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
                BackColor = Color.Blue
            };
            Controls.Add(speed);

            var countDesh = new Button()
            {
                Location = new Point(150, 100),
                Text = "Count Desh: " + game.CurentLevel.Player.State[Parameters.DashCount].ToString(),
                BackColor = Color.Blue
            };
            Controls.Add(countDesh);

            speed.Click += (sender, args) =>
            {
                game.CurentLevel.Player.DistributeSkills(Parameters.Speed);
                Controls.Clear();
                Controls.Add(speed);
                Controls.Add(countDesh);
            };

            countDesh.Click += (sender, args) =>
            {
                game.CurentLevel.Player.DistributeSkills(Parameters.DashCount);
                Controls.Clear();
                Controls.Add(countDesh);
                Controls.Add(speed);
            };
        }

        private void DrawPause(Graphics graphics)
        {
            graphics.DrawString("Вы в павузе)", new Font("arial", 40), Brushes.White, 100, 100);
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
            DrawRayCast(graphics, game.CurentLevel, 1000);
        }

        private void DrawRayCast(Graphics graphics, Level level, int rayCount)
        {
            var camera = level.Player.Location;
            var walls = level.Map.Walls;
            var chunkSize = level.Map.ChunkSize;
            var ray = new Ray(camera, 0);
            var brush = new TextureBrush(curentMapImage);
            var pen = new Pen(brush);
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
            graphics.FillEllipse(Brushes.Red, (camera.X - 0.3f) * chunkSize, (camera.Y - 0.3f) * chunkSize, chunkSize * 0.6f, chunkSize * 0.6f);
            graphics.DrawLine(Pens.Silver, camera * chunkSize, (camera + level.Player.Direction) * chunkSize);
        }
    }
}
