using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Graven
{
    class Player : GameObject
    {
        private const int termVelo = 15, maxSpeed = 3, jumpHeight = 8;
        public Texture2D texture;
        public float moveSpeed = 2.5f, timeGone = 1f, gravity = 1f;
        bool facingLeft = false;
        public verticalState vertState;
        private float previousBottom;


        private const float MoveAcceleration = 13000.0f;
        private const float MaxMoveSpeed = 1750.0f;
        private const float GroundDragFactor = 0.48f;
        private const float AirDragFactor = 0.58f;

        // Constants for controlling vertical movement
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -2000.0f;
        private const float GravityAcceleration = 3400.0f;
        private const float MaxFallSpeed = 550.0f;
        private const float JumpControlPower = 0.14f;
        float jumpTime = 0;
        float movement = 0;
        private bool isJumping;
        private bool wasJumping;
        private bool isOnGround;
        public int blockCount = 0;

        public int[] inventoryCount = new int[5];
        public InventoryType[] inventoryTypes = new InventoryType[5];
        public int activeInventorySlot = 0;

        public enum InventoryType {
            Empty,
            Spade,
            DirtTile,
            MetalTile,
            WaterBlock,
        }

        public void addInventory(int amount, InventoryType typeIn)
        {
            bool found = false;
            for (int i = 0; i < inventoryTypes.Length; i++)
            {
                if (typeIn == inventoryTypes[i]) // New item is already in inventory;
                {
                    inventoryCount[i] += amount;
                    return;
                }
            }

            // Non already in the inventory, select first empty slot and fill
            if (found == false)
            {
                for (int i = 0; i < inventoryTypes.Length; i++)
                {
                    if (inventoryTypes[i] == InventoryType.Empty)
                    {
                        inventoryTypes[i] = typeIn;
                        inventoryCount[i] += amount;
                        break;
                    }
                }
            }
        }

        public void updateInventory()
        {
            for (int i = 0; i < inventoryCount.Length; i++)
            {
                if (inventoryCount[i] == 0 && (inventoryTypes[i] == InventoryType.WaterBlock || inventoryTypes[i] == InventoryType.MetalTile || inventoryTypes[i] == InventoryType.DirtTile))
                {
                    inventoryTypes[i] = InventoryType.Empty;
                }
            }
        }

        public void selectSlot(int toSelect)
        {
            activeInventorySlot = toSelect;
        }
        
        public enum verticalState
        {
            Falling,
            Jumping,
            Ground,
            Jumped
        }

        public Player(Rectangle screenBounds, int totalHeightIn, int totalWidthIn)
        {
            boundingBox = screenBounds;
            initialPosition();
            accel = new Vector2(0.5f, 0);
            velocity = Vector2.Zero;
            vertState = verticalState.Falling;
            size.X = 16;
            size.Y = 32;
            totalHeight = totalHeightIn;
            totalWidth = totalWidthIn;
            inventoryTypes[0] = InventoryType.Spade;
        }

        public void initialPosition()
        {
            this.position.X = 150;
            this.position.Y = 150;
            tileX = getTileCoords("X");
            tileY = getTileCoords("Y");
        }

        public void Draw(SpriteBatch sB)
        {
            sB.Draw(texture, position - cameraPosition, new Rectangle(0, 0, (int)size.X, (int)size.Y), Color.White, 0f, new Vector2(0, 0), 1.0f, (facingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 0f);   
        }

        public void checkKeys(KeyboardState kS, Vector2 cameraPos)
        {
        }

        private Vector2 prevPos;
        public void Update(ref Tile[,,] tiles, KeyboardState keyboard, GameTime gt, Vector2 camPos)
        {
            cameraPosition = camPos;
            prevPos = position;
            timeGone = (float)gt.ElapsedGameTime.TotalSeconds;

            tileX = getTileCoords("X");
            tileY = getTileCoords("Y");

            movement = 0;

            if (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.W))
                isJumping = true;
            else
                isJumping = false;
           
            if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D)) // Move Right
            {
                if ( position.X < totalWidth * 16)
                    movement = 1.0f;
                facingLeft = false;

            }
            else if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A)) // Skid Left
            {
                if (position.X > 0)
                    movement = -1.0f;
                facingLeft = true;
            }     

            velocity.X += movement * MoveAcceleration * timeGone;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * timeGone, -MaxFallSpeed, MaxFallSpeed);
            velocity.Y = DoJump(velocity.Y, gt);

            if (isOnGround)
                velocity.X *= GroundDragFactor;
            else
                velocity.X *= AirDragFactor;

            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            position += velocity * timeGone;
            position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));

            HandleCollisions(ref tiles);

            if (position.X == prevPos.X)
                velocity.X = 0;

            if (position.Y == prevPos.Y)
                velocity.Y = 0;

            movement = 0.0f;
            isJumping = false;
        }

        private float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (isJumping)
            {
                // Begin or continue a jump
                if ((!wasJumping && isOnGround) || jumpTime > 0.0f)
                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;

            return velocityY;
        }

        public void setPosition(int x, int y)
        {
            position = new Vector2(x, y);
        }

        private void HandleCollisions(ref Tile[,,] tiles)
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = getBounds();
            int leftTile = (int)Math.Floor((float)bounds.Left / 16);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / 16)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / 16);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / 16)) - 1;


            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    if (x < 0 || x > totalWidth || y < 0 || y > totalHeight) 
                        continue;
                    // If this tile is collidable,
                    TileCollision collision = tiles[1, y, x].tileCollision;

                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = tiles[1,y, x].getBounds();

                        Vector2 depth = GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY <= absDepthX)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                    isOnGround = true;

                                // Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable || vertState == verticalState.Ground)
                                {
                                    // Resolve the collision along the Y axis.
                                    position = new Vector2(position.X, position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = getBounds();
                                }
                            }
                            else if (collision == TileCollision.Impassable) // Ignore platforms.
                            {
                                // Resolve the collision along the X axis.
                                position = new Vector2(position.X + depth.X, position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = getBounds();
                            }
                        }
                    }
                }
            }

            // Save the new bounds bottom.
            previousBottom = bounds.Bottom;
        }

    }
}
