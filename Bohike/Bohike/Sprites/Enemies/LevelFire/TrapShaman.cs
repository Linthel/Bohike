using Bohike.Sprites.Hurtboxes.ofEnemies;
using Bohike.Sprites.Hurtboxes.ofPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Sprites.Enemies
{
    public class TrapShaman : Enemy
    {
        public Spirit Spirit { get; set; }

        #region Variables

        // Sound
        private SoundEffectInstance _soundInstanceFireWhirl;
        private SoundEffectInstance _soundInstanceSprint;
        private SoundEffectInstance _soundInstanceWalk;

        // Health
        public float Health = 5f;

        // Actions
        public bool InAttack;
        public bool CanAttack;
        private float _attackPower = 1f;
        public bool _hasShot;

        #endregion

        public TrapShaman(Texture2D texture)
          : base(texture)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            if (AnimationManager != null)
            {
                SetAnimations();
                AnimationManager.Update(gameTime);
            }
            SetSoundInstances();

            Actions(gameTime);

            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer > 1337f)
                _timer = 0f;
            
            ResetEnemyAI();
        }

        private void SetSoundInstances()
        {
            if (_soundInstanceFireWhirl == null)
                _soundInstanceFireWhirl = SoundManager.CreateInstance(0);
            if (_soundInstanceSprint == null)
                _soundInstanceSprint = SoundManager.CreateInstance(9);
            if (_soundInstanceWalk == null)
                _soundInstanceWalk = SoundManager.CreateInstance(17);

            _soundInstanceFireWhirl.Volume = MathHelper.Clamp(1 - (AI.DistanceToTarget / 1500), 0, 1);
            _soundInstanceSprint.Volume = MathHelper.Clamp(1 - (AI.DistanceToTarget / 1500), 0, 1);
            _soundInstanceWalk.Volume = MathHelper.Clamp(1 - (AI.DistanceToTarget / 1500), 0, 1);
        }

        #region Actions

        private void Actions(GameTime gameTime)
        {
            Attack(gameTime);
        }

        private void Attack(GameTime gameTime)
        {
            if (_enemyState == EnemyStates.None)
            {
                if (AI.Attack && CanAttack)
                {
                    _velocity.X = 0f;

                    InAttack = true;
                    _attackPower -= (float)gameTime.ElapsedGameTime.TotalSeconds * 2f;

                    if (!_hasShot)
                    {
                        AddHurtbox(1f, HurtboxTypes.Spirit);
                        AddExplosion(ExplosionTypes.Shadow);
                        SoundManager.PlaySoundEffect(24);
                        _hasShot = true;
                    }
                }
            }

            if ((InAttack && !AI.Attack) || (InAttack && _enemyState != EnemyStates.None))
            {
                if (_attackPower >= 0.5f)
                    _attackPower = 0f;

                CanAttack = false;
                InAttack = false;
                _hasShot = false;
            }

            if (_attackPower <= 0f)
            {
                CanAttack = false;
                InAttack = false;
                _hasShot = false;
            }

            if (_attackPower >= 1f)
                CanAttack = true;

            if (!InAttack && _attackPower <= 1f)
                _attackPower += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f;

            if (InAttack)
            {
                _soundInstanceFireWhirl.Play();
            }
            else
                _soundInstanceFireWhirl.Stop();
        }

        #endregion

        #region Animation

        protected override void SetAnimations()
        {
            if (AnimationManager != null)
            {
                ////if (_stunned)
                ////{
                ////    AnimationManager.Play(Animations["Stunned"]);
                ////    Rotation = 0f;
                ////}
                //if (InAttack)
                //{
                //    AnimationManager.Play(Animations["Attack"]);
                //}
                ////else if (_inFastFall)
                ////{
                ////    AnimationManager.Play(Animations["FastFall"]);
                ////    Rotation = 0f;
                ////}
                ////else if (!_onGround && _velocity.Y > 0)
                ////{
                ////    AnimationManager.Play(Animations["Fall"]);
                ////}
                ////else if (_inJump && _velocity.X < 0)
                ////{
                ////    AnimationManager.Play(Animations["Jump"]);
                ////}
                ////else if (_velocity.X > 0 && _inSprint)
                ////{
                ////    AnimationManager.Play(Animations["SprintRight"]);
                ////}
                ////else if (_velocity.X < 0 && _inSprint)
                ////{
                ////    AnimationManager.Play(Animations["SprintLeft"]);
                ////}
                //else if (_velocity.X > 0)
                //{
                //    AnimationManager.Play(Animations["MoveRight"]);
                //}
                //else if (_velocity.X < 0)
                //{
                //    AnimationManager.Play(Animations["MoveLeft"]);
                //}
                //else if (_onGround || !InAttack)
                //    AnimationManager.Stop();
            }
        }

        #endregion

        #region Hurtboxes

        protected override void AddHurtbox(float damage, HurtboxTypes hurtboxType)
        {
            if (hurtboxType == HurtboxTypes.Spirit)
            {
                var hurtbox = Spirit.Clone() as Spirit;

                hurtbox.Position = this.Position + this._velocity + new Vector2(0f,-50f);
                hurtbox.HurtboxType = hurtboxType;
                hurtbox.Colour = this.Colour;
                hurtbox.Layer = 0.2f;
                hurtbox.LifeSpan = 4.5f;
                hurtbox.Parent = this;
                hurtbox.Target = Target;
                hurtbox.Damage = damage;
                hurtbox.Speed = 1f;

                Children.Add(hurtbox);
            }
        }

        #endregion
    }
}
