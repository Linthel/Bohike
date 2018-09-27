using Bohike.Tilemap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Sprites.Hurtboxes.ofEnemies
{
    public class Spirit : Hurtbox
    {
        private SoundEffectInstance _soundInstanceSpirit;

        public Spirit(Texture2D texture)
          : base(texture)
        {
            CollisionType = CollisionTypes.Hurtbox;
        }
        public override void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer >= LifeSpan)
                IsRemoved = true;
            if (_delayedIsRemoved)
            {
                _soundInstanceSpirit.Stop();
                IsRemoved = true;
            }

            if (_soundInstanceSpirit == null)
                _soundInstanceSpirit = SoundManager.CreateInstance(3);

            _soundInstanceSpirit.Play();

            AddExplosion(ExplosionTypes.Shadow);

            Velocity += Vector2.Normalize(new Vector2(Target.Position.X - Position.X, Target.Position.Y - Position.Y)) * 0.4f;
            Velocity /= 1.015f;

            Position += Velocity * Speed;

            var distance = (float)Math.Sqrt((Target.Position.X - Position.X) * (Target.Position.X - Position.X) + (Target.Position.Y - Position.Y) * (Target.Position.Y - Position.Y));
            _soundInstanceSpirit.Volume = MathHelper.Clamp((1 - (distance / 1000)), 0, 1);
        }

        public override void OnCollide(Sprite sprite)
        {
            if (sprite is Hurtbox)
                return;

            if (sprite is Enemy && this.Parent is Enemy)
                return;

            if (sprite is Player && this.Parent is Player)
                return;

            if (sprite is Player && this.Parent is Enemy)
            {
                if ((sprite as Player).IsHittable)
                {
                    AddExplosionOn(ExplosionTypes.Shadow, sprite);
                    SoundManager.PlaySoundEffect(Game1.Random.Next(0, 3));
                    SoundManager.PlaySoundEffect(4);
                    sprite.IsHit(Damage);
                    if (!_delayedIsRemoved)
                    {
                        _delayedIsRemoved = true;
                    }
                        
                }
            }
        }
    }
}

