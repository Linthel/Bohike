using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Sprites.Hurtboxes.ofPlayer
{
    public class ShadowWhirl : Hurtbox
    {
        public ShadowWhirl(Texture2D texture)
          : base(texture)
        {
            CollisionType = CollisionTypes.Hurtbox;
        }

        public override void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

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

            if (sprite is Player && this.Parent is Enemy)
            {
                if ((sprite as Player).IsHittable)
                {
                    SoundManager.PlaySoundEffect(Game1.Random.Next(0, 3));
                    AddExplosionOn(ExplosionTypes.Fire, sprite);
                    sprite.IsHit(Damage);
                }
            }
        }
    }
}
