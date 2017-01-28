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
    public class Player : GameObject
    {
        private const float gravity = 0.3f;
        private const float speed = 3f;
        private const float sprintSpeed = 5f;
        private const float jump = -10f;

        public Vector2 position;
        public Vector2 velocity;

        int size = 64;

        bool isAbleToJump = false;

        KeyboardState keyboard;
        KeyboardState oldKeyboard;

        Point[] collisionPoints;
        const int collisionBuffer = 1;

        public static Texture2D Texture { get; set; }
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)(position.X), (int)(position.Y), (int)(size), (int)(size));
            }
        }

        public Player(Vector2 position)
        {
            float scale = Game1.BackBufferWidth / 20;
            this.position = new Vector2(position.X * scale, position.Y * scale);

            velocity = Vector2.Zero;

            keyboard = Keyboard.GetState();

            collisionPoints = new Point[8];
            UpdateCollisionPoints();
        }

        public override void Update(List<GameObject> objects)
        {
            oldKeyboard = keyboard;
            keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
            {
                velocity.X = keyboard.IsKeyDown(Keys.LeftShift) ? -sprintSpeed : -speed;
            }

            if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
            {
                if (velocity.X == -speed || (keyboard.IsKeyDown(Keys.LeftShift) && velocity.X == -sprintSpeed))
                    velocity.X = 0;
                else
                    velocity.X = keyboard.IsKeyDown(Keys.LeftShift) ? sprintSpeed : speed;
            }

            if (keyboard.IsKeyDown(Keys.Space) && !oldKeyboard.IsKeyDown(Keys.Space))
            {
                if (isAbleToJump)
                {
                    velocity.Y = jump;
                    isAbleToJump = false;
                }
            }

            objects.OfType<Tile>().ToList().ForEach(tile =>
            {
                if (!tile.isEmpty)
                {
                    if (tile.isBlock)
                    {
                        if (tile.Bounds.Contains(collisionPoints[0]))
                        {
                            position.Y = tile.position.Y + Tile.Size + collisionBuffer;
                            velocity.Y *= -0.5f;
                        }

                        if (tile.Bounds.Contains(collisionPoints[1]) || tile.Bounds.Contains(collisionPoints[2]))
                        {
                            position.X = tile.position.X - size - collisionBuffer * 2;
                            velocity.X = 0;
                        }

                        if (tile.Bounds.Contains(collisionPoints[3]))
                        {
                            position.Y = tile.position.Y - size - collisionBuffer;

                            if (velocity.Y > 0) velocity.Y = 0;
                        }

                        if (tile.Bounds.Contains(collisionPoints[4]) || tile.Bounds.Contains(collisionPoints[5]))
                        {
                            position.X = tile.position.X + Tile.Size + collisionBuffer;
                            velocity.X = 0;
                        }
                    }
                    else
                    {
                        if (!(keyboard.IsKeyDown(Keys.LeftShift) && (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))))
                        {
                            if ((tile.Bounds.Contains(collisionPoints[3]) || tile.Bounds.Contains(collisionPoints[6]) || tile.Bounds.Contains(collisionPoints[7])) && velocity.Y > 0)
                            {
                                position.Y = tile.position.Y - size - collisionBuffer;
                                velocity.Y = 0;
                            }
                        }
                    }
                }
            });

            isAbleToJump = velocity.Y == 0;

            position += velocity;
            velocity.Y += gravity;
            velocity.X = 0;
            UpdateCollisionPoints();
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, Bounds, Color.White);

            // DebugCollisionPoints(sb);
        }

        private void DebugCollisionPoints(SpriteBatch sb)
        {
            for (int i = 0; i < collisionPoints.Length; i++)
            {
                sb.Draw(Texture, new Rectangle(collisionPoints[i].X - 2, collisionPoints[i].Y - 2, 4, 4), Color.White);
            }
        }

        private void UpdateCollisionPoints()
        {
            // Top
            collisionPoints[0] = new Point((int)(position.X + size / 2), (int)(position.Y - collisionBuffer));

            // Upper right
            collisionPoints[1] = new Point((int)(position.X + size + collisionBuffer), (int)(position.Y + size / 3));

            // Lower right
            collisionPoints[2] = new Point((int)(position.X + size + collisionBuffer), (int)(position.Y + size / 3 * 2));

            // Bottom
            collisionPoints[3] = new Point((int)(position.X + size / 2), (int)(position.Y + size + collisionBuffer));

            // Lower left
            collisionPoints[4] = new Point((int)(position.X - collisionBuffer), (int)(position.Y + size / 3 * 2));

            // Upper left
            collisionPoints[5] = new Point((int)(position.X - collisionBuffer), (int)(position.Y + size / 3));

            // Lower right corner
            collisionPoints[6] = new Point((int)(position.X + size), (int)(position.Y + size));

            // Lower left corner
            collisionPoints[7] = new Point((int)(position.X), (int)(position.Y + size));
        }
    }
}
