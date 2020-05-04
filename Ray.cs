using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace RayCasting
{
    public class Ray
    {
        public Vector Location;
        public Vector Direction;
        public Ray(Vector pos, double angle)
        {
            Location = pos;
            Direction = new Vector(1, 0).Rotate(angle);
            
        }
        public Vector GetIntersectionPoint(Wall wall)
        {
            var x1 = Location.X; var y1 = Location.Y;
            var x2 = (Location + Direction).X; var y2 = (Location + Direction).Y;
            var x3 = wall.Start.X; var y3 = wall.Start.Y;
            var x4 = wall.End.X; var y4 = wall.End.Y;
            var den = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (den == 0)
                return null;
            var t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / den;
            var u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / den;
            if (u >= 0 && u <= 1 && t >= 0)
                return new Vector(x1 + t * (x2 - x1), y1 + t * (y2 - y1));
            return null;
        }

        public Vector GetIntersectionPoint(Vector corner, int size)
        {
            var center = corner + new Vector(size, size) / 2;
            var sector = Math.Floor(((center - Location).Angle + Math.PI / 4) / (Math.PI / 2));
            switch(sector)
            {
                case 0:
                    return GetIntersectionPoint(new Wall(corner + new Vector(size, 0), corner + new Vector(size, size)));
                case 1:
                    return GetIntersectionPoint(new Wall(corner , corner + new Vector(size, 0)));
                case -1:
                    return GetIntersectionPoint(new Wall(corner + new Vector(0, size), corner + new Vector(size, size)));
                default:
                    return GetIntersectionPoint(new Wall(corner , corner + new Vector(0, size)));

            }
            return null;

        }

        public void Rotate(double angle)
        {
            Direction = Direction.Rotate(angle);
        }
    }

    public class Wall
    {
        public Vector Start;
        public Vector End;
        public Wall(int x1, int y1, int x2, int y2)
        {
            Start = new Vector(x1, y1);
            End = new Vector(x2, y2);
        }
        public Wall(Vector a, Vector b)
        {
            Start = a;
            End = b;
        }
        public Wall(Wall wall, Vector offset)
        {
            Start = wall.Start + offset;
            End = wall.End + offset;
        }

        public double GetWallPart(Vector p)
        {
            return (p - Start).Length / (End - Start).Length;
        }
    }
}
