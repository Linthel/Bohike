using Bohike.Sprites.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Sprites.Hurtboxes.ofPlayer
{
    public class GroundPound : Hurtbox
    {
        public GroundPound(Texture2D texture)
          : base(texture)
        {
            CollisionType = CollisionTypes.Hurtbox;
        }

        public override void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            AddExplosion(ExplosionTypes.GroundPoundExplosion);

            if (_timer >= LifeSpan)
                IsRemoved = true;
        }

        public override void OnCollide(Sprite sprite)
        {
            if (sprite is Hurtbox)
                return;

            if (sprite is Enemy && this.Parent is Enemy)
                return;

            if (sprite is Player && this.Parent is Player)
                return;

            if (sprite is Enemy && this.Parent is Player)
            {
                if ((sprite as Enemy).IsHittable)
                {
                    if (!(sprite is Chest))
                        SoundManager.PlaySoundEffect(Game1.Random.Next(0, 3));
                    AddExplosionOn(ExplosionTypes.Earth, sprite);
                    sprite.IsHit(Damage);
                }
            }
        }
    }
}
