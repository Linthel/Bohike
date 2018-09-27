using Bohike.Models;
using Bohike.Sprites.Collectibles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Sprites
{
    public enum EnemyStates
    {
        None,
        Stunned,
    }

    public class Enemy : Sprite, ICollidable
    {
        protected EnemyStates _enemyState;

        public Sprite Target;

        public Hurtbox Hurtbox { get; set; }
        public Money Money { get; set; }
        public PowerUp PowerUp { get; set; }
        public Explosion Explosion;
        public AI AI;
        public Vector2 StartingPosition;
        protected bool _hasStartingPosition;

        public bool IsHittable = true;
        protected bool _delayedIsRemoved;
        protected bool _hasDroppedMoney;
        protected bool _hasDroppedPowerUp;

        public Enemy(Texture2D texture)
          : base(texture)
        {
            Layer = 0.2f;
        }

        protected virtual void ResetEnemyAI()
        {
            AI.Left = false;
            AI.Right = false;
            AI.Jump = false;
            AI.FastFall = false;
            AI.Attack = false;
            AI.Sprint = false;
            if (!_hasStartingPosition && Position != null)
            {
                StartingPosition = new Vector2(Position.X, Position.Y);
                _hasStartingPosition = true;
            }
            if (_delayedIsRemoved)
                IsRemoved = true;
        }

        protected virtual void AddExplosion(ExplosionTypes explosionType)
        {
            if (Explosion == null)
                return;

            var explosion = Explosion.Clone() as Explosion;

            explosion.Position = this.Position;
            explosion.Animation = (int)explosionType;

            Children.Add(explosion);
        }

        protected virtual void AddHurtbox(float damage, HurtboxTypes hurtboxType)
        {
            var hurtbox = Hurtbox.Clone() as Hurtbox;

            hurtbox.Position = this.Position + this._velocity;
            hurtbox.HurtboxType = hurtboxType;
            hurtbox.Colour = this.Colour;
            hurtbox.Layer = 0.2f;
            hurtbox.LifeSpan = 0.1f;
            hurtbox.Velocity = new Vector2(0f, 0f);
            hurtbox.Parent = this;
            hurtbox.Damage = damage;

            Children.Add(hurtbox);
        }

        protected virtual void DropMoneyAndDie(int value)
        {
            if (!_hasDroppedMoney)
            {
                _hasDroppedMoney = true;

                var money = Money.Clone() as Money;

                money.Position = this.Position + this._velocity + new Vector2(0f, -50f);
                money.Velocity = new Vector2(Game1.Random.Next(-15,16), -30);
                money.CollectibleType = CollectibleTypes.Money;
                money.Colour = this.Colour;
                money.Layer = 0.2f;
                money.LifeSpan = 60f;
                money.Parent = this;
                money.Target = Target;
                money.Value = value;
                money.Speed = 1f;
                _delayedIsRemoved = true;

                Children.Add(money);
            }
        }

        protected virtual void DropPowerUpAndDie()
        {
            if (!_hasDroppedPowerUp)
            {
                _hasDroppedPowerUp = true;

                var powerup = PowerUp.Clone() as PowerUp;

                powerup.Position = this.Position + this._velocity + new Vector2(0f, -50f);
                powerup.Velocity = new Vector2(Game1.Random.Next(-15, 16), -30);
                powerup.CollectibleType = CollectibleTypes.Money;
                powerup.Colour = this.Colour;
                powerup.Layer = 0.2f;
                powerup.LifeSpan = 60f;
                powerup.Parent = this;
                powerup.Target = Target;
                powerup.Value = 1;
                powerup.Speed = 1f;
                _delayedIsRemoved = true;

                Children.Add(powerup);
            }
        }
    }
}
