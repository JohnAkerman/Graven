using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

namespace Graven
{
    class Level
    {
        public Tile[,,] tileLayers;
        public int levelWidth, levelHeight; // Total pixels of the level.
        int levelTileWidth, levelTileHeight;
        int tileSize = 16;
        int activeLayer = 1;
        int numLayers = 1;
        public int screenWidth, screenHeight; // Total pixels of game viewport area aka 1024 x 768
        public int screenTilesWidth, screenTilesHeight;
        Random rand;


        #region Textures

        Texture2D currentLevel;
        Texture2D tileGrassTop, tileDirt, WaterDropTex, mouseTex, sandTex, waterSand, topMost, waterSandTop, tileMetal, darkTex;
        
        #endregion


        public void loadTextures(ContentManager Content)
        {
            currentLevel = Content.Load<Texture2D>("Levels/levelOne");

            tileGrassTop = Content.Load<Texture2D>("Tiles/tileA");
            tileDirt = Content.Load<Texture2D>("Tiles/tileDirt");
            tileMetal = Content.Load<Texture2D>("Tiles/tileMetal");
            sandTex = Content.Load<Texture2D>("Tiles/sand");
            WaterDropTex = Content.Load<Texture2D>("waterdrop");
            waterSand = Content.Load<Texture2D>("Tiles/watersand");
            topMost = Content.Load<Texture2D>("Tiles/topmost");
            waterSandTop = Content.Load<Texture2D>("Tiles/watersandtop");
            darkTex = Content.Load<Texture2D>("Tiles/dark");

        }

        public Level(int screenHeight, int screenWidth) {
            this.screenHeight = screenHeight;
            this.screenWidth = screenWidth;
            screenTilesWidth = this.screenWidth / tileSize;
            screenTilesHeight = this.screenHeight / tileSize;
        }

      /*  public void resetTiles()
        {
            tileLayers = new Tile[2, currentLevel.Height, currentLevel.Width];
            for (int y = 0; y < levelHeight; y++)
            {
                for (int x = 0; x < levelWidth; x++)
                {
                    tileLayers[1, y, x] = new Tile(TileType.Empty, x, y, levelHeight, levelHeight, rand, TileCollision.Passable);
                }
            }
        }*/

        public void calculateTiles(Camera cam, Player player, bool initialLoad = false)
        {
           for (int y = 0; y < currentLevel.Height; y++)
            {
                for (int x = 0; x < currentLevel.Width; x++)
                {
                    tileLayers[1, y, x].calculateOrientation(ref tileLayers);
                  
                }
            }
            tileLayers[1, 0, 0].updateLight(ref tileLayers, player, initialLoad);
        }

    /*    public void createLighting(Camera cam)
        {
            for (int y = 0; y < currentLevel.Height; y++)
            {
                for (int x = 0; x < currentLevel.Width; x++)
                {
                    if (x < 0 || y < 0 ||  x >= levelTileWidth || y >= levelTileHeight) 
                        continue;

                    if (tileLayers[1, y, x] != null && tileLayers[1, y, x].render)
                    {
                        tileLayers[1, y, x].calculateOrientation(ref tileLayers);                       
                    }
                }
            }
        }*/

