using Bohike.Tilemap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Sprites.Hurtboxes.ofEnemies
{
    public class Dart : Hurtbox
    {
        public Dart(Texture2D texture)
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

            if (Target != null)
            {
                if (!HasTargeted)
                {
                    Velocity = Vector2.Normalize(new Vector2(Target.Position.X - Position.X, Target.Position.Y - Position.Y));
                    HasTargeted = true;
                }
            }

            AddExplosion(ExplosionTypes.FireSmall);

            Position += Velocity * Speed;
        }

        public override void OnCollide(Sprite sprite)
        {
            if (sprite is Tile)
            {
                if (sprite.CollisionType == CollisionTypes.Full)
                {
                    IsRemoved = true;
                }
            }
            
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
                    AddExplosionOn(ExplosionTypes.Fire, sprite);
                    SoundManager.PlaySoundEffect(Game1.Random.Next(0, 3));
                    sprite.IsHit(Damage);
                    if (!_delayedIsRemoved)
                        _delayedIsRemoved = true;
                }
            }
        }
    }
}
