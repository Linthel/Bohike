using Bohike.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Managers
{
    public class AnimationManager : ICloneable
    {
        private Animation _animation;
        private float _timer;

        public Animation CurrentAnimation
        {
            get
            {
                return _animation;
            }
        }

        public Texture2D Texture;
        public float Layer { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Scale = 1f;
        public Color Color = Color.White;

        public AnimationManager(Animation animation)
        {
            _animation = animation;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
              _animation.Texture,
              Position,
              new Rectangle(
                _animation.CurrentFrame * _animation.FrameWidth,
                0,
                _animation.FrameWidth,
                _animation.FrameHeight
                ),
              Color,
              Rotation,
              Origin,
              Scale,
              SpriteEffects.None,
              Layer
              );
        }

        public void Play(Animation animation)
        {
            if (_animation == animation)
                return;

            _animation = animation;
            _animation.CurrentFrame = 0;
            _timer = 0;
        }

        public void Stop()
        {
            _timer = 0f;
            _animation.CurrentFrame = 0;
        }

        public void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Origin = new Vector2(CurrentAnimation.FrameWidth / 2, CurrentAnimation.FrameHeight / 2);

            if (_timer > _animation.FrameSpeed)
            {
                _timer = 0f;

                _animation.CurrentFrame++;

                if (_animation.CurrentFrame >= _animation.FrameCount)
                    _animation.CurrentFrame = 0;
            }
        }

        public object Clone()
        {
            var animationManager = this.MemberwiseClone() as AnimationManager;

            animationManager._animation = animationManager._animation.Clone() as Animation;

            return animationManager;
        }
    }
}
