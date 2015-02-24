using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Drawing;

namespace _7WondersCore
{
    public class Game
    {
        public GameInterface gameinterface;
        public List<Card> Deck_I, Deck_II, Deck_III, DiscardPile;
        public List<Player> players;
        public Age current_age;
        //string image_folder = "D:/planchik/pics";
        string age_1_folder = "D:/planchik/Age I";
        string age_2_folder = "D:/planchik/Age II";
        string age_3_folder = "D:/planchik/Age III";
        string guilds_folder = "D:/planchik/Age III/Guilds";
        public Game()
        {
            this.players = new List<Player>();
            this.gameinterface = new GameInterface();
            this.gameinterface.game = this;
            Deck_I = new List<Card>();
            Deck_II = new List<Card>();
            Deck_III = new List<Card>();
            DiscardPile = new List<Card>();         
        }

        public void Execute(Player player, GameCommand command)
        {
            switch (command.type)
            {
                case "CardPick":
                    {
                        // !!!
                        // скипнули валидацию
                        player.picked_card = player.current_booster.Find(card => card.Id.ToString() == command.body);
                        if (!players.Where(plr => plr.picked_card == null).Any())
                        {
                            GameCommand response = new GameCommand("PlayCard", "");
                            foreach (Player plr in players)
                            {
                                IssueCommand(response, plr);
                            }
                        }
                        break;
                    }
                case "PlayMode":
                    {
                        // !!!
                        // и тут скипнули валидацию
                        switch (command.body)
                        {
                            case "Play":
                                {
                                    // validate costs
                                    // pay costs
                                    player.play_mode = "Play";
                                    if (!players.Where(plr => plr.play_mode == null).Any())
                                    {
                                        ProcessCardPlays();
                                        ResetPlayerStates();
                                        NextTurn();
                                    }                                    
                                    break;
                                }
                            case "Wonder":
                                {
                                    // validate costs
                                    // pay costs
                                    // play wonder
                                    break;
                                }
                            case "Sell":
                                {
                                    player.play_mode = "Sell";
                                    if (!players.Where(plr => plr.play_mode == null).Any())
                                    {
                                        // валидация здесь?
                                        ProcessCardPlays();
                                        ResetPlayerStates();                                        
                                        NextTurn();                                        
                                    }
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        public void ProcessCardPlays()
        {
            foreach (Player player in players)
            {
                switch (player.play_mode)
                {
                    case "Play":
                        {
                            player.board.Add(player.picked_card);
                            player.current_booster.Remove(player.picked_card);
                            // !!!
                            GameCommand command = new GameCommand("Board", String.Join(",", player.board.Select(x => x.Id)));
                            IssueCommand(command, player);
                            break;
                        }
                    case "Sell":
                        {
                            DiscardPile.Add(player.picked_card);
                            player.current_booster.Remove(player.picked_card);
                            // а здесь уже можно обнулить picked_card, но ресет пока отдельно
                            player.Gold += 3;
                            // Стейт надо научиться передавать более подробно
                            GameCommand game_command = new GameCommand("CurrentGold",player.Gold.ToString());
                            gameinterface.Send(player, game_command);
                            break;
                        }
                }
            }
            // заглушка
        }
        public void ResetPlayerStates()
        {
            foreach (Player player in players)
            {
                player.picked_card = null;
                player.play_mode = null;                
            }
        }
        public void IssueCommand(GameCommand command, Player player)
        {
            gameinterface.Send(player, command);
        }

        public void AddPlayer(Player player)
        {
            players.Add(player);
            player.Game = this;            
        }

        public void Begin()
        {
            InitializeDecks(players.Count);
            current_age = Age.I;
            foreach (Player player in players)
            {
                player.current_booster = GenerateBooster(Age.I);
                player.board = new List<Card>();
            }
            foreach (Player player in players)
            {
                player.Gold = 3;
                // можно доложить игроку, что у него три голды
                // а можно не докладывать, сам додумается
                string booster = String.Join(",", player.current_booster.Select(card => card.Id));
                GameCommand command = new GameCommand("Booster", booster);
                IssueCommand(command, player);                
            }
        }

        public void NextTurn()
        {
            // !!!
            if (players.First().current_booster.Count > 1)
            {
                PassBoosters();
            }
            else
            {
                foreach (Player player in players)
                {
                    DiscardPile.Add(player.current_booster.First());
                }
                // тут надо меряться пальцами
                if (current_age != Age.III)
                {
                    current_age++;
                    foreach (Player player in players)
                    {
                        player.current_booster = GenerateBooster(current_age);
                    }
                    PassBoosters();
                }
                else
                {
                    // ну тут надо игру заканчивать, че

                }
            }
        }

        public void PassBoosters()
        {
            var tmp = players[0].current_booster;            
            for (int i = 0; i < players.Count - 1; i++)
            {
                players[i].current_booster = players[i + 1].current_booster;                
            }
            players[players.Count - 1].current_booster = tmp;
            foreach (Player player in players)
            {
                string booster = String.Join(",", player.current_booster.Select(card => card.Id));
                GameCommand command = new GameCommand("Booster", booster);
                IssueCommand(command, player);
            }
        }

        public List<Card> GenerateBooster(Age age)
        {
            List<Card> booster = new List<Card>();
            List<Card> current_deck = Deck_I;
            switch (age)
            {
                case Age.I:
                    current_deck = Deck_I;
                    break;
                case Age.II:
                    current_deck = Deck_II;
                    break;
                case Age.III:
                    current_deck = Deck_III;
                    break;
            }
            for (int i = 0; i < 7; i++)
            {
                var tmp_card = current_deck.ElementAt(0);
                booster.Add(tmp_card);
                current_deck.Remove(tmp_card);
            }
            return booster;
        }

        public void InitializeDecks(int players_count)
        {
            foreach (Card card in ParseFolder(age_1_folder))
            {
                if (card.Players <= players_count)
                {
                    Deck_I.Add(card);
                }
            }
            foreach (Card card in ParseFolder(age_2_folder))
            {
                if (card.Players <= players_count)
                {
                    Deck_II.Add(card);
                }
            }
            foreach (Card card in ParseFolder(age_3_folder))
            {
                if (card.Players <= players_count)
                {
                    Deck_III.Add(card);
                }
            }
            List<Card> tmp = new List<Card>();
            // велосипед и костыли
            foreach (Card card in ParseFolder(guilds_folder))
            {
                tmp.Add(card);
            }
            tmp = tmp.OrderBy(x => Guid.NewGuid()).ToList();
            for (int i = 0; i < players_count + 2; i++)
            {
                Deck_III.Add(tmp[i]);
            }
            Deck_I = Deck_I.OrderBy(x => Guid.NewGuid()).ToList();
            Deck_II = Deck_II.OrderBy(x => Guid.NewGuid()).ToList();
            Deck_III = Deck_III.OrderBy(x => Guid.NewGuid()).ToList();
        }

        public static IEnumerable<Card> ParseFolder(string xml_folder)
        {
            Directory.GetFiles(xml_folder);
            foreach (string file in Directory.GetFiles(xml_folder))
            {
                StringBuilder sb = new StringBuilder();
                using (StreamReader sr = new StreamReader(file))
                {
                    String line;                    
                    while ((line = sr.ReadLine()) != null)
                    {
                        sb.AppendLine(line);
                    }
                }
                string xmlstring = sb.ToString();
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(xmlstring);
                XmlNodeList elemList = xml.FirstChild.ChildNodes;
                Card card = new Card();
                for (int i = 0; i < elemList.Count; i++)
                {
                    switch (elemList[i].Name)
                    {
                        case "id":
                            card.Id = Convert.ToInt32(elemList[i].InnerText);
                            break;
                        case "name":
                            card.Name = elemList[i].InnerText;
                            break;
                        case "age":
                            card.Age = (Age)Enum.Parse(typeof(Age), elemList[i].InnerText);
                            break;
                        case "color":
                            card.Color = Color.FromName(elemList[i].InnerText);
                            break;                            
                        case "fame":
                            card.Fame = elemList[i].InnerText;
                            break;
                        case "cost":
                            card.Cost = elemList[i].InnerText;
                            break;
                        case "players":
                            card.Players = Convert.ToInt32(elemList[i].InnerText);
                            break;             

                    }

                }
                yield return card;
            }            
        }
    }
}
