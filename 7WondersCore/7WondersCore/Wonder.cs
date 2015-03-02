using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7WondersCore
{
    public class Wonder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CurrentTier { get; set; }
        public List<Card> Tiers { get; set; }        
        public Wonder()
        {
            CurrentTier = 0;
            Tiers = new List<Card>();
        }        
    }
}
