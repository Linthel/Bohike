using Bohike.Managers;
using Bohike.Sprites;
using Bohike.States.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Tilemap
{
    public class Tile : Sprite, ICollidable
    {
        private static ContentManager _content;

        private float _maximumBreakingCounter = 0.2f;
        private float _breakingCounter;
        private Vector2 _startingPosition;

        public Tile(Texture2D texture) : base(texture)
        {
            Layer = 0.5f;
            _breakingCounter = _maximumBreakingCounter;
        }

        public static ContentManager Content
        {
            protected get { return _content; }
            set { _content = value; }
        }

        public static Texture2D TileType(Levels level, int number)
        {
            switch (level)
            {
                case Levels.TestLevel:
                    return Content.Load<Texture2D>("Video/Tiles/TestLevel/Tile" + number);

                default:
                    return null;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, null, Colour, _rotation, Origin, 1f, SpriteEffects.None, Layer);
        }

        public override void Update(GameTime gameTime)
        {
            if (CollisionType == CollisionTypes.TopFalling)
                FallingTile(gameTime);
        }

        private void FallingTile(GameTime gameTime)
        {
            if (WasSteppedOn)
            {
                if (_breakingCounter == _maximumBreakingCounter)
                {
                    _startingPosition = this.Position;
                    //Sound.PlaySoundEffect(Game1.Random.Next(0, 2));
                }
                _breakingCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
               

            if (_breakingCounter <= 0f)
            {
                _velocity.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 1f;
                _rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * 60 * _velocity.Y / 100;
            }
               

            if (_breakingCounter <= -5f)
            {
                this.Position = _startingPosition;
                WasSteppedOn = false;
                _breakingCounter = _maximumBreakingCounter;
                _velocity.Y = 0;
                _rotation = 0;
            }     
        }
    }
}
