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
        private SpriteFont _gameOverFont;
        private SpriteFont _scoreFont;

        public Random Rand;

        private int score;

        private List<GameObject> _objects;

        private Level _level;

        private bool _gameOver;

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
            CollectableManager.Instance.CollectableCollected();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _gameOverFont = Content.Load<SpriteFont>("GameOver");
            _scoreFont = Content.Load<SpriteFont>("Score");

            Tile.BlockSheet = Content.Load<Texture2D>("Blocks");
            Tile.PlatformSheet = Content.Load<Texture2D>("Platforms");
            Tile.EmptyBlock = Content.Load<Texture2D>("EmptyBlock");

            Player.PlayerSheet = Content.Load<Texture2D>("playerSheet");

            MuffinMan.SpriteSheet = Content.Load<Texture2D>("muffinman");

            Collectable.CollectableSheet = Content.Load<Texture2D>("Collectables/scribbles");

            LoadLevel();
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) Exit();

            _objects.ForEach(o =>
            {
                if (!_gameOver) 
                    o.Update(_objects);
            });

            _objects.OfType<Player>().ToList().ForEach(player =>
            {
                if (player.ToDestroy && !_gameOver)
                    _gameOver = true;
            });

            _objects = _objects.Where(o => !o.ToDestroy).ToList();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _objects.ForEach(o => o.Draw(_spriteBatch));

            _spriteBatch.DrawString(_scoreFont, "" + score, new Vector2(96, 64), Color.Black);

            if (_gameOver)
            {
                string text = "GAME OVER";
                Vector2 size = _gameOverFont.MeasureString(text);
                Vector2 position = new Vector2((float)GraphicsDevice.Viewport.Width / 2 - size.X / 2, (float)GraphicsDevice.Viewport.Height / 2 - size.Y / 2);
                _spriteBatch.DrawString(_gameOverFont, text, position, Color.Red);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void LoadLevel()
        {
            _level = new Level(_objects, Services, @"Content/Levels/Level01.txt");
        }

        public void IncrementScore(int amount)
        {
            score += amount;
        }

        public void AddCollectible(Collectable c)
        {
            _objects.Add(c);
        }
    }
}