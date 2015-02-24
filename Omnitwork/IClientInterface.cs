using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitwork
{
    public interface IClientInterface
    {
        void Execute(ApplicationCommand command);
        void GetData(int bytes);
        void SetConnectionState(StateObject state_object);
        void Output(string s);
    }
}
