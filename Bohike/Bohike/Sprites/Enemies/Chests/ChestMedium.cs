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
    public class ChestMedium : Chest
    {
        public ChestMedium(Texture2D texture)
          : base(texture)
        {

        }

        public override void IsHit(float damage)
        {
            if (IsHittable)
            {
                DropMoneyAndDie(5);
                SoundManager.PlaySoundEffect(Game1.Random.Next(0, 5));
            }
        }
    }
}
