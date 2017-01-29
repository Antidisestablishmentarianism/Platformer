using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer
{
    public class Tile : GameObject
    {
        public const int Size = 64;

        public static Texture2D BlockSheet, PlatformSheet, EmptyBlock;

        public Point Position;
        public Point SourcePos;

        public bool IsBlock;
        public bool IsEmpty;

        public Rectangle Bounds => new Rectangle(Position.X, Position.Y, Size, Size);

        private Rectangle SourceBounds => new Rectangle(SourcePos.X, SourcePos.Y, Size, IsBlock ? Size : Size / 4 * 3);

        public Tile(Point position, int sourcePos, bool isBlock)
        {
            const float scale = Game1.BackBufferWidth / (float)20;
            Position = new Point((int)(position.X * scale), (int)(position.Y * scale));
            IsBlock = isBlock;

            if (sourcePos >= 0)
                SourcePos = new Point(Game1.Instance.Rand.Next(5) * 64, sourcePos);
            else
                IsEmpty = true;
        }

        public override void Update(List<GameObject> objects)
        {
            
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(IsBlock ? BlockSheet : IsEmpty ? EmptyBlock : PlatformSheet, Bounds, SourceBounds, Color.White);
        }
    }
}
