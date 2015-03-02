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
        public List<Wonder> allwonders;
        // служебное поле, запоминающее, как именно игрок хочет разыграть пикнутую карту
        public string current_playmode;
        public ClientInterface(GUI gui)
        {
            allcards = new List<Card>();
            allwonders = new List<Wonder>();
            foreach (Card card in Game.ParseForCards("D:/planchik/Age I"))
            {
                allcards.Add(card);
            }
            foreach (Card card in Game.ParseForCards("D:/planchik/Age II"))
            {
                allcards.Add(card);
            }
            foreach (Card card in Game.ParseForCards("D:/planchik/Age III"))
            {
                allcards.Add(card);
            }
            foreach (Card card in Game.ParseForCards("D:/planchik/Age III/Guilds"))
            {
                allcards.Add(card);
            }
            foreach (Wonder wonder in Game.ParseForWonders("D:/planchik/Wonders", false))
            {
                allwonders.Add(wonder);
            }

            // !!!
            client = new Client();
            client.client_interface = this;
            this.gui = gui;
            client.StartClient();
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
                            Card card = allcards.Find(crd => crd.Id.ToString() == id);
                            if (card == null)
                            {
                                throw new Exception("CARD NOT FOUND: " + id.ToString());
                            }
                            tmp.Add(card);
                        }                        
                        gui.ShowBooster(tmp);
                        break;
                    }
                case "PlayCard":
                    {
                        if (current_playmode != null)
                        {
                            // !!!
                            // тоже костыль, надо сразу научить игру просить то, что надо
                            GameCommand response = new GameCommand("PlayMode", current_playmode);
                            Send(response);
                        }
                        else
                        {
                            throw new Exception("Empty card play mode");
                        }
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
                case "Wonder":
                    {
                        Wonder wonder = allwonders.Find(wnd => wnd.Id == Convert.ToInt32(command.body));                        
                        gui.DisplayWonder(wonder);
                        break;
                    }
                case "NewTier":
                    {
                        gui.DisplayNewTier();
                        break;
                    }
                case "GameState":
                    {
                        int gold = (int)game_command.Data["Gold"];
                        gui.DisplayGameState(gold);
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

        public void PickCard(int id, string playmode)
        {
            // превалидация
            current_playmode = playmode;
            GameCommand command = new GameCommand("CardPick", id.ToString());
            Send(command);
        }

        public void SetConnectionState(StateObject state_object)
        {
            this.connection_state = state_object;
        }
    }
}
