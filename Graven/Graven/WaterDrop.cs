using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Graven
{
    class WaterDrop : GameObject
    {
        float lastUpdate = 0;
        float updateTime = 0.06f;
        public float volume;
        public bool topMost = false;
        public bool waterSand = false;

        public WaterDrop(GraphicsDevice graphics, int xIn, int yIn, int widthIn, int heightIn, int volume = 0)
        {
            this.size = new Vector2(16, 16);
            this.position = new Vector2(xIn * size.X, yIn * size.Y);
            this.volume = volume;
            this.tileX = (int)this.position.X / 16;
            this.tileY = (int)this.position.Y / 16;
            this.totalHeight = heightIn;
            this.totalWidth = widthIn;
        }

        public void Draw(Vector2 cameraPosition, SpriteBatch sb, ref Texture2D WaterDropTex) {
            sb.Draw(WaterDropTex, this.position + new Vector2(8,16) - cameraPosition, new Rectangle(0, 0, 16, (int)this.volume), Color.White,  3.14f, new Vector2(8,0), 1.0f, SpriteEffects.FlipVertically, 0);
        }

        public bool belowClear()
        {
            return (tileY + 1 < totalHeight);
        }

        public int grabX(int newX)
        {

            if (tileX + newX < 0) // Too far left
                return tileX;
            else if (tileX + newX >= totalWidth) // too far right
                return tileX;
            else
            {
                return (tileX + newX);
            }
        }

        public int grabY(int newY)
        {
            if (tileY + newY < 0) // Too far left
                return tileY;
            else if (tileY + newY >= totalHeight) // too far up
                return tileY;
            else
            {
                return (tileY + newY);
            }
        }

        public void Update(ref Tile[,] tiles, ref WaterDrop [,] drops, float elapsedTime)
        {
            if (this.volume <= 0.0f || (elapsedTime - lastUpdate) < updateTime) { return; }

            lastUpdate = elapsedTime;

            if (tiles[tileY, tileX].tileType == TileType.Dirt)
            {
                this.volume = 0.0f;
                return;
            }

            if (tiles[tileY, tileX].tileType == TileType.Sand)
                waterSand = true;
            else
                waterSand = false;

            if (belowClear() && tiles[tileY + 1, tileX].tileType == TileType.Dirt || tiles[tileY + 1, tileX].tileType == TileType.Decoration || this.volume >= 1 && drops[grabY(1), tileX].volume >= 16)
             {
                 if (this.volume > drops[tileY, grabX(-1)].volume && this.volume > drops[tileY, grabX(1)].volume) // Going Left and Right
                 {
                     if (tileX == grabX(-1) || tileX == grabX(1)) return; // If Self, return
                     if (this.volume < 0.01f) { this.volume = 0.0f; return; }

                     float totalVol = drops[tileY, grabX(-1)].volume + this.volume + drops[tileY, grabX(1)].volume;

                     bool tmp = false;
                     int dividerAmount = 1;

                     if (tiles[tileY, grabX(-1)].tileType != TileType.Dirt) 
                         dividerAmount++;

                     if (tiles[tileY, grabX(1)].tileType != TileType.Dirt)
                         dividerAmount++;

                     if (tiles[tileY, grabX(-1)].tileType != TileType.Dirt)
                     {
                         drops[tileY, grabX(-1)].volume = (float)Math.Round(totalVol / dividerAmount, 2);
                         tmp = true;
                     }

                     if (tiles[tileY, grabX(1)].tileType != TileType.Dirt) {
                         drops[tileY, grabX(1)].volume = (float)Math.Round(totalVol / dividerAmount, 2);
                         tmp = true;
                     }
                       
                     if (tmp)
                        this.volume = (float)Math.Round(totalVol / dividerAmount, 2);
                 }
                 else if (this.volume > drops[tileY, grabX(-1)].volume) // Going Left
                 {
                     if (tileX == grabX(-1)) return;
                     if (this.volume < 0.01f) { this.volume = 0.0f; return; }
                 
                     if (tiles[tileY, grabX(-1)].tileType == TileType.Dirt)
                         return;
                     
                     float totalVol = drops[tileY, grabX(-1)].volume + this.volume;
                     totalVol = (float)Math.Round(totalVol / 2, 2);

                     drops[tileY, grabX(-1)].volume = totalVol;
                     this.volume = totalVol;
                 }
                 else if (this.volume > drops[tileY, grabX(1)].volume) // Going Right
                 {
                     if (tileX == grabX(1)) return;
                     if (this.volume < 0.01f) { this.volume = 0.0f; return; }

                     if (tiles[tileY, grabX(1)].tileType == TileType.Dirt)
                        return;
                     
                     float totalVol = drops[tileY, grabX(1)].volume + this.volume;
                     totalVol = (float)Math.Round(totalVol / 2,2);

                     drops[tileY, grabX(1)].volume = totalVol;
                     this.volume = totalVol;
                 }
             }

            // Check left empty AND left down is empty (then fall)

            if (belowClear() && tiles[tileY + 1, tileX].tileType != TileType.Dirt)
            {
                // this.position.Y += 16;
                // this.tileY = (int)this.position.Y / 16;
             
                // if the current volume is more than zero and bottom drop is empty, pour all of water into bottom drop
                if (this.volume > 0.0 && drops[grabY(1), tileX].volume <= 0.0)
                {
                    drops[grabY(1), tileX].volume = this.volume;
                    this.volume = 0;
                }

                if (drops[grabY(1), tileX].volume < 16)
                { // if the drop below isnt full, drip into it 

                    // Space left to pour into
                    float deltaPour = (16 - drops[grabY(1), tileX].volume);

                    // Delta between deltaPour and vol
                    float amountRemaining = (deltaPour - this.volume);

                    drops[grabY(1), tileX].volume += this.volume;

                    if (amountRemaining > 16)
                        this.volume = (amountRemaining - 16);
                    else
                        this.volume = 0.0f;
                }

                // If bottom, left and right drops are full, push water into one above.
                /*
                if (this.volume >= 16.0 && drops[grabY(1), tileX].volume >= 16.0f && drops[grabY(0), grabX(-1)].volume >= 16.0f && drops[grabY(0), grabX(1)].volume >= 16.0f && tiles[tileY - 1, tileX].tileType == TileType.Empty)
                {
                    float deltaPour = (16 - drops[grabY(-1), tileX].volume);

                    // Delta between deltaPour and vol
                    float amountRemaining = (deltaPour - this.volume);

                    drops[grabY(-1), tileX].volume += this.volume;

                    if (amountRemaining > 16)
                        this.volume = (amountRemaining - 16);
                    else
                        this.volume = 0.0f;
                }*/
            }

            if (this.volume > 16.0f)            
                this.volume = 16.0f;
           
           // if (this.volume >= 16.0f && tiles[grabY(-1), tileX].tileType == TileType.Empty)
            //    topMost = true;

            if (grabY(-1) > 0)
            {
                if (drops[grabY(-1), tileX].volume <= 0.0f)
                    topMost = true;
                else
                    topMost = false;
            }
        }
    }
}
