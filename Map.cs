using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using RayCasting;

namespace BloodyHell
{
    public enum ChunkType
    {
        None,
        Wall,
        Empty
    }

    public class MapChunk
    {
        public ChunkType Type;
        public Bitmap Texture;
        public MapChunk(ChunkType type, Bitmap texture)
        {
            Type = type;
            Texture = texture;
        }
    }

    

    public class Map
    {
        public int ChunkSize = 32;
        public int Width { get; private set; }
        public int Heigth { get; private set; }
        public MapChunk[,] Chunks;
        public List<Square> Walls;
        public Map(int width, int heigth)
        {
            Width = width;
            Heigth = heigth;
            Chunks = new MapChunk[width, heigth];
            Walls = new List<Square>();
            var wall = new MapChunk(ChunkType.Wall, new Bitmap("Textures\\TestLevel\\stone.jpg"));
            var empty = new MapChunk(ChunkType.Wall, new Bitmap("Textures\\TestLevel\\grass.jpg"));
            var random = new Random();
            for (var i = 0; i < Heigth; i++)
                for (var j = 0; j < Width; j++)
                {
                    if (random.Next(0, 5) == 0)
                    {
                        Chunks[j, i] = wall;
                        Walls.Add(new Square(new Vector(j, i), 1));
                    }
                    else
                        Chunks[j, i] = empty;
                }

        }

        public Map(string fileName)
        {
            ReadFromFile(fileName);
        }

        public void ReadFromFile(string fileName)
        {
            Walls = new List<Square>();
            var path = "Textures\\" + fileName + '\\';
            var file = new StreamReader(path + fileName + ".txt");
            var textureCount = int.Parse(file.ReadLine());
            var textureDictionary = new Dictionary<char, MapChunk>();
            for (var i = 0; i < textureCount; i++)
            {
                var textureInfo = file.ReadLine().Split();
                var type = ChunkType.None;
                if (!Enum.TryParse(textureInfo[1], out  type))
                    throw new Exception();
                textureDictionary[textureInfo[0][0]] = new MapChunk(type, new Bitmap(path + textureInfo[2]));
            }
            var size = file.ReadLine().Split().Select(x => int.Parse(x)).ToArray();
            Width = size[0];
            Heigth = size[1];
            Chunks = new MapChunk[Width, Heigth];
            for (var i = 0; i < Heigth; i++)
            {
                var line = file.ReadLine();
                for (var j = 0; j < Width; j++)
                {
                    Chunks[j, i] = textureDictionary[line[j]];
                    if (Chunks[j, i].Type == ChunkType.Wall)
                        Walls.Add(new Square(new Vector(j, i), 1));
                }
            }
        }

        public Bitmap GetMapImage()
        {
            var image = new Bitmap(Width * ChunkSize, Heigth * ChunkSize);
            var graphics = Graphics.FromImage(image);
            for (var i = 0; i < Heigth; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    graphics.DrawImage(Chunks[j, i].Texture, j * ChunkSize, i * ChunkSize);
                }
            }
            graphics.Dispose();
            return image;
        }
    }
}
