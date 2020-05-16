using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace BloodyHell
{
    public class TextureHolder
    {
        public PlayerTextureHolder Player = new PlayerTextureHolder();
        public EnemyTextureHolder Enemies = new EnemyTextureHolder();
        public static List<Bitmap> GetImageFromFolder(string path)
        {
            var result = new List<Bitmap>();
            if (Directory.Exists(path))
                foreach (var file in Directory.GetFiles(path))
                    result.Add(new Bitmap(file));
            return result;
        }
    }

    public class PlayerTextureHolder 
    {
        public List<Bitmap> Walk;
        public List<Bitmap> Attack;
        public List<Bitmap> Dash;
        public List<Bitmap> Dead;
        public PlayerTextureHolder()
        {
            Walk = TextureHolder.GetImageFromFolder("Textures/Player/Walk");
            Attack = TextureHolder.GetImageFromFolder("Textures/Player/Attack");
            Dash = TextureHolder.GetImageFromFolder("Textures/Player/Dash");
            Dead = TextureHolder.GetImageFromFolder("Textures/Player/Dead");
        }
    }

    public class EnemyTextureHolder
    {
        public List<Bitmap> PongBot;
        public List<Bitmap> ChakramBot;
        public EnemyTextureHolder()
        {
            PongBot = TextureHolder.GetImageFromFolder("Textures/Enemies/PongBot");
            ChakramBot = TextureHolder.GetImageFromFolder("Textures/Enemies/ChakramBot");
        }
    }
}
