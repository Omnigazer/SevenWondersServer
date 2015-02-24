using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitwork
{
    public interface IGameInterface
    {
        void SetServer(Server server);
        void OnPlayerDropped(object sender, GameConnectionEventArgs e);
        void OnNewConnection(object sender, GameConnectionEventArgs e);
        //void GetData(GameConnection connection);
        void Execute(GameConnection sender, ApplicationCommand command);
        void AddConnection(GameConnection connection);
        void Start();
    }
}
