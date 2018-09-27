using Bohike.Controls;
using Bohike.Core;
using Bohike.Managers;
using Bohike.Models;
using Bohike.Sprites;
using Bohike.Sprites.Collectibles;
using Bohike.Sprites.Enemies;
using Bohike.Sprites.Hurtboxes;
using Bohike.Sprites.Hurtboxes.ofEnemies;
using Bohike.Sprites.Hurtboxes.ofPlayer;
using Bohike.States.Levels;
using Bohike.Tilemap;
using Bohike.UserInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.States.Levels
{
    public class HubWorld : GameState
    {
        private bool _easterEggMusicIsPlaying;
        private float _easterEggTimer;

        #region Load Content

        public HubWorld(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _camera = new Camera();

            // --- TEXTURES ---

            var texture = _content.Load<Texture2D>("Square");
            var playerTexture = _content.Load<Texture2D>("Video/Player/Player");
            var chestTexture = _content.Load<Texture2D>("Video/Enemies/Chests/ChestSize");

            _staticBackgroundTexture = _content.Load<Texture2D>("Video/Backgrounds/LevelFire/StaticBackgroundDay");
            _farBackgroundTexture = _content.Load<Texture2D>("Video/Backgrounds/FarBackground");
            _midBackgroundTexture = _content.Load<Texture2D>("Video/Backgrounds/LevelFire/MidBackground");
            _directBackgroundTexture = _content.Load<Texture2D>("Video/Backgrounds/DirectBackground");
            _frontBackgroundTexture = _content.Load<Texture2D>("Video/Backgrounds/LevelFire/FrontBackground");

            _frontBackgroundPosition = new Vector2(0, 0);
            _midBackgroundPosition = new Vector2(0, 0);
            _farBackgroundPosition = new Vector2(0, 0);

            // --- SOUND ---

            var backgroundMusic = _content.Load<Song>("Audio/Music/EarthLevelMusic");

            var levelSoundEffects = new List<SoundEffect>()
            {
                //_content.Load<SoundEffect>("BallHitsBat"),
                //_content.Load<SoundEffect>("BallHitsWall"),
                //_content.Load<SoundEffect>("PlayerScores"),
            };

            var tileSoundEffects = new List<SoundEffect>()
            {
                //_content.Load<SoundEffect>("Sound/Tile/Break1"),
                //_content.Load<SoundEffect>("Sound/Tile/Break2"),
            };

            var playerSoundEffects = new List<SoundEffect>()
            {
                _content.Load<SoundEffect>("Audio/Sound/Player/FireWhirl"),     // 0
                _content.Load<SoundEffect>("Audio/Sound/Player/FastFall"),
                _content.Load<SoundEffect>("Audio/Sound/Player/Hurt1"),
                _content.Load<SoundEffect>("Audio/Sound/Player/Hurt2"),
                _content.Load<SoundEffect>("Audio/Sound/Player/Attack1"),
                _content.Load<SoundEffect>("Audio/Sound/Player/Attack2"),       // 5
                _content.Load<SoundEffect>("Audio/Sound/Player/Attack3"),
                _content.Load<SoundEffect>("Audio/Sound/Player/StopCast"),
                _content.Load<SoundEffect>("Audio/Sound/Player/GroundPound"),
                _content.Load<SoundEffect>("Audio/Sound/Player/Sprint"),
                _content.Load<SoundEffect>("Audio/Sound/Player/Jump1"),         // 10
                _content.Load<SoundEffect>("Audio/Sound/Player/Jump2"),
                _content.Load<SoundEffect>("Audio/Sound/Player/WallJump"),
                _content.Load<SoundEffect>("Audio/Sound/Player/Cast1"),
                _content.Load<SoundEffect>("Audio/Sound/Player/Cast2"),
                _content.Load<SoundEffect>("Audio/Sound/Player/Death"),         // 15
                _content.Load<SoundEffect>("Audio/Sound/Player/StopSprint"),
                _content.Load<SoundEffect>("Audio/Sound/Player/Walk"),
                _content.Load<SoundEffect>("Audio/Sound/Player/JumpPad"),
                _content.Load<SoundEffect>("Audio/Sound/Player/Respawn"),
            };

            var tikiEnemySoundEffects = new List<SoundEffect>()
            {
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/FireWhirl"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/FastFall"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/Hurt1"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/Hurt2"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/Attack1"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/Attack2"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/Attack3"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/StopCast"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/GroundPound"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/Sprint"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/Jump1"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/Jump2"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/WallJump"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/Cast1"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/Cast2"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/Death"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/StopSprint"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/Walk"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/Dart"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/Spirit"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/TorchCast"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/TorchLaunch"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/TorchFireLaunch"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/DartFire"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/TikiEnemy/SpiritFire"),
            };

            var chestSoundEffects = new List<SoundEffect>()
            {
                _content.Load<SoundEffect>("Audio/Sound/Enemies/Chest/Break1"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/Chest/Break2"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/Chest/Break3"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/Chest/Break4"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/Chest/Break5"),
                _content.Load<SoundEffect>("Audio/Sound/Enemies/Chest/BreakGreater"),
            };

            var spellSoundEffects = new List<SoundEffect>()
            {
                _content.Load<SoundEffect>("Audio/Sound/Hurtboxes/Fire1"),
                _content.Load<SoundEffect>("Audio/Sound/Hurtboxes/Fire2"),
                _content.Load<SoundEffect>("Audio/Sound/Hurtboxes/Fire3"),
                _content.Load<SoundEffect>("Audio/Sound/Hurtboxes/Spirit"),
                _content.Load<SoundEffect>("Audio/Sound/Hurtboxes/SpiritHit"),
                _content.Load<SoundEffect>("Audio/Sound/Hurtboxes/CollectMoney12"),
                _content.Load<SoundEffect>("Audio/Sound/Hurtboxes/CollectMoney5"),
                _content.Load<SoundEffect>("Audio/Sound/Hurtboxes/CollectMoney10"),
                _content.Load<SoundEffect>("Audio/Sound/Hurtboxes/CollectMoney25"),
                _content.Load<SoundEffect>("Audio/Sound/Hurtboxes/CollectPowerUp"),
            };

            _soundManager = new SoundManager(backgroundMusic, levelSoundEffects);
            _soundManager.PlayMusic();

            // --- HURTBOXES ---

            var groundPound = _content.Load<Texture2D>("Video/Hurtboxes/GroundPound");
            var fireWhirl = _content.Load<Texture2D>("Video/Effects/DefaultExplosion");
            var shadowWhirl = _content.Load<Texture2D>("Video/Effects/DefaultExplosion");
            var dart = _content.Load<Texture2D>("Square");
            var torch = _content.Load<Texture2D>("Square");
            var spirit = _content.Load<Texture2D>("Square");

            var explosion = new Dictionary<string, Models.Animation>()
            {
                { "Explode", new Animation(_content.Load<Texture2D>("Video/Effects/DefaultExplosion"), 1) { FrameSpeed = 0.5f, } },
                { "Water", new Animation(_content.Load<Texture2D>("Video/Effects/WaterExplosion"), 1) { FrameSpeed = 0.5f, } },
                { "Fire", new Animation(_content.Load<Texture2D>("Video/Effects/FireExplosion"), 1) { FrameSpeed = 0.5f, } },
                { "Earth", new Animation(_content.Load<Texture2D>("Video/Effects/EarthExplosion"), 1) { FrameSpeed = 0.5f, } },
                { "Air", new Animation(_content.Load<Texture2D>("Video/Effects/AirExplosion"), 1) { FrameSpeed = 0.5f, } },
                { "Shadow", new Animation(_content.Load<Texture2D>("Video/Effects/ShadowExplosion"), 1) { FrameSpeed = 0.5f, } },
                { "ShadowWhirl", new Animation(_content.Load<Texture2D>("Video/Effects/ShadowWhirl"), 1) { FrameSpeed = 0.5f, } },
                { "ShadowSmall", new Animation(_content.Load<Texture2D>("Video/Effects/ShadowExplosionSmall"), 1) { FrameSpeed = 0.5f, } },
                { "ShadowTiny", new Animation(_content.Load<Texture2D>("Video/Effects/ShadowExplosionTiny"), 1) { FrameSpeed = 0.5f, } },
                { "WaterSmall", new Animation(_content.Load<Texture2D>("Video/Effects/WaterExplosionSmall"), 1) { FrameSpeed = 0.5f, } },
                { "FireSmall", new Animation(_content.Load<Texture2D>("Video/Effects/FireExplosionSmall"), 1) { FrameSpeed = 0.5f, } },
                { "EarthSmall", new Animation(_content.Load<Texture2D>("Video/Effects/EarthExplosionSmall"), 1) { FrameSpeed = 0.5f, } },
                { "AirSmall", new Animation(_content.Load<Texture2D>("Video/Effects/AirExplosionSmall"), 1) { FrameSpeed = 0.5f, } },
                { "PowerUp", new Animation(_content.Load<Texture2D>("Video/Effects/PowerUpExplosion"), 1) { FrameSpeed = 1f, } },
                { "FireWhirl", new Animation(_content.Load<Texture2D>("Video/Effects/FireWhirl"), 1) { FrameSpeed = 1f, } },
                { "GroundPoundExplosion", new Animation(_content.Load<Texture2D>("Video/Effects/GroundPoundExplosion"), 1) { FrameSpeed = 0.5f, } },
            };
            var explosionPrefab = new Explosion(explosion);

            var groundPoundPrefab = new GroundPound(groundPound)
            {
                Explosion = explosionPrefab,
                SoundManager = new SoundManager(backgroundMusic, spellSoundEffects),
            };
            var fireWhirlPrefab = new FireWhirl(fireWhirl)
            {
                Explosion = explosionPrefab,
                SoundManager = new SoundManager(backgroundMusic, spellSoundEffects),
            };
            var dartPrefab = new Dart(dart)
            {
                Explosion = explosionPrefab,
                SoundManager = new SoundManager(backgroundMusic, spellSoundEffects),
            };
            var torchPrefab = new Torch(torch)
            {
                Explosion = explosionPrefab,
                SoundManager = new SoundManager(backgroundMusic, spellSoundEffects),
            };
            var spiritPrefab = new Spirit(spirit)
            {
                Explosion = explosionPrefab,
                SoundManager = new SoundManager(backgroundMusic, spellSoundEffects),
            };
            var shadowWhirlPrefab = new ShadowWhirl(shadowWhirl)
            {
                Explosion = explosionPrefab,
                SoundManager = new SoundManager(backgroundMusic, spellSoundEffects),
            };

            // --- COLLECTIBLES ---

            var money = _content.Load<Texture2D>("Square");
            var moneyPrefab = new Money(money)
            {
                Explosion = explosionPrefab,
                SoundManager = new SoundManager(backgroundMusic, spellSoundEffects),
            };
            var powerup = _content.Load<Texture2D>("Video/Hurtboxes/PowerUp");
            var powerupPrefab = new PowerUp(powerup)
            {
                Explosion = explosionPrefab,
                SoundManager = new SoundManager(backgroundMusic, spellSoundEffects),
            };

            // --- ANIMATIONS ---

            var playerAnimations = new Dictionary<string, Animation>()
            {
                {"MoveLeft", new Animation(_content.Load<Texture2D>("Video/Player/WalkingLeft"), 3)},
                {"MoveRight", new Animation(_content.Load<Texture2D>("Video/Player/WalkingRight"), 3)},
                {"Attack", new Animation(_content.Load<Texture2D>("Video/Player/Attack"), 2) { FrameSpeed = 0.05f, } } ,
            };
            var tikiWarriorAnimations = new Dictionary<string, Animation>()
            {
                {"MoveLeft", new Animation(_content.Load<Texture2D>("Video/Enemies/TikiEnemy/Enemy2WalkingLeft"), 3)},
                {"MoveRight", new Animation(_content.Load<Texture2D>("Video/Enemies/TikiEnemy/Enemy2WalkingRight"), 3)},
                {"Attack", new Animation(_content.Load<Texture2D>("Video/Enemies/TikiEnemy/Enemy2Attack"), 2) { FrameSpeed = 0.05f, } } ,
            };
            var tikiDarterAnimations = new Dictionary<string, Animation>()
            {
                {"MoveLeft", new Animation(_content.Load<Texture2D>("Video/Enemies/TikiEnemy/Enemy2WalkingLeft"), 3)},
                {"MoveRight", new Animation(_content.Load<Texture2D>("Video/Enemies/TikiEnemy/Enemy2WalkingRight"), 3)},
                {"Attack", new Animation(_content.Load<Texture2D>("Video/Enemies/TikiEnemy/Enemy2Attack"), 2) { FrameSpeed = 0.05f, } } ,
            };
            var tikiTorcherAnimations = new Dictionary<string, Animation>()
            {
                {"MoveLeft", new Animation(_content.Load<Texture2D>("Video/Enemies/TikiEnemy/Enemy2WalkingLeft"), 3)},
                {"MoveRight", new Animation(_content.Load<Texture2D>("Video/Enemies/TikiEnemy/Enemy2WalkingRight"), 3)},
                {"Attack", new Animation(_content.Load<Texture2D>("Video/Enemies/TikiEnemy/Enemy2Attack"), 2) { FrameSpeed = 0.05f, } } ,
            };
            var tikiShamanAnimations = new Dictionary<string, Animation>()
            {
                {"MoveLeft", new Animation(_content.Load<Texture2D>("Video/Enemies/TikiEnemy/EnemyWalkingLeft"), 3)},
                {"MoveRight", new Animation(_content.Load<Texture2D>("Video/Enemies/TikiEnemy/EnemyWalkingRight"), 3)},
                {"Attack", new Animation(_content.Load<Texture2D>("Video/Enemies/TikiEnemy/EnemyAttack"), 2) { FrameSpeed = 0.05f, } } ,
            };
            var enemyAnimations = new Dictionary<string, Animation>()
            {
                {"MoveLeft", new Animation(_content.Load<Texture2D>("Video/Enemies/TikiEnemy/EnemyWalkingLeft"), 3)},
                {"MoveRight", new Animation(_content.Load<Texture2D>("Video/Enemies/TikiEnemy/EnemyWalkingRight"), 3)},
                {"Attack", new Animation(_content.Load<Texture2D>("Video/Enemies/TikiEnemy/EnemyAttack"), 2) { FrameSpeed = 0.05f, } } ,
            };
            var chestSmallAnimations = new Dictionary<string, Animation>()
            {
                {"MoveLeft", new Animation(_content.Load<Texture2D>("Video/Enemies/Chests/ChestSmall"), 1)},
            };
            var chestMediumAnimations = new Dictionary<string, Animation>()
            {
                {"MoveLeft", new Animation(_content.Load<Texture2D>("Video/Enemies/Chests/ChestMedium"), 1)},
            };
            var chestLargeAnimations = new Dictionary<string, Animation>()
            {
                {"MoveLeft", new Animation(_content.Load<Texture2D>("Video/Enemies/Chests/ChestLarge"), 1)},
            };
            var chestPowerUpAnimations = new Dictionary<string, Animation>()
            {
                {"MoveLeft", new Animation(_content.Load<Texture2D>("Video/Enemies/Chests/ChestPowerUp"), 1)},
            };
            var shopPowerUpAnimations = new Dictionary<string, Animation>()
            {
                {"MoveLeft", new Animation(_content.Load<Texture2D>("Video/Enemies/Chests/ShopPowerUp"), 1)},
            };
            var chestCheckPointAnimations = new Dictionary<string, Animation>()
            {
                {"MoveLeft", new Animation(_content.Load<Texture2D>("Video/Enemies/Chests/ChestCheckPoint"), 1)},
            };
            var trapAnimations = new Dictionary<string, Animation>()
            {
                {"MoveLeft", new Animation(_content.Load<Texture2D>("Video/Enemies/Trap"), 1)},
            };

            // --- MAP ---

            Tile.Content = _content;

            _map = new Map()
            {
                Sound = new SoundManager(backgroundMusic, playerSoundEffects),
            };

            String inputTiles = File.ReadAllText("Levels/HubWorld/HubWorldTilemap.txt");

            _gridWidth = 25;
            _gridHeight = 14;
            _grid = new int[_gridHeight, _gridWidth];

            int i = 0, j = 0;
            foreach (var row in inputTiles.Split('\n'))
            {
                j = 0;
                foreach (var col in row.Trim().Split(','))
                {
                    _grid[i, j] = int.Parse(col.Trim());
                    j++;
                }
                i++;
            }

            _map.Generate(_grid, _tileSize, _level);

            // --- PLAYER ---

            _sprites = new List<Sprite>()
            {
                new Player(playerTexture)
                {
                    Game = _game,
                    Level = Levels.HubWorld,
                    Position = new Vector2(12*92, 2*92),
                    SoundManager = new SoundManager(backgroundMusic, playerSoundEffects),
                    CollisionType = CollisionTypes.Full,
                    GroundPound = groundPoundPrefab,
                    FireWhirl = fireWhirlPrefab,
                    Explosion = explosionPrefab,
                    Layer = 0.1f,
                    CameraTarget = true,
                    Input = new Input()
                    {
                        Left = Keys.A,
                        Right = Keys.D,
                        Jump = Keys.Space,
                        Attack = Keys.RightControl,
                        Sprint = Keys.LeftShift,
                        FastFall = Keys.S,
                        StrongJump = Keys.W,
                    },
                    Animations = new Dictionary<string, Animation>(playerAnimations),
                    AnimationManager = new AnimationManager(playerAnimations.First().Value)
                    {
                        Texture = texture,
                    },
                },

                new ShopPowerUp(chestTexture)
                {
                    Position = new Vector2(12*92, 10*92),
                    Game = _game,
                    SoundManager = new SoundManager(backgroundMusic, chestSoundEffects),
                    CollisionType = CollisionTypes.Full,
                    Money = moneyPrefab,
                    PowerUp = powerupPrefab,
                    Explosion = explosionPrefab,
                    Layer = 0.3f,
                    AI = new AI(),
                    Animations = new Dictionary<string, Animation>(shopPowerUpAnimations),
                    AnimationManager = new AnimationManager(shopPowerUpAnimations.First().Value)
                    {
                        Texture = texture,
                    },
                },
            };

            foreach (var tile in _map.CollisionTiles)
            {
                _sprites.Add(tile);
            }

            // --- ENEMIES ---

            String inputEnemies = File.ReadAllText("Levels/HubWorld/HubWorldEnemymap.txt");

            _gridWidth = 25;
            _gridHeight = 14;
            _grid = new int[_gridHeight, _gridWidth];

            i = 0;
            j = 0;
            foreach (var row in inputEnemies.Split('\n'))
            {
                j = 0;
                foreach (var col in row.Trim().Split(','))
                {
                    _grid[i, j] = int.Parse(col.Trim());

                    switch (_grid[i, j])
                    {
                        case 10:
                            _sprites.Add(new TikiWarrior(playerTexture)
                            {
                                Position = new Vector2(j * 92, i * 92),
                                SoundManager = new SoundManager(backgroundMusic, tikiEnemySoundEffects),
                                CollisionType = CollisionTypes.Full,
                                ShadowWhirl = shadowWhirlPrefab,
                                Money = moneyPrefab,
                                Explosion = explosionPrefab,
                                Layer = 0.1f,
                                AI = new AI(),
                                Animations = new Dictionary<string, Animation>(tikiWarriorAnimations),
                                AnimationManager = new AnimationManager(tikiWarriorAnimations.First().Value)
                                {
                                    Texture = texture,
                                },
                            });
                            break;
                        case 11:
                            _sprites.Add(new TikiDarter(playerTexture)
                            {
                                Position = new Vector2(j * 92, i * 92),
                                SoundManager = new SoundManager(backgroundMusic, tikiEnemySoundEffects),
                                CollisionType = CollisionTypes.Full,
                                Dart = dartPrefab,
                                Money = moneyPrefab,
                                Explosion = explosionPrefab,
                                Layer = 0.1f,
                                AI = new AI(),
                                Animations = new Dictionary<string, Animation>(tikiDarterAnimations),
                                AnimationManager = new AnimationManager(tikiDarterAnimations.First().Value)
                                {
                                    Texture = texture,
                                },
                            });
                            break;
                        case 12:
                            _sprites.Add(new TikiTorcher(playerTexture)
                            {
                                Position = new Vector2(j * 92, i * 92),
                                SoundManager = new SoundManager(backgroundMusic, tikiEnemySoundEffects),
                                CollisionType = CollisionTypes.Full,
                                Torch = torchPrefab,
                                Money = moneyPrefab,
                                Explosion = explosionPrefab,
                                Layer = 0.1f,
                                AI = new AI(),
                                Animations = new Dictionary<string, Animation>(tikiTorcherAnimations),
                                AnimationManager = new AnimationManager(tikiTorcherAnimations.First().Value)
                                {
                                    Texture = texture,
                                },
                            });
                            break;
                        case 13:
                            _sprites.Add(new TikiShaman(playerTexture)
                            {
                                Position = new Vector2(j * 92, i * 92),
                                SoundManager = new SoundManager(backgroundMusic, tikiEnemySoundEffects),
                                CollisionType = CollisionTypes.Full,
                                Spirit = spiritPrefab,
                                Money = moneyPrefab,
                                Explosion = explosionPrefab,
                                Layer = 0.1f,
                                AI = new AI(),
                                Animations = new Dictionary<string, Animation>(tikiShamanAnimations),
                                AnimationManager = new AnimationManager(tikiShamanAnimations.First().Value)
                                {
                                    Texture = texture,
                                },
                            });
                            break;
                        case 14:
                            _sprites.Add(new TrapDarter(playerTexture)
                            {
                                Position = new Vector2(j * 92, i * 92),
                                SoundManager = new SoundManager(backgroundMusic, tikiEnemySoundEffects),
                                CollisionType = CollisionTypes.Full,
                                Dart = dartPrefab,
                                Money = moneyPrefab,
                                Explosion = explosionPrefab,
                                Layer = 0.1f,
                                AI = new AI(),
                                Animations = new Dictionary<string, Animation>(trapAnimations),
                                AnimationManager = new AnimationManager(trapAnimations.First().Value)
                                {
                                    Texture = texture,
                                },
                            });
                            break;
                        case 15:
                            _sprites.Add(new TrapTorcherRotateCW(playerTexture)
                            {
                                Position = new Vector2(j * 92, i * 92),
                                SoundManager = new SoundManager(backgroundMusic, tikiEnemySoundEffects),
                                CollisionType = CollisionTypes.Full,
                                Torch = torchPrefab,
                                Money = moneyPrefab,
                                Explosion = explosionPrefab,
                                Layer = 0.1f,
                                AI = new AI(),
                                Animations = new Dictionary<string, Animation>(trapAnimations),
                                AnimationManager = new AnimationManager(trapAnimations.First().Value)
                                {
                                    Texture = texture,
                                },
                            });
                            break;
                        case 16:
                            _sprites.Add(new TrapTorcherRotateCCW(playerTexture)
                            {
                                Position = new Vector2(j * 92, i * 92),
                                SoundManager = new SoundManager(backgroundMusic, tikiEnemySoundEffects),
                                CollisionType = CollisionTypes.Full,
                                Torch = torchPrefab,
                                Money = moneyPrefab,
                                Explosion = explosionPrefab,
                                Layer = 0.1f,
                                AI = new AI(),
                                Animations = new Dictionary<string, Animation>(trapAnimations),
                                AnimationManager = new AnimationManager(trapAnimations.First().Value)
                                {
                                    Texture = texture,
                                },
                            });
                            break;
                        case 17:
                            _sprites.Add(new TrapShaman(playerTexture)
                            {
                                Position = new Vector2(j * 92, i * 92),
                                SoundManager = new SoundManager(backgroundMusic, tikiEnemySoundEffects),
                                CollisionType = CollisionTypes.Full,
                                Spirit = spiritPrefab,
                                Money = moneyPrefab,
                                Explosion = explosionPrefab,
                                Layer = 0.1f,
                                AI = new AI(),
                                Animations = new Dictionary<string, Animation>(trapAnimations),
                                AnimationManager = new AnimationManager(trapAnimations.First().Value)
                                {
                                    Texture = texture,
                                },
                            });
                            break;
                        case 18:
                            _sprites.Add(new ChestSmall(chestTexture)
                            {
                                Position = new Vector2(j * 92, i * 92),
                                SoundManager = new SoundManager(backgroundMusic, chestSoundEffects),
                                CollisionType = CollisionTypes.Full,
                                Money = moneyPrefab,
                                PowerUp = powerupPrefab,
                                Explosion = explosionPrefab,
                                Layer = 0.3f,
                                AI = new AI(),
                                Animations = new Dictionary<string, Animation>(chestSmallAnimations),
                                AnimationManager = new AnimationManager(chestSmallAnimations.First().Value)
                                {
                                    Texture = texture,
                                },
                            });
                            break;
                        case 19:
                            _sprites.Add(new ChestMedium(chestTexture)
                            {
                                Position = new Vector2(j * 92, i * 92),
                                SoundManager = new SoundManager(backgroundMusic, chestSoundEffects),
                                CollisionType = CollisionTypes.Full,
                                Money = moneyPrefab,
                                PowerUp = powerupPrefab,
                                Explosion = explosionPrefab,
                                Layer = 0.3f,
                                AI = new AI(),
                                Animations = new Dictionary<string, Animation>(chestMediumAnimations),
                                AnimationManager = new AnimationManager(chestMediumAnimations.First().Value)
                                {
                                    Texture = texture,
                                },
                            });
                            break;
                        case 20:
                            _sprites.Add(new ChestLarge(chestTexture)
                            {
                                Position = new Vector2(j * 92, i * 92),
                                SoundManager = new SoundManager(backgroundMusic, chestSoundEffects),
                                CollisionType = CollisionTypes.Full,
                                Money = moneyPrefab,
                                PowerUp = powerupPrefab,
                                Explosion = explosionPrefab,
                                Layer = 0.3f,
                                AI = new AI(),
                                Animations = new Dictionary<string, Animation>(chestLargeAnimations),
                                AnimationManager = new AnimationManager(chestLargeAnimations.First().Value)
                                {
                                    Texture = texture,
                                },
                            });
                            break;
                        case 21:
                            _sprites.Add(new ChestPowerUp(chestTexture)
                            {
                                Position = new Vector2(j * 92, i * 92),
                                SoundManager = new SoundManager(backgroundMusic, chestSoundEffects),
                                CollisionType = CollisionTypes.Full,
                                Money = moneyPrefab,
                                PowerUp = powerupPrefab,
                                Explosion = explosionPrefab,
                                Layer = 0.3f,
                                AI = new AI(),
                                Animations = new Dictionary<string, Animation>(chestPowerUpAnimations),
                                AnimationManager = new AnimationManager(chestPowerUpAnimations.First().Value)
                                {
                                    Texture = texture,
                                },
                            });
                            break;
                        case 22:
                            _sprites.Add(new ChestCheckPoint(chestTexture)
                            {
                                Position = new Vector2(j * 92, i * 92),
                                SoundManager = new SoundManager(backgroundMusic, chestSoundEffects),
                                CollisionType = CollisionTypes.Full,
                                Money = moneyPrefab,
                                PowerUp = powerupPrefab,
                                Explosion = explosionPrefab,
                                Layer = 0.3f,
                                AI = new AI(),
                                Animations = new Dictionary<string, Animation>(chestCheckPointAnimations),
                                AnimationManager = new AnimationManager(chestCheckPointAnimations.First().Value)
                                {
                                    Texture = texture,
                                },
                            });
                            break;
                    }
                    j++;
                }
                i++;
            }

            // --- USER INTERFACE ---

            var playerPortraitTexture = _content.Load<Texture2D>("Video/UserInterface/PlayerInfo/PlayerHealth3");

            foreach (var player in _sprites.Select(c => c as Player))
            {
                if (player is Player)
                    _player = player;
            }

            _components = new List<Component>()
            {
                new PlayerInfo(playerPortraitTexture)
                {
                    Position = new Vector2(50, 50),
                    Text = "Quit",
                    Player = _player,
                    PlayerHealth2 = _content.Load<Texture2D>("Video/UserInterface/PlayerInfo/PlayerHealth2"),
                    PlayerHealth1 = _content.Load<Texture2D>("Video/UserInterface/PlayerInfo/PlayerHealth1"),
                    PlayerHealth0 = _content.Load<Texture2D>("Video/UserInterface/PlayerInfo/PlayerHealth0"),
                },
            };
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // --- STATIC BACKGROUND ---

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap);
            spriteBatch.Draw(_staticBackgroundTexture, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), new Rectangle(0, 0, _staticBackgroundTexture.Width, _staticBackgroundTexture.Height), Color.White);
            spriteBatch.End();

            // --- FAR BACKGROUND ---

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap);
            spriteBatch.Draw(_farBackgroundTexture, new Rectangle(0, 0, (int)(Game1.ScreenWidth), (int)(Game1.ScreenHeight)), new Rectangle((int)((_farBackgroundPosition.X + _cameraTarget.X) / 4), (int)(_farBackgroundPosition.Y), _farBackgroundTexture.Width, _farBackgroundTexture.Height), Color.White);
            spriteBatch.End();

            // --- MID BACKGROUND ---

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap);
            spriteBatch.Draw(
                    _midBackgroundTexture,
                    new Rectangle(0, 0, (int)(Game1.ScreenWidth), (int)(Game1.ScreenHeight)),
                    new Rectangle(
                        (int)(_cameraTarget.X * 0.5f - _midBackgroundPosition.X),
                        MathHelper.Clamp((int)(_cameraTarget.Y * 0.5f - _midBackgroundPosition.Y) + _midBackgroundTexture.Height / 2 - Game1.ScreenHeight - 300, 0, _midBackgroundTexture.Height - Game1.ScreenHeight),
                        Game1.ScreenWidth,
                        Game1.ScreenHeight),
                    Color.White);
            spriteBatch.End();

            // --- SPRITES ---

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, transformMatrix: _camera.Transform * Matrix.CreateScale(_cameraZoom));
            foreach (var sprite in _sprites)
            {
                if (sprite is Explosion && (sprite as Explosion).Animation == (int)ExplosionTypes.PowerUp)
                {
                    if ((float)Math.Sqrt((sprite.Position.X - _cameraTarget.X) * (sprite.Position.X - _cameraTarget.X) + (sprite.Position.Y - _cameraTarget.Y) * (sprite.Position.Y - _cameraTarget.Y)) < 1500)
                        sprite.Draw(gameTime, spriteBatch);
                }
            }
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, transformMatrix: _camera.Transform * Matrix.CreateScale(_cameraZoom));
            foreach (var sprite in _sprites)
            {
                if (!(sprite is Explosion))
                    if ((float)Math.Sqrt((sprite.Position.X - _cameraTarget.X) * (sprite.Position.X - _cameraTarget.X) + (sprite.Position.Y - _cameraTarget.Y) * (sprite.Position.Y - _cameraTarget.Y)) < 1500)
                        sprite.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, transformMatrix: _camera.Transform * Matrix.CreateScale(_cameraZoom));
            foreach (var sprite in _sprites)
            {
                if (sprite is Explosion && !((sprite as Explosion).Animation >= (int)ExplosionTypes.Shadow && (sprite as Explosion).Animation <= (int)ExplosionTypes.ShadowTiny) && (sprite as Explosion).Animation != (int)ExplosionTypes.PowerUp)
                    if ((float)Math.Sqrt((sprite.Position.X - _cameraTarget.X) * (sprite.Position.X - _cameraTarget.X) + (sprite.Position.Y - _cameraTarget.Y) * (sprite.Position.Y - _cameraTarget.Y)) < 1500)
                        sprite.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, Subtractive, transformMatrix: _camera.Transform * Matrix.CreateScale(_cameraZoom));
            foreach (var sprite in _sprites)
            {
                if (sprite is Explosion && (sprite as Explosion).Animation >= (int)ExplosionTypes.Shadow && (sprite as Explosion).Animation <= (int)ExplosionTypes.ShadowTiny)
                    if ((float)Math.Sqrt((sprite.Position.X - _cameraTarget.X) * (sprite.Position.X - _cameraTarget.X) + (sprite.Position.Y - _cameraTarget.Y) * (sprite.Position.Y - _cameraTarget.Y)) < 1500)
                        sprite.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();

            // --- FRONT BACKGROUND ---

            //if (_easterEggTimer >= 23f)
            //    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearWrap);
            //else
            //    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap);
            //if (_easterEggTimer >= 23f)
            //    spriteBatch.Draw(_frontBackgroundTexture, new Rectangle(0, 0, (int)(Game1.ScreenWidth * 4f), (int)(Game1.ScreenHeight * 4f)), new Rectangle((int)(_cameraTarget.X * 0.03f - _frontBackgroundPosition.X), (int)(_cameraTarget.Y * 0.03f - _frontBackgroundPosition.Y), _frontBackgroundTexture.Width, _frontBackgroundTexture.Height), Color.White);
            //else
            //   spriteBatch.Draw(_frontBackgroundTexture, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), new Rectangle(0, 0, _staticBackgroundTexture.Width, _staticBackgroundTexture.Height), Color.White);
            //spriteBatch.End();

            // --- USER INTERFACE ---

            spriteBatch.Begin();
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);
            _game.GlobalVariables.Draw(spriteBatch);
            spriteBatch.End();
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            StartEasterEgg(gameTime);
            ControlCameraHub(_grid);

            foreach (var component in _components)
                component.Update(gameTime);

            ControlEnemies();
            MoveBackgrounds();

            int spriteCount = _sprites.Count;
            for (int i = 0; i < spriteCount; i++)
            {
                var sprite = _sprites[i];
                foreach (var child in sprite.Children)
                    _sprites.Add(child);

                sprite.Children = new List<Sprite>();
            }

            foreach (var sprite in _sprites)
            {
                if ((float)Math.Sqrt((sprite.Position.X - _cameraTarget.X) * (sprite.Position.X - _cameraTarget.X) + (sprite.Position.Y - _cameraTarget.Y) * (sprite.Position.Y - _cameraTarget.Y)) < 1400)
                {
                    if (!((sprite is Tile && sprite.CollisionType == CollisionTypes.TopFalling) || sprite is Spirit || sprite is Player))
                        sprite.Update(gameTime);
                }
                else if (!((sprite is Tile && sprite.CollisionType == CollisionTypes.TopFalling) || sprite is Spirit || sprite is Player))
                    sprite.Velocity = new Vector2(0, 0);

                if ((sprite is Tile && sprite.CollisionType == CollisionTypes.TopFalling) || sprite is Spirit || sprite is Player)
                    sprite.Update(gameTime);
            }
        }

        private void MoveBackgrounds()
        {
            if (_easterEggTimer >= 23f)
            {
                _farBackgroundPosition.X += 1;
                _farBackgroundPosition.Y += 1;
                _midBackgroundPosition.X += 1;
                _midBackgroundPosition.Y += 1;
                _frontBackgroundPosition.X += 4;
                _frontBackgroundPosition.Y += 1;
            }
            _frontBackgroundPosition.X -= 2;
            _farBackgroundPosition.X -= 3;
        }

        private void StartEasterEgg(GameTime gameTime)
        {
            foreach (var player in _sprites.Select(c => c as Player))
            {
                if (player is Player)
                {
                    if (player.Position.Y > 5500)
                    {
                        _easterEgg = true;
                        _easterEggTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (!_easterEggMusicIsPlaying)
                        {
                            SoundManager _soundManager = new SoundManager(_content.Load<Song>("Audio/Music/EasterEgg"), new List<SoundEffect>());
                            _soundManager.PlayMusic();
                            _easterEggMusicIsPlaying = true;
                        }

                        if (_easterEggTimer >= 23f)
                        {
                            _staticBackgroundTexture = _content.Load<Texture2D>("Video/Backgrounds/StaticEasterEggBackground");
                            _farBackgroundTexture = _content.Load<Texture2D>("Video/Backgrounds/MobileEasterEggBackground");
                            _directBackgroundTexture = _content.Load<Texture2D>("Video/Backgrounds/MobileEasterEggBackground");
                            _midBackgroundTexture = _content.Load<Texture2D>("Video/Backgrounds/MobileEasterEggBackground");
                            _frontBackgroundTexture = _content.Load<Texture2D>("Video/Backgrounds/MobileEasterEggBackground");
                        }
                    }

                    if (player.Position.X > 22 * 92 || player.Position.X < 2 * 92)
                        _game.ChangeState(new TestLevel(_game, _graphicsDevice, _content));
                    //if (/*player.Position.X > 22 * 92 ||*/ player.Position.X < 2 * 92)
                    //    _game.ChangeState(new BossRoom(_game, _graphicsDevice, _content));

                    if (player.Health <= 0)
                        _game.ChangeState(new TestLevel(_game, _graphicsDevice, _content));
                }
            }
        }

        #endregion
    }
}
