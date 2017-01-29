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

namespace Platformer
{
    class Tile : GameObject
    {
        public const int size = 64;

        public static Texture2D BlockSheet, PlatformSheet, EmptyBlock;

        public Point position;
        public Point sourcePos;

        public bool isBlock;
        public bool isEmpty;

        public static int Size { get { return (int)(size); } }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)(position.X), (int)(position.Y), (int)(size), (int)(size));
            }
        }

        public Rectangle SourceBounds
        {
            get
            {
                return new Rectangle(sourcePos.X, sourcePos.Y, size, isBlock ? size : size / 4 * 3);
            }
        }

        public Tile(Point position, int sourcePos, bool isBlock)
        {
            float scale = Game1.BackBufferWidth / 20;
            this.position = new Point((int)(position.X * scale), (int)(position.Y * scale));
            this.isBlock = isBlock;

            if (sourcePos >= 0)
                this.sourcePos = new Point(Game1.Instance.rand.Next(5) * 64, sourcePos);
            else
                isEmpty = true;
        }

        public override void Update(List<GameObject> objects)
        {
            
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(isBlock ? BlockSheet : isEmpty ? EmptyBlock : PlatformSheet, Bounds, SourceBounds, Color.White);
        }
    }
}
