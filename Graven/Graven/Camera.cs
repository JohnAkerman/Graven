using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Graven
{
    class Camera : GameObject
    {
        public Camera(Vector2 posIn)
        {
            position = posIn;
            size = new Vector2(16, 16);
        }

        public int getCameraX(int widthIn)
        {

            int xmp = (int)Math.Round((position.X + widthIn) * 0.0625);

            if (xmp > 0)
                return xmp;
            else
                return 0;
        }

        public int getCameraY(int widthIn)
        {
            int xmp = (int)Math.Round((position.Y + widthIn) * 0.0625);

            if (xmp > 0)
                return xmp;
            else
                return 0;
        }
    }
}
