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
        float scrollRate;

        public Layer(ContentManager content, string stringBase, float scrollRate) {
            this.texture = content.Load<Texture2D>(stringBase);
            this.scrollRate = scrollRate;
        }

        public void Draw(SpriteBatch sb, Vector2 camPos)
        {
            sb.Draw(this.texture, Vector2.Zero, new Rectangle(Convert.ToInt32(camPos.X * scrollRate), Convert.ToInt32(camPos.Y * scrollRate), texture.Width, texture.Height), Color.White);
        }
    }
}
