using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace Platformer
{
    class Level : IDisposable
    {
        private Tile[,] tiles;

        public ContentManager Content { get { return content; } }
        ContentManager content;

        public int Width { get { return tiles.GetLength(0); } }
        public int Height { get { return tiles.GetLength(1); } }

        public Level(List<GameObject> objects, IServiceProvider serviceProvider, string path)
        {
            content = new ContentManager(serviceProvider, "Content");

            LoadTiles(objects, path);
        }

        private void LoadTiles(List<GameObject> objects, string path)
        {
            int numTilesAcross = 0;
            List<string> lines = new List<string>();

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        numTilesAcross = line.Length;

                        lines.Add(line);
                        int nextLineWidth = line.Length;
                        if (nextLineWidth != numTilesAcross)
                        {
                            throw new Exception(String.Format(
                                "The length of line {0} is different from all preceding lines.",
                                lines.Count));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file {0} could not be read", path);
                Console.WriteLine(e.Message);
            }

            tiles = new Tile[numTilesAcross, lines.Count];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    string currentRow = lines[y];
                    char tileType = currentRow[x];
                    tiles[x, y] = LoadTile(objects, tileType, x, y);

                    objects.Add(tiles[x, y]);
                }
            }
        }

        private Tile LoadTile(List<GameObject> objects, char tileType, int x, int y)
        {
            switch (tileType)
            {
                case 'P':
                    objects.Add(new Player(new Vector2(x, y)));
                    return new Tile(new Point(x, y), -1, false);

                case '.':
                    return new Tile(new Point(x, y), -1, false);

                case 'B':
                    return new Tile(new Point(x, y), 16, false);
                case 'G':
                    return new Tile(new Point(x, y), 80, false);
                case 'O':
                    return new Tile(new Point(x, y), 144, false);
                case 'R':
                    return new Tile(new Point(x, y), 208, false);
                case 'Y':
                    return new Tile(new Point(x, y), 272, false);

                case 'b':
                    return new Tile(new Point(x, y), 0, true);
                case 'g':
                    return new Tile(new Point(x, y), 64, true);
                case 'o':
                    return new Tile(new Point(x, y), 128, true);
                case 'r':
                    return new Tile(new Point(x, y), 192, true);
                case 'y':
                    return new Tile(new Point(x, y), 256, true);

                default:
                    throw new NotSupportedException(String.Format(
                        "Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        public void Dispose()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    tiles[x, y].Destroy();
                }
            }

            Content.Unload();
        }
    }
}