    /*    public void resetLightingCount(Camera cam)
        {
            for (int y = 0; y < currentLevel.Height; y++)
            {
                for (int x = 0; x < currentLevel.Width; x++)
                {
                    if (x < 0 || y < 0 || x >= levelTileWidth || y >= levelTileHeight)
                        continue;

                    if (tileLayers[1, y, x] != null && tileLayers[1, y, x].render)
                    {
                        tileLayers[1, y, x].lightingHits = 0;
                        tileLayers[1, y, x].lightValue = 0;
                    }
                }
            }
        }
        */
        public void setUpTile(string path, Player player, Camera cam)
        {
            Color[] levelData = new Color[currentLevel.Height * currentLevel.Width];

            levelWidth = currentLevel.Width * tileSize;
            levelHeight = currentLevel.Height * tileSize;
            levelTileWidth = currentLevel.Width;
            levelTileHeight = currentLevel.Height;

            player.totalWidth = levelTileWidth;
            player.totalHeight = levelTileHeight;

            tileLayers = new Tile[2, currentLevel.Height, currentLevel.Width];

            currentLevel.GetData<Color>(levelData);

            rand = new Random();

            int runningTotal = 0;

            for (int y = 0; y < currentLevel.Height; y++)
            {
                for (int x = 0; x < currentLevel.Width; x++)
                {
                    if (runningTotal >= levelData.Length)
                        break;
                    
                    if (levelData[runningTotal] == Color.Lime)
                        tileLayers[1, y, x] = new Tile(TileType.Decoration, x, y, levelTileWidth, levelTileHeight, rand, 0);
                    else if (levelData[runningTotal] == Color.Blue)
                        tileLayers[1, y, x] = new Tile(TileType.Dirt, x, y, levelTileWidth, levelTileHeight, rand, TileCollision.Impassable);
                    else if (levelData[runningTotal] == Color.White)
                        tileLayers[1, y, x] = new Tile(TileType.Empty, x, y, levelTileWidth, levelTileHeight, rand, TileCollision.Passable);
                    else if (levelData[runningTotal] == Color.Yellow)
                        tileLayers[1, y, x] = new Tile(TileType.Sand, x, y, levelTileWidth, levelTileHeight, rand, TileCollision.Impassable);
                    else if (levelData[runningTotal] == new Color(204,204,204))
                        tileLayers[1, y, x] = new Tile(TileType.Decoration, x, y, levelTileWidth, levelTileHeight, rand, TileCollision.Passable, 1);
                    else if (levelData[runningTotal] == new Color(53, 34, 18)) // Tree Foot
                        tileLayers[1, y, x] = new Tile(TileType.Tree, x, y, levelTileWidth, levelTileHeight, rand, TileCollision.Passable, 0);
                    else if (levelData[runningTotal] == new Color(109, 69, 35)) // Tree Truck
                        tileLayers[1, y, x] = new Tile(TileType.Tree, x, y, levelTileWidth, levelTileHeight, rand, TileCollision.Passable, 1);
                    else if (levelData[runningTotal] == new Color(24, 100, 12)) // Tree Top
                        tileLayers[1, y, x] = new Tile(TileType.Tree, x, y, levelTileWidth, levelTileHeight, rand, TileCollision.Passable, 2);
                    else if (levelData[runningTotal] == new Color(119, 119, 119)) // Metal
                        tileLayers[1, y, x] = new Tile(TileType.Metal, x, y, levelTileWidth, levelTileHeight, rand, TileCollision.Impassable);
                    else if (levelData[runningTotal] == new Color(255, 0, 0)) // Player start
                    {
                        tileLayers[1, y, x] = new Tile(TileType.Empty, x, y, levelTileWidth, levelTileHeight, rand, TileCollision.Passable);
                        player.setPosition(x * tileSize, y * tileSize);
                    }

                    runningTotal++;
                }
            }

            calculateTiles(cam, player, true);
        }

