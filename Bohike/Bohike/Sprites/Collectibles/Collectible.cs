using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Sprites.Collectibles
{
    public enum CollectibleTypes
    {
       Money,
       Powerup,
    }

    public class Collectible : Sprite, ICollidable
    {
        public Explosion Explosion;
        public float LifeSpan { get; set; }
        public int Value;
        public CollectibleTypes CollectibleType;
        public Sprite Target;
        public float Speed;
        protected bool _hasTargeted;
        protected bool _delayedIsRemoved;
        protected bool _hasBeenCollected;

        public Collectible(Texture2D texture)
          : base(texture)
        {
            CollisionType = CollisionTypes.Collectible;
        }

        protected virtual void AddExplosion(ExplosionTypes explosionType)
        {
            if (Explosion == null)
                return;

            var explosion = Explosion.Clone() as Explosion;

            explosion.Position = this.Position + this._velocity;
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
