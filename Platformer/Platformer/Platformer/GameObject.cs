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
    public abstract class GameObject
    {
        public bool toDestroy = false;

        public void Destroy()
        {
            toDestroy = true;
        }

        public abstract void Update(List<GameObject> objects);
        public abstract void Draw(SpriteBatch sb);
    }
}
