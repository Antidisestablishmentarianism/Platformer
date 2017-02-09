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

        private int Score { get; set; }

        private SpriteBatch _spriteBatch;
        private SpriteFont _gameOverFont;
        private SpriteFont _scoreFont;

        public Random Rand;

        private List<GameObject> _objects;

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

            var players = _objects.OfType<Player>().ToList();

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

            TextureManager.Instance.AddTexture("BlockSheet", Content.Load<Texture2D>("Blocks"));
            TextureManager.Instance.AddTexture("PlatformSheet", Content.Load<Texture2D>("Platforms"));
            TextureManager.Instance.AddTexture("EmptyBlock", Content.Load<Texture2D>("EmptyBlock"));
            TextureManager.Instance.AddTexture("PlayerSheet", Content.Load<Texture2D>("playerSheet"));
            TextureManager.Instance.AddTexture("MuffinManSheet", Content.Load<Texture2D>("muffinman"));
            TextureManager.Instance.AddTexture("CollectableSheet", Content.Load<Texture2D>("Collectables/scribbles"));

            if (TextureManager.DebugCollisionPoints)
                TextureManager.Instance.AddTexture("DebugCollisionPoints", Content.Load<Texture2D>("DebugCollisionPoints"));

            LoadLevel("01");
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

            _spriteBatch.DrawString(_scoreFont, "" + Score, new Vector2(96, 64), Color.Black);

            if (_gameOver)
            {
                const string text = "GAME OVER";
                var size = _gameOverFont.MeasureString(text);
                var position = new Vector2((float)GraphicsDevice.Viewport.Width / 2 - size.X / 2, (float)GraphicsDevice.Viewport.Height / 2 - size.Y / 2);
                _spriteBatch.DrawString(_gameOverFont, text, position, Color.Red);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void LoadLevel(string num)
        {
            var level = new Level(_objects, Services, @"Content/Levels/Level" + num + ".txt");
        }

        public void IncrementScore(int amount)
        {
            Score += amount;
        }

        public void AddCollectible(Collectable c)
        {
            _objects.Add(c);
        }
    }
}