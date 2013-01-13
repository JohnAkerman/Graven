using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Graven
{
   
    public class Game : Microsoft.Xna.Framework.Game
    {
        
        #region Initialisations.

        FrameRateCounter fpsCount;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Rectangle screenRectangle;
        const int screenHeight = 800;
        const int screenWidth = 1200;
        const int screenHeightHalf = screenHeight / 2;
        const int screenWidthHalf = screenWidth / 2;

        float elapsed, totalElapsed = 1.25f, pressCheckDelay = 1.25f, waterCheck = 0;

        Texture2D spadeIcon, treeTex, inventoryTex, mouseTex;
        
        //Tile[,] tiles;
        Layer[] layers;
        Level level;
        Random rand = new Random();
        Player player;
        Camera camera;

        int prevMouseScroll, mouseScroll;

        bool pauseWater = false;
        bool DEBUG = true;
        public static SpriteFont font;

        float frameCount, timeCount, fps, sinceLast;

        Color textRed = new Color(255, 255, 255);
        WaterDrop[,] droplets;

        float dropCount;

        #endregion

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            Window.Title = "Graven v0.01";

            screenRectangle = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            
            level = new Level(screenHeight, screenWidth);
            layers = new Layer[1];
            camera = new Camera(new Vector2(0, 0));
            fpsCount = new FrameRateCounter();
          }

        protected override void Initialize()
        {
            font = Content.Load<SpriteFont>("font");
            spadeIcon = Content.Load<Texture2D>("spadeIcon");
            treeTex = Content.Load<Texture2D>("tree");
            inventoryTex = Content.Load<Texture2D>("inventory");
            mouseTex = Content.Load<Texture2D>("mouse");
            
            base.Initialize();
        }

        private void UpdateMouse()
        {           
            int mouse_x = Mouse.GetState().X - 4 + (int)camera.position.X;
            int mouse_y = Mouse.GetState().Y - 4 + (int)camera.position.Y;

            if (mouse_x > 0 && mouse_x < level.levelWidth && mouse_y > 0 && mouse_y < level.levelHeight)
            {
                if (player.inventoryTypes[player.activeInventorySlot] == Player.InventoryType.Spade && 
                    level.checkBreakable(level.tileLayers[1, mouse_y / 16, mouse_x / 16].tileType))
                    spriteBatch.Draw(spadeIcon, new Vector2(mouse_x, mouse_y-8) - camera.position, Color.White);
                else
                    spriteBatch.Draw(mouseTex, new Vector2(mouse_x, mouse_y) - camera.position, Color.White);
            }
        }

        //public void setupDrops()
        //{
        //    droplets = new WaterDrop[levelOne.Height, levelOne.Width];
        //    Random rand = new Random();
        //    for (int y = 0; y < levelOne.Height; y++)
        //    {
        //        for (int x = 0; x < levelOne.Width; x++)
        //        {
        //            droplets[y, x] = new WaterDrop(graphics.GraphicsDevice, x, y, levelWidth, levelHeight, 0);
        //        }
        //    }
        //}

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);


            level.loadTextures(Content);
            player = new Player(screenRectangle, level.levelHeight, level.levelWidth);
            player.texture = Content.Load<Texture2D>("player");
            level.setUpTile("Levels/levelOne.png", player, camera);
            

            layers[0] = new Layer(Content, "backgrounds/clouds", 0.1f);
           // layers[1] = new Layer(Content, "backgrounds/hills", 0.3f);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
           
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            totalElapsed += elapsed;
            
            /* FPS */
            frameCount++;
            sinceLast = gameTime.ElapsedGameTime.Milliseconds;
            timeCount += sinceLast;
            if (timeCount >= 1000)
            {
                fps = frameCount;
                frameCount = 0;
                timeCount = 0;
            }
     
            #region Keyboard Checking

            KeyboardState keyS = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

           int mouseScroll = (int)Mouse.GetState().ScrollWheelValue / 120;
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyS.IsKeyDown(Keys.Escape))
                this.Exit();

            if ((totalElapsed - waterCheck) > 0.2f)
            {
                if (keyS.IsKeyDown(Keys.X))
                {
                    waterCheck = totalElapsed;
                   // makeDrop(14.0f);
                }
                else if (keyS.IsKeyDown(Keys.C))
                {
                    waterCheck = totalElapsed;
                   // makeDrop(0.0f);
                }
                else if (keyS.IsKeyDown(Keys.Delete))
                {
                    waterCheck = totalElapsed;
                    //clearDrops();
                }

                else if (keyS.IsKeyDown(Keys.R))
                {
                    waterCheck = totalElapsed;
                    //clearDrops();
                    level.setUpTile("Levels/levelOne.png", player, camera);
                }
                else if (keyS.IsKeyDown(Keys.T))
                {
                    waterCheck = totalElapsed;
                    pauseWater = !pauseWater;
                }

                else if (keyS.IsKeyDown(Keys.D1) )
                {
                    waterCheck = totalElapsed;
                    player.activeInventorySlot = 0;
                    level.setTile(TileType.Empty, camera, player);
                }
                else if (keyS.IsKeyDown(Keys.D2))
                {
                    waterCheck = totalElapsed;
                    player.activeInventorySlot = 1;
                    level.setTile(TileType.Decoration, camera, player, 0);
                }
                else if (keyS.IsKeyDown(Keys.D3) || Mouse.GetState().RightButton == ButtonState.Pressed)
                {
                    waterCheck = totalElapsed;
                    player.activeInventorySlot = 2;
                  /*  if (player.blockCount > 0)
                    {
                        setTile(TileType.Dirt);
                        player.blockCount--;
                    }
                    */

                }
                else if (keyS.IsKeyDown(Keys.D4))
                {
                    player.activeInventorySlot = 3;
                    waterCheck = totalElapsed;
                    level.setTile(TileType.Sand, camera, player);
                }
                else if (keyS.IsKeyDown(Keys.D5))
                {
                    player.activeInventorySlot = 4;
                    waterCheck = totalElapsed;
                    level.setTile(TileType.Decoration, camera, player, 1);
                }
                else if (prevMouseScroll != mouseScroll)
                {
                    player.activeInventorySlot += (prevMouseScroll - mouseScroll);

                    if (player.activeInventorySlot > 4)
                        player.activeInventorySlot = 4;
                    else if (player.activeInventorySlot < 0)
                        player.activeInventorySlot = 0;
                }

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    switch (player.inventoryTypes[player.activeInventorySlot])
                    {
                        case Player.InventoryType.Spade:
                            level.setTileHealth(camera, player);
                            break;
                        case Player.InventoryType.DirtTile:
                            if (player.inventoryCount[player.activeInventorySlot] > 0)
                            {
                                if (level.setTile(TileType.Dirt, camera, player))
                                    player.inventoryCount[player.activeInventorySlot]--;
                            }
                            break;

                        case Player.InventoryType.MetalTile:
                            if (player.inventoryCount[player.activeInventorySlot] > 0)
                            {
                                if (level.setTile(TileType.Metal, camera, player))
                                    player.inventoryCount[player.activeInventorySlot]--;
                            }
                            break;
                    }
                }
            }

            if (totalElapsed >= pressCheckDelay)
            {
                pressCheckDelay = totalElapsed;
              /*
               * if (keyS.IsKeyDown(Keys.Left))
                {
                    waterCheck = totalElapsed;
                    if (cameraPosition.X > 32)
                        cameraPosition.X -= 32;
                    else
                        cameraPosition.X = 0;
                }
                else if (keyS.IsKeyDown(Keys.Right))
                {
                    waterCheck = totalElapsed;
                    if (cameraPosition.X + 32 < totalWidth - screenWidth)
                        cameraPosition.X += 32;
                    else
                        cameraPosition.X = totalWidth - screenWidth;
                }
               * 
                //player.checkKeys(keyS, cameraPosition);
               */

                if (keyS.IsKeyDown(Keys.D))
                {
                    if (player.position.X - camera.position.X >= 800)
                        camera.position.X += 2;
                }

                if (keyS.IsKeyDown(Keys.A))
                {
                    if (player.position.X - camera.position.X >= 100)
                    {
                        if (camera.position.X > 0)
                            camera.position.X -= 2;
                    }
                }
            }

            #endregion

            #region Update Water and Tiles

            //if (!pauseWater)
            //{
            //    for (int y = 0; y < levelHeight - 1; y++)
            //    {
            //        for (int x = 0; x < levelWidth - 1; x++)
            //        {
            //            droplets[y, x].Update(ref tiles, ref droplets, totalElapsed);
            //        }
            //    }
            //}

            level.updateTiles(totalElapsed, camera);

            #endregion

            player.Update(ref level.tileLayers, keyS, gameTime, camera.position);

            if (camera.getCameraX(0) >= 0)
                camera.position.X = player.position.X - screenWidthHalf;
            else
                camera.position.X = 0;

            if (camera.getCameraY(0) >= 0)
                camera.position.Y = player.position.Y - screenHeightHalf;
            else
                camera.position.Y = 0;

            player.updateInventory();

            prevMouseScroll = mouseScroll;

            fpsCount.Update(gameTime);
            base.Update(gameTime);
        }

        public void drawInventory()
        {
            int xOffset = 300;
            int yOffset = 10;
            for (int i = 0; i < player.inventoryTypes.Length; i++)
            {
                switch(player.inventoryTypes[i])
                {
                    case Player.InventoryType.Empty: 
                        spriteBatch.Draw(inventoryTex, new Vector2(xOffset + (60 * i), yOffset), new Rectangle(0, 0, 60, 56), Color.White);
                        break;
                    case Player.InventoryType.DirtTile:
                        spriteBatch.Draw(inventoryTex, new Vector2(xOffset + (60 * i), yOffset), new Rectangle(60, 0, 60, 56), Color.White);
                        spriteBatch.DrawString(font, player.inventoryCount[i].ToString(), new Vector2(xOffset + (60 * i) + 50, yOffset + 50), Color.Black);
                        break;
                    case Player.InventoryType.Spade:
                        spriteBatch.Draw(inventoryTex, new Vector2((int)xOffset + (60 * i), yOffset), new Rectangle(120, 0, 60, 56), Color.White);
                        break;
                    case Player.InventoryType.MetalTile:
                        spriteBatch.Draw(inventoryTex, new Vector2((int)xOffset + (60 * i), yOffset), new Rectangle(240, 0, 60, 56), Color.White);
                        spriteBatch.DrawString(font, player.inventoryCount[i].ToString(), new Vector2(xOffset + (60 * i) + 50, yOffset + 50), Color.Black);
                        break;
                }
            }

            spriteBatch.Draw(inventoryTex, new Vector2(xOffset + (60 * player.activeInventorySlot), yOffset), new Rectangle(180, 0, 60, 56), Color.White);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            for (int i = 0; i < layers.Length; i++)
            {
              layers[i].Draw(spriteBatch, camera.position);
            }

            level.drawTiles(camera, spriteBatch);

            player.Draw(spriteBatch);
            //drawDroplets();
            UpdateMouse();
            drawInventory();
            if (DEBUG)
               DrawDebug();

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void DrawDebug()
        {
            fpsCount.Draw(spriteBatch, font);
            spriteBatch.DrawString(font, "Droplets: " + dropCount.ToString(), new Vector2(10, 25), textRed);
            spriteBatch.DrawString(font, "Water: " + (pauseWater ? "Paused" : "Running"), new Vector2(10, 40), textRed);
            spriteBatch.DrawString(font, "Camera : " + camera.position.ToString(), new Vector2(10, 55), textRed);
            spriteBatch.DrawString(font, "Player : " + player.position.ToString() + " Velo " + player.velocity.ToString(), new Vector2(10, 70), textRed);
            spriteBatch.DrawString(font, "Diff : " + (player.position.X - camera.position.X), new Vector2(10, 85), textRed);
            spriteBatch.DrawString(font, "Player CAM : " + player.cameraPosition.ToString(), new Vector2(10, 100), textRed);
            spriteBatch.DrawString(font, "Block Count : " + player.blockCount.ToString(), new Vector2(10, 115), textRed);
            spriteBatch.DrawString(font, "Scroll : " + mouseScroll.ToString(), new Vector2(10, 145), textRed);
        }
        //public void countDrops()
        //{
        //    dropCount = 0;

        //    for (int y = getCameraPosY(0); y < getCameraPosY(screenHeight) - 1; y++)
        //    {
        //        for (int x = getCameraPosX(0); x < getCameraPosX(screenWidth) - 1; x++)
        //        {
        //            dropCount += droplets[y, x].volume;
        //        }
        //    }
        //}

        //public void clearDrops()
        //{
        //    for (int y = 0; y < levelHeight; y++)
        //    {
        //        for (int x = 0; x < levelWidth; x++)
        //        {
        //            droplets[y, x].volume = 0;
        //        }
        //    }
        //}

        //public void drawDroplets()
        //{
        //    for (int y = getCameraPosY(0); y < getCameraPosY(screenHeight); y++)
        //    {
        //        for (int x = getCameraPosX(0); x < getCameraPosX(screenWidth); x++)
        //        {
        //            if (droplets[y,x].topMost && droplets[y,x].waterSand)
        //                droplets[y, x].Draw(cameraPosition, spriteBatch, ref waterSandTop);
        //            else if (droplets[y, x].topMost && droplets[y, x].waterSand == false)
        //                droplets[y, x].Draw(cameraPosition, spriteBatch, ref topMost);
        //           else if (droplets[y,x].waterSand &&  droplets[y,x].topMost == false)
        //                droplets[y, x].Draw(cameraPosition, spriteBatch, ref waterSand);
        //           else
        //                droplets[y, x].Draw(cameraPosition, spriteBatch, ref WaterDropTex);
        //        }
        //    }
        //}

        //public void makeDrop(float amount)
        //{
        //    int mouse_x = Mouse.GetState().X - 4 + (int)cameraPosition.X;
        //    int mouse_y = Mouse.GetState().Y - 4;

        //    if (mouse_x > 0 && mouse_x < totalWidth && mouse_y > 0 && mouse_y < totalHeight)
        //    {
        //        droplets[mouse_y / 16, mouse_x / 16].volume = amount;
        //    }
        //}

        public int grabX(int tileX, int newX = 0)
        {
            if (tileX + newX < 0) // Too far left
                return tileX;
            else if (tileX + newX >= level.levelWidth) // too far right
                return tileX;
            else
            {
                return (tileX + newX);
            }
        }

        public int grabY(int tileY, int newY = 0)
        {
            if (tileY + newY < 0) // Too far left
                return tileY;
            else if (tileY + newY >= level.levelHeight) // too far up
                return tileY;
            else
            {
                return (tileY + newY);
            }
        }
    }
}
