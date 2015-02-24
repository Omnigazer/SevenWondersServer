using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitwork
{
    [Serializable]
    public class ApplicationCommand : Command
    {
        public ApplicationCommand(string type, string body)
            : base(type, body)
        {

        }
    }
}
