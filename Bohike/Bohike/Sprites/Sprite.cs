using Bohike.Managers;
using Bohike.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Sprites
{
    public enum CollisionTypes
    {
        None,
        Full,
        Top,
        TopFalling,
        Hurtbox,
        Spikes,
        Lava,
        Collectible,
        Bounce,
    }

    public class Sprite : Component, ICloneable
    {
        public Game1 Game;

        public Input Input;
        public List<Sprite> Children { get; set; }
        public Sprite Parent;
        public bool CameraTarget = false;
        protected float _layer { get; set; }
        protected Vector2 _origin { get; set; }
        protected Vector2 _position { get; set; }
        protected float _rotation { get; set; }
        public float Scale = 1f;
        protected float _timer;

        protected Texture2D _texture;
        public AnimationManager AnimationManager;
        public Dictionary<string, Animation> Animations;
        public SoundManager SoundManager;

        protected Vector2 _velocity;
        public Vector2 Velocity { get { return _velocity; } set { _velocity = value; } }
        public float VelocityX { get { return _velocity.X; } set { _velocity.X = value; } }
        public float VelocityY { get { return _velocity.Y; } set { _velocity.Y = value; } }
        public CollisionTypes CollisionType { get; set; }
        public bool IsRemoved { get; set; }
        public Color Colour { get; set; }

        public bool WasSteppedOn;

        public float Layer
        {
            get { return _layer; }
            set
            {
                _layer = value;

                if (AnimationManager != null)
                {
                    AnimationManager.Layer = value;
                    _layer = value;
                }
            }
        }

        public Vector2 Origin
        {
            get { return _origin; }
            set
            {
                _origin = value;

                if (AnimationManager != null)
                    AnimationManager.Origin = new Vector2(AnimationManager.CurrentAnimation.FrameWidth / 2, AnimationManager.CurrentAnimation.FrameHeight / 2);
            }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;

                if (AnimationManager != null && _texture != null)
                    AnimationManager.Position = new Vector2(_position.X, _position.Y + (_texture.Height - AnimationManager.CurrentAnimation.FrameHeight)/2 + 20);

                if (AnimationManager != null && _texture == null)
                    AnimationManager.Position = new Vector2(_position.X, _position.Y);
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X - (int)Origin.X, (int)Position.Y - (int)Origin.Y, _texture.Width, _texture.Height);
            }
        }

        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;

                if (AnimationManager != null)
                    AnimationManager.Rotation = _rotation;
            }
        }

        public Sprite(Texture2D texture)
        {
            _texture = texture;

            Children = new List<Sprite>();

            Origin = new Vector2(_texture.Width / 2, _texture.Height / 2);

            CollisionType = CollisionTypes.None;

            Colour = Color.White;
        }

        public Sprite(Dictionary<string, Animation> animations)
        {
            _texture = null;

            Children = new List<Sprite>();

            Colour = Color.White;

            Animations = animations;
            var animation = Animations.FirstOrDefault().Value;
            AnimationManager = new AnimationManager(animation);

            Origin = new Vector2(animation.FrameWidth / 2, animation.FrameHeight / 2);
        }

        public override void Update(GameTime gameTime)
        {
            if (AnimationManager != null)
            {
                SetAnimations();

                AnimationManager.Update(gameTime);
            }
        }

        public void PostUpdate(GameTime gameTime)
        {
            Position += _velocity;
        }

        protected virtual void SetAnimations()
        {
            //AnimationManager.Origin = _origin;

            if (AnimationManager != null)
            {
                AnimationManager.Play(Animations["Idle"]);
            }
        }

        public virtual void IsHit(float damage)
        {

        }

        public virtual void OnCollide(Sprite sprite)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (AnimationManager == null)
                spriteBatch.Draw(_texture, Position, null, Colour, _rotation, Origin, Scale, SpriteEffects.None, Layer);
            if (AnimationManager != null)
                AnimationManager.Draw(spriteBatch);
        }

        public bool WillIntersect(Sprite sprite)
        {
            return this.Rectangle.Right + this._velocity.X > sprite.Rectangle.Left &&
              this.Rectangle.Left + this._velocity.X < sprite.Rectangle.Right &&
              this.Rectangle.Top + this._velocity.Y < sprite.Rectangle.Bottom &&
              this.Rectangle.Bottom + this._velocity.Y >= sprite.Rectangle.Top;
        }

        public object Clone()
        {
            var sprite = this.MemberwiseClone() as Sprite;

            if (Animations != null)
            {
                sprite.Animations = this.Animations.ToDictionary(c => c.Key, v => v.Value.Clone() as Animation);
                sprite.AnimationManager = sprite.AnimationManager.Clone() as AnimationManager;
            }

            return sprite;
        }
    }
}
