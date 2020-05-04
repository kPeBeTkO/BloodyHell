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
            var map = new Map("TestLevel");
            var mapImage = map.GetMapImage();
            var camera = new Vector(0, 0);
            var timer = new Timer() { Interval = 40 };
            timer.Tick += (sender, args) => Invalidate();
            timer.Start();
            MouseMove += (sender, args) => camera = new Vector(args.Location);
            Paint += (sender, args) => DrawRayCast(args.Graphics, mapImage, camera, map.Walls, 1000);
            Invalidate();
        }

        private Vector FirstIntersectionOfRay(Ray ray, List<Wall> walls)
        {
            if (walls.Count == 0)
                return null;
            var closestPoint = ray.GetIntersectionPoint(walls[0]);
            foreach(var wall in walls)
            {
                var point = ray.GetIntersectionPoint(wall);
                if (closestPoint == null || (point != null && (point - ray.Location).Length < (closestPoint - ray.Location).Length))
                    closestPoint = point;
            }
            return closestPoint;
        }

        private void DrawRayCast(Graphics graphics, Bitmap background, Vector camera, List<Wall> walls, int rayCount)
        {
            var ray = new Ray(camera, 0);
            var pen = new Pen(new TextureBrush(background));
            for (var i = 0; i < rayCount; i++)
            {
                graphics.DrawLine(pen, camera, FirstIntersectionOfRay(ray, walls));
                ray.Rotate(Math.PI * 2 / rayCount);
            }
        }
    }
}
