using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Omnitwork
{
    public class GameConnection
    {        
        public Socket socket;
        public Server server;
        // !!!        
        // раньше сервисы сами забирали оттуда данные и десериализовывали, буфер в этом классе устарел, вынести в сервер
        public MemoryStream CurrentStream { get; private set; }
        public GameConnection(Socket socket, Server server)
        {
            this.socket = socket;
            this.server = server;
            byte[] current_buffer = new byte[65536];
            CurrentStream = new MemoryStream(current_buffer);
        }
        
    }
}
