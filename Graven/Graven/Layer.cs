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
        bool isAnimated;
        int lastFrame, milPerFrame;
        Vector2 curFrame, sheetSize;

        public Layer(ContentManager content, string stringBase, Vector2 scrollRate, Vector2 sheetSize, bool isAnimated = false, int milPerFrame = 0) {
            this.texture = content.Load<Texture2D>(stringBase);
            this.scrollRate = scrollRate;
            this.isAnimated = isAnimated;
            this.sheetSize = sheetSize;
            this.milPerFrame = milPerFrame;
        }

        public void Draw(SpriteBatch sb, Vector2 camPos)
        {
            sb.Draw(this.texture, new Vector2(0,-450), new Rectangle(Convert.ToInt32((camPos.X * scrollRate.X) + curFrame.X), Convert.ToInt32(camPos.Y * scrollRate.Y), texture.Width, texture.Height), Color.White);
        }

        public void updateFrame(GameTime gt)
        {
            if (isAnimated == false) return;

            lastFrame += gt.ElapsedGameTime.Milliseconds;
            if (lastFrame > milPerFrame)
            {
                lastFrame = 0;
                curFrame.X++;
                if (curFrame.X >= sheetSize.X)
                {
                    curFrame.X = 0;
                }
            }
        }
    }
}
