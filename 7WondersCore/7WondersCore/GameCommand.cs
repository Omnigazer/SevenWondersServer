using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnitwork;

namespace _7WondersCore
{
    [Serializable]
    public class GameCommand : ApplicationCommand
    {        
        public GameCommand(string type)
            : base(type, "")
        {

        }

        public GameCommand(string type, string body)
            : base(type, body)
        {

        }
        
    }
}
