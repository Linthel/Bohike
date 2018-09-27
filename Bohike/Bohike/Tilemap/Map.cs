using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bohike.Sprites;
using Bohike.Managers;
using Bohike.States.Levels;

namespace Bohike.Tilemap
{
    public class Map
    {
        private List<Tile> _collisionTiles = new List<Tile>();
        public SoundManager Sound;

        public List<Tile> CollisionTiles
        {
            get { return _collisionTiles; }
        }
        private int _width, _height;
        public int Width
        {
            get { return _width; }
        }
        public int Height
        {
            get { return _height; }
        }
        public void Generate(int[,] map, int size, Levels level)
        {
            for (int x = 0; x < map.GetLength(1); x++)
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    int number = map[y, x];

                    if (number >= 0)
                    {
                        switch (level)
                        {
                            case Levels.TestLevel:
                                switch (number)
                                {
                                    case 0:
                                    case 6:
                                    case 8:
                                        _collisionTiles.Add(new Tile(Tile.TileType(level, number))
                                        {
                                            Position = new Vector2(x * size, y * size),
                                            Colour = Color.White,
                                            CollisionType = CollisionTypes.Full,
                                        });
                                        break;
                                    case 1:
                                    case 7:
                                        _collisionTiles.Add(new Tile(Tile.TileType(level, number))
                                        {
                                            Position = new Vector2(x * size, y * size),
                                            Colour = Color.White,
                                            CollisionType = CollisionTypes.None,
                                        });
                                        break;
                                    case 2:
                                        _collisionTiles.Add(new Tile(Tile.TileType(level, number))
                                        {
                                            Position = new Vector2(x * size, y * size),
                                            Colour = Color.White,
                                            SoundManager = Sound,
                                            CollisionType = CollisionTypes.TopFalling,
                                        });
                                        break;
                                    case 3:
                                        _collisionTiles.Add(new Tile(Tile.TileType(level, number))
                                        {
                                            Position = new Vector2(x * size, y * size),
                                            Colour = Color.White,
                                            SoundManager = Sound,
                                            CollisionType = CollisionTypes.Spikes,
                                        });
                                        break;
                                    case 4:
                                        _collisionTiles.Add(new Tile(Tile.TileType(level, number))
                                        {
                                            Position = new Vector2(x * size, y * size),
                                            Colour = Color.White,
                                            SoundManager = Sound,
                                            CollisionType = CollisionTypes.Lava,
                                        });
                                        break;
                                    case 5:
                                        _collisionTiles.Add(new Tile(Tile.TileType(level, number))
                                        {
                                            Position = new Vector2(x * size, y * size),
                                            Colour = Color.White,
                                            SoundManager = Sound,
                                            CollisionType = CollisionTypes.Bounce,
                                        });
                                        break;
                                   
                                }
                                break;
                        }
                    }

                    _width = (x + 1) * size;

                    _height = (y + 1) * size;
                }
        }
    
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var tile in _collisionTiles)
            {
                tile.Draw(spriteBatch);
            }
        }
    }
}
