using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;

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
        public Vector GetIntersectionPoint(Line line)
        {
            var x1 = Location.X; var y1 = Location.Y;
            var x2 = (Location + Direction).X; var y2 = (Location + Direction).Y;
            var x3 = line.Start.X; var y3 = line.Start.Y;
            var x4 = line.End.X; var y4 = line.End.Y;
            var den = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (den == 0)
                return null;
            var t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / den;
            var u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / den;
            if (u >= 0 && u <= 1 && t >= 0)
                return new Vector(x1 + t * (x2 - x1), y1 + t * (y2 - y1));
            return null;
        }

        public Vector GetIntersectionPoint(Square square)
        {
            Vector min = null;
            foreach(var line in square)
            {
                var v = GetIntersectionPoint(line);
                if (min == null || (v != null && (min - Location).Length > (v - Location).Length))
                    min = v;
            }
            return min;
        }

        public void Rotate(double angle)
        {
            Direction = Direction.Rotate(angle);
        }

        public Tuple<Vector, Square> FirstIntersectionOfRay(List<Square> walls)
        {
            if (walls.Count == 0)
                return null;
            Vector closestPoint = null;
            Square closestWall = null;
            foreach (var wall in walls)
            {
                if ((wall.Center - Location).Length > 10)
                    continue;
                var point = GetIntersectionPoint(wall);
                if (closestPoint == null ||
                    (point != null && (point - Location).Length < (closestPoint - Location).Length))
                {
                    closestPoint = point;
                    closestWall = wall;
                }
            }
            return Tuple.Create(closestPoint, closestWall);
        }
    }

    public class Square : IEnumerable<Line>
    {
        public Vector Location;
        public int Size;
        public Vector Center { get { return Location + new Vector(Size, Size) / 2; } }
        public Square(Vector location, int size)
        {
            Location = location;
            Size = size;
        }

        public IEnumerator<Line> GetEnumerator()
        {
            yield return new Line(Location + new Vector(Size, 0), Location + new Vector(Size, Size));
            yield return new Line(Location, Location + new Vector(Size, 0));
            yield return new Line(Location + new Vector(0, Size), Location + new Vector(Size, Size));
            yield return new Line(Location, Location + new Vector(0, Size));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Line
    {
        public Vector Start;
        public Vector End;
        public Line(int x1, int y1, int x2, int y2)
        {
            Start = new Vector(x1, y1);
            End = new Vector(x2, y2);
        }
        public Line(Vector a, Vector b)
        {
            Start = a;
            End = b;
        }
        public Line(Line wall, Vector offset)
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
