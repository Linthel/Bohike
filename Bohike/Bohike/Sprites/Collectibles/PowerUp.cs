using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Sprites.Collectibles
{
    public class PowerUp : Collectible
    {
        public PowerUp(Texture2D texture)
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

            AddExplosion(ExplosionTypes.PowerUp);

            Velocity += Vector2.Normalize(new Vector2(Target.Position.X - Position.X, Target.Position.Y - Position.Y)) * 1f;
            Velocity /= 1.15f;

            Layer = 0.0f;

            var rotation = (float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 0.1f +
                           ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * Math.Abs(_velocity.Y / 200)) +
                           ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * Math.Abs(_velocity.X / 200));

            Rotation += rotation;

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
                AddExplosionOn(ExplosionTypes.PowerUp, sprite);
                //SoundManager.PlaySoundEffect(Game1.Random.Next(0, 3));
                if (!_hasBeenCollected)
                {
                    (sprite as Player).CollectPowerUp();
                    SoundManager.PlaySoundEffect(9);
                    _hasBeenCollected = true;
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

            Children.Add(explosion);
        }
    }
}
