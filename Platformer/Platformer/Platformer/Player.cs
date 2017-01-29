using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platformer
{
    public class Player : GameObject
    {
        private const float Gravity = 0.3f;
        private const float Speed = 3f;
        private const float SprintSpeed = 5f;
        private const float Jump = -10f;

        public Vector2 Position;
        public Vector2 Velocity;

        private static int Size => 64;

        private const float Tolerance = 0.0001f;

        private bool _isAbleToJump = true;

        private KeyboardState _keyboard;
        private KeyboardState _oldKeyboard;

        private readonly Point[] _collisionPoints;
        private const int CollisionBuffer = 1;

        public static Texture2D Texture { get; set; }
        public Rectangle Bounds => new Rectangle((int)(Position.X), (int)(Position.Y), Size, Size);

        public Player(Vector2 position)
        {
            const float scale = Game1.BackBufferWidth / (float)20;
            Position = new Vector2(position.X * scale, position.Y * scale);

            Velocity = Vector2.Zero;

            _keyboard = Keyboard.GetState();

            _collisionPoints = new Point[8];
            UpdateCollisionPoints();
        }

        public override void Update(List<GameObject> objects)
        {
            _oldKeyboard = _keyboard;
            _keyboard = Keyboard.GetState();

            HandleKeyboardInput();
            HandleTileCollisions(objects);

            _isAbleToJump = Math.Abs(Velocity.Y) < Tolerance;

            Position += Velocity;
            Velocity.Y += Gravity;
            Velocity.X = 0;
            UpdateCollisionPoints();
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, Bounds, Color.White);

            //DebugCollisionPoints(sb);
        }

        private void HandleKeyboardInput()
        {
            if (_keyboard.IsKeyDown(Keys.A) || _keyboard.IsKeyDown(Keys.Left))
            {
                Velocity.X = _keyboard.IsKeyDown(Keys.LeftShift) ? -SprintSpeed : -Speed;
            }

            if (_keyboard.IsKeyDown(Keys.D) || _keyboard.IsKeyDown(Keys.Right))
            {
                if (Math.Abs(Velocity.X - (-Speed)) < Tolerance || (_keyboard.IsKeyDown(Keys.LeftShift) && Math.Abs(Velocity.X - (-SprintSpeed)) < Tolerance))
                    Velocity.X = 0;
                else
                    Velocity.X = _keyboard.IsKeyDown(Keys.LeftShift) ? SprintSpeed : Speed;
            }

            if (_keyboard.IsKeyDown(Keys.Space) && !_oldKeyboard.IsKeyDown(Keys.Space))
            {
                if (_isAbleToJump)
                {
                    Velocity.Y = Jump;
                    _isAbleToJump = false;
                }
            }
        }

        private void HandleTileCollisions(List<GameObject> objects)
        {
            objects.OfType<Tile>().ToList().ForEach(tile =>
            {
                if (!tile.IsEmpty)
                {
                    if (tile.IsBlock)
                    {
                        if (tile.Bounds.Contains(_collisionPoints[0]))
                        {
                            Position.Y = tile.Position.Y + Tile.Size + CollisionBuffer;
                            Velocity.Y *= -0.5f;
                        }

                        if (tile.Bounds.Contains(_collisionPoints[1]) || tile.Bounds.Contains(_collisionPoints[2]))
                        {
                            Position.X = tile.Position.X - Size - CollisionBuffer * 2;
                            Velocity.X = 0;
                        }

                        if (tile.Bounds.Contains(_collisionPoints[3]))
                        {
                            Position.Y = tile.Position.Y - Size - CollisionBuffer;

                            if (Velocity.Y > 0) Velocity.Y = 0;
                        }

                        if (tile.Bounds.Contains(_collisionPoints[4]) || tile.Bounds.Contains(_collisionPoints[5]))
                        {
                            Position.X = tile.Position.X + Tile.Size + CollisionBuffer;
                            Velocity.X = 0;
                        }
                    }
                    else
                    {
                        if (!(_keyboard.IsKeyDown(Keys.LeftShift) && (_keyboard.IsKeyDown(Keys.S) || _keyboard.IsKeyDown(Keys.Down))))
                        {
                            if ((tile.Bounds.Contains(_collisionPoints[3]) || tile.Bounds.Contains(_collisionPoints[6]) || tile.Bounds.Contains(_collisionPoints[7])) && Velocity.Y > 0)
                            {
                                Position.Y = tile.Position.Y - Size - CollisionBuffer;
                                Velocity.Y = 0;
                            }
                        }
                    }
                }
            });
        }

        private void DebugCollisionPoints(SpriteBatch sb)
        {
            for (var i = 0; i < _collisionPoints.Length; i++)
            {
                sb.Draw(Texture, new Rectangle(_collisionPoints[i].X - 2, _collisionPoints[i].Y - 2, 4, 4), Color.White);
            }
        }

        private void UpdateCollisionPoints()
        {
            // Top
            _collisionPoints[0] = new Point((int)(Position.X + Size / (float)2), (int)(Position.Y - CollisionBuffer));

            // Upper right
            _collisionPoints[1] = new Point((int)(Position.X + Size + CollisionBuffer), (int)(Position.Y + Size / (float)3));

            // Lower right
            _collisionPoints[2] = new Point((int)(Position.X + Size + CollisionBuffer), (int)(Position.Y + Size / 3 * 2));

            // Bottom
            _collisionPoints[3] = new Point((int)(Position.X + Size / (float)2), (int)(Position.Y + Size + CollisionBuffer));

            // Lower left
            _collisionPoints[4] = new Point((int)(Position.X - CollisionBuffer), (int)(Position.Y + Size / 3 * 2));

            // Upper left
            _collisionPoints[5] = new Point((int)(Position.X - CollisionBuffer), (int)(Position.Y + Size / (float)3));

            // Lower right corner
            _collisionPoints[6] = new Point((int)(Position.X + Size + CollisionBuffer), (int)(Position.Y + Size + CollisionBuffer));

            // Lower left corner
            _collisionPoints[7] = new Point((int)(Position.X - CollisionBuffer), (int)(Position.Y + Size + CollisionBuffer));
        }
    }
}
