using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike
{
    public class GlobalVariables
    {
        private SpriteFont _font;

        public int Money = 0;
        public int _moneyCheckpoint;
        public float Health = 1f;
        public Vector2 CheckPointPosition  = new Vector2(4 * 92, (44 * 92) - 20);

        public GlobalVariables(SpriteFont font)
        {
            _font = font;
        }

        public void Update(GameTime gameTime)
        {
            if (Money <= _moneyCheckpoint)
            {
                _moneyCheckpoint = Money;
            }
        }

        public void NewMoneyCheckpoint()
        {
            _moneyCheckpoint = Money;
        }

        public void LoseMoneyToCheckpoint()
        {
            Money = _moneyCheckpoint;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!string.IsNullOrEmpty(Money.ToString()))
            {
                var x = (_font.MeasureString(Money.ToString()).X / 2);

                spriteBatch.DrawString(_font, Money.ToString(), new Vector2(250 - x, 233), Color.Yellow);
            }
        }

        public void Continue()
        {
            String inputSave = File.ReadAllText("Savefile.txt");

            int[] array = new int[4];

            int i = 0;
            foreach (var number in inputSave.Split(new char[] { ' ', '\t', '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                array[i] = int.Parse(number.Trim());
                i++;
            }

            CheckPointPosition = new Vector2(array[0], array[1]);
            Money = array[2];
            //Health = array[3];
        }

        public void NewGame()
        {
            StreamWriter writer = new StreamWriter("Savefile.txt");
            string input = String.Join(",", 368, 4048,0,1);
            writer.WriteLine(input);
            writer.Dispose();

            CheckPointPosition = new Vector2(368, 4048);
        }
    }
}
