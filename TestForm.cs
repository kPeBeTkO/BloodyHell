using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using RayCasting;

namespace BloodyHell
{
    public class TestForm : Form
    {
        public TestForm()
        {
            BackColor = Color.Black;
            Width = 600;
            Height = 600;
            DoubleBuffered = true;
            var map = new Map(60, 30);
            var mapImage = map.GetMapImage();
            var camera = new Vector(150, 150);
            var timer = new Timer() { Interval = 20 };
            timer.Tick += (sender, args) => Invalidate();
            timer.Start();
            MouseMove += (sender, args) => camera = new Vector(args.Location);
            Paint += (sender, args) => DrawRayCast(args.Graphics, mapImage, camera, map.Walls, 500);
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
                if ((wall.Location - ray.Location).Length > 320)
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

        private void DrawRayCast(Graphics graphics, Bitmap background, Vector camera, List<Square> walls, int rayCount)
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
                    graphics.DrawLine(pen, camera, a.Item1);
                else
                    graphics.DrawLine(pen, camera, camera + ray.Direction * 320);
                ray.Rotate(Math.PI * 2 / rayCount);
            }
            foreach (var square in HittedWalls)
            {
                graphics.FillRectangle(brush, square);
            }
        }
    }
}
