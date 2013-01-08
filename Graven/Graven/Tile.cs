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
        public void calculateOrientation(ref Tile[,]tiles)
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

            if (tiles[grabY(-1), tileX].tileType != tileType)
                topMost = true;
            else
                topMost = false;

            if (tiles[grabY(1), tileX].tileType != tileType)
                bottomMost = true;
            else
                bottomMost = false;

            if (tiles[tileY, grabX(1)].tileType != tileType)
                rightMost = true;
            else
                rightMost = false;

            if (tileX > 0 && tiles[tileY, grabX(-1)].tileType != tileType)
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

        public void updateTile(ref Tile[,] tiles, float elapsedTime, Vector2 camPos)
        {
            //cameraPosition = camPos;

            tileX = getTileCoords("X");
            tileY = getTileCoords("Y");

            if (this.tileType != TileType.Sand) return;

            if ((elapsedTime - lastUpdate) < this.updateTime) { return; }
            lastUpdate = elapsedTime;

            if (moveNext)
            {
                tiles[tileY + 1, tileX].tileType = TileType.Sand;
                tiles[tileY, tileX].tileType = TileType.Empty;
                calculateOrientation(ref tiles);
                moveNext = false;
            }

            if (tileY <= 37)
            {
                if (tiles[tileY + 1, tileX].tileType == TileType.Empty)
                {
                    moveNext = true;
                }
                else
                    moveNext = false;
            }
        }
    }
}
