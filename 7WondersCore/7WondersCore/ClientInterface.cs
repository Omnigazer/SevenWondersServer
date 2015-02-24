using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Omnitwork;

namespace _7WondersCore
{
    public class ClientInterface : IClientInterface
    {
        //public Game game;
        public Client client;
        public StateObject connection_state;
        public GUI gui;
        public List<Card> allcards;
        public ClientInterface()
        {
            allcards = new List<Card>();
            foreach (Card card in Game.ParseFolder("D:/planchik/Age I"))
            {
                allcards.Add(card);
            }
            foreach (Card card in Game.ParseFolder("D:/planchik/Age II"))
            {
                allcards.Add(card);
            }
            foreach (Card card in Game.ParseFolder("D:/planchik/Age III"))
            {
                allcards.Add(card);
            }
        }
        
        public void Execute(ApplicationCommand command)
        {
            GameCommand game_command = (GameCommand)command;
            switch (game_command.type)
            {
                case "Message":
                    {
                        gui.ShowStringMessage(command.body);
                        break;
                    }
                case "Booster":
                    {
                        string[] ids = game_command.body.Split(',');                           
                        List<Card> tmp = new List<Card>();                        
                        foreach (string id in ids)
                        {
                            tmp.Add(allcards.Find(card => card.Id.ToString() == id));
                        }
                        //gui.ShowCards(game_command.body);
                        gui.ShowBooster(tmp);
                        break;
                    }
                case "PlayCard":
                    {
                        gui.PromptPlayMode();
                        break;
                    }
                case "CurrentGold":
                    {
                        gui.DisplayGold(command.body);
                        break;
                    }
                case "Board":
                    {
                        string[] ids = game_command.body.Split(',');
                        List<Card> tmp = new List<Card>();
                        foreach (string id in ids)
                        {
                            tmp.Add(allcards.Find(card => card.Id.ToString() == id));
                        }
                        gui.DisplayBoard(tmp);
                        break;
                    }
            }            
        }

        public void Output(string message)
        {
            gui.ShowStringMessage(message);
        }

        public void GetData(int bytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            byte[] tmp = new byte[bytes];
            for (int i = 0; i < bytes; i++)
            {
                tmp[i] = connection_state.buffer[i];
            }
            //MemoryStream mem_stream = new MemoryStream(connection_state.buffer, 0, bytes);            
            MemoryStream mem_stream = new MemoryStream(tmp, 0, bytes);
            GameCommand command = (GameCommand)bf.Deserialize(mem_stream);
            mem_stream.Dispose();
            Console.WriteLine(command.body);
        }



        public void Send(Command game_command)
        {
            MemoryStream mem_stream = new MemoryStream(new byte[65536]);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(mem_stream, game_command);            
            byte[] tmp = new byte[mem_stream.Position];
            mem_stream.Position = 0;
            mem_stream.Read(tmp, 0, tmp.Length);            
            client.Send(connection_state.workSocket, tmp);
        }


        public void SetConnectionState(StateObject state_object)
        {
            this.connection_state = state_object;
        }
    }
}
