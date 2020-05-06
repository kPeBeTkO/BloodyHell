using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using RayCasting;
using System.Diagnostics;

namespace BloodyHell
{
    public class TestForm : Form
    {
        private long lastFrame;
        public TestForm()
        {
            BackColor = Color.Black;
            Width = 600;
            Height = 600;
            DoubleBuffered = true;
            var map = new Map("TestLevel");
            var mapImage = map.GetMapImage();
            var camera = new Vector(150, 150);
            var timer = new Timer() { Interval = 10 };
            timer.Tick += (sender, args) => Invalidate();
            timer.Start();
            var watch = new Stopwatch();
            watch.Start();
            MouseMove += (sender, args) => camera = new Vector(args.Location) / map.ChunkSize;
            Paint += (sender, args) => DrawRayCast(args.Graphics, mapImage, camera, map.Walls, 500, map.ChunkSize);
            Paint += (sender, args) =>
            {
                var curent = watch.ElapsedMilliseconds;
                var frameTime = curent - lastFrame;
                args.Graphics.DrawString((1000 / frameTime).ToString(), new Font("arial", 10), Brushes.Red, 0, 0);
                lastFrame = curent;
            };
            Invalidate();
        }

        private Tuple<Vector,Square> FirstIntersectionOfRay(Ray ray, List<Square> walls)
        {
            if (walls.Count == 0)
                return null;
            Vector closestPoint = null;
            Square closestWall = null;
            foreach(var wall in walls)
            {
                if ((wall.Center - ray.Location).Length > 10)
                    continue;
                var point = ray.GetIntersectionPoint(wall);
                if (closestPoint == null ||
                    (point != null && (point - ray.Location).Length < (closestPoint - ray.Location).Length))
                {
                    closestPoint = point;
                    closestWall = wall;
                }
            }
            return Tuple.Create(closestPoint, closestWall);
        }

        private void DrawRayCast(Graphics graphics, Bitmap background, Vector camera, List<Square> walls, int rayCount, int chunkSize)
        {
            var ray = new Ray(camera, 0);
            var brush = new TextureBrush(background);
            var pen = new Pen(brush);
            var HittedWalls = new HashSet<Square>();
            for (var i = 0; i < rayCount; i++)
            {
                var a = FirstIntersectionOfRay(ray, walls);
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
    }
}
