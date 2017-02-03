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
        private const float Speed = 4f;
        private const float SprintSpeed = 6f;
        private const float Jump = -10f;
        private const float AnimationLength = 0.4f;

        public Vector2 Position;
        public Vector2 Velocity;

        private SpriteEffects _spriteEffects;

        private static readonly Point Size = new Point(64, 64);

        private const float Tolerance = 0.0001f;

        private bool _isAbleToJump = true;

        private KeyboardState _keyboard;
        private KeyboardState _oldKeyboard;

        private readonly Point[] _collisionPoints;
        private const int CollisionBuffer = 1;

        private int _currentWalkingFrame;
        private int _currentJumpingFrame;
        private float _timer;
        private List<Rectangle> WalkingAnimation { get; } = new List<Rectangle>
        {
            new Rectangle(0, 0, 96, 96),
            new Rectangle(96, 0, 96, 96),
            new Rectangle(192, 0, 96, 96),
            new Rectangle(288, 0, 96, 96),
            new Rectangle(0, 96, 96, 96),
            new Rectangle(96, 96, 96, 96),
            new Rectangle(192, 96, 96, 96)
        };

        private List<Rectangle> JumpingAnimation { get; } = new List<Rectangle>
        {
            new Rectangle(288, 96, 96, 96),
            new Rectangle(0, 192, 96, 96),
            new Rectangle(96, 192, 96, 96),
            new Rectangle(192, 192, 96, 96),
            new Rectangle(288, 192, 96, 96),
            new Rectangle(0, 288, 96, 96),
        };

        public static Texture2D PlayerSheet { get; set; }
        public Rectangle Bounds => new Rectangle((int)(Position.X), (int)(Position.Y), Size.X, Size.Y);

        public Player(Vector2 position)
        {
            const float scale = Game1.BackBufferWidth / (float)20;
            Position = new Vector2(position.X * scale, position.Y * scale);

            Velocity = Vector2.Zero;

            _keyboard = Keyboard.GetState();

            _timer = AnimationLength;

            _collisionPoints = new Point[8];
            UpdateCollisionPoints();
        }

        public override void Update(List<GameObject> objects)
        {
            _oldKeyboard = _keyboard; _keyboard = Keyboard.GetState();
            HandleKeyboardInput();
            HandleTileCollisions(objects);
            HandleEnemyCollisions(objects);

            _isAbleToJump = Math.Abs(Velocity.Y) < Tolerance;

            if (_isAbleToJump)
            {
                if (_timer <= 0)
                    _currentWalkingFrame++;
                _currentJumpingFrame = 0;

                if (_currentWalkingFrame >= WalkingAnimation.Count)
                    _currentWalkingFrame = 0;
            }
            else
            {
                if (_timer <= 0)
                    _currentJumpingFrame++;
                _currentWalkingFrame = 0;

                if (_currentJumpingFrame >= JumpingAnimation.Count)
                    _currentJumpingFrame = 0;
            }

            if (_timer <= 0)
                _timer = AnimationLength;
            else
                _timer -= 0.05f;

            Position += Velocity;
            Velocity.Y += Gravity;
            Velocity.X = 0;
            UpdateCollisionPoints();
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(PlayerSheet, Bounds, _isAbleToJump ? WalkingAnimation[_currentWalkingFrame] : JumpingAnimation[_currentJumpingFrame], Color.White, 0f, Vector2.Zero, _spriteEffects, 1f);

            //DebugCollisionPoints(sb);
        }

        private void HandleEnemyCollisions(IEnumerable<GameObject> objects)
        {
            objects.OfType<MuffinMan>().ToList().ForEach(muffinMan =>
            {
                if (muffinMan.Bounds.Contains(_collisionPoints[3]) || muffinMan.Bounds.Contains(_collisionPoints[6]) || muffinMan.Bounds.Contains(_collisionPoints[7]))
                {
                    Velocity.Y = Jump;
                    muffinMan.Squash();
                }

                if (muffinMan.Bounds.Contains(_collisionPoints[0]) ||
                    muffinMan.Bounds.Contains(_collisionPoints[1]) ||
                    muffinMan.Bounds.Contains(_collisionPoints[2]) ||
                    muffinMan.Bounds.Contains(_collisionPoints[4]) ||
                    muffinMan.Bounds.Contains(_collisionPoints[5]))
                {
                    Destroy();
                }
            });
        }

        private void HandleKeyboardInput()
        {
            if (_keyboard.IsKeyDown(Keys.A) || _keyboard.IsKeyDown(Keys.Left))
            {
                Velocity.X = _keyboard.IsKeyDown(Keys.LeftShift) ? -SprintSpeed : -Speed;
                _spriteEffects = SpriteEffects.FlipHorizontally;
            }

            if (_keyboard.IsKeyDown(Keys.D) || _keyboard.IsKeyDown(Keys.Right))
            {
                if (Math.Abs(Velocity.X - (-Speed)) < Tolerance ||
                    (_keyboard.IsKeyDown(Keys.LeftShift) && Math.Abs(Velocity.X - (-SprintSpeed)) < Tolerance))
                    Velocity.X = 0;
                else
                {
                    Velocity.X = _keyboard.IsKeyDown(Keys.LeftShift) ? SprintSpeed : Speed;
                    _spriteEffects = new SpriteEffects();
                }
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

        private void HandleTileCollisions(IEnumerable<GameObject> objects)
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
                            Position.X = tile.Position.X - Size.X - CollisionBuffer * 2;
                            Velocity.X = 0;
                        }

                        if (tile.Bounds.Contains(_collisionPoints[3]))
                        {
                            Position.Y = tile.Position.Y - Size.Y - CollisionBuffer;

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
                                Position.Y = tile.Position.Y - Size.Y - CollisionBuffer;
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
                sb.Draw(PlayerSheet, new Rectangle(_collisionPoints[i].X - 2, _collisionPoints[i].Y - 2, 4, 4), Color.White);
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