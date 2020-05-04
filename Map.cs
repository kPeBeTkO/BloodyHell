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
        public List<Wall> Walls;
        public Map(){ }

        public Map(string fileName)
        {
            ReadFromFile(fileName);
            CreateWallsList();
        }

        public void CreateWallsList()
        {
            Walls = new List<Wall>();
            var defaultWalls = new List<Wall>();
            defaultWalls.Add(new Wall(0, 0, 0, ChunkSize));
            defaultWalls.Add(new Wall(0, ChunkSize, ChunkSize, ChunkSize));
            defaultWalls.Add(new Wall(ChunkSize, ChunkSize, ChunkSize, 0));
            defaultWalls.Add(new Wall(0, 0, ChunkSize, 0));
            for (var i = 0; i < Heigth; i++)
                for (var j = 0; j < Width; j++)
                {
                    if (Chunks[j,i].Type == ChunkType.Wall)
                    {
                        foreach (var wall in defaultWalls)
                            Walls.Add(new Wall(wall, new Vector(j * ChunkSize, i * ChunkSize)));
                    }
                }
        }

        public void ReadFromFile(string fileName)
        {
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
                    Chunks[j, i] = textureDictionary[line[j]];
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
