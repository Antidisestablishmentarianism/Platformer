using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platformer
{
    public class Game1 : Game
    {
        public const int TargetFrameRate = 60;
        public const int BackBufferWidth = 1280;
        public const int BackBufferHeight = 720;

        public GraphicsDeviceManager Graphics { get; }
        private SpriteBatch _spriteBatch;

        public Random Rand;

        private List<GameObject> _objects;

        private Level _level;

        public static Game1 Instance;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = BackBufferWidth,
                PreferredBackBufferHeight = BackBufferHeight
            };
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);

            Instance = this;
        }

        protected override void Initialize()
        {
            _objects = new List<GameObject>();
            Rand = new Random();

            base.Initialize();

            List<Player> players = _objects.OfType<Player>().ToList();

            _objects.ForEach(o =>
            {
                if (o is Player)
                    _objects.Remove(o);
            });

            _objects.AddRange(players);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Tile.BlockSheet = Content.Load<Texture2D>("Blocks");
            Tile.PlatformSheet = Content.Load<Texture2D>("Platforms");
            Tile.EmptyBlock = Content.Load<Texture2D>("EmptyBlock");

            Player.Texture = Content.Load<Texture2D>("Player");
            Player.PlayerSheet = Content.Load<Texture2D>("playerSheet");

            LoadLevel();
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) Exit();

            _objects.ForEach(o => o.Update(_objects));
            _objects = _objects.Where(o => !o.ToDestroy).ToList();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _objects.ForEach(o => o.Draw(_spriteBatch));
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void LoadLevel()
        {
            _level = new Level(_objects, Services, @"Content/Levels/Level01.txt");
        }
    }
}