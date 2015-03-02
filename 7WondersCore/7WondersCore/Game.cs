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
        public List<Wonder> wonders;
        public List<Player> players;
        public Age current_age;
        //string image_folder = "D:/planchik/pics";
        string wonders_folder = "D:/planchik/Wonders";
        string age_1_folder = "D:/planchik/Age I";
        string age_2_folder = "D:/planchik/Age II";
        string age_3_folder = "D:/planchik/Age III";
        string guilds_folder = "D:/planchik/Age III/Guilds";
        public Game()
        {
            this.players = new List<Player>();
            this.gameinterface = new GameInterface();
            this.gameinterface.game = this;
            this.wonders = new List<Wonder>();
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
                            // Похоже, тут свитч уже не нужен
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
                                    player.play_mode = "Wonder";
                                    if (!players.Where(plr => plr.play_mode == null).Any())
                                    {
                                        ProcessCardPlays();
                                        ResetPlayerStates();
                                        NextTurn();
                                    }
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
                            // etb
                            var etb = player.picked_card.ETB;
                            player.Gold += Calculate(player, etb);                            
                            // !!!

                            GameCommand command = new GameCommand("Board", String.Join(",", player.board.Select(x => x.Id)));
                            IssueCommand(command, player);                            

                            GameCommand game_command = new GameCommand("CurrentGold", player.Gold.ToString());
                            gameinterface.Send(player, game_command);

                            // !!!
                            // Если строкового боди нет, его надо скрыть в наследниках
                            game_command = new GameCommand("GameState", "");
                            // индексатор по объекту сразу?
                            game_command.Data["Gold"] = player.Gold;
                            gameinterface.Send(player, game_command);
                            //
                            
                            break;
                        }
                    case "Wonder":
                        {
                            player.current_booster.Remove(player.picked_card);
                            player.wonder.CurrentTier++;
                            var etb = player.wonder.Tiers[player.wonder.CurrentTier - 1].ETB;
                            player.Gold += Calculate(player, etb);
                            GameCommand game_command = new GameCommand("CurrentGold", player.Gold.ToString());
                            gameinterface.Send(player, game_command);
                            game_command = new GameCommand("NewTier", "");
                            gameinterface.Send(player, game_command);
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
            InitializeWonders(players.Count);
            current_age = Age.I;
            foreach (Player player in players)
            {
                player.current_booster = GenerateBooster(Age.I);
                player.board = new List<Card>();
                GameCommand command = new GameCommand("Wonder", player.wonder.Id.ToString());
                gameinterface.Send(player, command);
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
                ProcessMilitary();
                //
                if (current_age != Age.III)
                {
                    foreach (Player player in players)
                    {
                        GameCommand command = new GameCommand("Message", "Your victory tokens for " + current_age.ToString() + ": " + player.VictoryTokens[current_age].ToString());
                        gameinterface.Send(player, command);
                        command = new GameCommand("Message", "Your defeat tokens: " + player.defeat_tokens.ToString());
                        gameinterface.Send(player, command);
                    }
                    current_age++;
                    foreach (Player player in players)
                    {
                        player.current_booster = GenerateBooster(current_age);                        
                    }
                    PassBoosters();
                }
                else
                {
                    foreach (Player player in players)
                    {
                        GameCommand command = new GameCommand("Message", "Your victory tokens for " + current_age.ToString() + ": " + player.VictoryTokens[current_age].ToString());
                        gameinterface.Send(player, command);
                        command = new GameCommand("Message", "Your defeat tokens: " + player.defeat_tokens.ToString());
                        gameinterface.Send(player, command);

                    }
                    foreach (Player player in players)
                    {
                        GameCommand command = new GameCommand("Message", "Game ended");                        
                        gameinterface.Send(player, command);
                        command = new GameCommand("Message", "Your fame: " + CalculateFame(player).ToString());
                        gameinterface.Send(player, command);                        
                    }                    
                }
            }
        }

        public int CalculateFame(Player player)
        {
            int fame = 0;
            // Fame for board
            foreach (Card card in player.board.Where(crd => crd.Fame.value != 0))
            {
                fame += Calculate(player, card.Fame);
            }
            // Fame for wonder
            foreach (Card tier in player.GetWonderCards().Where(tier => tier.Fame.value != 0))
            {
                fame += Calculate(player, tier.Fame);
            }
            // Fame for tech            
            fame += CalculateTechFame(player);
            // Fame for military
            fame += player.VictoryTokens[Age.I];
            fame += player.VictoryTokens[Age.II] * 3;
            fame += player.VictoryTokens[Age.III] * 5;
            fame -= player.defeat_tokens;            
            // Fame for gold
            fame += player.Gold / 3;
            return fame;
        }

        public int CalculateTechFame(Player player)
        {            
            // Можно вынести в игрока?
            int fame = 0;
            int gears = player.board.Where(crd => crd.Tech == Tech.Gear).Count() + player.GetWonderCards().Where(crd => crd.Tech == Tech.Gear).Count();
            int compasses = player.board.Where(crd => crd.Tech == Tech.Compass).Count() + player.GetWonderCards().Where(crd => crd.Tech == Tech.Compass).Count();
            int tablets = player.board.Where(crd => crd.Tech == Tech.Tablet).Count() + player.GetWonderCards().Where(crd => crd.Tech == Tech.Tablet).Count();
            int any_techs = player.board.Where(crd => crd.Tech == Tech.Any).Count() + player.GetWonderCards().Where(crd => crd.Tech == Tech.Any).Count();
            fame = TechCalculator.Euristic(gears, compasses, tablets, any_techs);
            return fame;
        }

        public int Calculate(Player player, Card.Calculator calc)
        {
            int sum = 0;
            if (calc.value == 0) { return 0; }
            switch (calc.mode)
            {
                case Card.Calculator.PerMode.Single:
                    {
                        // вряд ли здесь понадобится проверять таргеты
                        return calc.value;
                    }
                case Card.Calculator.PerMode.Victory:
                    {
                        foreach (Player target in GetTargetsForPlayer(player, calc.targets))
                        {
                            sum += calc.value * target.VictoryTokens.Sum(x => x.Value);
                        }
                        return sum;
                    }
                case Card.Calculator.PerMode.Defeat:
                    {
                        foreach (Player target in GetTargetsForPlayer(player, calc.targets))
                        {
                            sum += calc.value * target.defeat_tokens;
                        }
                        return sum;
                    }
                case Card.Calculator.PerMode.Wonder:
                    {
                        foreach (Player target in GetTargetsForPlayer(player, calc.targets))
                        {
                            sum += calc.value * target.wonder.CurrentTier;
                        }
                        return sum;
                    }
                // частный случай, при желании можно выпилить
                case Card.Calculator.PerMode.BrownGrayPurple:
                    {
                        foreach (Player target in GetTargetsForPlayer(player, calc.targets))
                        {
                            sum += calc.value * target.board.Where(x => x.Color == CardColor.Brown).Count();
                            sum += calc.value * target.board.Where(x => x.Color == CardColor.Gray).Count();
                            sum += calc.value * target.board.Where(x => x.Color == CardColor.Purple).Count();
                        }
                        return sum;
                    }
                // !!!
                // в этот кейз должны попадать только цвета
                default:
                    {
                        foreach (Player target in GetTargetsForPlayer(player, calc.targets))
                        {
                            sum += calc.value * target.board.Where(x => x.Color == (CardColor)Enum.Parse(typeof(CardColor), calc.mode.ToString())).Count();
                        }
                        return sum;
                    }
            }
        }

        public IEnumerable<Player> GetTargetsForPlayer(Player player, Card.Calculator.Targets target_mode)
        {
            List<Player> targets = new List<Player>();
            switch (target_mode)
            {
                case Card.Calculator.Targets.Left:
                    {
                        targets.Add(players[(players.IndexOf(player) + players.Count - 1) % players.Count]);
                        break;
                    }
                case Card.Calculator.Targets.Right:
                    {
                        targets.Add(players[(players.IndexOf(player) + 1) % players.Count]);
                        break;
                    }
                case Card.Calculator.Targets.Neighbours:
                    {
                        targets.Add(players[(players.IndexOf(player) + players.Count - 1) % players.Count]);
                        targets.Add(players[(players.IndexOf(player) + 1) % players.Count]);
                        break;
                    }
                case Card.Calculator.Targets.Self:
                    {
                        targets.Add(player);                        
                        break;
                    }
                case Card.Calculator.Targets.All:
                    {
                        targets.Add(player);
                        targets.Add(players[(players.IndexOf(player) + players.Count - 1) % players.Count]);
                        targets.Add(players[(players.IndexOf(player) + 1) % players.Count]);
                        break;
                    }
            }
            return targets;
        }

        public void ProcessMilitary()
        {            
            // тут можно подумать о дипломатии
            // войну проводим по кругу в сторону соседа справа
            for (int i = 0; i < players.Count - 1; i++)
            {
                if (players[i].GetMilitary() < players[i + 1].GetMilitary())
                {
                    players[i].defeat_tokens += 1;
                    players[i + 1].VictoryTokens[current_age]++;
                }
                else if (players[i].GetMilitary() > players[i + 1].GetMilitary())
                {
                    players[i].VictoryTokens[current_age]++;
                    players[i + 1].defeat_tokens += 1;
                }
            }
            if (players.First().GetMilitary() < players.Last().GetMilitary())
            {
                players.First().defeat_tokens += 1;
                players.Last().VictoryTokens[current_age]++;
            }
            else if (players.First().GetMilitary() > players.Last().GetMilitary())
            {
                players.First().VictoryTokens[current_age]++;
                players.Last().defeat_tokens += 1;
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
            foreach (Card card in ParseForCards(age_1_folder))
            {
                if (card.Players <= players_count)
                {
                    Deck_I.Add(card);
                }
            }
            foreach (Card card in ParseForCards(age_2_folder))
            {
                if (card.Players <= players_count)
                {
                    Deck_II.Add(card);
                }
            }
            foreach (Card card in ParseForCards(age_3_folder))
            {
                if (card.Players <= players_count)
                {
                    Deck_III.Add(card);
                }
            }
            List<Card> tmp = new List<Card>();
            // велосипед и костыли
            foreach (Card card in ParseForCards(guilds_folder))
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

        public void InitializeWonders(int players_count)
        {
            //var wonders = Directory.GetDirectories(wonders_folder).OrderBy(x => Guid.NewGuid());
            foreach (var wonder in ParseForWonders(wonders_folder, true))
            {
                // опять костыль, чудес в любом случае семь, но стороны рандомные
                wonders.Add(wonder);
            }
            wonders = wonders.OrderBy(x => Guid.NewGuid()).ToList();
            for (int i = 0; i < players_count; i++)
            {
                players[i].wonder = wonders[i];
            }
        }

        // костыль
        // второй параметр false - парсит все чудеса, иначе только рандомную сторону
        public static IEnumerable<Wonder> ParseForWonders(string xml_folder, bool random)
        {
            // !!!
            // говнокод
            foreach (string dir in Directory.GetDirectories(xml_folder))
            {
                List<string> tmp = new List<string>();
                if (random)
                {
                    tmp.Add((Guid.NewGuid().GetHashCode() % 2 == 0) ? Directory.GetDirectories(dir).First() : Directory.GetDirectories(dir).Last());
                }
                else
                {
                    tmp.Add(Directory.GetDirectories(dir).First());
                    tmp.Add(Directory.GetDirectories(dir).Last());
                }
                foreach (string wonder_path in tmp)
                {

                    string filename = Directory.GetFiles(wonder_path,"*.xml").First();
                    StringBuilder sb = new StringBuilder();
                    using (StreamReader sr = new StreamReader(filename))
                    {
                        String line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            sb.AppendLine(line);
                        }
                    }
                    Wonder wonder = new Wonder();
                    string xmlstring = sb.ToString();
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(xmlstring);
                    var wondernode = xml.FirstChild;                    
                    wonder.Id = Convert.ToInt32(wondernode.Attributes["Id"].Value);
                    wonder.Name = wondernode.Attributes["Name"].Value;
                    foreach (XmlNode child in wondernode.ChildNodes)
                    {
                        Card card = new Card();
                        foreach (XmlNode card_node in child.ChildNodes)
                        {
                            switch (card_node.Name)
                            {
                                case "military":
                                    card.Military = Convert.ToInt32(card_node.InnerText);
                                    break;
                                case "tech":
                                    card.Tech = (Tech)Enum.Parse(typeof(Tech), card_node.InnerText);
                                    break;
                                case "etb":
                                    {
                                        Card.Calculator.PerMode mode = Card.Calculator.PerMode.Single;
                                        Card.Calculator.Targets targets = Card.Calculator.Targets.Self;
                                        int value = Convert.ToInt32(card_node.InnerText);
                                        foreach (XmlAttribute attr in card_node.Attributes)
                                        {
                                            switch (attr.Name)
                                            {
                                                case "PerMode":
                                                    {
                                                        mode = (Card.Calculator.PerMode)Enum.Parse(typeof(Card.Calculator.PerMode), attr.Value);
                                                        break;
                                                    }
                                                case "Targets":
                                                    {
                                                        targets = (Card.Calculator.Targets)Enum.Parse(typeof(Card.Calculator.Targets), attr.Value);
                                                        break;
                                                    }
                                            }
                                        }
                                        card.ETB = new Card.Calculator() { mode = mode, targets = targets, value = value };
                                        break;
                                    }
                                case "fame":
                                    {
                                        Card.Calculator.PerMode mode = Card.Calculator.PerMode.Single;
                                        Card.Calculator.Targets targets = Card.Calculator.Targets.Self;
                                        int value = Convert.ToInt32(card_node.InnerText);
                                        foreach (XmlAttribute attr in card_node.Attributes)
                                        {
                                            switch (attr.Name)
                                            {
                                                case "PerMode":
                                                    {
                                                        mode = (Card.Calculator.PerMode)Enum.Parse(typeof(Card.Calculator.PerMode), attr.Value);
                                                        break;
                                                    }
                                                case "Targets":
                                                    {
                                                        targets = (Card.Calculator.Targets)Enum.Parse(typeof(Card.Calculator.Targets), attr.Value);
                                                        break;
                                                    }
                                            }
                                        }
                                        card.Fame = new Card.Calculator() { mode = mode, targets = targets, value = value };
                                        break;
                                    }
                                case "cost":
                                    card.Cost = card_node.InnerText;
                                    break;
                            }
                        }
                        wonder.Tiers.Add(card);
                    }
                    yield return wonder;
                }
            }
        }

        public static IEnumerable<Card> ParseForCards(string xml_folder)
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
                            card.Color = (CardColor)Enum.Parse(typeof(CardColor), elemList[i].InnerText);
                            break;
                        case "military":
                            card.Military = Convert.ToInt32(elemList[i].InnerText);
                            break;
                        case "tech":
                            card.Tech = (Tech)Enum.Parse(typeof(Tech), elemList[i].InnerText);
                            break;
                        case "etb":
                            {
                                Card.Calculator.PerMode mode = Card.Calculator.PerMode.Single;
                                Card.Calculator.Targets targets = Card.Calculator.Targets.Self;
                                int value = Convert.ToInt32(elemList[i].InnerText);
                                foreach (XmlAttribute attr in elemList[i].Attributes)
                                {
                                    switch (attr.Name)
                                    {
                                        case "PerMode":
                                            {
                                                mode = (Card.Calculator.PerMode)Enum.Parse(typeof(Card.Calculator.PerMode), attr.Value);
                                                break;
                                            }
                                        case "Targets":
                                            {
                                                targets = (Card.Calculator.Targets)Enum.Parse(typeof(Card.Calculator.Targets), attr.Value);
                                                break;
                                            }
                                    }
                                }
                                card.ETB = new Card.Calculator() { mode = mode, targets = targets, value = value };
                                break;
                            }
                        case "fame":
                            {
                                Card.Calculator.PerMode mode = Card.Calculator.PerMode.Single;
                                Card.Calculator.Targets targets = Card.Calculator.Targets.Self;
                                int value = Convert.ToInt32(elemList[i].InnerText);
                                foreach (XmlAttribute attr in elemList[i].Attributes)
                                {
                                    switch (attr.Name)
                                    {
                                        case "PerMode":
                                            {
                                                mode = (Card.Calculator.PerMode)Enum.Parse(typeof(Card.Calculator.PerMode), attr.Value);
                                                break;
                                            }
                                        case "Targets":
                                            {
                                                targets = (Card.Calculator.Targets)Enum.Parse(typeof(Card.Calculator.Targets), attr.Value);
                                                break;
                                            }
                                    }
                                }
                                card.Fame = new Card.Calculator() { mode = mode, targets = targets, value = value };
                                break;
                            }
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
