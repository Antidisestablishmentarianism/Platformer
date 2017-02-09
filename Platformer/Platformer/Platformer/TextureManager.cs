using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer
{
    public class TextureManager
    {
        private static TextureManager _instance;
        private readonly Dictionary<string, Texture2D> _textures;

        public static TextureManager Instance => _instance ?? (_instance = new TextureManager());
        public static bool DebugCollisionPoints { get; } = false;

        public Func<string, Texture2D> GetTexture;

        private TextureManager()
        {
            _textures = new Dictionary<string, Texture2D>();
            GetTexture = name => _textures[name];
        }

        public void AddTexture(string name, Texture2D texture)
        {
            _textures.Add(name, texture);
        }

        public void Unload()
        {
            _textures.Clear();
        }
    }
}
