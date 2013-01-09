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
   
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        
        #region Initialisations.

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Rectangle screenRectangle;
        Texture2D tileGrassTop, tileDirt, levelOne, backgroundTex, WaterDropTex, mouseTex, sandTex, waterSand, topMost, waterSandTop, spadeIcon,treeTex, tileMetal, inventoryTex, cloudTex, bgPara, hillsTex;
        const int screenHeight = 600;
        const int screenWidth = 1000;
        int levelHeight = 38;
        int levelWidth = 50;
        int totalWidth = 0, totalHeight = 0;
        float elapsed, totalElapsed = 1.25f, pressCheckDelay = 1.25f, waterCheck = 0;
        Tile[,] tiles;
        Random rand = new Random();
        Player player;
        int prevMouseScroll, mouseScroll;

        bool pauseWater = false;
        bool DEBUG = true;
        public static SpriteFont font;

        float frameCount, timeCount, fps, sinceLast;

        public Vector2 cameraPosition;
        
        Color textRed = new Color(99, 99, 99);
        WaterDrop[,] droplets;

        float dropCount;

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            Window.Title = "Graven v0.01";

            screenRectangle = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            cameraPosition = new Vector2(0, 0);
           }

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void resetTiles()
        {
            tiles = new Tile[levelHeight, levelWidth];
            for (int y = 0; y < levelHeight; y++)
            {
                for (int x = 0; x < levelWidth; x++)
                {
                    tiles[y, x] = new Tile(TileType.Empty, x, y, levelWidth, levelHeight, rand, TileCollision.Passable);
                }
            }

            calculateTiles();
        }

        private void setUpTile(string Path)
        {
         //   resetTiles();
            Color[] levelData = new Color[levelOne.Height * levelOne.Width];

            tiles = null;
            tiles = new Tile[levelOne.Height, levelOne.Width]; // Height, Width

            levelOne.GetData<Color>(levelData);

            int runningTotal = 0;

            for (int y = 0; y < levelOne.Height; y++)
            {
                for (int x = 0; x < levelOne.Width; x++)
                {
                    if (runningTotal >= levelData.Length)
                    {
                        break;
                    }

                    if (levelData[runningTotal] == Color.Lime)
                    {
                        tiles[y, x] = new Tile(TileType.Decoration, x, y, levelWidth, levelHeight, rand, 0);
                    }
                    else if (levelData[runningTotal] == Color.Blue)
                    {
                        tiles[y, x] = new Tile(TileType.Dirt, x, y, levelWidth, levelHeight, rand, TileCollision.Impassable);
                    }
                    else if (levelData[runningTotal] == Color.White)
                    {
                        tiles[y, x] = new Tile(TileType.Empty, x, y, levelWidth, levelHeight, rand, TileCollision.Passable);
                    }
                    else if (levelData[runningTotal] == Color.Yellow)
                    {
                        tiles[y, x] = new Tile(TileType.Sand, x, y, levelWidth, levelHeight, rand, TileCollision.Impassable);
                    }
                    else if (levelData[runningTotal] == new Color(204,204,204))
                    {
                        tiles[y, x] = new Tile(TileType.Decoration, x, y, levelWidth, levelHeight, rand,TileCollision.Passable,  1);
                    }
                    else if (levelData[runningTotal] == new Color(53, 34, 18)) // Tree Foot
                    {
                        tiles[y, x] = new Tile(TileType.Tree, x, y, levelWidth, levelHeight, rand, TileCollision.Passable, 0);
                    }
                    else if (levelData[runningTotal] == new Color(109, 69, 35)) // Tree Truck
                    {
                        tiles[y, x] = new Tile(TileType.Tree, x, y, levelWidth, levelHeight, rand, TileCollision.Passable, 1);
                    }
                    else if (levelData[runningTotal] == new Color(24, 100, 12)) // Tree Top
                    {
                        tiles[y, x] = new Tile(TileType.Tree, x, y, levelWidth, levelHeight, rand, TileCollision.Passable, 2);
                    }
                    else if (levelData[runningTotal] == new Color(119, 119, 119)) // Metal
                    {
                        tiles[y, x] = new Tile(TileType.Metal, x, y, levelWidth, levelHeight, rand, TileCollision.Impassable);
                    }
                    runningTotal++;
                }
            }

           
            calculateTiles();
        }

        public bool checkBreakable(TileType tileIn) {
            if (tileIn == TileType.Tree || tileIn == TileType.Sand || tileIn == TileType.Dirt || tileIn == TileType.Metal) 
                return true;
            else 
                return false;
        }


        private void UpdateMouse()
        {
            int mouse_x = Mouse.GetState().X -4;
            int mouse_y = Mouse.GetState().Y - 4;

            if (mouse_x > 0 && mouse_x < totalWidth && mouse_y > 0 && mouse_y < totalHeight)
            {
                if (player.inventoryTypes[player.activeInventorySlot] == Player.InventoryType.Spade && checkBreakable(tiles[mouse_y / 16, mouse_x / 16].tileType))
                    spriteBatch.Draw(spadeIcon, new Vector2(mouse_x, mouse_y-8), Color.White);
                else
                    spriteBatch.Draw(mouseTex, new Vector2(mouse_x, mouse_y), Color.White);

               // spriteBatch.DrawString(font, "Tile Health: " + tiles[mouse_y / 16, mouse_x / 16].health.ToString(), new Vector2(10, 80), textRed);
            }
        }

        public void setupDrops()
        {
            droplets = new WaterDrop[levelOne.Height, levelOne.Width];
            Random rand = new Random();
            for (int y = 0; y < levelOne.Height; y++)
            {
                for (int x = 0; x < levelOne.Width; x++)
                {
                    droplets[y, x] = new WaterDrop(graphics.GraphicsDevice, x, y, levelWidth, levelHeight, 0);
                }
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
           
            levelOne = Content.Load<Texture2D>("levelOne");
            tileGrassTop = Content.Load<Texture2D>("tileA");
            tileDirt = Content.Load<Texture2D>("tileDirt");
            tileMetal = Content.Load<Texture2D>("tileMetal");
            sandTex = Content.Load<Texture2D>("sand");
            backgroundTex = Content.Load<Texture2D>("bg");
            WaterDropTex = Content.Load<Texture2D>("waterdrop");
            mouseTex = Content.Load<Texture2D>("mouse");
            font = Content.Load<SpriteFont>("font");
            waterSand = Content.Load<Texture2D>("watersand");
            topMost = Content.Load<Texture2D>("topmost");
            waterSandTop = Content.Load<Texture2D>("watersandtop");
            spadeIcon = Content.Load<Texture2D>("spadeIcon");
            treeTex = Content.Load<Texture2D>("tree");
            inventoryTex = Content.Load<Texture2D>("inventory");
            cloudTex = Content.Load<Texture2D>("cloud1");
            bgPara = Content.Load<Texture2D>("bg-parallax");
            hillsTex = Content.Load<Texture2D>("hillsTex");

            levelHeight = levelOne.Height;
            levelWidth = levelOne.Width;
            totalWidth = levelWidth * 16;
            totalHeight = levelHeight * 16;
            player = new Player(screenRectangle, totalHeight, totalWidth);
            player.texture = Content.Load<Texture2D>("player");
            setUpTile("levelOne.png");
            setupDrops();
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
                    makeDrop(14.0f);
                }
                else if (keyS.IsKeyDown(Keys.C))
                {
                    waterCheck = totalElapsed;
                    makeDrop(0.0f);
                }
                else if (keyS.IsKeyDown(Keys.Delete))
                {
                    waterCheck = totalElapsed;
                    clearDrops();
                }

                else if (keyS.IsKeyDown(Keys.R))
                {
                    waterCheck = totalElapsed;
                    clearDrops();
                    setUpTile("levelOne.png");
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
//                   setTile(TileType.Empty);
                }
                else if (keyS.IsKeyDown(Keys.D2))
                {
                    waterCheck = totalElapsed;
                    player.activeInventorySlot = 1;
                   // setTile(TileType.Decoration, 0);
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
                   // setTile(TileType.Sand);
                }
                else if (keyS.IsKeyDown(Keys.D5))
                {
                    player.activeInventorySlot = 4;
                    waterCheck = totalElapsed;
                   // setTile(TileType.Decoration, 1);
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
                            setTileHealth();
                            break;
                        case Player.InventoryType.DirtTile:
                            if (player.inventoryCount[player.activeInventorySlot] > 0)
                            {
                                if (setTile(TileType.Dirt))
                                    player.inventoryCount[player.activeInventorySlot]--;
                            }
                            break;

                        case Player.InventoryType.MetalTile:
                            if (player.inventoryCount[player.activeInventorySlot] > 0)
                            {
                                if (setTile(TileType.Metal))
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
                    if (player.position.X - cameraPosition.X >= 800)
                      cameraPosition.X += 2;
                }

                if (keyS.IsKeyDown(Keys.A))
                {
                    if (player.position.X - cameraPosition.X >= 100)
                    {
                        if (cameraPosition.X > 0)
                           cameraPosition.X-=2;
                    }
                }
            }

            #endregion

            #region Update Water and Tiles

            if (!pauseWater)
            {
                for (int y = 0; y < levelHeight - 1; y++)
                {
                    for (int x = 0; x < levelWidth - 1; x++)
                    {
                        droplets[y, x].Update(ref tiles, ref droplets, totalElapsed);
                    }
                }
            }

            for (int y = 0; y < levelHeight - 1; y++)
            {
                for (int x = 0; x < levelWidth - 1; x++)
                {
                    tiles[y, x].updateTile(ref tiles, totalElapsed, cameraPosition);
                }
            }

            #endregion

            player.Update(ref tiles, keyS, gameTime, cameraPosition);
            player.updateInventory();

            prevMouseScroll = mouseScroll;
            base.Update(gameTime);
        }

        public int getCameraPosX(int widthIn) {
            return (int)(Math.Round((cameraPosition.X + widthIn) * 0.0625)); 
        }

        public int getCameraPosY(int heightIn)
        {
            return (int)(Math.Round((cameraPosition.Y + heightIn) * 0.0625));
        }

        private bool setTile(TileType tileIn, int decoration = -1)
        {
            int mouse_x = Mouse.GetState().X - 4 + (int)cameraPosition.X;
            int mouse_y = Mouse.GetState().Y - 4 + (int)cameraPosition.Y;

            if (mouse_x > 0 && mouse_x < totalWidth && mouse_y > 0 && mouse_y < totalHeight)
            {
                if (tiles[mouse_y / 16, mouse_x / 16].tileType == TileType.Empty)
                {
                    tiles[mouse_y / 16, mouse_x / 16].tileType = tileIn;
                    tiles[mouse_y / 16, mouse_x / 16].decorationValue = decoration;
                    if (tileIn == TileType.Decoration || tileIn == TileType.Empty || tileIn == TileType.Tree)
                        tiles[mouse_y / 16, mouse_x / 16].tileCollision = TileCollision.Passable;
                    else
                        tiles[mouse_y / 16, mouse_x / 16].tileCollision = TileCollision.Impassable;
                    calculateTiles();

                    return true;
                }else
                    return false;
            }
            else
                return false;

        }

        public void calculateTiles()
        {
            for (int y = 0; y < levelOne.Height; y++)
            {
                for (int x = 0; x < levelOne.Width; x++)
                {
                    tiles[y, x].calculateOrientation(ref tiles);
                }
            }
        }

        public TileType tmpBlock;
        public Vector2 delta;

        private void setTileHealth()
        {
            int mouse_x = Mouse.GetState().X - 4 + (int)cameraPosition.X;
            int mouse_y = Mouse.GetState().Y - 4 + (int)cameraPosition.Y;

            if (mouse_x > 0 && mouse_x < totalWidth && mouse_y > 0 && mouse_y < totalHeight)
            {
                int mouseX = mouse_x / 16;
                int mouseY = mouse_y / 16;

                if (tiles[grabY(mouseY, -1), mouseX].tileType == TileType.Tree) { return; }

                tmpBlock = tiles[mouseY, mouseX].tileType;
                if (tiles[mouseY, mouseX].doDamage() == true)
                {
                    calculateTiles();
                    delta =  player.getMiddle() - tiles[mouseY, mouseX].getMiddle();


                    if (Math.Abs(delta.X) <= 32 && Math.Abs(delta.Y) <= 32) // Player in range collect block
                    {
                        if (tmpBlock == TileType.Dirt)
                            player.addInventory(1, Player.InventoryType.DirtTile);
                        else if (tmpBlock == TileType.Metal)
                            player.addInventory(1, Player.InventoryType.MetalTile);
                    }
                }
            }
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

        /*    spriteBatch.Begin();
            spriteBatch.Draw(backgroundTex, Vector2.Zero, new Rectangle((int)cameraPosition.X, 0, screenWidth, screenHeight), Color.White);
            spriteBatch.End();*/


            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);
            spriteBatch.Draw(bgPara, Vector2.Zero, new Rectangle(Convert.ToInt32(cameraPosition.X * 0.1f), Convert.ToInt32(cameraPosition.Y * 0.1f), cloudTex.Width, cloudTex.Height), Color.White);
            spriteBatch.Draw(hillsTex, Vector2.Zero, new Rectangle(Convert.ToInt32(cameraPosition.X * 0.3f), Convert.ToInt32(cameraPosition.Y * 0.3f), cloudTex.Width, cloudTex.Height), Color.White);

            spriteBatch.Draw(cloudTex, Vector2.Zero, new Rectangle(Convert.ToInt32(cameraPosition.X  * 0.5f), Convert.ToInt32(cameraPosition.Y * 0.5f), cloudTex.Width, cloudTex.Height), Color.White);
            spriteBatch.Draw(cloudTex, Vector2.Zero, new Rectangle(Convert.ToInt32(cameraPosition.X * 0.8f), Convert.ToInt32(cameraPosition.Y * 0.8f), cloudTex.Width, cloudTex.Height), Color.White);
            //spriteBatch.Draw(texture2, position, new Rectangle(cameraX * 0.8f, cameraY * 0.8f, texture2.Width, texture2.Height), Color.White);
           // spriteBatch.Draw(texture3, position, new Rectangle(cameraX * 1.0f, cameraY * 1.0f, texture3.Width, texture3.Height), Color.White);
            spriteBatch.End();

            spriteBatch.Begin();
            drawTiles();
            player.Draw(spriteBatch);
            drawDroplets();
            UpdateMouse();
            countDrops();

            drawInventory();
            if (DEBUG)
                DrawDebug();

            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        public void DrawDebug()
        {
            spriteBatch.DrawString(font, "FPS: " + fps.ToString(), new Vector2(10, 10), textRed); 
            spriteBatch.DrawString(font, "Droplets: " + dropCount.ToString(), new Vector2(10, 25), textRed);
            spriteBatch.DrawString(font, "Water: " + (pauseWater ? "Paused" : "Running"), new Vector2(10, 40), textRed);
            spriteBatch.DrawString(font, "Camera : " + cameraPosition.ToString(), new Vector2(10, 55), textRed);
            spriteBatch.DrawString(font, "Player : " + player.position.ToString() + " Velo "  + player.velocity.ToString(), new Vector2(10, 70), textRed);
            spriteBatch.DrawString(font, "Diff : " + (player.position.X - cameraPosition.X), new Vector2(10, 85), textRed);
            spriteBatch.DrawString(font, "Player CAM : " + player.cameraPosition.ToString(), new Vector2(10, 100), textRed);
            spriteBatch.DrawString(font, "Block Count : " + player.blockCount.ToString(), new Vector2(10, 115), textRed);
            spriteBatch.DrawString(font, "Delta : " + delta.ToString(), new Vector2(10, 130), textRed);
            spriteBatch.DrawString(font, "Scroll : " + mouseScroll.ToString(), new Vector2(10, 145), textRed);

        }
        public void countDrops()
        {
            dropCount = 0;

            for (int y = getCameraPosY(0); y < getCameraPosY(screenHeight) - 1; y++)
            {
                for (int x = getCameraPosX(0); x < getCameraPosX(screenWidth) - 1; x++)
                {
                    dropCount += droplets[y, x].volume;
                }
            }
        }

        public void clearDrops()
        {
            for (int y = 0; y < levelHeight; y++)
            {
                for (int x = 0; x < levelWidth; x++)
                {
                    droplets[y, x].volume = 0;
                }
            }
        }

        public void drawDroplets()
        {
            for (int y = getCameraPosY(0); y < getCameraPosY(screenHeight); y++)
            {
                for (int x = getCameraPosX(0); x < getCameraPosX(screenWidth); x++)
                {
                    if (droplets[y,x].topMost && droplets[y,x].waterSand)
                        droplets[y, x].Draw(cameraPosition, spriteBatch, ref waterSandTop);
                    else if (droplets[y, x].topMost && droplets[y, x].waterSand == false)
                        droplets[y, x].Draw(cameraPosition, spriteBatch, ref topMost);
                   else if (droplets[y,x].waterSand &&  droplets[y,x].topMost == false)
                        droplets[y, x].Draw(cameraPosition, spriteBatch, ref waterSand);
                   else
                        droplets[y, x].Draw(cameraPosition, spriteBatch, ref WaterDropTex);
                }
            }
        }

        public void makeDrop(float amount)
        {
            int mouse_x = Mouse.GetState().X - 4 + (int)cameraPosition.X;
            int mouse_y = Mouse.GetState().Y - 4;

            if (mouse_x > 0 && mouse_x < totalWidth && mouse_y > 0 && mouse_y < totalHeight)
            {
                droplets[mouse_y / 16, mouse_x / 16].volume = amount;
            }
        }

        public void drawTiles()
        {
            for (int y = getCameraPosY(0); y < getCameraPosY(screenHeight + 16); y++)
            {
                for (int x = getCameraPosX(- 16); x < getCameraPosX(screenWidth + 16); x++)
                {
                    if (x < 0 || y < 0 || x >= levelWidth || y >= levelHeight) continue;
                    if (tiles[y,x] != null && tiles[y, x].render)
                    {
                        switch (tiles[y, x].tileType)
                        {
                            case TileType.Dirt:
                                spriteBatch.Draw(tileDirt, tiles[y, x].position - cameraPosition, new Rectangle(16 * (int)tiles[y, x].tileOrientation, 16 * (int)tiles[y,x].damage, 16, 16), Color.White);
                                break;
                            case TileType.Decoration:
                                spriteBatch.Draw(tileGrassTop, tiles[y, x].position - cameraPosition, new Rectangle(16 * tiles[y,x].decorationValue, 0,16,16), Color.White);
                                break;
                            case TileType.Sand:
                                spriteBatch.Draw(sandTex, tiles[y, x].position - cameraPosition, new Rectangle(16 * (int)tiles[y, x].tileOrientation, 0, 16, 16), Color.White);
                                break;
                            case TileType.Tree:
                                spriteBatch.Draw(treeTex, tiles[y, x].position - cameraPosition, new Rectangle(16 * tiles[y, x].decorationValue, 0, 16, 16), Color.White);
                                break;
                            case TileType.Metal:
                                spriteBatch.Draw(tileMetal, tiles[y, x].position - cameraPosition, new Rectangle(16 * (int)tiles[y, x].tileOrientation, 16 * (int)tiles[y, x].damage, 16, 16), Color.White);
                                break;
                        }
                    }
                }
            }
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
            if (tileY + newY < 0) // Too far left
                return tileY;
            else if (tileY + newY >= totalHeight) // too far up
                return tileY;
            else
            {
                return (tileY + newY);
            }
        }
    }
}
