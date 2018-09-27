using Bohike.Sprites;
using Bohike.States;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Core
{
    public class Camera
    {
        public Matrix Transform { get; private set; }

        public void Adjust(Vector2 target, float scale)
        {
            var position = Matrix.CreateTranslation(
                -target.X,
                -target.Y,
                0);

            var offset = Matrix.CreateTranslation(
                Game1.ScreenWidth / (2 * scale),
                Game1.ScreenHeight / (2 * scale),
                0);

            Transform = position * offset;
        }
    }
}
