using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Graven
{
    public class FrameRateCounter
    {
        ContentManager content;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        public bool show = true;

        public void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        public void Draw(SpriteBatch sb, SpriteFont spriteFont)
        {
            frameCounter++;
            if (!show) return;

            string fps = string.Format("FPS: {0}", frameRate);
            sb.DrawString(spriteFont, fps, new Vector2(11, 11), Color.Black);
            sb.DrawString(spriteFont, fps, new Vector2(10, 10), Color.White);
        }
    }
}
