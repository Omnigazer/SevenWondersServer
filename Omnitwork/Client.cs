using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace Omnitwork
{
    public class Client
    {
        // The port number for the remote device.
        private const int port = 8888;
        public Socket client;
        public IClientInterface client_interface;
        public MemoryStream buffer = new MemoryStream(65536);

        // ManualResetEvent instances signal completion.
        private ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        // The response from the remote device.
        private String response = String.Empty;

        public void StartClient()
        {
            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // The name of the 
                // remote device is "host.contoso.com".            
                IPAddress ipAddress = IPAddress.Loopback;
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.
                client = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.            
                //Send(client, "This is a test<EOF>");
                //sendDone.WaitOne();

                // Receive the response from the remote device.
                Receive(client);
                //receiveDone.WaitOne();

                // Write the response to the console.
                //Console.WriteLine("Response received : {0}", response);

                // Release the socket.
                //client.Shutdown(SocketShutdown.Both);
                //client.Close();

            }
            catch (Exception e)
            {
                client_interface.Output(e.Message);
                Console.WriteLine(e.ToString());
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.            
                connectDone.Set();
            }
            catch (Exception e)
            {
                client_interface.Output(e.Message);
                Console.WriteLine(e.ToString());
            }
        }

        private void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;
                client_interface.SetConnectionState(state);
                receiveDone.Set();
                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                client_interface.Output(e.Message);
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);
                buffer.Write(state.buffer, 0, bytesRead);
                if (bytesRead > 0)
                {
                    if (state.waiting_for_header)
                    {
                        if (buffer.Position >= 4)
                        {
                            int buffer_offset = (int)buffer.Position;
                            byte[] tmp = new byte[4];
                            buffer.Position = 0;
                            buffer.Read(tmp, 0, 4);
                            state.expected_bytes = BitConverter.ToInt32(tmp, 0);
                            state.waiting_for_header = false;
                            buffer.Position = buffer_offset;
                        }
                    }
                    if (!state.waiting_for_header)
                    {
                        if (buffer.Position >= state.expected_bytes + 4)
                        {
                            int buffer_offset = (int)buffer.Position;
                            byte[] body = new byte[state.expected_bytes];
                            buffer.Position = 4;
                            buffer.Read(body, 0, body.Length);
                            int remainder_length = buffer_offset - (int)buffer.Position;
                            MemoryStream mem_stream = new MemoryStream(body.Length);
                            mem_stream.Write(body, 0, body.Length);
                            BinaryFormatter bf = new BinaryFormatter();
                            mem_stream.Position = 0;
                            object command = bf.Deserialize(mem_stream);
                            if (command is ApplicationCommand)
                            {
                                Console.WriteLine("==============");
                                Console.WriteLine("Read {0} bytes from socket.", body.Length);
                                Console.WriteLine("Type : {0}", ((ApplicationCommand)command).type);
                                Console.WriteLine("Message : {0}", ((ApplicationCommand)command).body);
                                Console.WriteLine("==============");
                                // execute command
                                client_interface.Execute((ApplicationCommand)command);
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
                                buffer.Read(remainder, 0, remainder_length);
                                buffer.SetLength(65536);
                                buffer.Position = 0;
                                buffer.Write(remainder, 0, remainder.Length);
                                state.sb.Clear();
                            }
                            else
                            {
                                buffer.Position = 0;
                                buffer.SetLength(65536);
                            }
                            state.waiting_for_header = true;
                        }
                    }
                    //Console.WriteLine("Read {0} bytes from server", bytesRead);
                    // Get the rest of the data.
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    //receiveDone.Set();                                
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
            }
            catch (Exception e)
            {
                client_interface.Output(e.Message);
                Console.WriteLine(e.ToString());
            }
        }

        public void Send(Socket client, byte[] data)
        {
            // Convert the string data to byte data using ASCII encoding.
            //byte[] byteData = Encoding.ASCII.GetBytes(data);
            byte[] union = new byte[4 + data.Length];
            BitConverter.GetBytes(data.Length).CopyTo(union, 0);
            data.CopyTo(union, 4);

            // Begin sending the data to the remote device.
            client.BeginSend(union, 0, union.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                client_interface.Output(e.Message);
                Console.WriteLine(e.ToString());
            }
        }
    }
}