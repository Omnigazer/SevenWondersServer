using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7WondersCore
{
    public class Player
    {        
        public string Name { get; set; }
        public Game Game { get; set; }
        public int Gold { get; set; }
        public List<Card> current_booster;
        public List<Card> board;
        public Card picked_card;                
        public string play_mode;

        public int GetMilitary()
        {
            return 4;
        }
    }
}
