using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7WondersCore
{
    public interface GUI
    {
        void ShowBooster(List<Card> cards);        
        void ShowStringMessage(string s);
        void DisplayGold(string s);
        void DisplayBoard(List<Card> cards);
        void DisplayWonder(Wonder wonder);
        void DisplayNewTier();
        void DisplayGameState(int gold);
    }
}
