using System;
using System.Collections.Generic;
using System.Drawing;
using RayCasting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BloodyHell.Entities;
using System.Media;

namespace BloodyHell
{
    public class GameForm : Form
    {
        private GameModel game;
        private Bitmap curentMapImage;
        private TextureHolder Textures = new TextureHolder();
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
            SoundPlayer simpleSound = new SoundPlayer("Media/music.wav");
            //simpleSound.PlayLooping();
            SetUserControls();
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

        private void SetUserControls()
        {
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
            MouseMove += (sender, args) => mouse = new Vector(args.Location) - new Vector(Width / 2, Height / 2);
        }

        private void CreateButton()
        {
            var speed = new Button()
            {
                Location = new Point(50, 100),
                Text = "Speed: " + game.CurentLevel.Player.Stats[Parameters.Speed].ToString(),
                BackColor = Color.White,
                Width = 75
            };
            Controls.Add(speed);

            var countDesh = new Button()
            {
                Location = new Point(150, 100),
                Text = "Dashes: " + game.CurentLevel.Player.Stats[Parameters.DashCount].ToString(),
                BackColor = Color.White,
                Width = 75
            };
            Controls.Add(countDesh);

            var restart = new Button()
            {
                Location = new Point(250, 100),
                Text = "Restart",
                BackColor = Color.White,
                Width = 75
            };
            Controls.Add(restart);

            var level = new Label()
            {
                Location = new Point(50, 150),
                Text = "level: " + game.CurentLevel.Player.Stats[Parameters.Level].ToString(),
                BackColor = Color.White,
                Width = 75,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(level);

            var skillPoints = new Label()
            {
                Location = new Point(50, 200),
                Text = "skillPoints: " + game.CurentLevel.Player.Stats[Parameters.SkillPoints].ToString(),
                BackColor = Color.White,
                Width = 75,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(skillPoints);

            var experience = new Label()
            {
                Location = new Point(50, 250),
                Text = "experience: " + game.CurentLevel.Player.Stats[Parameters.Experience].ToString(),
                BackColor = Color.White,
                Width = 75,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(experience);

            restart.Click += (sender, args) =>
            {
                game.CurentLevel.Restart();
                Controls.Clear();
                game.Pause();
            };

            speed.Click += (sender, args) =>
            {
                game.CurentLevel.Player.DistributeSkills(Parameters.Speed);
                speed.Text = "speed: " + game.CurentLevel.Player.Stats[Parameters.Speed].ToString();
                Controls.Clear();
                Controls.Add(speed);
                Controls.Add(countDesh);
                Controls.Add(restart);
                Controls.Add(level);
                Controls.Add(skillPoints);
                Controls.Add(experience);
            };

            countDesh.Click += (sender, args) =>
            {
                game.CurentLevel.Player.DistributeSkills(Parameters.DashCount);
                countDesh.Text = "dashes: " + game.CurentLevel.Player.Stats[Parameters.DashCount].ToString();
                Controls.Clear();
                Controls.Add(countDesh);
                Controls.Add(speed);
                Controls.Add(restart);
                Controls.Add(level);
                Controls.Add(skillPoints);
                Controls.Add(experience);
            };
        }

        private void DrawPause(Graphics graphics)
        {
            graphics.DrawString("Вы в павузе)", new Font("arial", 40), Brushes.White, 40, 20);
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
            var size = game.CurentLevel.Map.ChunkSize;
            var camera = game.CurentLevel.Player.Location;
            var height = Height / (float)Width * 20;
            graphics.TranslateTransform(-(camera.X - 10) * (float)Width / 20, -(camera.Y - height / 2) * (float)Height / height);
            graphics.ScaleTransform((float)Width / (20 * size), (float)Height / (height * size));
            DrawRayCast(graphics, game.CurentLevel, 500, 7);
            //DrawExit(graphics, game.CurentLevel.Exit, size);
            DrawMonster(graphics, game.CurentLevel.Enemies, size, 7);
            DrawPlayer(graphics, game.CurentLevel.Player, size);
        }

        private void DrawRayCast(Graphics graphics, Level level, int rayCount, float viewDistance)
        {
            var camera = level.Player.Location;
            var walls = level.Map.Walls;
            var chunkSize = level.Map.ChunkSize;
            var ray = new Ray(camera, 0);
            var brush = new TextureBrush(curentMapImage);
            var hittedWalls = new HashSet<Square>();
            var points = new List<Vector>();
            for (var i = 0; i < rayCount; i++)
            {
                var a = ray.FirstIntersectionOfRay(walls, viewDistance);
                hittedWalls.Add(a.Item2);
                if (a.Item1 != null && camera.DistanceTo(a.Item1) < viewDistance)
                    points.Add(a.Item1 * chunkSize);
                else
                    points.Add((camera + ray.Direction * viewDistance) * chunkSize);
                ray.Rotate(Math.PI * 2 / rayCount);
            }
            graphics.FillPolygon(brush, points.Select(x => x.ToPoint()).ToArray());
            foreach (var square in hittedWalls)
            {
                if (square != null)
                    graphics.FillRectangle(brush, square.Location.X * chunkSize, square.Location.Y * chunkSize, chunkSize, chunkSize);
            }
        }

        private void DrawExit(Graphics graphics, Vector exit, int chunkSize)
        {
            graphics.FillRectangle(Brushes.GreenYellow, new Square(exit * chunkSize, chunkSize));
        }

        private void DrawPlayer(Graphics graphics, Player player, int size)
        {
            //нужно будет сделать свитч по состояниям игрока, а потом и по спрайту
            var playerImage = Textures.Player.Walk[0];
            var camera = player.Location;
            if (player.Attacing)
            {
                graphics.FillPie(Brushes.Red, (camera.X - 2) * size, (camera.Y - 2) * size, size * 4, size * 4, (float)(player.Direction.Angle * 180 / Math.PI - 45), 90);
                playerImage = Textures.Player.Attack[0];
            }
            var height = Height / (float)Width * 20;
            //как же тяжко вращать картинку :(
            graphics.ResetTransform();
            graphics.TranslateTransform(Width / 2, Height / 2);
            graphics.RotateTransform((float)(player.Direction.Angle * 180 / Math.PI + 90 ));
            graphics.ScaleTransform((float)Width / (20 * size) * Player.Size * 2 * (float)size / playerImage.Width, Height / (height * size) * Player.Size * 2 * (float)size / playerImage.Height);
            graphics.DrawImage(playerImage, -playerImage.Width / 2, -playerImage.Height / 2);
            graphics.ResetTransform();
        }

        private void DrawMonster(Graphics graphics, List<Enemy> monsters, int chunkSize, float viewDistance)
        {
            var camera = game.CurentLevel.Player.Location;
            var walls = game.CurentLevel.Map.Walls;
            foreach (var monster in monsters)
            {
                var location = monster.Location;
                var firstWall = new Ray(camera, location - camera).FirstIntersectionOfRay(walls, viewDistance).Item1;
                if (firstWall != null && location.DistanceTo(camera) < firstWall.DistanceTo(camera) || firstWall == null && location.DistanceTo(camera) < viewDistance)
                {
                    var image = GetEnemyImage(monster);
                    graphics.DrawImage(image, (location.X - 0.5f) * chunkSize, (location.Y - 0.5f) * chunkSize);

                    if (monster.Alive)
                        graphics.DrawEllipse(Pens.Red, (location.X - monster.HitRange) * chunkSize, (location.Y - monster.HitRange) * chunkSize, monster.HitRange * 2 * chunkSize, monster.HitRange * 2 * chunkSize);
                }
            }
        }

        private Bitmap GetEnemyImage(Enemy enemy)
        {
            if (enemy is ChakramBot)
                return Textures.Enemies.ChakramBot[0];
            if (enemy is PongBot)
            {
                if (!enemy.Alive)
                    return Textures.Enemies.PongBot[0];

                var picture = Textures.Enemies.PongBot[enemy.countFrame];

                enemy.countFrame++;
                if (enemy.countFrame == 2)
                    enemy.countFrame = 0;

                return picture;
            }
            return null;
        }
    }
}
