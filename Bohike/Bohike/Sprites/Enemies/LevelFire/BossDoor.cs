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
    public class BossDoor : Chest
    {
        public bool GotTriggered;

        public BossDoor(Texture2D texture)
          : base(texture)
        {

        }

        public override void IsHit(float damage)
        {
            GotTriggered = true;
        }
    }
}
