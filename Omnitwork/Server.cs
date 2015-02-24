using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;

namespace Omnitwork
{
    public class Server
    {
        public Server()
        {
            
        }
        public void RegisterGame(IGameInterface game_interface)
        {
            games.Add(game_interface);            
            game_interface.SetServer(this);
            PlayerDropped += new EventHandler<GameConnectionEventArgs>(game_interface.OnPlayerDropped);
            NewConnection += new EventHandler<GameConnectionEventArgs>(game_interface.OnNewConnection);
        }
        //
        public event EventHandler<GameConnectionEventArgs> PlayerDropped = delegate { };
        public event EventHandler<GameConnectionEventArgs> NewConnection = delegate { }; 
        public ManualResetEvent allDone = new ManualResetEvent(false);        
        public List<IGameInterface> games = new List<IGameInterface>();        
        public Dictionary<Socket, GameConnection> reserved_connections = new Dictionary<Socket, GameConnection>();
        public int max_connections = 50;
        public void StartListening()
        {            
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8888);            
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);            
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();
                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);
                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        public void AcceptCallback(IAsyncResult ar)
        {            
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            if (reserved_connections.Count < max_connections)
            {
                reserved_connections.Add(handler, new GameConnection(handler, this));
            }            
            // Signal the main thread to continue.
            allDone.Set();
            NewConnection(this, new GameConnectionEventArgs(reserved_connections[handler]));
            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;            
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            if (!reserved_connections.ContainsKey(handler))
            {
                // close connection?
                return;
            }            
            int bytesRead = 0;
            try
            {
                bytesRead = handler.EndReceive(ar);
            }
            catch (SocketException exception)
            {                
                if (reserved_connections[handler] != null)
                {
                    GameConnectionEventArgs pa = new GameConnectionEventArgs(reserved_connections[handler]);
                    reserved_connections.Remove(handler);
                    PlayerDropped(this, pa);
                }
                return;
            }

            if (bytesRead > 0)
            {                  
                MemoryStream current_stream = reserved_connections[handler].CurrentStream;
                current_stream.Write(state.buffer, 0, bytesRead);
                Console.WriteLine("Read {0} bytes from socket.", bytesRead);
                object command = Slicer.Slice(current_stream, state);
                while (command != null)
                {
                    HandleCommand(handler, command);
                    command = Slicer.Slice(current_stream, state);
                }                
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
            }
        }

        public void HandleCommand(Socket source, object command)
        {
            if (command is ApplicationCommand)
            {
                Console.WriteLine("==============");                
                Console.WriteLine("Type : {0}", ((Command)command).type);
                Console.WriteLine("Message : {0}", ((Command)command).body);
                Console.WriteLine("==============");
                // execute command
                // ВАРНУНГ !!!
                // КАСТЫЛЬ
                foreach (IGameInterface game in games)
                {
                    game.Execute(reserved_connections[source], (ApplicationCommand)command);
                }
            }
            else if (command is Command)
            {
                // execute command
            }
            else
            {
                throw new NotImplementedException("Unknown message format");
            }
        }

        public void Send(Socket handler, byte[] data)
        {            
            byte[] union = new byte[4 + data.Length];
            BitConverter.GetBytes(data.Length).CopyTo(union, 0);
            data.CopyTo(union, 4);                        
            handler.BeginSend(union, 0, union.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private  void SendCallback(IAsyncResult ar)
        {
            try
            {                
                Socket handler = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
    public class GameConnectionEventArgs : EventArgs
    {
        public GameConnectionEventArgs()
        {

        }
        public GameConnectionEventArgs(GameConnection connection)
        {
            Connection = connection;
        }
        public GameConnection Connection { get; set; }
    }
}
