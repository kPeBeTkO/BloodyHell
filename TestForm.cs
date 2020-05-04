using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BloodyHell
{
    public class TestForm : Form
    {
        public TestForm()
        {
            var map = new Map("TestLevel");
            var mapImage = map.GetMapImage();
            Paint += (sender, args) => args.Graphics.DrawImage(mapImage, 0, 0);
            Invalidate();
        }
    }
}
