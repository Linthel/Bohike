using Bohike.Controls;
using Bohike.Managers;
using Bohike.States.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.States.Levels
{
    public class TempleRoom : State
    {
        private List<Component> _components;
        private MouseState _mousePosition;

        private SoundManager _soundManager;
        private Texture2D _staticBackgroundTexture;
        private Texture2D _midBackgroundTexture;
        private Texture2D _frontBackgroundTexture;
        private Texture2D _farBackgroundTexture;
        private Vector2 _midBackgroundPosition;
        private Vector2 _frontBackgroundPosition;
        private Vector2 _farBackgroundPosition;

        public TempleRoom(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) 
            : base(game, graphicsDevice, content)
        {
            // --- TEXTURES ---
            _staticBackgroundTexture = _content.Load<Texture2D>("Video/Backgrounds/StaticBackground");

            _farBackgroundTexture = _content.Load<Texture2D>("Video/Backgrounds/FarBackground");

            _midBackgroundTexture = _content.Load<Texture2D>("Video/Backgrounds/MidBackground");

            _frontBackgroundTexture = _content.Load<Texture2D>("Video/Backgrounds/FrontBackground");

            _farBackgroundPosition = new Vector2(0, 0);
            _midBackgroundPosition = new Vector2(0, 0);
            _frontBackgroundPosition = new Vector2(0, 0);
            
            var buttonTexture = _content.Load<Texture2D>("Video/Controls/Button");
            var buttonFont = _content.Load<SpriteFont>("Video/Fonts/Font");
            var buttonPosition = new Vector2((Game1.ScreenWidth - buttonTexture.Width) / 2, Game1.ScreenHeight / 2 + 200);

            // --- SOUND ---

            var backgroundMusic = _content.Load<Song>("Audio/Music/TempleMusic");
            var soundEffects = new List<SoundEffect>()
            {
                //_content.Load<SoundEffect>("BallHitsBat"),
                //_content.Load<SoundEffect>("BallHitsWall"),
                //_content.Load<SoundEffect>("PlayerScores"),
            };

            _soundManager = new SoundManager(backgroundMusic, soundEffects);
            _soundManager.PlayMusic();

            // --- BUTTONS ---

            var hubButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosition.X, buttonPosition.Y),
                Text = "Hub World",
            };

            hubButton.Click += HubButton_Click;

            _components = new List<Component>()
            {
               hubButton,
            };
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // --- STATIC BACKGROUND ---

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap);
            spriteBatch.Draw(_staticBackgroundTexture, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), new Rectangle(0, 0, _staticBackgroundTexture.Width, _staticBackgroundTexture.Height), Color.White);
            spriteBatch.End();

            /// --- FAR BACKGROUND(S) ---

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap);
            spriteBatch.Draw(_farBackgroundTexture, new Rectangle(0, 0, (int)(Game1.ScreenWidth), (int)(Game1.ScreenHeight)), new Rectangle((int)( _farBackgroundPosition.X), (int)(_farBackgroundPosition.Y), _farBackgroundTexture.Width, _farBackgroundTexture.Height), Color.White);
            spriteBatch.End();

            /// --- MID BACKGROUND(S) ---

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap);
            spriteBatch.Draw(_midBackgroundTexture, new Rectangle(0, 0, (int)(Game1.ScreenWidth*1.5f), (int)(Game1.ScreenHeight*1.5f)), new Rectangle((int)(_midBackgroundPosition.X), (int)(_midBackgroundPosition.Y), _midBackgroundTexture.Width, _midBackgroundTexture.Height), Color.White);
            spriteBatch.End();

            // --- SPRITES ---

            spriteBatch.Begin();
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            /// --- FRONT BACKGROUND(S) ---

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap);
            spriteBatch.Draw(_frontBackgroundTexture, new Rectangle(0, 0, Game1.ScreenWidth*4, Game1.ScreenHeight*4), new Rectangle((int)(_frontBackgroundPosition.X), (int)(_frontBackgroundPosition.Y), _frontBackgroundTexture.Width, _frontBackgroundTexture.Height), Color.White);
            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {
            // remove sprites if they're not needed
        }

        public override void Update(GameTime gameTime)
        {
            _mousePosition = Mouse.GetState();

            _farBackgroundPosition.X += (float)(_mousePosition.X - Game1.ScreenWidth / 2) / 400;
            _farBackgroundPosition.Y += (float)(_mousePosition.Y - Game1.ScreenHeight / 2) / 800;

            _midBackgroundPosition.X += (float)(_mousePosition.X - Game1.ScreenWidth / 2) / 200;
            _midBackgroundPosition.Y += (float)(_mousePosition.Y - Game1.ScreenHeight / 2) / 400;

            _frontBackgroundPosition.X += (float)(_mousePosition.X - Game1.ScreenWidth / 2) / 100;
            _frontBackgroundPosition.Y += (float)(_mousePosition.Y - Game1.ScreenHeight / 2) / 200;
            
            foreach (var component in _components)
                component.Update(gameTime);
        }


        private void HubButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new HubWorld(_game, _graphicsDevice, _content));
        }
    }
}
