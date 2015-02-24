using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnitwork;
using _7WondersCore;

namespace SevenWondersProductionServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            Server server = new Server();
            server.RegisterGame(game.gameinterface);
            server.StartListening();            
        }
    }
}
