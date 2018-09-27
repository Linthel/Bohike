using Bohike.Sprites.Collectibles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Sprites.Enemies
{
    public class Chest : Enemy
    {
        #region Variables

        protected float Health = 1f;
        protected float _damageCooldown;
        protected bool _onGround;

        #endregion

        public Chest(Texture2D texture)
          : base(texture)
        {
            CollisionType = CollisionTypes.Full;
        }

        public override void Update(GameTime gameTime)
        {
            if (AnimationManager != null)
            {
                SetAnimations();
                AnimationManager.Update(gameTime);
            }

            ManageHealth(gameTime);

            _onGround = false;

            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer > 1337f)
                _timer = 0f;

            ResetEnemyAI();
        }

        #region Health

        protected virtual void ManageHealth(GameTime gameTime)
        {
            _damageCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_damageCooldown <= 0f)
            {
                Layer = 0.0f;
                _damageCooldown = 0;
                IsHittable = true;
            }
        }

        public override void IsHit(float damage)
        {
            if (IsHittable)
            {
                _enemyState = EnemyStates.Stunned;
                Rotation = 0;
                Health -= damage;
                _damageCooldown = 0.3f;
                IsHittable = false;

                if (Health <= 0)
                {
                    DropMoneyAndDie(Game1.Random.Next(1,3));
                    SoundManager.PlaySoundEffect(15);
                }
                else
                    SoundManager.PlaySoundEffect(Game1.Random.Next(2, 4));
            }
        }

        #endregion

        private void Gravity(GameTime gameTime)
        {
            if (_velocity.Y < 40f)
                _velocity.Y += +((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 1.5f);
            else if (_velocity.Y > 40f)
                _velocity.Y /= ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 1.01f);

            if (_onGround)
                _velocity.Y = 0f;
        }

        #region Animation

        protected override void SetAnimations()
        {
            if (AnimationManager != null)
            {
                AnimationManager.Play(Animations["MoveLeft"]);
            }
        }

        #endregion

        #region Collision

        public override void OnCollide(Sprite sprite)
        {
            if (sprite.CollisionType == CollisionTypes.Full)
            {
                if (_velocity.Y > 0)
                {
                    if (this.Rectangle.Bottom + this._velocity.Y > sprite.Rectangle.Top &&
                        this.Rectangle.Top < sprite.Rectangle.Top &&
                        this.Rectangle.Right > sprite.Rectangle.Left &&
                        this.Rectangle.Left < sprite.Rectangle.Right)
                    {
                        _velocity.Y = 0;
                        _onGround = true;
                        Rotation = 0f;
                    }
                }
            }
        }

        #endregion
    }
}
