using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Models
{
    public class Input
    {
        public Keys Left { get; set; }
        public Keys Right { get; set; }

        public Keys Jump { get; set; }
        public Keys Attack { get; set; }
        public Keys Sprint { get; set; }
        public Keys FastFall { get; set; }
        public Keys StrongJump { get; set; }
    }

    public class InputGamePad : Input
    {
        new public Buttons Left { get; set; }
        new public Buttons Right { get; set; }

        new public Buttons Jump { get; set; }
        new public Buttons Attack { get; set; }
        new public Buttons Sprint { get; set; }
        new public Buttons FastFall { get; set; }
        new public Buttons StrongJump { get; set; }
    }
}
