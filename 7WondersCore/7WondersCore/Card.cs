using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Drawing;

namespace _7WondersCore
{
    public class Card
    {
        public int Id { get; set; }
        public Age Age { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string Name { get; set; }
        public CardColor Color { get; set; }
        // !!!
        // фейм - это уже не просто строка, у нее есть МОД
        public string Fame { get; set; }
        public string Cost { get; set; }
        public int Players { get; set; }        
        public Card()
        {
            Width = 90;
            Height = 150;
        }
        /*
        public void Draw(Graphics gr,float x, float y)
        {            
            gr.DrawRectangle(new Pen(Color.Black), x, y, Width, Height);
            gr.FillRectangle(new SolidBrush(Color), x, y, Width, Height);
            gr.DrawString(Name, new Font(FontFamily.Families.First(), 15.56f), new SolidBrush(Color.Black), x, y);
        }
         */
    }
}
