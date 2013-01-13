using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Graven
{
    class GameObject
    {
        public enum ObjectType
        {
            Player,
            Enemy,
            Item
        }

        //public ObjectType objectType;
        public Vector2 position, velocity, accel, size, cameraPosition;
        public Rectangle boundingBox;
        public bool render;
        public int frameCount = 1, tileX, tileY, frameSize, timeSinceLastFrame, millisecondsPerFrame;
        public Vector2 currentFrame, sheetSize;
        public int totalHeight, totalWidth; // Total Height/Width of the map.

        public TileType nextPos;
        

        public  virtual void Update(GameTime gt)
        {
        }

        public void updateFrame(GameTime gt)
        {
            timeSinceLastFrame += gt.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame = 0;
                currentFrame.X++;
                if (currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                }
            }
        }
        public Rectangle getBounds()
        {
            return new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }
        public Vector2 getMiddle()
        {
            return new Vector2((float)position.X + (float)(size.X * 0.5), position.Y + (float)(size.Y * 0.5));
        }
        public bool checkBounding(GameObject gO)
        {
            if (getBounds().Intersects(gO.getBounds()))
                return true;
            else
                return false;
        }

        public bool checkCollidable(TileType tileIn)
        {
            nextPos = tileIn;
    
            if (tileIn == TileType.Tree || tileIn == TileType.Sand || tileIn == TileType.Dirt || tileIn == TileType.Metal)
                return true;
            else
                return false;

        }
        public int grabX(int tileX, int newX = 0)
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

        public int grabY(int tileY, int newY = 0)
        {
            if (tileY + newY <= 0) // Too far left
                return tileY;
            else if (tileY + newY >= totalHeight) // too far up
                return tileY;
            else
            {
                return (tileY + newY);
            }
        }

        public int getTileCoords(string val) {
            if (val == "X")
                return (int)Math.Round(position.X / size.X);
            else
                return (int)Math.Round(position.Y / size.Y);
        }

        public int getTileCoords(string val, int usedSize)
        {
            if (val == "X")
                return (int)Math.Round(position.X / usedSize);
            else
                return (int)Math.Round(position.Y / usedSize);
        }

        public int getTileCoords(float curPos, int usedSize)
        {
           return (int)Math.Round(curPos / usedSize);
        }

        public Vector2 GetIntersectionDepth(Rectangle rectA, Rectangle rectB)
        {
            // Calculate half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }
    }
}
