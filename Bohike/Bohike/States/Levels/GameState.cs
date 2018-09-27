using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bohike.Core;
using Bohike.Managers;
using Bohike.Sprites;
using Bohike.Sprites.Enemies;
using Bohike.Tilemap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bohike.States.Levels
{
    public enum Levels
    {
        TestLevel,
        HubWorld,
        BossRoom,
    }

    public class GameState : State
    {
        protected List<Sprite> _sprites;
        protected List<Component> _components;
        protected Camera _camera;
        protected Texture2D _staticBackgroundTexture;
        protected Texture2D _farBackgroundTexture;
        protected Texture2D _midBackgroundTexture;
        protected Texture2D _directBackgroundTexture;
        protected Texture2D _frontBackgroundTexture;
        protected Vector2 _cameraTarget;
        protected float _cameraZoomTarget;
        protected Map _map;
        protected int[,] _grid;
        protected int[,] _enemyGrid;
        protected int _tileSize = 92;
        protected int _gridWidth;
        protected int _gridHeight;
        protected int _enemyGridWidth;
        protected int _enemyGridHeight;
        protected SoundManager _soundManager;
        protected Player _target;
        protected Player _player;
        protected Vector2 _frontBackgroundPosition;
        protected Vector2 _midBackgroundPosition;
        protected Vector2 _farBackgroundPosition;
        protected float _cameraZoom = 1f;
        protected bool _easterEgg;

        protected Levels _level;
        protected TestLevel _testLevel;

        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) 
            : base(game, graphicsDevice, content)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public void Pause(State state)
        {
            _game.ChangeState(new PauseState(_game, _graphicsDevice, _content)
            {
                LastState = state,
            });
        }

        #region PostUpdate

        public override void PostUpdate(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Pause(this);

            var collidableSprites = _sprites.Select(c => c as Sprite).Where(c => c.CollisionType != CollisionTypes.None && Math.Sqrt((c.Position.X - _cameraTarget.X) * (c.Position.X - _cameraTarget.X) + (c.Position.Y - _cameraTarget.Y) * (c.Position.Y - _cameraTarget.Y)) < 1500);

            foreach (var spriteA in collidableSprites)
            {
                if (!(spriteA is Tile || spriteA is Explosion || spriteA is Chest))
                {
                    if (spriteA is Hurtbox && (float)Math.Sqrt((spriteA.Position.X - _cameraTarget.X) * (spriteA.Position.X - _cameraTarget.X) + (spriteA.Position.Y - _cameraTarget.Y) * (spriteA.Position.Y - _cameraTarget.Y)) < 1500) 
                    {
                        foreach (var spriteB in collidableSprites)
                        {
                            if ((float)Math.Sqrt((spriteB.Position.X - _cameraTarget.X) * (spriteB.Position.X - _cameraTarget.X) + (spriteB.Position.Y - _cameraTarget.Y) * (spriteB.Position.Y - _cameraTarget.Y)) < 1500)
                            {
                                // Don't do anything if they're the same sprite!
                                if (spriteA == spriteB)
                                    continue;

                                if (spriteA.WillIntersect(spriteB))
                                    spriteA.OnCollide(spriteB);
                            }
                        }
                    }
                    else if ((float)Math.Sqrt((spriteA.Position.X - _cameraTarget.X) * (spriteA.Position.X - _cameraTarget.X) + (spriteA.Position.Y - _cameraTarget.Y) * (spriteA.Position.Y - _cameraTarget.Y)) < 1500)
                    {
                        foreach (var spriteB in collidableSprites)
                        {
                            if ((float)Math.Sqrt((spriteB.Position.X - _cameraTarget.X) * (spriteB.Position.X - _cameraTarget.X) + (spriteB.Position.Y - _cameraTarget.Y) * (spriteB.Position.Y - _cameraTarget.Y)) < 1500)
                            {
                                // Don't do anything if they're the same sprite!
                                if (spriteA == spriteB)
                                    continue;

                                if (spriteA.WillIntersect(spriteB))
                                    spriteA.OnCollide(spriteB);
                            }
                        }
                    }
                    
                }
            }

            foreach (var component in _sprites)
            {
                component.PostUpdate(gameTime);
            }

            ControlCamera(_grid);

            for (int i = 0; i < _sprites.Count; i++)
            {
                if (_sprites[i].IsRemoved)
                {
                    _sprites.RemoveAt(i);
                    i--;
                }
            }
        }

        protected virtual void ControlCamera(int[,] grid)
        {
            foreach (var component in _sprites)
            {
                if (component.CameraTarget)
                {
                    if (Math.Abs(_cameraTarget.X - component.Position.X) > 1200)
                        _cameraTarget = component.Position;

                    _cameraTarget.X += -(_cameraTarget.X - component.Position.X) / 200;
                    if (Keyboard.GetState().IsKeyDown(Keys.A) || GamePad.GetState(0).IsButtonDown(Buttons.DPadLeft))
                        _cameraTarget.X += -(_cameraTarget.X - component.Position.X - component.Velocity.X * 80) / 60;
                    if (Keyboard.GetState().IsKeyDown(Keys.D) || GamePad.GetState(0).IsButtonDown(Buttons.DPadRight))
                        _cameraTarget.X += -(_cameraTarget.X - component.Position.X - component.Velocity.X * 80) / 60;

                    if (_cameraTarget.Y < component.Position.Y - 100)
                        _cameraTarget.Y += -(_cameraTarget.Y - component.Position.Y + 100) / 5;
                    if (_cameraTarget.Y > component.Position.Y - 100)
                        _cameraTarget.Y += -(_cameraTarget.Y - component.Position.Y + 100) / 30;


                    _cameraZoom += -(_cameraZoom - _cameraZoomTarget) / 150;


                    if ((component.Velocity.Y > 25f || component.Velocity.Y < -25f) || (component.Velocity.X > 8f || component.Velocity.X < -8f))
                    {
                        _cameraZoomTarget = 0.88f;
                    }
                    if ((component.Velocity.X > 16f || component.Velocity.X < -16f) || (component.Velocity.Y > 40f || component.Velocity.Y < -40f))
                    {
                        _cameraZoomTarget = 0.7f;
                    }
                    if (!((component.Velocity.Y > 25f || component.Velocity.Y < -25f) ||
                         (component.Velocity.X > 8f || component.Velocity.X < -8f)))
                    {
                        _cameraZoomTarget = 1f;
                    }
                    if (!_easterEgg)
                    {
                        if (_cameraTarget.X >= (grid.GetLength(1) * _tileSize) - ((Game1.ScreenWidth / 2) * (1 / _cameraZoom)) - _tileSize / 2)
                        {
                            _cameraTarget.X = (grid.GetLength(1) * _tileSize) - ((Game1.ScreenWidth / 2) * (1 / _cameraZoom)) - _tileSize / 2;
                        }
                        if (_cameraTarget.X <= (Game1.ScreenWidth / 2) * (1 / _cameraZoom) - _tileSize / 2)
                        {
                            _cameraTarget.X = (Game1.ScreenWidth / 2) * (1 / _cameraZoom) - _tileSize / 2;
                        }
                        if (_cameraTarget.Y >= (grid.GetLength(0) * _tileSize) - ((Game1.ScreenHeight / 2) * (1 / _cameraZoom)) - _tileSize / 2)
                        {
                            _cameraTarget.Y = (grid.GetLength(0) * _tileSize) - ((Game1.ScreenHeight / 2) * (1 / _cameraZoom)) - _tileSize / 2;
                        }
                    }
                }

                _camera.Adjust(_cameraTarget, _cameraZoom);
            }
        }

        protected virtual void ControlCameraHub(int[,] grid)
        {
            foreach (var component in _sprites)
            {
                if (component.CameraTarget)
                {
                    if (!_easterEgg)
                    {
                        if (_cameraTarget.Y <= (Game1.ScreenHeight / 2) * (1 / _cameraZoom) - _tileSize / 2)
                        {
                            _cameraTarget.Y = (Game1.ScreenHeight / 2) * (1 / _cameraZoom) - _tileSize / 2;
                        }
                    }
                }
                _cameraZoom = 1f;
                _camera.Adjust(_cameraTarget, _cameraZoom);
            }
        }

        protected virtual void ControlCameraBoss(int[,] grid)
        {
            foreach (var component in _sprites)
            {
                if (component.CameraTarget)
                {
                    if (!_easterEgg)
                    {
                        if (_cameraTarget.Y <= (Game1.ScreenHeight / 2) * (1 / _cameraZoom) - _tileSize / 2)
                        {
                            _cameraTarget.Y = (Game1.ScreenHeight / 2) * (1 / _cameraZoom) - _tileSize / 2;
                        }
                    }
                }
                _cameraZoom = 0.85f;
                _camera.Adjust(_cameraTarget, _cameraZoom);
            }
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        protected virtual void ControlEnemies()
        {
            var targetMaxRange = 1500;
            var targetTrapRange = 1000;

            _target = null;

            foreach (var player in _sprites.Select(c => c as Player))
            {
                if (player is Player)
                    _target = player;
            }
            foreach (var enemy in _sprites.Select(c => c as Enemy))
            {
                if (enemy != null)
                {
                    enemy.Target = _target;
                }
            }

            // --- TIKI WARRIOR ---

            foreach (var enemy in _sprites.Select(c => c as TikiWarrior))
            {
                if (enemy != null && _target != null)
                {
                    enemy.AI.DistanceX = Math.Abs(_target.Position.X - enemy.Position.X);
                    enemy.AI.DistanceY = Math.Abs(_target.Position.Y - enemy.Position.Y);
                    enemy.AI.DistanceToTarget = (float)Math.Sqrt((_target.Position.X - enemy.Position.X) * (_target.Position.X - enemy.Position.X) + (_target.Position.Y - enemy.Position.Y) * (_target.Position.Y - enemy.Position.Y));
                }

                if (_target != null)
                {
                    if (enemy is TikiWarrior && enemy.AI.DistanceY <= 1000)
                    {
                        if (_target.Position.X < enemy.Position.X && enemy.AI.DistanceToTarget <= targetMaxRange)
                        {
                            if (!_target.InAttack && enemy.AI.DistanceX >= 100)
                                enemy.AI.Left = true;
                            else if (_target.InAttack && enemy.AI.DistanceToTarget <= 1000 && !enemy.InAttack)
                                enemy.AI.Right = true;
                            else if (enemy.InAttack)
                                enemy.AI.Left = true;
                        }
                        else if (_target.Position.X > enemy.Position.X && enemy.AI.DistanceToTarget <= targetMaxRange)
                        {
                            if (!_target.InAttack && enemy.AI.DistanceX >= 100)
                                enemy.AI.Right = true;
                            else if (_target.InAttack && enemy.AI.DistanceToTarget <= 1000 && !enemy.InAttack)
                                enemy.AI.Left = true;
                            else if (enemy.InAttack)
                                enemy.AI.Right = true;
                        }

                        if (_target.Position.Y < enemy.Position.Y - 100 && enemy.AI.DistanceX <= 500)
                        {
                            enemy.AI.Jump = true;
                        }

                        if (enemy.AI.DistanceToTarget <= 1000 && enemy.CanAttack)
                        {
                            enemy.AI.Attack = true;
                        }
                    }
                    if (enemy?.AI.DistanceY > 1000 || enemy?.AI.DistanceToTarget > targetMaxRange)
                    {
                        if (enemy.Position.X < enemy.StartingPosition.X)
                            enemy.AI.Right = true;
                        if (enemy.Position.X > enemy.StartingPosition.X)
                            enemy.AI.Left = true;
                    }
                }
            }

            // --- TIKI DARTER ---

            foreach (var enemy in _sprites.Select(c => c as TikiDarter))
            {
                if (enemy != null && _target != null)
                {
                    enemy.AI.DistanceX = Math.Abs(_target.Position.X - enemy.Position.X);
                    enemy.AI.DistanceY = Math.Abs(_target.Position.Y - enemy.Position.Y);
                    enemy.AI.DistanceToTarget = (float)Math.Sqrt((_target.Position.X - enemy.Position.X) * (_target.Position.X - enemy.Position.X) + (_target.Position.Y - enemy.Position.Y) * (_target.Position.Y - enemy.Position.Y));
                }

                if (_target != null)
                {
                    if (enemy is TikiDarter && enemy.AI.DistanceY <= 1000)
                    {
                        if (_target.Position.X < enemy.Position.X && enemy.AI.DistanceToTarget <= targetMaxRange)
                        {
                            if (enemy.AI.DistanceX >= 1100)
                                enemy.AI.Left = true;
                            else if (enemy.AI.DistanceToTarget <= 900)
                                enemy.AI.Right = true;
                        }
                        else if (_target.Position.X > enemy.Position.X && enemy.AI.DistanceToTarget <= targetMaxRange)
                        {
                            if (enemy.AI.DistanceX >= 1100)
                                enemy.AI.Right = true;
                            else if (enemy.AI.DistanceToTarget <= 900)
                                enemy.AI.Left = true;
                            else if (enemy.InAttack)
                                enemy.AI.Right = true;
                        }

                        if (_target.Position.Y < enemy.Position.Y - 100 && enemy.AI.DistanceX <= 500)
                        {
                            enemy.AI.Jump = true;
                        }

                        if (enemy.AI.DistanceToTarget <= targetMaxRange && enemy.CanAttack)
                        {
                            enemy.AI.Attack = true;
                        }
                    }
                    if (enemy?.AI.DistanceY > 1000 || enemy?.AI.DistanceToTarget > targetMaxRange)
                    {
                        if (enemy.Position.X < enemy.StartingPosition.X)
                            enemy.AI.Right = true;
                        if (enemy.Position.X > enemy.StartingPosition.X)
                            enemy.AI.Left = true;
                    }
                }
            }

            // --- TIKI TORCHER ---

            foreach (var enemy in _sprites.Select(c => c as TikiTorcher))
            {
                if (enemy != null && _target != null)
                {
                    enemy.AI.DistanceX = Math.Abs(_target.Position.X - enemy.Position.X);
                    enemy.AI.DistanceY = Math.Abs(_target.Position.Y - enemy.Position.Y);
                    enemy.AI.DistanceToTarget = (float)Math.Sqrt((_target.Position.X - enemy.Position.X) * (_target.Position.X - enemy.Position.X) + (_target.Position.Y - enemy.Position.Y) * (_target.Position.Y - enemy.Position.Y));
                }

                if (_target != null)
                {
                    if (enemy is TikiTorcher && enemy.AI.DistanceY <= 2000)
                    {
                        enemy.AI.Attack = true;
                        //enemy.AI.Sprint = true;

                        if (_target.Position.X < enemy.Position.X && enemy.AI.DistanceToTarget <= targetMaxRange)
                        {
                            if (!_target.InAttack && enemy.AI.DistanceX >= 100)
                                enemy.AI.Left = true;
                            else if (_target.InAttack && enemy.AI.DistanceToTarget <= 1000 && !enemy.InAttack)
                                enemy.AI.Right = true;
                            else if (enemy.InAttack)
                                enemy.AI.Left = true;
                        }
                        else if (_target.Position.X > enemy.Position.X && enemy.AI.DistanceToTarget <= targetMaxRange)
                        {
                            if (!_target.InAttack && enemy.AI.DistanceX >= 100)
                                enemy.AI.Right = true;
                            else if (_target.InAttack && enemy.AI.DistanceToTarget <= 1000 && !enemy.InAttack)
                                enemy.AI.Left = true;
                            else if (enemy.InAttack)
                                enemy.AI.Right = true;
                        }

                        if (_target.Position.Y < enemy.Position.Y - 100 && enemy.AI.DistanceX <= 500)
                        {
                            enemy.AI.Jump = true;
                        }

                        if (enemy.AI.DistanceToTarget <= 2000 && enemy.CanAttack)
                        {
                            enemy.AI.Attack = true;
                        }
                    }
                    if (enemy?.AI.DistanceY > 2000 || enemy?.AI.DistanceToTarget > targetMaxRange)
                    {
                        if (enemy.Position.X < enemy.StartingPosition.X)
                            enemy.AI.Right = true;
                        if (enemy.Position.X > enemy.StartingPosition.X)
                            enemy.AI.Left = true;
                    }
                }
            }

            // --- TIKI SHAMAN ---

            foreach (var enemy in _sprites.Select(c => c as TikiShaman))
            {
                if (enemy != null && _target != null)
                {
                    enemy.AI.DistanceX = Math.Abs(_target.Position.X - enemy.Position.X);
                    enemy.AI.DistanceY = Math.Abs(_target.Position.Y - enemy.Position.Y);
                    enemy.AI.DistanceToTarget = (float)Math.Sqrt((_target.Position.X - enemy.Position.X) * (_target.Position.X - enemy.Position.X) + (_target.Position.Y - enemy.Position.Y) * (_target.Position.Y - enemy.Position.Y));
                }

                if (_target != null)
                {
                    if (enemy is TikiShaman && enemy.AI.DistanceY <= 1000)
                    {
                        if (_target.Position.X < enemy.Position.X && enemy.AI.DistanceToTarget <= targetMaxRange)
                        {
                            if (enemy.AI.DistanceX >= 1100)
                                enemy.AI.Left = true;
                            else if (enemy.AI.DistanceToTarget <= 900)
                                enemy.AI.Right = true;
                        }
                        else if (_target.Position.X > enemy.Position.X && enemy.AI.DistanceToTarget <= targetMaxRange)
                        {
                            if (enemy.AI.DistanceX >= 1100)
                                enemy.AI.Right = true;
                            else if (enemy.AI.DistanceToTarget <= 900)
                                enemy.AI.Left = true;
                            else if (enemy.InAttack)
                                enemy.AI.Right = true;
                        }

                        if (_target.Position.Y < enemy.Position.Y - 100 && enemy.AI.DistanceX <= 500)
                        {
                            enemy.AI.Jump = true;
                        }

                        if (enemy.AI.DistanceToTarget <= targetMaxRange && enemy.CanAttack)
                        {
                            enemy.AI.Attack = true;
                        }
                    }
                    if (enemy?.AI.DistanceY > 1000 || enemy?.AI.DistanceToTarget > targetMaxRange)
                    {
                        if (enemy.Position.X < enemy.StartingPosition.X)
                            enemy.AI.Right = true;
                        if (enemy.Position.X > enemy.StartingPosition.X)
                            enemy.AI.Left = true;
                    }
                }
            }

            // --- TORCH TRAP ---

            foreach (var enemy in _sprites.Select(c => c as TrapTorcherRotateCW))
            {
                if (enemy != null && _target != null)
                {
                    enemy.AI.DistanceX = Math.Abs(_target.Position.X - enemy.Position.X);
                    enemy.AI.DistanceY = Math.Abs(_target.Position.Y - enemy.Position.Y);
                    enemy.AI.DistanceToTarget = (float)Math.Sqrt((_target.Position.X - enemy.Position.X) * (_target.Position.X - enemy.Position.X) + (_target.Position.Y - enemy.Position.Y) * (_target.Position.Y - enemy.Position.Y));
                }

                if (_target != null && enemy != null)
                {
                    if (enemy.AI.DistanceToTarget <= targetMaxRange)
                    {
                        enemy.AI.Attack = true;
                    } 
                }
            }

            foreach (var enemy in _sprites.Select(c => c as TrapTorcherRotateCCW))
            {
                if (enemy != null && _target != null)
                {
                    enemy.AI.DistanceX = Math.Abs(_target.Position.X - enemy.Position.X);
                    enemy.AI.DistanceY = Math.Abs(_target.Position.Y - enemy.Position.Y);
                    enemy.AI.DistanceToTarget = (float)Math.Sqrt((_target.Position.X - enemy.Position.X) * (_target.Position.X - enemy.Position.X) + (_target.Position.Y - enemy.Position.Y) * (_target.Position.Y - enemy.Position.Y));
                }

                if (_target != null && enemy != null)
                {
                    if (enemy.AI.DistanceToTarget <= targetMaxRange)
                    {
                        enemy.AI.Attack = true;
                    }
                }
            }
            
            // --- DART TRAP ---

            foreach (var enemy in _sprites.Select(c => c as TrapDarter))
            {
                if (enemy != null && _target != null)
                {
                    enemy.AI.DistanceX = Math.Abs(_target.Position.X - enemy.Position.X);
                    enemy.AI.DistanceY = Math.Abs(_target.Position.Y - enemy.Position.Y);
                    enemy.AI.DistanceToTarget = (float)Math.Sqrt((_target.Position.X - enemy.Position.X) * (_target.Position.X - enemy.Position.X) + (_target.Position.Y - enemy.Position.Y) * (_target.Position.Y - enemy.Position.Y));
                }

                if (_target != null && enemy != null)
                {
                    if (enemy.AI.DistanceToTarget <= targetMaxRange)
                    {
                        enemy.AI.Attack = true;
                    }
                }
            }

            // --- SPIRIT TRAP ---

            foreach (var enemy in _sprites.Select(c => c as TrapShaman))
            {
                if (enemy != null && _target != null)
                {
                    enemy.AI.DistanceX = Math.Abs(_target.Position.X - enemy.Position.X);
                    enemy.AI.DistanceY = Math.Abs(_target.Position.Y - enemy.Position.Y);
                    enemy.AI.DistanceToTarget = (float)Math.Sqrt((_target.Position.X - enemy.Position.X) * (_target.Position.X - enemy.Position.X) + (_target.Position.Y - enemy.Position.Y) * (_target.Position.Y - enemy.Position.Y));
                }

                if (_target != null && enemy != null)
                {
                    if (enemy.AI.DistanceToTarget <= targetTrapRange)
                    {
                        enemy.AI.Attack = true;
                    }
                }
            }
        }
    }
}
