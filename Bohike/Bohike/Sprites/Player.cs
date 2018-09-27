using Bohike.Managers;
using Bohike.Models;
using Bohike.Sprites.Hurtboxes;
using Bohike.Sprites.Hurtboxes.ofPlayer;
using Bohike.States.Levels;
using Bohike.Tilemap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Sprites
{
    public enum PlayerStates
    {
        None,
        Stunned,
    }

    public enum PlayerSounds
    {
        FireWhirl,      // 0
        FastFall,       // 1
        Hurt1,          // 2
        Hurt2,          // 3
        Attack1,        // 4
        Attack2,        // 5
        Attack3,        // 6
        StopCast,       // 7
        GroundPound,    // 8
        Sprint,         // 9
        Jump1,          // 10
        Jump2,          // 11
        WallJump,       // 12
        Cast1,          // 13
        Cast2,          // 14
        Death,          // 15
        StopSprint,     // 16
        Walk,           // 17
    }

    public class Player : Sprite, ICollidable
    {
      
        private KeyboardState _currentKeyboard;
        private KeyboardState _previousKeyboard;
        private GamePadState _currentGamePad;
        private GamePadState _previousGamePad;

        private bool _gamePadSet;

        public Levels Level;
        private PlayerStates _playerState;

        public InputGamePad InputGP;

        #region Variables

        // Hurtboxes
        public GroundPound GroundPound { get; set; }
        public FireWhirl FireWhirl { get; set; }

        // Effects
        public Explosion Explosion;

        // Sound
        private SoundEffectInstance _soundInstanceFireWhirl;
        private SoundEffectInstance _soundInstanceFastFall;
        private SoundEffectInstance _soundInstanceSprint;
        private SoundEffectInstance _soundInstanceWalk;

        // Health
        public float Health = 1f;
        private float _damageCooldown;
        public bool IsHittable = true;

        // Actions
        private bool _inFastFall;
        public bool InAttack;
        public bool CanAttack;
        private float _attackPower = 1f;

        // Movement
        private float _movementSpeed = 5f;
        private bool _onGround;
        private bool _onWall;
        private int _wallJumpCount;
        private bool _momentumLeft;
        private bool _momentumRight;
        private bool _inJump;
        private bool _hasJumped;
        private int _jumpCount;
        private float _jumpPower;
        private bool _inSprint;
        private bool _facesLeft;

        // CheckPoint
        private bool _teleportedOnCheckPoint;

        #endregion

        public Player(Texture2D texture)
          : base(texture)
        {
            Layer = 0.2f;
        }

        public override void Update(GameTime gameTime)
        {
            if (!_gamePadSet)
            {
                InputGP = new InputGamePad()
                {
                    Left = Buttons.DPadLeft,
                    Right = Buttons.DPadRight,
                    Jump = Buttons.A,
                    Attack = Buttons.X,
                    Sprint = Buttons.RightTrigger,
                    FastFall = Buttons.DPadDown,
                    StrongJump = Buttons.DPadUp,
                };
                _gamePadSet = true;
            }

            if (Health >= Game.GlobalVariables.Health)
                Game.GlobalVariables.Health = Health;

            _previousKeyboard = _currentKeyboard;
            _currentKeyboard = Keyboard.GetState();

            _previousGamePad = _currentGamePad;
            _currentGamePad = GamePad.GetState(0);
           
            if (!_teleportedOnCheckPoint)
            {
                switch (Level)
                {
                    case Levels.HubWorld:
                        _teleportedOnCheckPoint = true;
                        Health = Game.GlobalVariables.Health;
                        break;
                    case Levels.TestLevel:
                        Position = Game.GlobalVariables.CheckPointPosition;
                        _teleportedOnCheckPoint = true;
                        SoundManager.PlaySoundEffect(19);
                        Health = Game.GlobalVariables.Health;
                        break;
                    case Levels.BossRoom:
                        _teleportedOnCheckPoint = true;
                        Health = Game.GlobalVariables.Health;
                        break;
                }
            }

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
            _onWall = false;
            
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer > 1337f)
                _timer = 0f;
        }

        private void SetSoundInstances()
        {
            if (_soundInstanceFireWhirl == null)
                _soundInstanceFireWhirl = SoundManager.CreateInstance(0);
            if (_soundInstanceFastFall == null)
                _soundInstanceFastFall = SoundManager.CreateInstance(1);
            if (_soundInstanceSprint == null)
                _soundInstanceSprint = SoundManager.CreateInstance(9);
            if (_soundInstanceWalk == null)
                _soundInstanceWalk = SoundManager.CreateInstance(17);
        }

        public void IncreaseMoney(int amount)
        {
            Game.GlobalVariables.Money += amount;
        }

        public void CollectPowerUp()
        {
            if (Health < 3)
                Health += 1;
            else
                IncreaseMoney(25);
        }

        public void TriggerCheckPoint(Vector2 position)
        {
            Game.GlobalVariables.CheckPointPosition = position;

            StreamWriter writer = new StreamWriter("Savefile.txt");
            string input = String.Join(",", position.X.ToString(), position.Y.ToString(), Game.GlobalVariables.Money, Game.GlobalVariables.Health);
            writer.WriteLine(input);
            writer.Dispose();

            Game.GlobalVariables.NewMoneyCheckpoint();
        }

        #region Health

        private void ManageHealth(GameTime gameTime)
        {
            _damageCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_damageCooldown < 2.5f)
            {
                _playerState = PlayerStates.None;

                if (_damageCooldown % 0.2f <= 0.05)
                {
                    Layer = -1f;
                }
                else if ((_damageCooldown + 0.1f) % 0.2 <= 0.05)
                {
                    Layer = 0.1f;
                }
            }
            
            if (_damageCooldown <= 0f)
            {
                Layer = 0.1f;
                _damageCooldown = 0;
                IsHittable = true;
            }
        }

        public override void IsHit(float damage)
        {
            if (IsHittable)
            {
                _playerState = PlayerStates.Stunned;
                Rotation = 0;
                Health -= damage;
                _damageCooldown = 3f;
                IsHittable = false;

                if (Health <= 0)
                {
                    //IsRemoved = true;
                    //Game.GlobalVariables.LoseMoneyToCheckpoint();
                    SoundManager.PlaySoundEffect(15);
                    _soundInstanceFastFall.Stop();
                    _soundInstanceFireWhirl.Stop();
                    _soundInstanceSprint.Stop();
                    _soundInstanceWalk.Stop();
                    Game.GlobalVariables.Health = 1f;
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
            FastFall(gameTime);
        }

        private void FastFall(GameTime gameTime)
        {
            if (!(_currentGamePad.IsConnected))
            {
                if (_playerState == PlayerStates.None)
                {
                    if (_currentKeyboard.IsKeyDown(Input.FastFall) && _velocity.Y < 60f)
                    {
                        _velocity.Y += +1f;
                        _velocity.X = 0;
                    }

                    if (_currentKeyboard.IsKeyDown(Input.FastFall) && _velocity.Y >= 42f)
                    {
                        AddExplosion(ExplosionTypes.Earth);
                        _inFastFall = true;
                    }
                    if (_currentKeyboard.IsKeyDown(Input.FastFall) && _inFastFall && _onGround)
                    {
                        AddHurtbox(3f, HurtboxTypes.GroundPound);
                        AddExplosion(ExplosionTypes.Earth);
                        _inFastFall = false;
                        SoundManager.PlaySoundEffect(8);
                    }
                    if (_currentKeyboard.IsKeyUp(Input.FastFall) && _inFastFall)
                    {
                        _inFastFall = false;
                        SoundManager.PlaySoundEffect(7);
                    }

                    if (_inFastFall)
                    {
                        _soundInstanceFastFall.Play();
                    }
                    else
                        _soundInstanceFastFall.Stop();
                }
            }
            else
            {
                if (_playerState == PlayerStates.None)
                {
                    if (_currentGamePad.IsButtonDown(InputGP.FastFall) && _velocity.Y < 60f)
                    {
                        _velocity.Y += +1f;
                        _velocity.X = 0;
                    }

                    if (_currentGamePad.IsButtonDown(InputGP.FastFall) && _velocity.Y >= 42f)
                    {
                        AddExplosion(ExplosionTypes.Earth);
                        _inFastFall = true;
                    }
                    if (_currentGamePad.IsButtonDown(InputGP.FastFall) && _inFastFall && _onGround)
                    {
                        AddHurtbox(3f, HurtboxTypes.GroundPound);
                        AddExplosion(ExplosionTypes.Earth);
                        _inFastFall = false;
                        SoundManager.PlaySoundEffect(8);
                    }
                    if (_currentGamePad.IsButtonUp(InputGP.FastFall) && _inFastFall)
                    {
                        _inFastFall = false;
                        SoundManager.PlaySoundEffect(7);
                    }

                    if (_inFastFall)
                    {
                        _soundInstanceFastFall.Play();
                    }
                    else
                        _soundInstanceFastFall.Stop();
                }
            }
        }

        private void Attack(GameTime gameTime)
        {
            if (!(_currentGamePad.IsConnected))
            {
                if (_playerState == PlayerStates.None)
                {
                    if (_currentKeyboard.IsKeyDown(Input.Attack) && CanAttack)
                    {
                        if (_velocity.Y > 0)
                            _velocity.Y /= 1.3f;

                        if (!InAttack)
                            //SoundManager.PlaySoundEffect(Game1.Random.Next(13, 15));

                            InAttack = true;
                        _attackPower -= (float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f;

                        AddHurtbox(1f, HurtboxTypes.FireWhirl);
                        AddExplosion(ExplosionTypes.FireWhirl);
                    }
                }

                if ((InAttack && _currentKeyboard.IsKeyUp(Input.Attack)) || (InAttack && _playerState != PlayerStates.None))
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
                    _attackPower += (float)gameTime.ElapsedGameTime.TotalSeconds * 1f;

                if (InAttack)
                {
                    _soundInstanceFireWhirl.Play();
                }
                else
                    _soundInstanceFireWhirl.Stop();
            }
            else
            {
                if (_playerState == PlayerStates.None)
                {
                    if (_currentGamePad.IsButtonDown(InputGP.Attack) && CanAttack)
                    {
                        if (_velocity.Y > 0)
                            _velocity.Y /= 1.3f;

                        if (!InAttack)
                            //SoundManager.PlaySoundEffect(Game1.Random.Next(13, 15));

                            InAttack = true;
                        _attackPower -= (float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f;

                        AddHurtbox(1f, HurtboxTypes.FireWhirl);
                        AddExplosion(ExplosionTypes.FireWhirl);
                    }
                }

                if ((InAttack && _currentGamePad.IsButtonUp(InputGP.Attack)) || (InAttack && _playerState != PlayerStates.None))
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
                    _attackPower += (float)gameTime.ElapsedGameTime.TotalSeconds * 1f;

                if (InAttack)
                {
                    _soundInstanceFireWhirl.Play();
                }
                else
                    _soundInstanceFireWhirl.Stop();
            }
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

            if (!(_currentGamePad.IsConnected))
            {
                if (_playerState == PlayerStates.None)
                {
                    if (_currentKeyboard.IsKeyDown(Input.Jump) && _previousKeyboard.IsKeyUp(Input.Jump) && !_hasJumped && _onGround)
                    {
                        _velocity.Y = -25f;
                        SoundManager.PlaySoundEffect(Game1.Random.Next(10, 12));
                        _inJump = true;
                        _hasJumped = true;
                        _jumpCount--;
                        AddExplosion(ExplosionTypes.Air);
                    }
                    // Wall Jump & Double Jump
                    else if (_currentKeyboard.IsKeyDown(Input.Jump) && _previousKeyboard.IsKeyUp(Input.Jump) && _wallJumpCount > 0 && _onWall && !_onGround)
                    {
                        _velocity.Y = -25f;
                        SoundManager.PlaySoundEffect(Game1.Random.Next(10, 12));
                        _jumpPower = 20f;
                        _inJump = true;
                        _hasJumped = true;
                        _wallJumpCount--;
                        AddExplosion(ExplosionTypes.Earth);
                    }
                    else if (_currentKeyboard.IsKeyDown(Input.Jump) && _previousKeyboard.IsKeyUp(Input.Jump) && _jumpCount > 0 && !_onGround)
                    {
                        _velocity.Y = -25f;
                        SoundManager.PlaySoundEffect(Game1.Random.Next(10, 12));
                        _jumpPower = 20f;
                        _inJump = true;
                        _hasJumped = true;
                        _jumpCount--;
                        AddExplosion(ExplosionTypes.Air);
                    }

                    // Jump Break
                    if (_currentKeyboard.IsKeyUp(Input.Jump))
                    {
                        _inJump = false;
                        if (_velocity.Y < 0)
                        {
                            _velocity.Y /= ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 1.1f);
                        }
                    }

                    // Jump Power
                    if (_currentKeyboard.IsKeyDown(Input.Jump) && _inJump && _jumpPower > 0)
                    {
                        _velocity.Y += ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * -0.5f);
                        _jumpPower -= ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 1f);
                    }

                    if ((_jumpCount == 0 && !(_onWall)) || (_onWall && _wallJumpCount == 0 && _jumpCount == 0))
                    {
                        if (!(InAttack || _inFastFall))
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
            // --- GamePad ---
            else
            {
                if (_playerState == PlayerStates.None)
                {
                    if (_currentGamePad.IsButtonDown(InputGP.Jump) && _previousGamePad.IsButtonUp(InputGP.Jump) && !_hasJumped && _onGround)
                    {
                        _velocity.Y = -25f;
                        SoundManager.PlaySoundEffect(Game1.Random.Next(10, 12));
                        _inJump = true;
                        _hasJumped = true;
                        _jumpCount--;
                        AddExplosion(ExplosionTypes.Air);
                    }
                    // Wall Jump & Double Jump
                    else if (_currentGamePad.IsButtonDown(InputGP.Jump) && _previousGamePad.IsButtonUp(InputGP.Jump) && _wallJumpCount > 0 && _onWall && !_onGround)
                    {
                        _velocity.Y = -25f;
                        SoundManager.PlaySoundEffect(Game1.Random.Next(10, 12));
                        _jumpPower = 20f;
                        _inJump = true;
                        _hasJumped = true;
                        _wallJumpCount--;
                        AddExplosion(ExplosionTypes.Earth);
                    }
                    else if (_currentGamePad.IsButtonDown(InputGP.Jump) && _previousGamePad.IsButtonUp(InputGP.Jump) && _jumpCount > 0 && !_onGround)
                    {
                        _velocity.Y = -25f;
                        SoundManager.PlaySoundEffect(Game1.Random.Next(10, 12));
                        _jumpPower = 20f;
                        _inJump = true;
                        _hasJumped = true;
                        _jumpCount--;
                        AddExplosion(ExplosionTypes.Air);
                    }

                    // Jump Break
                    if (_currentGamePad.IsButtonUp(InputGP.Jump))
                    {
                        _inJump = false;
                        if (_velocity.Y < 0)
                        {
                            _velocity.Y /= ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 1.1f);
                        }
                    }

                    // Jump Power
                    if (_currentGamePad.IsButtonDown(InputGP.Jump) && _inJump && _jumpPower > 0)
                    {
                        _velocity.Y += ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * -0.5f);
                        _jumpPower -= ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 1f);
                    }

                    if ((_jumpCount == 0 && !(_onWall)) || (_onWall && _wallJumpCount == 0 && _jumpCount == 0))
                    {
                        if (!(InAttack || _inFastFall))
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
        }

        private void Movement(GameTime gameTime)
        {
            if (!(_currentGamePad.IsConnected))
            {
                if (!(_currentKeyboard.IsKeyDown(Input.Left)))
                    _momentumLeft = false;
                if (!(_currentKeyboard.IsKeyDown(Input.Right)))
                    _momentumRight = false;

                if (_playerState == PlayerStates.Stunned)
                {
                    _velocity.X /= ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 1.5f);
                }

                if (_playerState == PlayerStates.None)
                {
                    if (_currentKeyboard.IsKeyDown(Input.Left))
                    {
                        if (!_momentumLeft)
                            _velocity.X = -_movementSpeed;
                        if (_velocity.X > -_movementSpeed)
                            _velocity.X = -_movementSpeed;
                        if (_momentumLeft && _velocity.X > -2 * _movementSpeed)
                            _velocity.X += -((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * _movementSpeed / 40);
                        if (_momentumLeft && _velocity.X > -4 * _movementSpeed && _currentKeyboard.IsKeyDown(Input.Sprint))
                            _velocity.X += -((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * _movementSpeed / 20);

                        _momentumLeft = true;
                        _momentumRight = false;
                        _soundInstanceWalk.Play();
                    }
                    if (_currentKeyboard.IsKeyDown(Input.Right))
                    {
                        if (!_momentumRight)
                            _velocity.X = _movementSpeed;
                        if (_velocity.X < _movementSpeed)
                            _velocity.X = _movementSpeed;
                        if (_momentumRight && _velocity.X < 2 * _movementSpeed)
                            _velocity.X += ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * _movementSpeed / 40);
                        if (_momentumRight && _velocity.X < 4 * _movementSpeed && _currentKeyboard.IsKeyDown(Input.Sprint))
                            _velocity.X += ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * _movementSpeed / 20);

                        _momentumRight = true;
                        _momentumLeft = false;
                        _soundInstanceWalk.Play();
                    }
                    if (!((_currentKeyboard.IsKeyDown(Input.Left)) || (_currentKeyboard.IsKeyDown(Input.Right))))
                    {
                        _velocity.X = 0;
                        _soundInstanceWalk.Stop();
                    }
                    if ((_currentKeyboard.IsKeyDown(Input.Left)) && (_currentKeyboard.IsKeyDown(Input.Right)))
                    {
                        _velocity.X = 0;
                        _soundInstanceWalk.Stop();
                    }
                    if ((_velocity.X > 3 * _movementSpeed || _velocity.X < -3 * _movementSpeed) && _currentKeyboard.IsKeyDown(Input.Sprint))
                    {
                        _inSprint = true;
                        _soundInstanceSprint.Play();
                        _soundInstanceWalk.Stop();
                        if (!InAttack)
                            AddExplosion(ExplosionTypes.Water);
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
            // --- GamePad ---
            else
            {
                if (!(_currentGamePad.IsButtonDown(InputGP.Left)))
                    _momentumLeft = false;
                if (!(_currentGamePad.IsButtonDown(InputGP.Right)))
                    _momentumRight = false;

                if (_playerState == PlayerStates.Stunned)
                {
                    _velocity.X /= ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 1.5f);
                }

                if (_playerState == PlayerStates.None)
                {
                    if (_currentGamePad.IsButtonDown(InputGP.Left))
                    {
                        if (!_momentumLeft)
                            _velocity.X = -_movementSpeed;
                        if (_velocity.X > -_movementSpeed)
                            _velocity.X = -_movementSpeed;
                        if (_momentumLeft && _velocity.X > -2 * _movementSpeed)
                            _velocity.X += -((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * _movementSpeed / 40);
                        if (_momentumLeft && _velocity.X > -4 * _movementSpeed && _currentGamePad.IsButtonDown(InputGP.Sprint))
                            _velocity.X += -((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * _movementSpeed / 20);

                        _momentumLeft = true;
                        _momentumRight = false;
                        _soundInstanceWalk.Play();
                    }
                    if (_currentGamePad.IsButtonDown(InputGP.Right))
                    {
                        if (!_momentumRight)
                            _velocity.X = _movementSpeed;
                        if (_velocity.X < _movementSpeed)
                            _velocity.X = _movementSpeed;
                        if (_momentumRight && _velocity.X < 2 * _movementSpeed)
                            _velocity.X += ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * _movementSpeed / 40);
                        if (_momentumRight && _velocity.X < 4 * _movementSpeed && _currentGamePad.IsButtonDown(InputGP.Sprint))
                            _velocity.X += ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * _movementSpeed / 20);

                        _momentumRight = true;
                        _momentumLeft = false;
                        _soundInstanceWalk.Play();
                    }
                    if (!((_currentGamePad.IsButtonDown(InputGP.Left)) || (_currentGamePad.IsButtonDown(InputGP.Right))))
                    {
                        _velocity.X = 0;
                        _soundInstanceWalk.Stop();
                    }
                    if ((_currentGamePad.IsButtonDown(InputGP.Left)) && (_currentGamePad.IsButtonDown(InputGP.Right)))
                    {
                        _velocity.X = 0;
                        _soundInstanceWalk.Stop();
                    }
                    if ((_velocity.X > 3 * _movementSpeed || _velocity.X < -3 * _movementSpeed) && _currentGamePad.IsButtonDown(InputGP.Sprint))
                    {
                        _inSprint = true;
                        _soundInstanceSprint.Play();
                        _soundInstanceWalk.Stop();
                        if (!InAttack)
                            AddExplosion(ExplosionTypes.Water);
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
            
        }

        private void Gravity(GameTime gameTime)
        {
            if (_velocity.X < 0)
                _facesLeft = true;
            if (_velocity.X > 0)
                _facesLeft = false;

            if (_onWall && _wallJumpCount > 0)
                _velocity.Y /= ((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 2);

            if (_velocity.Y < 40f)
                _velocity.Y += +((float)gameTime.ElapsedGameTime.TotalSeconds * 60 * 1.5f);
            else if (_velocity.Y > 40f && !_inFastFall)
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
            if (sprite.CollisionType == CollisionTypes.Full)
            {
                CollideFull(sprite);
            }

            if (sprite.CollisionType == CollisionTypes.Top || sprite.CollisionType == CollisionTypes.TopFalling)
            {
                CollideTop(sprite);
            }

            if (sprite.CollisionType == CollisionTypes.Lava)
            {
                CollideLava(sprite);
            }

            if (sprite.CollisionType == CollisionTypes.Spikes)
            {
                IsHit(1f);
                CollideFull(sprite);
            }

            if (sprite.CollisionType == CollisionTypes.Bounce)
            {
                CollideBounce(sprite);
            }
        }

        private void CollideLava(Sprite sprite)
        {
            if (this.Rectangle.Bottom + this._velocity.Y > sprite.Rectangle.Bottom &&
                                      this.Rectangle.Top < sprite.Rectangle.Bottom &&
                                      this.Rectangle.Right > sprite.Rectangle.Left &&
                                      this.Rectangle.Left < sprite.Rectangle.Right)
            {
                IsHittable = true;
                IsHit(999f);
            }
        }

        private void CollideTop(Sprite sprite)
        {
            if (_velocity.Y > 0 && !Keyboard.GetState().IsKeyDown(Input.FastFall))
            {
                if (this.Rectangle.Bottom + this._velocity.Y > sprite.Rectangle.Top &&
                    this.Rectangle.Top < sprite.Rectangle.Top &&
                    this.Rectangle.Right > sprite.Rectangle.Left &&
                    this.Rectangle.Left < sprite.Rectangle.Right)
                {
                    _velocity.Y = 0;
                    _jumpPower = 20f;
                    _onGround = true;
                    _wallJumpCount = 1;
                    _jumpCount = 2;
                    _inJump = false;
                    _hasJumped = false;
                    Rotation = 0f;
                    sprite.WasSteppedOn = true;
                }

                if (this.Rectangle.Bottom + this._velocity.Y > sprite.Rectangle.Top - 20 &&
                    this.Rectangle.Top < sprite.Rectangle.Top &&
                    this.Rectangle.Right > sprite.Rectangle.Left &&
                    this.Rectangle.Left < sprite.Rectangle.Right)
                    _onGround = true;
            }
        }

        private void CollideBounce(Sprite sprite)
        {
            if (_velocity.Y > 0)
            {
                if (this.Rectangle.Bottom + this._velocity.Y > sprite.Rectangle.Top &&
                    this.Rectangle.Top < sprite.Rectangle.Top &&
                    this.Rectangle.Right > sprite.Rectangle.Left &&
                    this.Rectangle.Left < sprite.Rectangle.Right)
                {
                    _velocity.Y = -60f;
                    SoundManager.PlaySoundEffect(Game1.Random.Next(10, 12));
                    SoundManager.PlaySoundEffect(18);
                    _inJump = true;
                    _inFastFall = false;
                    _wallJumpCount = 1;
                    _hasJumped = true;
                    _jumpCount = 1;
                    _onGround = false;
                    AddExplosion(ExplosionTypes.Air);
                }

                if (this.Rectangle.Bottom + this._velocity.Y > sprite.Rectangle.Top - 20 &&
                    this.Rectangle.Top < sprite.Rectangle.Top &&
                    this.Rectangle.Right > sprite.Rectangle.Left &&
                    this.Rectangle.Left < sprite.Rectangle.Right)
                    _onGround = true;
            }

            if (_velocity.X > 0)
            {
                if (this.Rectangle.Right + this._velocity.X > sprite.Rectangle.Left &&
                    this.Rectangle.Left < sprite.Rectangle.Left &&
                    this.Rectangle.Bottom > sprite.Rectangle.Top &&
                    this.Rectangle.Top < sprite.Rectangle.Bottom)
                {
                    _velocity.X = 0;
                    _onWall = true;
                    _momentumRight = false;
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
                    _onWall = true;
                    _momentumLeft = false;
                }
            }

            if (_velocity.Y < 0 && !(sprite is Enemy))
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

        private void CollideFull(Sprite sprite)
        {
            if (_velocity.X > 0)
            {
                if (this.Rectangle.Right + this._velocity.X > sprite.Rectangle.Left &&
                    this.Rectangle.Left < sprite.Rectangle.Left &&
                    this.Rectangle.Bottom > sprite.Rectangle.Top &&
                    this.Rectangle.Top < sprite.Rectangle.Bottom)
                {
                    _velocity.X = 0;
                    _onWall = true;
                    _momentumRight = false;
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
                    _onWall = true;
                    _momentumLeft = false;
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
                    _wallJumpCount = 1;
                    _jumpCount = 2;
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

            if (_velocity.Y < 0 && !(sprite is Enemy))
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

        #endregion

        #region Hurtboxes

        private void AddExplosion(ExplosionTypes explosionType)
        {
            if (Explosion == null)
                return;

            var explosion = Explosion.Clone() as Explosion;

            explosion.Position = this.Position + this._velocity;
            explosion.Animation = (int)explosionType;

            Children.Add(explosion);
        }

        private void AddHurtbox(float damage, HurtboxTypes hurtboxType)
        {
            if (hurtboxType == HurtboxTypes.GroundPound)
            {
                var hurtbox = GroundPound.Clone() as GroundPound;

                hurtbox.Position = this.Position + this._velocity;
                hurtbox.HurtboxType = hurtboxType;
                hurtbox.Colour = this.Colour;
                hurtbox.Layer = 0.3f;
                hurtbox.LifeSpan = 0.10f;
                hurtbox.Velocity = new Vector2(0f, 0f);
                hurtbox.Parent = this;
                hurtbox.Damage = damage;

                Children.Add(hurtbox);
            }
            if (hurtboxType == HurtboxTypes.FireWhirl)
            {
                var hurtbox = FireWhirl.Clone() as FireWhirl;

                hurtbox.Position = this.Position + this._velocity;
                hurtbox.HurtboxType = hurtboxType;
                hurtbox.Colour = this.Colour;
                hurtbox.Layer = 0.3f;
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
