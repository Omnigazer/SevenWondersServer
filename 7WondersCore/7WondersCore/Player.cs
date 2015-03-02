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
        // !!!
        // нечего тут инициализировать, передавать снаружи надо
        public Wonder wonder = new Wonder();
        public Card picked_card;                
        public string play_mode;

        public int defeat_tokens = 0;
        public Dictionary<Age, int> VictoryTokens = new Dictionary<Age, int>() { { Age.I, 0 }, { Age.II, 0 }, { Age.III, 0 } };

        // возвращаем список "карт", сформированный из тиров чуда
        public IEnumerable<Card> GetWonderCards()
        {
            return wonder.Tiers.Where(card => wonder.Tiers.IndexOf(card) < wonder.CurrentTier);
        }

        public int GetMilitary()
        {            
            // !!!
            // считаем только красные карточки                        
            return board.Where(card => card.Color == CardColor.Red).Sum(card => card.Military) + GetWonderCards().Sum(card => card.Military);
        }
    }
}
