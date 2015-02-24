using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitwork
{
    [Serializable]
    public class Command
    {
        public Command(string type, string body)
        {
            this.type = type;
            this.body = body;
        }
        public string type;
        public string body;
    }
}
