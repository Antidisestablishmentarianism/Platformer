using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Platformer
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public const int TargetFrameRate = 60;
        public const int BackBufferWidth = 1280;
        public const int BackBufferHeight = 720;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public float scale = 1f;

        public Random rand;

        List<GameObject> objects;

        private Level level;

        public static Game1 Instance;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = BackBufferWidth;
            graphics.PreferredBackBufferHeight = BackBufferHeight;
            Content.RootDirectory = "Content";

            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);

            Instance = this;
        }

        protected override void Initialize()
        {
            objects = new List<GameObject>();
            rand = new Random();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Tile.BlockSheet = Content.Load<Texture2D>("Blocks");
            Tile.PlatformSheet = Content.Load<Texture2D>("Platforms");
            Tile.EmptyBlock = Content.Load<Texture2D>("EmptyBlock");

            LoadLevel();
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            objects.ForEach(o => o.Update(objects));
            objects = objects.Where(o => !o.toDestroy).ToList();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            objects.ForEach(o => o.Draw(spriteBatch));
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void LoadLevel()
        {
            level = new Level(objects, Services, @"Content/Levels/Level01.txt");
        }
    }
}