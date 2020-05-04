using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCasting
{
    public static class DrawingExtentions
    {
        public static void DrawLine(this Graphics graphics, Pen pen, Vector a, Vector b)
        {
            if (a != null && b != null)
                graphics.DrawLine(pen, a.X, a.Y, b.X, b.Y);
        }
    }
}
