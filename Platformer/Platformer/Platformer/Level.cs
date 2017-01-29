using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace Platformer
{
    public class Level : IDisposable
    {
        private Tile[,] _tiles;

        public ContentManager Content { get; }

        public int Width => _tiles.GetLength(0);
        public int Height => _tiles.GetLength(1);

        public Level(ICollection<GameObject> objects, IServiceProvider serviceProvider, string path)
        {
            Content = new ContentManager(serviceProvider, "Content");

            LoadTiles(objects, path);
        }

        private void LoadTiles(ICollection<GameObject> objects, string path)
        {
            var numTilesAcross = 0;
            var lines = new List<string>();

            try
            {
                using (var reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line == null) continue;
                        numTilesAcross = line.Length;

                        lines.Add(line);
                        var nextLineWidth = line.Length;
                        if (nextLineWidth != numTilesAcross)
                        {
                            throw new Exception($"The length of line {lines.Count} is different from all preceding lines.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"The file {path} could not be read");
                Console.WriteLine(e.Message);
            }

            _tiles = new Tile[numTilesAcross, lines.Count];

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var currentRow = lines[y];
                    var tileType = currentRow[x];
                    _tiles[x, y] = LoadTile(objects, tileType, x, y);

                    objects.Add(_tiles[x, y]);
                }
            }
        }

        private static Tile LoadTile(ICollection<GameObject> objects, char tileType, int x, int y)
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
                    throw new NotSupportedException($"Unsupported tile type character '{tileType}' at position {x}, {y}.");
            }
        }

        public void Dispose()
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    _tiles[x, y].Destroy();
                }
            }

            Content.Unload();
        }
    }
}
