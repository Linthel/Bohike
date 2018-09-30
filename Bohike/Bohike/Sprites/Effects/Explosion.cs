using Bohike.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Sprites
{
    public enum ExplosionTypes
    {
        Default,
        Water,
        Fire,
        Earth,
        Air,
        Shadow,
        ShadowWhirl,
        ShadowSmall,
        ShadowTiny,
        WaterSmall,
        FireSmall,
        EarthSmall,
        AirSmall,
        PowerUp,
        FireWhirl,
        GroundPoundExplosion,
    }

    public class Explosion : Sprite
    {
        public int Animation;

        public Explosion(Dictionary<string, Animation> animations) : base(animations)
        {

        }

        public override void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (Animation)
            {
                case 0:
                    AnimationManager.Play(Animations["Explode"]);
                    AnimationManager.Scale = Scale - (_timer / 0.5f)* Scale;
                    AnimationManager.Color.R = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.G = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.B = (byte)((1 - (_timer / 0.5f)) * 256);
                    break;
                case 1:
                    AnimationManager.Play(Animations["Water"]);
                    AnimationManager.Scale = Scale - (_timer / 0.5f) * Scale;
                    AnimationManager.Color.R = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.G = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.B = (byte)((1 - (_timer / 0.5f)) * 256);
                    break;
                case 2:
                    AnimationManager.Play(Animations["Fire"]);
                    AnimationManager.Scale = Scale - (_timer / 0.5f) * Scale;
                    AnimationManager.Color.R = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.G = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.B = (byte)((1 - (_timer / 0.5f)) * 256);
                    break;
                case 3:
                    AnimationManager.Play(Animations["Earth"]);
                    AnimationManager.Scale = Scale - (_timer / 0.5f) * Scale;
                    AnimationManager.Color.R = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.G = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.B = (byte)((1 - (_timer / 0.5f)) * 256);
                    break;
                case 4:
                    AnimationManager.Play(Animations["Air"]);
                    AnimationManager.Scale = Scale - (_timer / 0.5f) * Scale;
                    AnimationManager.Color.R = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.G = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.B = (byte)((1 - (_timer / 0.5f)) * 256);
                    break;
                case 5:
                    AnimationManager.Play(Animations["Shadow"]);
                    AnimationManager.Scale = Scale - (_timer / 0.5f) * Scale;
                    AnimationManager.Color.R = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.G = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.B = (byte)((1 - (_timer / 0.5f)) * 256);
                    break;
                case 6:
                    AnimationManager.Play(Animations["ShadowWhirl"]);
                    AnimationManager.Scale = Scale - (_timer / 0.5f) * Scale;
                    AnimationManager.Color.R = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.G = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.B = (byte)((1 - (_timer / 0.5f)) * 256);
                    break;
                case 7:
                    AnimationManager.Play(Animations["ShadowSmall"]);
                    AnimationManager.Scale = Scale - (_timer / 0.5f) * Scale;
                    AnimationManager.Color.R = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.G = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.B = (byte)((1 - (_timer / 0.5f)) * 256);
                    break;
                case 8:
                    AnimationManager.Play(Animations["ShadowTiny"]);
                    AnimationManager.Scale = Scale - (_timer / 0.5f) * Scale;
                    AnimationManager.Color.R = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.G = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.B = (byte)((1 - (_timer / 0.5f)) * 256);
                    break;
                case 9:
                    AnimationManager.Play(Animations["WaterSmall"]);
                    AnimationManager.Scale = Scale - (_timer / 0.5f) * Scale;
                    AnimationManager.Color.R = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.G = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.B = (byte)((1 - (_timer / 0.5f)) * 256);
                    break;
                case 10:
                    AnimationManager.Play(Animations["FireSmall"]);
                    AnimationManager.Scale = Scale - (_timer / 0.5f) * Scale;
                    AnimationManager.Color.R = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.G = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.B = (byte)((1 - (_timer / 0.5f)) * 256);
                    break;
                case 11:
                    AnimationManager.Play(Animations["EarthSmall"]);
                    AnimationManager.Scale = Scale - (_timer / 0.5f) * Scale;
                    AnimationManager.Color.R = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.G = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.B = (byte)((1 - (_timer / 0.5f)) * 256);
                    break;
                case 12:
                    AnimationManager.Play(Animations["AirSmall"]);
                    AnimationManager.Scale = Scale - (_timer / 0.5f) * Scale;
                    AnimationManager.Color.R = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.G = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.B = (byte)((1 - (_timer / 0.5f)) * 256);
                    break;
                case 13:
                    AnimationManager.Play(Animations["PowerUp"]);
                    AnimationManager.Scale = Scale - (_timer / 1f) * Scale;
                    AnimationManager.Color.R = (byte)((1 - (_timer / 1f)) * 256);
                    AnimationManager.Color.G = (byte)((1 - (_timer / 1f)) * 256);
                    AnimationManager.Color.B = (byte)((1 - (_timer / 1f)) * 256);
                    break;
                case 14:
                    AnimationManager.Play(Animations["FireWhirl"]);
                    AnimationManager.Scale = Scale - (_timer / 1f) * Scale;
                    AnimationManager.Color.R = (byte)((1 - (_timer / 1f)) * 256);
                    AnimationManager.Color.G = (byte)((1 - (_timer / 1f)) * 256);
                    AnimationManager.Color.B = (byte)((1 - (_timer / 1f)) * 256);
                    break;
                case 15:
                    AnimationManager.Play(Animations["GroundPoundExplosion"]);
                    AnimationManager.Scale = Scale - (_timer / 0.5f) * Scale;
                    AnimationManager.Color.R = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.G = (byte)((1 - (_timer / 0.5f)) * 256);
                    AnimationManager.Color.B = (byte)((1 - (_timer / 0.5f)) * 256);
                    break;
            }

            AnimationManager.Update(gameTime);

            if (_timer > AnimationManager.CurrentAnimation.FrameCount * AnimationManager.CurrentAnimation.FrameSpeed)
                IsRemoved = true;
        }
    }
}
