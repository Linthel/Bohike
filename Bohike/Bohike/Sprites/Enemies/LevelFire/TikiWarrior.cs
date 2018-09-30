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
    public class TikiWarrior : Enemy
    {
        public ShadowWhirl ShadowWhirl { get; set; }

        #region Variables

        // Sound
        private SoundEffectInstance _soundInstanceFireWhirl;
        private SoundEffectInstance _soundInstanceSprint;
        private SoundEffectInstance _soundInstanceWalk;

        // Health
        public float Health = 3f;
        private float _damageCooldown;

        // Actions
        public bool InAttack;
        public bool CanAttack;
        private float _attackPower = 1f;

        // Movement
        private float _movementSpeed = 5f;
        private bool _onGround;
        private bool _momentumLeft;
        private bool _momentumRight;
        private bool _inJump;
        private bool _hasJumped = true;
        private int _jumpCount;
        private float _jumpPower;
        private bool _inSprint;
        private bool _facesLeft;

        #endregion

        public TikiWarrior(Texture2D texture)
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

            Move(gameTime);
            ManageHealth(gameTime);
            Actions(gameTime);

            _onGround = false;

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

            _soundInstanceFireWhirl.Volume = MathHelper.Clamp(1 - (AI.DistanceToTarget / 2500),0,1);
            _soundInstanceSprint.Volume = MathHelper.Clamp(1 - (AI.DistanceToTarget / 2500), 0, 1);
            _soundInstanceWalk.Volume = MathHelper.Clamp(1 - (AI.DistanceToTarget / 2500), 0, 1);
        }

        #region Health

        private void ManageHealth(GameTime gameTime)
        {
            if (_damageCooldown < 0.1f)
            {
                _enemyState = EnemyStates.None;

                if (_damageCooldown % 0.2f <= 0.05)
                {
                    Layer = -1f;
                }
                else if ((_damageCooldown + 0.1f) % 0.2 <= 0.05)
                {
                    Layer = 0.1f;
                }
            }

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
                    DropMoneyAndDie(25);
                    SoundManager.PlaySoundEffect(15);
                    _soundInstanceFireWhirl.Stop();
                    _soundInstanceSprint.Stop();
                    _soundInstanceWalk.Stop();
                }
                else
                    SoundManager.PlaySoundEffect(Game1.Random.Next(2, 4));
            }
        }

        #endregion

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
                    if (_velocity.Y > 0)
                        _velocity.Y /= 1.3f;

                        InAttack = true;
                    _attackPower -= (float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f;

                    AddHurtbox(1f, HurtboxTypes.ShadowWhirl);
                    AddExplosion(ExplosionTypes.FireWhirl);
                }
            }

            if ((InAttack && !AI.Attack) || (InAttack && _enemyState != EnemyStates.None))
            {
                if (_attackPower >= 0.5f)
                    _attackPower = 0.5f;

                CanAttack = false;
                InAttack = false;
                SoundManager.PlaySoundEffect(7);
            }

            if (_attackPower <= 0f)
            {
                CanAttack = false;
                InAttack = false;
                SoundManager.PlaySoundEffect(7);
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

        #region Movement

        private void Move(GameTime gameTime)
        {
            Gravity(gameTime);
            Movement(gameTime);
            Jumping(gameTime);
        }

        private void Jumping(GameTime gameTime)
        {
            if ((_velocity.Y < -10 || _velocity.Y > 10) && !_hasJumped)
                _jumpCount = 1;

            if (_enemyState == EnemyStates.None)
            {
                if (AI.Jump && !_hasJumped && _onGround)
                {
                    _velocity.Y = -25f;
                    SoundManager.PlaySoundEffect(Game1.Random.Next(10, 12));
                    _inJump = true;
                    _hasJumped = true;
                    _jumpCount--;
                }

                // Jump Break
                if (!AI.Jump)
                {
                    _inJump = false;
                    if (_velocity.Y < 0)
                    {
                        _velocity.Y /= ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 1.1f);
                    }
                }

                // Jump Power
                if (AI.Jump && _inJump && _jumpPower > 0)
                {
                    _velocity.Y += ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * -0.5f);
                    _jumpPower -= ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 1f);
                }

                if (_jumpCount == 0)
                {
                    if (!InAttack)
                    {
                        var rotation = (float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 0.1f +
                                       ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * Math.Abs(_velocity.Y / 200)) +
                                       ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * Math.Abs(_velocity.X / 200));

                        if (_velocity.X < 0)
                            Rotation -= rotation;
                        else if (_velocity.X > 0)
                            Rotation += rotation;
                        else if (_velocity.X == 0)
                        {
                            if (_facesLeft)
                                Rotation -= rotation;
                            else if (!_facesLeft)
                                Rotation += rotation;
                        }
                    }
                    else
                        Rotation = 0f;
                }
                else
                    Rotation = 0f;

            }
        }

        private void Movement(GameTime gameTime)
        {
            if (!AI.Left)
                _momentumLeft = false;
            if (!AI.Right)
                _momentumRight = false;

            if (_enemyState == EnemyStates.Stunned)
            {
                _velocity.X /= ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 1.5f);
            }

            if (_enemyState == EnemyStates.None)
            {
                if (AI.Left)
                {
                    if (!_momentumLeft)
                        _velocity.X = -_movementSpeed;
                    if (_velocity.X > -_movementSpeed)
                        _velocity.X = -_movementSpeed;
                    if (_momentumLeft && _velocity.X > -2 * _movementSpeed)
                        _velocity.X += -((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * _movementSpeed / 40);
                    if (_momentumLeft && _velocity.X > -4 * _movementSpeed && AI.Sprint)
                        _velocity.X += -((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * _movementSpeed / 20);

                    _momentumLeft = true;
                    _momentumRight = false;
                    _soundInstanceWalk.Play();
                }
                if (AI.Right)
                {
                    if (!_momentumRight)
                        _velocity.X = _movementSpeed;
                    if (_velocity.X < _movementSpeed)
                        _velocity.X = _movementSpeed;
                    if (_momentumRight && _velocity.X < 2 * _movementSpeed)
                        _velocity.X += ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * _movementSpeed / 40);
                    if (_momentumRight && _velocity.X < 4 * _movementSpeed && AI.Sprint)
                        _velocity.X += ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * _movementSpeed / 20);

                    _momentumRight = true;
                    _momentumLeft = false;
                    _soundInstanceWalk.Play();
                }
                if (!(AI.Left || AI.Right))
                {
                    _velocity.X = 0;
                    _soundInstanceWalk.Stop();
                }
                if (AI.Left && AI.Right)
                {
                    _velocity.X = 0;
                    _soundInstanceWalk.Stop();
                }
                if ((_velocity.X > 3 * _movementSpeed || _velocity.X < -3 * _movementSpeed) && AI.Sprint)
                {
                    _inSprint = true;
                    _soundInstanceSprint.Play();
                    _soundInstanceWalk.Stop();
                    if (!InAttack)
                        AddExplosion(ExplosionTypes.ShadowSmall);
                }
                else if (_inSprint)
                {
                    _inSprint = false;
                    SoundManager.PlaySoundEffect(16);
                    _soundInstanceSprint.Stop();
                }
                if (_velocity.X == 0 || _jumpCount != 2)
                {
                    _soundInstanceWalk.Stop();
                }

            }
        }

        private void Gravity(GameTime gameTime)
        {
            if (_velocity.X < 0)
                _facesLeft = true;
            if (_velocity.X > 0)
                _facesLeft = false;

            if (_velocity.Y < 40f)
                _velocity.Y += +((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 1.5f);
            else if (_velocity.Y > 40f)
                _velocity.Y /= ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 1.01f);

            if (_onGround)
                _velocity.Y = 0f;
        }

        #endregion

        #region Animation

        protected override void SetAnimations()
        {
            if (AnimationManager != null)
            {
                //if (_stunned)
                //{
                //    AnimationManager.Play(Animations["Stunned"]);
                //    Rotation = 0f;
                //}
                if (InAttack)
                {
                    AnimationManager.Play(Animations["Attack"]);
                }
                //else if (_inFastFall)
                //{
                //    AnimationManager.Play(Animations["FastFall"]);
                //    Rotation = 0f;
                //}
                //else if (!_onGround && _velocity.Y > 0)
                //{
                //    AnimationManager.Play(Animations["Fall"]);
                //}
                //else if (_inJump && _velocity.X < 0)
                //{
                //    AnimationManager.Play(Animations["Jump"]);
                //}
                //else if (_velocity.X > 0 && _inSprint)
                //{
                //    AnimationManager.Play(Animations["SprintRight"]);
                //}
                //else if (_velocity.X < 0 && _inSprint)
                //{
                //    AnimationManager.Play(Animations["SprintLeft"]);
                //}
                else if (_velocity.X > 0)
                {
                    AnimationManager.Play(Animations["MoveRight"]);
                }
                else if (_velocity.X < 0)
                {
                    AnimationManager.Play(Animations["MoveLeft"]);
                }
                else if (_onGround || !InAttack)
                    AnimationManager.Stop();
            }
        }

        #endregion

        #region Collision

        public override void OnCollide(Sprite sprite)
        {
            if (sprite.CollisionType == CollisionTypes.Full || sprite.CollisionType == CollisionTypes.Spikes)
            {
                if (_velocity.X > 0)
                {
                    if (this.Rectangle.Right + this._velocity.X > sprite.Rectangle.Left &&
                        this.Rectangle.Left < sprite.Rectangle.Left &&
                        this.Rectangle.Bottom > sprite.Rectangle.Top &&
                        this.Rectangle.Top < sprite.Rectangle.Bottom)
                    {
                        _velocity.X = 0;
                        _momentumRight = false;
                        AI.Jump = true;
                    }
                }

                if (_velocity.X < 0)
                {
                    if (this.Rectangle.Left + this._velocity.X < sprite.Rectangle.Right &&
                        this.Rectangle.Right > sprite.Rectangle.Right &&
                        this.Rectangle.Bottom > sprite.Rectangle.Top &&
                        this.Rectangle.Top < sprite.Rectangle.Bottom)
                    {
                        _velocity.X = 0;
                        _momentumLeft = false;
                        AI.Jump = true;
                    }
                }

                if (_velocity.Y >= 0)
                {
                    if (this.Rectangle.Bottom + this._velocity.Y > sprite.Rectangle.Top &&
                        this.Rectangle.Top < sprite.Rectangle.Top &&
                        this.Rectangle.Right > sprite.Rectangle.Left &&
                        this.Rectangle.Left < sprite.Rectangle.Right)
                    {
                        _velocity.Y = 0;
                        _jumpPower = 20f;
                        _onGround = true;
                        _jumpCount = 1;
                        _inJump = false;
                        _hasJumped = false;
                        Rotation = 0f;
                    }

                    if (this.Rectangle.Bottom + this._velocity.Y > sprite.Rectangle.Top - 20 &&
                        this.Rectangle.Top < sprite.Rectangle.Top &&
                        this.Rectangle.Right > sprite.Rectangle.Left &&
                        this.Rectangle.Left < sprite.Rectangle.Right)
                        _onGround = true;
                }

                if (_velocity.Y < 0)
                {
                    if (this.Rectangle.Top + this._velocity.Y < sprite.Rectangle.Bottom &&
                        this.Rectangle.Bottom > sprite.Rectangle.Bottom &&
                        this.Rectangle.Right > sprite.Rectangle.Left &&
                        this.Rectangle.Left < sprite.Rectangle.Right)
                    {
                        _velocity.Y = 1;
                    }
                }
            }

            if (sprite.CollisionType == CollisionTypes.Top || sprite.CollisionType == CollisionTypes.TopFalling)
            {
                if (_velocity.Y > 0)
                {
                    if (this.Rectangle.Bottom + this._velocity.Y > sprite.Rectangle.Top &&
                        this.Rectangle.Top < sprite.Rectangle.Top &&
                        this.Rectangle.Right > sprite.Rectangle.Left &&
                        this.Rectangle.Left < sprite.Rectangle.Right)
                    {
                        _velocity.Y = 0;
                        _jumpPower = 20f;
                        _onGround = true;
                        _jumpCount = 1;
                        _inJump = false;
                        _hasJumped = false;
                        Rotation = 0f;
                    }

                    if (this.Rectangle.Bottom + this._velocity.Y > sprite.Rectangle.Top - 20 &&
                        this.Rectangle.Top < sprite.Rectangle.Top &&
                        this.Rectangle.Right > sprite.Rectangle.Left &&
                        this.Rectangle.Left < sprite.Rectangle.Right)
                        _onGround = true;
                }
            }
        }

        #endregion

        #region Hurtboxes

        protected override void AddHurtbox(float damage, HurtboxTypes hurtboxType)
        {
            if (hurtboxType == HurtboxTypes.ShadowWhirl)
            {
                var hurtbox = ShadowWhirl.Clone() as ShadowWhirl;

                hurtbox.Position = this.Position + this._velocity;
                hurtbox.HurtboxType = hurtboxType;
                hurtbox.Colour = this.Colour;
                hurtbox.Layer = 0.2f;
                hurtbox.LifeSpan = 0.01f;
                hurtbox.Velocity = new Vector2(0f, 0f);
                hurtbox.Parent = this;
                hurtbox.Damage = damage;

                Children.Add(hurtbox);
            }
        }

        #endregion
    }
}
