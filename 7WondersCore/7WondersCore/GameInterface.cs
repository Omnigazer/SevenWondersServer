using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnitwork;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace _7WondersCore
{
    public class GameInterface : IGameInterface
    {
        public Game game;
        public Server server;
        public GameInterface()
        {
            connections = new Dictionary<Player, GameConnection>();
            players = new Dictionary<GameConnection, Player>();
        }

        public void SetServer(Server server)
        {
            this.server = server;
        }
        public Dictionary<Player, GameConnection> connections;
        public Dictionary<GameConnection, Player> players;               

        public void Execute(GameConnection sender, ApplicationCommand command)
        {           
            game.Execute(players[sender], (GameCommand)command);
        }

        public void AddConnection(GameConnection connection)
        {
            Player player = new Player() { Name = "John Doe" };            
            connections.Add(player, connection);
            players.Add(connection, player);            
        }

        public void OnNewConnection(object sender, GameConnectionEventArgs e)
        {
            Console.WriteLine("Player connected");
            AddConnection(e.Connection);
            if (players.Count >= 3)
            {
                Start();
            }
        }

        public void Send(Player player, GameCommand command)
        {
            Console.WriteLine("==============");
            Console.WriteLine("Type : {0}", command.type);
            Console.WriteLine("Message : {0}", command.body);
            Console.WriteLine("==============");
            MemoryStream mem_stream = new MemoryStream(new byte[65536]);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(mem_stream, command);
            byte[] tmp = new byte[mem_stream.Position];
            mem_stream.Position = 0;
            mem_stream.Read(tmp, 0, tmp.Length);
            server.Send(connections[player].socket, tmp);              
        }
        
        public void OnPlayerDropped(object sender, GameConnectionEventArgs e)
        {
            lock (players)
            {
                if (players.ContainsKey(e.Connection))
                {
                    Player dropped_player = players[e.Connection];
                    players.Remove(e.Connection);
                    foreach (Player player in players.Values)
                    {
                        Send(player, new GameCommand("Message", "Player " + dropped_player.Name + " dropped"));
                    }
                }
            }
        }

        public void Start()
        {
            foreach (var player in connections.Keys)
            {
                game.AddPlayer(player);                
            }
            game.Begin();
        }
    }
}
