using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class CollectableManager
    {
        private static CollectableManager _instance;
        private readonly List<Collectable> _collectables;
        private readonly Color[] _colors =
        {
            Color.Blue,
            Color.Green,
            Color.Yellow,
            Color.Orange,
            Color.Red
        };

        public static CollectableManager Instance => _instance ?? (_instance = new CollectableManager());

        private CollectableManager()
        {
            _collectables = new List<Collectable>();
        }

        public void AddCollectible(Collectable c)
        {
            _collectables.Add(c);
        }

        public void CollectableCollected()
        {
            var index = Game1.Instance.Rand.Next(_collectables.Count);
            var c = _collectables[index];
            c.Color = _colors[Game1.Instance.Rand.Next(_colors.Length)];
            Game1.Instance.AddCollectible(c.Clone());
        }

        public void Reset()
        {
            _collectables.Clear();
        }
    }
}
