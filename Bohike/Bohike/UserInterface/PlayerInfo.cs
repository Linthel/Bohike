using Bohike.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.UserInterface
{
    public class PlayerInfo : Component
    {
        #region Fields

        private MouseState _currentMouse;
        private SpriteFont _font;
        private bool _isHovering;
        private MouseState _previousMouse;
        private Texture2D _texture;
        public Texture2D PlayerHealth2;
        public Texture2D PlayerHealth1;
        public Texture2D PlayerHealth0;

        #endregion

        #region Properties

        public event EventHandler Click;
        public Player Player;
        public bool Clicked { get; private set; }
        public Color PenColor { get; set; }
        public Vector2 Position { get; set; }

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }

        public string Text { get; set; }

        #endregion

        #region Methods

        public PlayerInfo(Texture2D texture)
        {
            _texture = texture;

            PenColor = Color.Black;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Player != null)
            {
                switch (Player.Health)
                {
                    case 3f:
                        spriteBatch.Draw(_texture, Rectangle, Color.White);
                        break;
                    case 2f:
                        spriteBatch.Draw(PlayerHealth2, Rectangle, Color.White);
                        break;
                    case 1f:
                        spriteBatch.Draw(PlayerHealth1, Rectangle, Color.White);
                        break;
                    case 0:
                        spriteBatch.Draw(PlayerHealth0, Rectangle, Color.White);
                        break;

                    default:
                        break;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        #endregion
    }
}
