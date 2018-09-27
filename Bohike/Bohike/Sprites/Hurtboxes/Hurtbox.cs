using Bohike.Sprites.Enemies;
using Bohike.Sprites.Hurtboxes.ofEnemies;
using Bohike.Tilemap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Sprites
{
    public enum HurtboxTypes
    {
        Hurtbox,
        FireWhirl,
        GroundPound,
        Dart,
        Torch,
        Spirit,
        ShadowWhirl,
        TorchFire,
        DartFire,
    }

    public class Hurtbox : Sprite, ICollidable
    {
        public Explosion Explosion;
        public float LifeSpan { get; set; }
        public float Damage;
        public HurtboxTypes HurtboxType;
        public Sprite Target;
        public float Speed = 1f;
        public bool HasTargeted;
        protected bool _delayedIsRemoved;

        public Hurtbox(Texture2D texture)
          : base(texture)
        {
            CollisionType = CollisionTypes.Hurtbox;
            Layer = 0.0f;
        }

        protected virtual void AddExplosion(ExplosionTypes explosionType)
        {
            if (Explosion == null)
                return;
  
            var explosion = Explosion.Clone() as Explosion;

            if (!(Parent is Spirit))
                explosion.Position = this.Position + this._velocity*2;
            else
                explosion.Position = this.Position;
            explosion.Animation = (int)explosionType;

            Children.Add(explosion);
        }

        protected virtual void AddExplosionOn(ExplosionTypes explosionType, Sprite sprite)
        {
            if (Explosion == null)
                return;

            var explosion = Explosion.Clone() as Explosion;

            explosion.Position = sprite.Position + sprite.Velocity;
            explosion.Animation = (int)explosionType;

            Children.Add(explosion);
        }
    }
}
