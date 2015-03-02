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
        public struct Calculator
        {
            public enum PerMode { Single, Brown, Gray, Gold, Red, Purple, Blue, Green, BrownGrayPurple, Wonder, Victory, Defeat };
            public enum Targets { Self, Left, Right, Neighbours, All };
            public int value;
            public PerMode mode;
            public Targets targets;
        }
        public int Id { get; set; }
        public Age Age { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string Name { get; set; }
        public CardColor Color { get; set; }
        public Tech Tech { get; set; }
        public Calculator Fame { get; set; }      
        // На самом деле это ETBMoney или просто Money
        public Calculator ETB { get; set; }
        public string Cost { get; set; }
        public int Players { get; set; }
        public int Military { get; set; }

        public Card()
        {
            Width = 90;
            Height = 150;
            this.Tech = Tech.None;
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
