using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer
{
    public abstract class GameObject
    {
        public bool ToDestroy;

        public void Destroy()
        {
            ToDestroy = true;
        }

        public abstract void Update(List<GameObject> objects);
        public abstract void Draw(SpriteBatch sb);
    }
}
