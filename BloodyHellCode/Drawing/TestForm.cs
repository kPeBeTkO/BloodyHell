using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using RayCasting;
using System.Diagnostics;
using BloodyHell.Entities;

namespace BloodyHell
{
    public class TestForm : Form
    {
        private long lastFrame;
        private long frameTime;
        private Bitmap curentMapImage;
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
        private Vector mouse;
        private HashSet<Keys> keysPressed = new HashSet<Keys>();
        private readonly Dictionary<Keys, Vector> directions = new Dictionary<Keys, Vector>() 
        {
            [Keys.W] = new Vector(0,1),
            [Keys.S] = new Vector(0, -1),
            [Keys.A] = new Vector(1, 0),
            [Keys.D] = new Vector(-1, 0),
        };
        public TestForm()
        {
            BackColor = Color.Black;
            Width = 600;
            Height = 600;
            DoubleBuffered = true;
            var map = new Map("TestLevel");
            curentMapImage = map.GetMapImage();
            mouse = new Vector(5, 5);
            var watch = new Stopwatch();
            watch.Start();
            var timer = new Timer() { Interval = 10 };
            var level = new Level(map, new Player(mouse));
            timer.Tick += (sender, args) =>
            {
                level.Player.SetVelosity(userInput, mouse);
                var curent = watch.ElapsedMilliseconds;
                frameTime = curent - lastFrame;
                lastFrame = curent;
                level.Update(frameTime);
                Invalidate();
            };
            timer.Start();
            KeyUp += (sender, args) =>
            {
                if (directions.ContainsKey(args.KeyCode))
                    keysPressed.Remove(args.KeyCode);
            };
            PreviewKeyDown += (sender, args) =>
            {
                if (directions.ContainsKey(args.KeyCode))
                    keysPressed.Add(args.KeyCode);
            };
            MouseMove += (sender, args) => mouse = new Vector(args.Location) / map.ChunkSize;
            Paint += (sender, args) => DrawRayCast(args.Graphics,  level, 500);
            Paint += (sender, args) =>
            {
                args.Graphics.DrawString((1000.0 / frameTime).ToString(), new Font("arial", 10), Brushes.Red, 0, 0);
            };
        }

        private void DrawRayCast(Graphics graphics,  Level level, int rayCount)
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
            graphics.FillEllipse(Brushes.Red, (camera.X - 0.3f) * chunkSize, (camera.Y - 0.3f) * chunkSize, chunkSize * 0.6f , chunkSize * 0.6f);
            graphics.DrawLine(Pens.Silver, camera * chunkSize, (camera + level.Player.Direction) * chunkSize);
        }
    }
}
