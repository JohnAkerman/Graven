using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Graven
{
    class Layer
    {
        Texture2D texture;
        Vector2 scrollRate;

        public Layer(ContentManager content, string stringBase, Vector2 scrollRate) {
            this.texture = content.Load<Texture2D>(stringBase);
            this.scrollRate = scrollRate;
        }

        public void Draw(SpriteBatch sb, Vector2 camPos)
        {
            sb.Draw(this.texture, new Vector2(0,-450), new Rectangle(Convert.ToInt32(camPos.X * scrollRate.X), Convert.ToInt32(camPos.Y * scrollRate.Y), texture.Width, texture.Height), Color.White);
        }
    }
}
