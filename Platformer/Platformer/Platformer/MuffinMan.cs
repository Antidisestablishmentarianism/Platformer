using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer
{
    public class MuffinMan : GameObject
    {
        private const float Gravity = 0.3f;
        private const float Speed = 1f;
        private const float AnimationLength = 0.1f;
        private const float AnimationDecrement = 0.05f;

        public Vector2 Position;
        public Vector2 Velocity;

        private static readonly Point Size = new Point(48, 48);

        private bool _squashed;

        private readonly Point[] _collisionPoints;
        private const int CollisionBuffer = 1;

        private int _currentFrame;
        private float _timer;
        private List<Rectangle> SquashingAnimation { get; } = new List<Rectangle>
        {
            new Rectangle(0, 0, 96, 96),
            new Rectangle(96, 0, 96, 96),
            new Rectangle(192, 0, 96, 96),
            new Rectangle(288, 0, 96, 96),
            new Rectangle(0, 96, 96, 96),
            new Rectangle(96, 96, 96, 96),
        };

        public Rectangle Bounds => new Rectangle((int)(Position.X), (int)(Position.Y), Size.X, Size.Y);

        public MuffinMan(Vector2 position)
        {
            const float scale = Game1.BackBufferWidth / (float)20;
            Position = new Vector2(position.X * scale, position.Y * scale);

            Velocity = new Vector2(Game1.Instance.Rand.Next(2) == 0 ? Speed : -Speed, 0);

            _timer = AnimationLength;

            _collisionPoints = new Point[8];
            UpdateCollisionPoints();
        }

        public override void Update(List<GameObject> objects)
        {
            HandleTileCollisions(objects);
            HandleEntityCollisions(objects);

            objects.OfType<Player>().ToList().ForEach(player =>
            {
                if (player.Bounds.Contains(_collisionPoints[0]))
                    _squashed = true;
            });

            if (_squashed)
            {
                if (_timer <= 0)
                    _currentFrame++;

                if (_currentFrame >= SquashingAnimation.Count)
                    Destroy();
            }

            if (_timer <= 0)
                _timer = AnimationLength;
            else
                _timer -= AnimationDecrement;

            Position += Velocity;
            Velocity.Y += Gravity;
            UpdateCollisionPoints();
        }

        public void Squash()
        {
            _squashed = true;
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(TextureManager.Instance.GetTexture("MuffinManSheet"), Bounds, SquashingAnimation[_currentFrame], Color.White);

            if (TextureManager.DebugCollisionPoints)
                DebugCollisionPoints(sb);
        }

        private void HandleEntityCollisions(IEnumerable<GameObject> objects)
        {
            objects.OfType<MuffinMan>().ToList().ForEach(muffinMan =>
            {
                if (muffinMan != this)
                {
                    if (muffinMan.Bounds.Contains(_collisionPoints[1]) || muffinMan.Bounds.Contains(_collisionPoints[2]))
                    {
                        Velocity.X *= -1f;
                    }

                    if (muffinMan.Bounds.Contains(_collisionPoints[4]) || muffinMan.Bounds.Contains(_collisionPoints[5]))
                    {
                        Velocity.X *= -1f;
                    }
                }
            });
        }

        private void HandleTileCollisions(IEnumerable<GameObject> objects)
        {
            objects.OfType<Tile>().ToList().ForEach(tile =>
            {
                if (!tile.IsEmpty)
                {
                    if (tile.IsBlock)
                    {
                        if (tile.Bounds.Contains(_collisionPoints[1]) || tile.Bounds.Contains(_collisionPoints[2]))
                        {
                            Position.X = tile.Position.X - Size.X - CollisionBuffer * 2;
                            Velocity.X *= -1f;
                        }

                        if (tile.Bounds.Contains(_collisionPoints[3]))
                        {
                            Position.Y = tile.Position.Y - Size.Y - CollisionBuffer;

                            if (Velocity.Y > 0) Velocity.Y = 0;
                        }

                        if (tile.Bounds.Contains(_collisionPoints[4]) || tile.Bounds.Contains(_collisionPoints[5]))
                        {
                            Position.X = tile.Position.X + Tile.Size + CollisionBuffer;
                            Velocity.X *= -1f;
                        }
                    }
                    else
                    {
                        if ((tile.Bounds.Contains(_collisionPoints[3]) || tile.Bounds.Contains(_collisionPoints[6]) || tile.Bounds.Contains(_collisionPoints[7])) && Velocity.Y > 0)
                        {
                            Position.Y = tile.Position.Y - Size.Y - CollisionBuffer;
                            Velocity.Y = 0;
                        }
                    }
                }
            });
        }

        private void DebugCollisionPoints(SpriteBatch sb)
        {
            for (var i = 0; i < _collisionPoints.Length; i++)
            {
                sb.Draw(TextureManager.Instance.GetTexture("DebugCollisionPoints"), new Rectangle(_collisionPoints[i].X - 2, _collisionPoints[i].Y - 2, 4, 4), Color.White);
            }
        }

        private void UpdateCollisionPoints()
        {
            // Top
            _collisionPoints[0] = new Point((int)(Position.X + Size.X / (float)2), (int)(Position.Y - CollisionBuffer));

            // Upper right
            _collisionPoints[1] = new Point((int)(Position.X + Size.X + CollisionBuffer), (int)(Position.Y + Size.Y / (float)3));

            // Lower right
            _collisionPoints[2] = new Point((int)(Position.X + Size.X + CollisionBuffer), (int)(Position.Y + Size.Y / 3 * 2));

            // Bottom
            _collisionPoints[3] = new Point((int)(Position.X + Size.X / (float)2), (int)(Position.Y + Size.Y + CollisionBuffer));

            // Lower left
            _collisionPoints[4] = new Point((int)(Position.X - CollisionBuffer), (int)(Position.Y + Size.Y / 3 * 2));

            // Upper left
            _collisionPoints[5] = new Point((int)(Position.X - CollisionBuffer), (int)(Position.Y + Size.Y / (float)3));

            // Lower right corner
            _collisionPoints[6] = new Point((int)(Position.X + Size.X + CollisionBuffer), (int)(Position.Y + Size.Y + CollisionBuffer));

            // Lower left corner
            _collisionPoints[7] = new Point((int)(Position.X - CollisionBuffer), (int)(Position.Y + Size.Y + CollisionBuffer));
        }
    }
}