        public bool setTile(TileType tileIn, Camera camera, Player player, int decoration = -1)
        {
            int mouse_x = Mouse.GetState().X - 4 + (int)camera.position.X;
            int mouse_y = Mouse.GetState().Y - 4 + (int)camera.position.Y;

            if (mouse_x > 0 && mouse_x < levelWidth && mouse_y > 0 && mouse_y < levelHeight)
            {
                if (tileLayers[1, mouse_y / 16, mouse_x / 16].tileType == TileType.Empty)
                {
                    tileLayers[1, mouse_y / 16, mouse_x / 16].tileType = tileIn;
                    tileLayers[1, mouse_y / 16, mouse_x / 16].decorationValue = decoration;
                    if (tileIn == TileType.Decoration || tileIn == TileType.Empty || tileIn == TileType.Tree)
                        tileLayers[1, mouse_y / 16, mouse_x / 16].tileCollision = TileCollision.Passable;
                    else
                        tileLayers[1, mouse_y / 16, mouse_x / 16].tileCollision = TileCollision.Impassable;

                   calculateTiles(camera, player);

                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public void setTileHealth(Camera camera, Player player)
        {
            int mouse_x = Mouse.GetState().X - 4;
            int mouse_y = Mouse.GetState().Y - 4;

            TileType tmpBlock;
            Vector2 delta;

            if (mouse_x > 0 && mouse_x < screenWidth && mouse_y > 0 && mouse_y < screenHeight)
            {
                int mouseX = (mouse_x + (int)camera.position.X) / 16;
                int mouseY = (mouse_y + (int)camera.position.Y) / 16;

                if (mouseY > levelHeight)
                    return;

                if (tileLayers[1, grabY(mouseY, -1), mouseX].tileType == TileType.Tree) { return; }

                tmpBlock = tileLayers[1, mouseY, mouseX].tileType;
                if (tileLayers[1,mouseY, mouseX].doDamage() == true)
                {
                    calculateTiles(camera, player);

                    delta = player.getMiddle() - tileLayers[1, mouseY, mouseX].getMiddle();

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

        public void updateTiles(float elapsed, Camera cam)
        {
            for (int y = cam.getTileCoords("y"); y < screenTilesHeight - 1; y++)
            {
                for (int x = cam.getTileCoords("x"); x < screenTilesHeight - 1; x++)
                {
                    if (y > screenHeight || x > screenWidth) { continue; }

                    tileLayers[1, y, x].updateTile(ref tileLayers, elapsed, cam.position);
                }
            }
        }
        public int grabY(int tileY, int newY = 0)
        {
            if (tileY + newY <= 0) // Too far left
                return tileY;
            else if (tileY + newY >= levelHeight) // too far up
                return tileY;
            else
            {
                return (tileY + newY);
            }
        }

        public bool checkBreakable(TileType tileIn)
        {
            if (tileIn == TileType.Tree || tileIn == TileType.Sand || tileIn == TileType.Dirt || tileIn == TileType.Metal)
                return true;
            else
                return false;
        }

        public void drawTiles(Camera cam, SpriteBatch sb)
        {
            int z = 1;

            for (int y = cam.getCameraY(0); y < cam.getCameraY(screenHeight + 16); y++)
            {
                for (int x = cam.getCameraX(-16); x < cam.getCameraX(screenWidth + 16); x++)
                {
                    if (x < 0 || y < 0 ||  x >= levelTileWidth || y >= levelTileHeight) { 
                        continue;
                    }

                    if (tileLayers[z, y, x] != null && tileLayers[z, y, x].render)
                    {
                        switch (tileLayers[1, y, x].tileType)
                        {
                            case TileType.Dirt:
                                sb.Draw(tileDirt, tileLayers[z, y, x].position - cam.position, new Rectangle(16 * (int)tileLayers[z, y, x].tileOrientation, 16 * (int)tileLayers[z, y, x].damage, 16, 16), Color.White);
                                break;
                            case TileType.Decoration:
                                sb.Draw(tileGrassTop, tileLayers[z, y, x].position - cam.position, new Rectangle(16 * tileLayers[z, y, x].decorationValue, 0, 16, 16), Color.White);
                                break;
                            case TileType.Sand:
                                sb.Draw(sandTex, tileLayers[z, y, x].position - cam.position, new Rectangle(16 * (int)tileLayers[z, y, x].tileOrientation, 0, 16, 16), Color.White);
                                break;
                            case TileType.Metal:
                                sb.Draw(tileMetal, tileLayers[z, y, x].position - cam.position, new Rectangle(16 * (int)tileLayers[z, y, x].tileOrientation, 16 * (int)tileLayers[z, y, x].damage, 16, 16), Color.White);
                                break;
                        }

                        sb.Draw(darkTex, tileLayers[z, y, x].position - cam.position, new Color(0, 0, 0, (1.0f - tileLayers[z, y, x].lightValue)));
                    }
                }
            }
        }
    }
}
