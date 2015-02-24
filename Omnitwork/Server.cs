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

                //state.sb.Append(Encoding.ASCII.GetString(
                //    state.buffer, 0, bytesRead));                
                //content = state.sb.ToString();

                if (state.waiting_for_header)
                {
                    if (current_stream.Position >= 4)
                    {
                        int buffer_offset = (int)current_stream.Position;
                        byte[] tmp = new byte[4];
                        current_stream.Position = 0;
                        current_stream.Read(tmp, 0, 4);
                        state.expected_bytes = BitConverter.ToInt32(tmp, 0);
                        state.waiting_for_header = false;
                        current_stream.Position = buffer_offset;
                    }
                }
                if (!state.waiting_for_header)
                {
                    if (current_stream.Position >= state.expected_bytes + 4)
                    {
                        int buffer_offset = (int)current_stream.Position;
                        byte[] body = new byte[state.expected_bytes];
                        current_stream.Position = 4;
                        current_stream.Read(body, 0, body.Length);
                        int remainder_length = buffer_offset - (int)current_stream.Position;
                        MemoryStream mem_stream = new MemoryStream(body.Length);
                        mem_stream.Write(body, 0, body.Length);
                        BinaryFormatter bf = new BinaryFormatter();
                        mem_stream.Position = 0;
                        object command = bf.Deserialize(mem_stream);
                        if (command is ApplicationCommand)
                        {
                            Console.WriteLine("==============");
                            Console.WriteLine("Read {0} bytes from socket.", body.Length);
                            Console.WriteLine("Type : {0}", ((Command)command).type);
                            Console.WriteLine("Message : {0}", ((Command)command).body);
                            Console.WriteLine("==============");
                            // execute command
                            // ВАРНУНГ !!!
                            // КАСТЫЛЬ
                            foreach (IGameInterface game in games)
                            {                                
                                game.Execute(reserved_connections[handler],(ApplicationCommand)command);
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
                        if (remainder_length > 0)
                        {
                            byte[] remainder = new byte[remainder_length];
                            current_stream.Read(remainder, 0, remainder_length);
                            current_stream.SetLength(65536);
                            current_stream.Position = 0;
                            current_stream.Write(remainder, 0, remainder.Length);
                            
                            state.sb.Clear();
                        }
                        else 
                        {
                            current_stream.Position = 0;
                            current_stream.SetLength(65536);                            
                        }
                        state.waiting_for_header = true;
                    }
                }
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
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
