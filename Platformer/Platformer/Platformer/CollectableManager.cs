using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class CollectableManager
    {
        private static CollectableManager instance;
        private List<Collectable> collectables;
        private Color[] colors =
        {
            Color.Blue,
            Color.Green,
            Color.Yellow,
            Color.Orange,
            Color.Red
        };

        public static CollectableManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new CollectableManager();

                return instance;
            }
        }

        private CollectableManager()
        {
            collectables = new List<Collectable>();
        }

        public void AddCollectible(Collectable c)
        {
            collectables.Add(c);
        }

        public void CollectableCollected()
        {
            int index = Game1.Instance.Rand.Next(collectables.Count);
            Collectable c = collectables[index];
            c.Color = colors[Game1.Instance.Rand.Next(colors.Length)];
            Game1.Instance.AddCollectible(c.Clone());
        }
    }
}
