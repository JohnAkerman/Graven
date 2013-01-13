using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Graven
{
    public enum TileType
    {
        Empty,
        Decoration,
        Dirt,
        Sand,
        Tree,
        Metal,
    }

    public enum TileCollision
    {
        Passable,
        Impassable,
    }

    public enum TileOrientation
    {
        Regular = 0,
        Top = 1,
        TopRight = 2,
        Right = 3,
        BottomRight = 4,
        Bottom = 5,
        BottomLeft = 6,
        Left = 7,
        TopLeft = 8,
        LeftRight = 9,
        TopBottom = 10,
        TopBottomLeft = 11,
        TopBottomRight = 12,      
        TopLeftRight = 13,
        BottomLeftRight = 14,
        Single = 15,
    }

    class Tile : GameObject
    {
        public TileType tileType;
        public TileOrientation tileOrientation;
        public TileCollision tileCollision;

        float lastUpdate = 0;
        public float updateTime = 0.03f;
        public bool moveNext = false;
        public int decorationValue;
        public bool topMost = false, bottomMost = false, rightMost = false, leftMost = false;
        public int damage = 0, health = 100, hitsRequired = 5;
        public float lightValue = 0.0f;
        public int lightingHits = 0;
        public int lightRad;

        public Tile(TileType typeIn, int xIn, int yIn, int totalWidthIn, int totalHeightIn, Random rand, TileCollision collision, int decoration = -1) 
        {
            tileType = typeIn;
            size.X = size.Y = 16;
            position = new Vector2(xIn * size.X, yIn * size.Y);
            render = true;
            decorationValue = decoration;
            tileOrientation = TileOrientation.Regular;
            totalHeight = totalHeightIn;
            totalWidth = totalWidthIn;
            tileCollision = collision;

            if (tileType == TileType.Dirt)
            {
                hitsRequired = 1;
                if (rand.Next(1, 25) == 5)
                {
                    //damage = rand.Next(1, 3);
                    //  health = damage * 30;
                }
            }
            else if (tileType == TileType.Metal)
                hitsRequired = 5;
        }

        /*   Tile Possibilities
         *   Top
         *   Top Right
         *   Right
         *   Bottom Right
         *   Bottom
         *   Bottom Left
         *   Left
         *   Top Left
         *   
         *   Top Bottom
         *   Left Right
         *   
         */

        public bool doDamage()
        {
            if (tileType == TileType.Metal)
            {
                if (hitsRequired >= 1)
                    hitsRequired--;

                if (hitsRequired <= 0)
                {
                    health = 0;
                    tileType = TileType.Empty;
                    tileCollision = TileCollision.Passable;
                    return true;
                }

                if (health >= 0)
                {
                    health -= 20;

                    if (health < 90)
                        damage = 1;

                    if (health < 50)
                       damage = 2;

                    if (health < 35)
                        damage = 3;
                }
            }
            else if (tileType == TileType.Dirt || tileType == TileType.Sand)
            {
                tileType = TileType.Empty;
                tileCollision = TileCollision.Passable;
                return true;
            }

            return false;
            /*
            if (tiles[mouseY, mouseX].health >= 0)
            {
                tiles[mouseY, mouseX].health -= 20;

                if (tiles[mouseY, mouseX].health < 90)
                    tiles[mouseY, mouseX].damage = 1;

                if (tiles[mouseY, mouseX].health < 50)
                    tiles[mouseY, mouseX].damage = 2;

                if (tiles[mouseY, mouseX].health < 35)
                    tiles[mouseY, mouseX].damage = 3;
            }*/

           
        }
        public void calculateOrientation(ref Tile[,,]tiles)
        {
            tileX = getTileCoords("X");
            tileY = getTileCoords("Y");

            int tmpY = grabY(-1);
            int tmpY2 = grabY(1);
            int tmpX = grabX(-1);
            int tmpX2 = grabX(1);

            if (tmpY == -1 || tmpY2 == -1)
                return;
            if (tmpX == -1 || tmpX2 == -1)
               return;

            if (tileX == 0)
                tileOrientation = TileOrientation.Single;

            if (tiles[1, grabY(-1), tileX].tileType != tileType)
                topMost = true;
            else
                topMost = false;

            if (tiles[1, grabY(1), tileX].tileType != tileType)
                bottomMost = true;
            else
                bottomMost = false;

            if (tiles[1, tileY, grabX(1)].tileType != tileType)
                rightMost = true;
            else
                rightMost = false;

            if (tileX > 0 && tiles[1, tileY, grabX(-1)].tileType != tileType)
                leftMost = true;
            else
                leftMost = false;

            if (topMost && !rightMost && !leftMost && !bottomMost)
                tileOrientation = TileOrientation.Top;
            else if (topMost && rightMost && !leftMost && !bottomMost)
                tileOrientation = TileOrientation.TopRight;
            else if (!topMost && rightMost && !leftMost && !bottomMost)
                tileOrientation = TileOrientation.Right;
            else if (!topMost && rightMost && !leftMost && bottomMost)
                tileOrientation = TileOrientation.BottomRight;
            else if (!topMost && !rightMost && !leftMost && bottomMost)
                tileOrientation = TileOrientation.Bottom;
            else if (!topMost && !rightMost && leftMost && bottomMost)
                tileOrientation = TileOrientation.BottomLeft;
            else if (!topMost && !rightMost && leftMost && !bottomMost)
                tileOrientation = TileOrientation.Left;
            else if (topMost && !rightMost && leftMost && !bottomMost)
                tileOrientation = TileOrientation.TopLeft;
            else if (topMost && !rightMost && !leftMost && bottomMost)
                tileOrientation = TileOrientation.TopBottom;
            else if (!topMost && rightMost && leftMost && !bottomMost)
                tileOrientation = TileOrientation.LeftRight;
            else if (!topMost && !rightMost && !leftMost && !bottomMost)
                tileOrientation = TileOrientation.Single;
            else if (topMost && !rightMost && leftMost && bottomMost)
                tileOrientation = TileOrientation.TopBottomLeft;
            else if (topMost && rightMost && !leftMost && bottomMost)
                tileOrientation = TileOrientation.TopBottomRight;
            else if (topMost && rightMost && leftMost && !bottomMost)
                tileOrientation = TileOrientation.TopLeftRight;
            else if (!topMost && rightMost && leftMost && bottomMost)
                tileOrientation = TileOrientation.BottomLeftRight;
            else
                tileOrientation = TileOrientation.Regular;     
        }

        public int grabX(int newX)
        {
            if (tileX <= 0)
                return 0;
            else if (tileX == totalWidth-1)
                return totalWidth -1;


            if (tileX + newX < 0) // Too far left
                return -1;
            else if (tileX + newX >= totalWidth) // too far right
                return -1;
            else
                return (tileX + newX);
        }

        public int grabY(int newY)
        {
            if (tileY <= 0)
                return 0;
            else if (tileY >= totalHeight-1)
                return totalHeight - 1;

            if (tileY + newY < 0) // Too far left
                return -1;
            else if (tileY + newY >= totalHeight) // too far up
                return -1;
            else
                return (tileY + newY);
        }


        public void updateLight(ref Tile[,,] tiles, Player player, bool initialLoad = false) {
            resetLights(ref tiles, player);

            if (initialLoad)
            {
                for (int y = 0; y < totalHeight; y++)
                {
                    for (int x = 0; x < totalWidth; x++)
                    {
                        if (x > totalWidth - 1 || x < 0 || y + 1 > totalHeight - 1 || y < 0)
                            continue;

                        if (tiles[1, y, x].tileType == TileType.Empty || tiles[1, y, x].tileType == TileType.Decoration || tiles[1, y, x].tileType == TileType.Tree)
                        {
                            tiles[1, y, x].lightValue = 1;
                            tiles[1, y, x].lightRad = 5;
                            lightRadius(ref tiles, x, y, player);
                        }
                    }
                }
            } 
            else
                for (int x = (int)(player.position.X / size.X) - 50; x < (int)(player.position.X / size.X) + 50; x++)
                {
                    for (int y = (int)(player.position.Y / size.Y) - 50; y < (int)(player.position.Y / size.Y) + 50; y++)
                    {

                        if (x > totalWidth - 1 || x < 0 || y + 1 > totalHeight - 1 || y < 0)
                            continue;

                        if (tiles[1, y, x].tileType == TileType.Empty || tiles[1, y, x].tileType == TileType.Decoration || tiles[1, y, x].tileType == TileType.Tree)
                        {
                            tiles[1, y, x].lightValue = 1;
                            tiles[1, y, x].lightRad = 5;
                            lightRadius(ref tiles, x, y, player);
                        }
                    }
                }
        }

        public void resetLights(ref Tile[,,] tiles, Player player) {
            for (int x = (int)(player.position.X / size.X) - 50; x < (int)(player.position.X / size.X) + 50; x++)
                for (int y = (int)(player.position.Y / size.Y) - 50; y < (int)(player.position.Y / size.Y) + 50; y++)
                {
                    if (x > totalWidth - 1 || x < 0 || y > totalHeight - 1 || y < 0)
                        continue;
                    //if (tiles[1, y, x].tileType == TileType.Torch)
                   //     continue;
                    tiles[1, y, x].lightValue = 0;
                }
            player.lightValue = 0;
        }

        public void lightRadius(ref Tile[,,] tiles, int tileX, int tileY, Player player)
        {
            int radius = tiles[1, tileY, tileX].lightRad;
            float masterLight = tiles[1, tileY, tileX].lightValue;

            for (int x = tileX - radius; x < tileX + radius + 1; x++)
            {
                for (int y = tileY - radius; y < tileY + radius + 1; y++)
                {
                    //Make sure we don't exceed the boundaries of the world array.
                    if (x < totalWidth - 1 && x > 0 && y < totalHeight - 1 && y > 0)
                    {
                        if (x == tileX && y == tileY || tiles[1,y,x].tileType == TileType.Decoration) // || tiles[1, y,x].tileType == TileType.Torch)
                            continue;

                        //Checks that we're still within the radius of the original tile.
                        if ((new Vector2(tileX, tileY) - new Vector2(x, y)).Length() <= radius)
                        {
                            float tmpLightLevel = 0;

                            //If the tile isn't solid.
                           // if (tiles[1, y, x].tileType == TileType.Empty)
                          //  {
                                //     tmpLightLevel = (short)(tiles[1, y, x].lightValue - ((new Vector2(tileX, tileY) - new Vector2(x, y)).Length() * 10));
                                //  else //else if the tile is solid.
                                tmpLightLevel = Math.Abs(masterLight - ((new Vector2(tileX, tileY) - new Vector2(x, y)).Length()) / 5);
                          //  }
                            if (tiles[1, y, x].lightValue < tmpLightLevel)
                                tiles[1, y, x].lightValue = tmpLightLevel;
                            if (tiles[1, y, x].lightValue < 0.0)
                                tiles[1, y, x].lightValue = 0;
                        }
                    }
                }
            }
            //Apply light to the player if he is within the light radius.
            if ((new Vector2(tileX, tileY) - (player.position / size.X)).Length() < radius)
                if (player.lightValue < tiles[1, tileY, tileX].lightValue / (int)((new Vector2(tileX, tileY) - (player.position / size.X)).Length() + 1))
                    player.lightValue = (ushort)(tiles[1, tileY, tileX].lightValue / (int)((new Vector2(tileX, tileY) - (player.position / size.X)).Length() + 1));
        }


        int divierAmount = 4;
        /*
        public void updateLighting(ref Tile[, ,] tiles, float addedDark = -1)
        {
            if (this.tileType == TileType.Empty || this.tileType == TileType.Tree)
               lightValue = 1.0f;

            if (lightingHits > 4)
                return;

            if (addedDark != -1)
                this.lightValue += addedDark;

            lightingHits++;

            //   if (tileY > 0 && this.tileType == TileType.Empty)
       //         tiles[1, tileY - 1, tileX].updateLighting(ref tiles, lightValue / divierAmount);
            
            if (tileY+1 < totalHeight)
                tiles[1, tileY + 1, tileX].updateLighting(ref tiles, lightValue / divierAmount);

        //    if (tileX > 0)
        //        tiles[1, tileY, tileX - 1].updateLighting(ref tiles, lightValue / divierAmount);

            if (tileX+1 < totalWidth)
                tiles[1, tileY, tileX + 1].updateLighting(ref tiles, lightValue / divierAmount);

          //  if (tileX > 0 && tileY > 0)
          //      tiles[1, tileY - 1, tileX - 1].updateLighting(ref tiles, lightValue / divierAmount);

            if (tileX+1 < totalWidth && tileY+1 < totalHeight)
                tiles[1, tileY + 1, tileX + 1].updateLighting(ref tiles, lightValue / divierAmount);
        }
        */
        public void updateTile(ref Tile[,,] tiles, float elapsedTime, Vector2 camPos)
        {

            tileX = getTileCoords("X");
            tileY = getTileCoords("Y");

            if (this.tileType != TileType.Sand) return;

            if ((elapsedTime - lastUpdate) < this.updateTime) { return; }
            lastUpdate = elapsedTime;

            if (moveNext)
            {
                tiles[1, tileY + 1, tileX].tileType = TileType.Sand;
                tiles[1, tileY, tileX].tileType = TileType.Empty;
                calculateOrientation(ref tiles);
                moveNext = false;
            }

            if (tileY <= 37)
            {
                if (tiles[1, tileY + 1, tileX].tileType == TileType.Empty)
                {
                    moveNext = true;
                }
                else
                    moveNext = false;
            }
        }
    }
}
