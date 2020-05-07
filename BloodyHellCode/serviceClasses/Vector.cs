using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCasting
{
	public class Vector
	{
		public Vector(PointF point)
		{
			X = point.X;
			Y = point.Y;
		}
		public Vector(float x, float y)
		{
			X = x;
			Y = y;
		}

		public readonly float X;
		public readonly float Y;
		public float Length { get { return (float)Math.Sqrt(X * X + Y * Y); } }
		public double Angle { get { return Math.Atan2(Y, X); } }
		public static Vector Zero = new Vector(0, 0);

		public override string ToString()
		{
			return string.Format("X: {0}, Y: {1}", X, Y);
		}

		protected bool Equals(Vector other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Vector)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (X.GetHashCode() * 397) ^ Y.GetHashCode();
			}
		}

		public static Vector operator -(Vector a, Vector b)
		{
			return new Vector(a.X - b.X, a.Y - b.Y);
		}

		public static Vector operator *(Vector a, float k)
		{
			return new Vector(a.X * k, a.Y * k);
		}

		public static Vector operator /(Vector a, float k)
		{
			return new Vector(a.X / k, a.Y / k);
		}

		public static Vector operator *(float k, Vector a)
		{
			return a * k;
		}

		public static Vector operator +(Vector a, Vector b)
		{
			return new Vector(a.X + b.X, a.Y + b.Y);
		}

		public float DistanceTo(Vector a)
		{
			return (float)Math.Sqrt((X - a.X) * (X - a.X) + (Y - a.Y) * (Y - a.Y));
		}
		public Vector Normalize()
		{
			return Length > 0 ? this * (float)(1 / Length) : this;
		}

		public Vector Rotate(double angle)
		{
			return new Vector(X * (float)Math.Cos(angle) - Y * (float)Math.Sin(angle), X * (float)Math.Sin(angle) + Y * (float)Math.Cos(angle));
		}

		public Vector BoundTo(Size size)
		{
			return new Vector(Math.Max(0, Math.Min(size.Width, X)), Math.Max(0, Math.Min(size.Height, Y)));
		}

		public PointF ToPoint()
		{
			return new PointF(X, Y);
		}
	}
}
