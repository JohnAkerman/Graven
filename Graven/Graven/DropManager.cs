using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Graven
{
    class DropManager
    {
        public WaterDrop[,] droplets;
        Texture2D topMost, waterSandTop, WaterDropTex, waterSand;
        public int levelWidth, levelHeight, screenHeight, screenWidth;
        public bool pauseWater = false;

        public DropManager(int levelWidth, int levelHeight, int screenWidth, int screenHeight)
        {
            this.levelHeight = levelHeight;
            this.levelWidth = levelWidth;
            this.screenHeight = screenHeight;
            this.screenWidth = screenWidth;
        }

        public void makeDrop(float amount, Vector2 cameraPosition)
        {
            int mouse_x = Mouse.GetState().X - 4 + (int)cameraPosition.X;
            int mouse_y = Mouse.GetState().Y - 4 + (int)cameraPosition.Y;

            if (mouse_x > 0 && mouse_x < levelWidth * 16 && mouse_y > 0 && mouse_y < levelHeight * 16)
            {
                droplets[mouse_y / 16, mouse_x / 16].volume = amount;
            }
        }


        public void loadTextures(ContentManager Content)
        {
            WaterDropTex = Content.Load<Texture2D>("waterdrop");
            topMost = Content.Load<Texture2D>("Tiles/topmost");
            waterSandTop = Content.Load<Texture2D>("Tiles/watersandtop");
            waterSand = Content.Load<Texture2D>("Tiles/watersand");
        }

        public void setupDrops()
        {
            droplets = new WaterDrop[levelHeight, levelWidth];
            Random rand = new Random();
            for (int y = 0; y < levelHeight; y++)
            {
                for (int x = 0; x < levelWidth; x++)
                {
                    droplets[y, x] = new WaterDrop(x, y, levelWidth, levelHeight, 0);
                }
            }
        }

        public void emptyDrops()
        {
            for (int y = 0; y < levelHeight; y++)
            {
                for (int x = 0; x < levelWidth; x++)
                {
                    droplets[y, x].volume = 0;
                }
            }
        }

        public void updateWater(Camera cam, ref Tile[,,] tiles, float totalElapsed)
        {
            if (pauseWater) return; 
            for (int y = cam.getCameraY(0); y < cam.getCameraY(screenHeight); y++)
            {
                for (int x = cam.getCameraX(0); x < cam.getCameraX(screenWidth); x++)
                {
                    droplets[y, x].Update(ref tiles, ref droplets, totalElapsed);
                }
            }
        }

        public void drawDroplets(Camera cam, SpriteBatch sb)
        {
            for (int y = cam.getCameraY(0); y < cam.getCameraY(screenHeight); y++)
            {
                for (int x = cam.getCameraX(0); x < cam.getCameraX(screenWidth); x++)
                {
                    if (droplets[y, x].topMost && droplets[y, x].waterSand)
                        droplets[y, x].Draw(cam.position, sb, ref waterSandTop);
                    else if (droplets[y, x].topMost && droplets[y, x].waterSand == false)
                        droplets[y, x].Draw(cam.position, sb, ref topMost);
                    else if (droplets[y, x].waterSand && droplets[y, x].topMost == false)
                        droplets[y, x].Draw(cam.position, sb, ref waterSand);
                    else
                        droplets[y, x].Draw(cam.position, sb, ref WaterDropTex);
                }
            }
        }
    }
}
