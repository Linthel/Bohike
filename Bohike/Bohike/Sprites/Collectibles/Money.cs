using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Sprites.Collectibles
{
    public class Money : Collectible
    {
        public Money(Texture2D texture)
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
                IsRemoved = true;

            switch (Game1.Random.Next(0, 5))
            {
                case 1:
                    AddExplosion(ExplosionTypes.FireSmall);
                    break;
                case 2:
                    AddExplosion(ExplosionTypes.WaterSmall);
                    break;
                case 3:
                    AddExplosion(ExplosionTypes.EarthSmall);
                    break;
                case 4:
                    AddExplosion(ExplosionTypes.AirSmall);

                    break;
            }
            if (Target != null)
                Velocity += Vector2.Normalize(new Vector2(Target.Position.X - Position.X, Target.Position.Y - Position.Y)) * 1f;
            Velocity /= 1.05f;

            Position += Velocity * Speed;
        }

        public override void OnCollide(Sprite sprite)
        {
            if (sprite is Hurtbox || sprite is Collectible)
                return;

            if (sprite is Enemy && this.Parent is Enemy)
                return;

            if (sprite is Player && this.Parent is Player)
                return;

            if (sprite is Player)
            {
                AddExplosionOn(ExplosionTypes.Air, sprite);
                //SoundManager.PlaySoundEffect(Game1.Random.Next(0, 3));
                if (!_hasBeenCollected)
                {
                    (sprite as Player).IncreaseMoney(Value);
                    _hasBeenCollected = true;

                    switch (Value)
                    {
                        case 1:
                        case 2:
                            SoundManager.PlaySoundEffect(5);
                            break;
                        case 5:
                            SoundManager.PlaySoundEffect(6);
                            break;
                        case 10:
                            SoundManager.PlaySoundEffect(7);
                            break;
                        case 25:
                            SoundManager.PlaySoundEffect(8);
                            break;
                    }
                }
                if (!_delayedIsRemoved)
                    _delayedIsRemoved = true;
            }
        }

        protected override void AddExplosion(ExplosionTypes explosionType)
        {
            if (Explosion == null)
                return;

            var explosion = Explosion.Clone() as Explosion;

            explosion.Position = this.Position + this._velocity;
            explosion.Animation = (int)explosionType;

            explosion.Scale = 0.5f * (float)Math.Sqrt(Value);

            Children.Add(explosion);
        }
    }
}
