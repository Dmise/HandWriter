using System.Text.Json.Nodes;
using Engine.OrLog.Models.Hand;
using HandDataCollector.Models;

namespace HandDataCollector
{
    internal static class MessageReader
    {

        public static int GetActiveSeatIndex(JsonNode message)
        {
            var index = message?["data"]?["countdown"]?["seat"]?.ToString();
            if (index != null)
                return Convert.ToInt32(index);
            return -1;
        }

        public static decimal GetTotalPot(JsonNode message)
        {
            try
            {
                var a = Convert.ToDecimal(message["data"]?["game_info"]?["pot"].ToString().Replace(".",","));
                return a;
            }
            catch (NullReferenceException ex)
            {
                throw ex;
            }
        }

        public static decimal GetRakeValue(JsonNode message)
        {
            return GetTotalPot(message) - GetPrizeValue(message);
        }

        public static decimal GetPrizeValue(JsonNode message)
        {
            decimal prizeTotal = 0;
            var seats = message?["data"]?["game_result"]?["seats"]?.AsObject();
            if (seats != null)
            {
                foreach (var seat in seats)
                {
                    var prize = Convert.ToDecimal(seat.Value?["prize"]?.AsArray()?.ElementAt(0)?.ToString().Replace(".", ",")); // "[\r\n  0.76\r\n]"
                    prizeTotal += prize;
                }
                return prizeTotal;
            }
            else
            {
                 
                return 0;
            }
            
        }

        public static Round GetRound(JsonNode message)
        {
            var gameState = message?["data"]?["game_info"]?["state"]?.ToString();
            switch (gameState)
            {
                case "2":
                    return Round.GameStart;
                case "3":
                    return Round.Preflop;
                case "4":
                    return Round.Flop;
                case "5":
                    return Round.Turn;
                case "6":
                    return Round.River;
                default:
                    return Round.Undefined;
            }
        }

        
        public static int GetPlayersInPlay(JsonNode message)
        {

            return Convert.ToInt32(message["data"]?["game_info"]?["gamers_count"]?.ToString());
        }

        public static List<CardData> ReadHeroCardsFromPromptMessage(JsonNode message)
        {

            var gameseat = message["data"]?["game_seat"]?.AsObject();

            if (gameseat != null)
            {
                var cards = gameseat.ElementAt(0).Value?["cards"];



                if (cards != null)
                {
                    var cart1 = new CardData(cards[0].ToString());
                    var cart2 = new CardData(cards[1].ToString());
                    List<CardData> cardsList = new List<CardData>();
                    cardsList.Add(cart1);
                    cardsList.Add(cart2);
                    return cardsList;
                }

            }
            return null;
        }

        internal static string LastRaise(JsonNode message)
        {
            var lastRaise = message?["data"]?["game_status"]?["last_raise"]?.ToString().Replace(".",",");
            if(lastRaise != null)
                return lastRaise;
            return "0";
        }

        public static int ReadHeroUidfromPrompMessage(JsonNode message)
        {
            var gameseat = message["data"]?["game_seat"]?.AsObject();

            if (gameseat != null)
            {
                return Convert.ToInt32(gameseat.ElementAt(0).Value?["uid"]);
            }
            return -1;
        }

        public static int ReadHeroIndexFromPromptMessage(JsonNode message)
        {
            var index = message["data"]?["game_seat"]?.AsObject().ElementAt(0).Value?["seat"]?.ToString();
            if (index != null)
            {
                return Convert.ToInt32(index);
            }
            return -1;
        }
        public static JsonObject? GetReamainPlayersFromGameOver(JsonNode message)
        {

            return message?["data"]?["game_result"]?["seats"]?.AsObject();
        }

        public static string GetPatternsFromGameOver(JsonNode message)
        {
            
            var pattern = message["data"]?["game_result"]?["patterns"];
            if (pattern != null)
            {
                return pattern.AsArray().ElementAt(0).ToString();
            }
            return "Winning pattern";
        }

        public static string GetPrizeFromGameOverByUid(JsonNode message, string uid)
        {
            string prize = "";
            var value = message["data"]?["game_result"]?["seats"]?.AsObject().FirstOrDefault(x => x.Key == uid).Value?["prize"]?.AsArray().ElementAt(0).ToString();
            if (value != null)
                return value;
            //var prizee = message["data"]?["game_result"]?["seats"];
            return prize;

        }

        

        public static List<CardData> GetLightCardsFromGameOver(JsonNode message)
        {
            var cards = new List<CardData>();
            var cardArrays = message["data"]?["game_result"]?["lightcards"]?.AsArray();
            if (cardArrays != null)
            {
                foreach (var cardArray in cardArrays)
                {
                    var arr = cardArray.AsArray();
                    foreach(var card in arr)
                    {
                        var cardStr = Convert.ToString(card);
                        cards.Add(new CardData(cardStr));
                    }
                    //cards.Add(card);

                }
            }
            return cards;
        }



        public static int? GetIndexByRole(JsonNode message, ActionType role)
        {
            var game_seat = message["data"]?["game_seat"]?.AsObject();
            if (game_seat != null)
            {
                var roleMatches = ActionDataHub.actionTypeDict.Where(x => x.Value == role);
                List<string> rolesList = new List<string>();

                if (roleMatches.Count() == 1)
                {
                    rolesList.Add(roleMatches.First().Key);
                }
                else if (roleMatches.Count() > 1)
                {
                    foreach (var r in roleMatches)
                    {
                        rolesList.Add(r.Key);
                    }

                }
                else if (roleMatches.Count() == 0)
                {
                    return null;
                }

                foreach (var seat in game_seat)
                {
                    for (int i = 0; i < rolesList.Count; i++)
                    {
                        string? playerRole = seat.Value?["role"]?.ToString();

                        if (playerRole == rolesList[i])
                        {
                            return Convert.ToInt32(seat.Value?["seat"]?.ToString());
                        }
                    }

                }

            }
            return null;
        }

        public static SeatDataSuprema GetSeatData(JsonNode message, int index)
        {
            SeatDataSuprema seatData = new SeatDataSuprema();
            var game_seat = message["data"]?["game_seat"]?.AsObject();
            foreach (var seat in game_seat)
            {
                var seatIndex = Convert.ToInt32(seat.Value["seat"].ToString());
                if (seatIndex == index)
                {
                    seatData.Index = seatIndex;
                    seatData.Uid = Convert.ToInt32(seat.Value["uid"].ToString());
                    var player = HandWriter.currentHand.Players.FirstOrDefault(p => p.Index == index);
                    if (player != null)
                    {
                        seatData.Name = player.Name;
                    }
                    seatData.Stack = Convert.ToDecimal(seat.Value["coins"]?.ToString().Replace('.', ','));
                    seatData.Pot = Convert.ToDecimal(seat.Value["chips_round"]?.ToString().Replace('.', ','));
                }
            }
            return seatData;
        }

        internal static MessageType GetMessageType(JsonNode message)
        {
            Console.WriteLine(message.ToString());
            var messageEvent = message["event"]?.ToString();
            if (messageEvent == null)
            {
                return MessageType.Undefined;
            }
            try
            {
                switch (messageEvent)
                {

                    case "moveturn":
                        return MessageType.Moveturn;
                    case "countdown":
                        return MessageType.Countdown;
                    case "gamestart":
                        return MessageType.Gamestart;
                    case "prompt":
                        return MessageType.Prompt;
                    case "gameover":
                        return MessageType.Gameover;
                    case "notify":
                        return MessageType.Notify;
                    case "initinfo":
                        return MessageType.InitInfo;
                    default:
                        return MessageType.Undefined;
                }
            }
            catch
            {
                throw;
            }

        }

        // может быть прочитано только из первого game_start
        public static int GetTableSize(JsonNode message)
        {
            var seats = message["data"]?["room_status"]?["seats"]?.AsArray();
            if (seats != null)
            {
                return seats.Count();
            }
            return 0;
        }

        public static ActionType GetActionTypeByIndex(JsonNode message, int playerIndex)
        {
            var action = ActionType.Undefined;

            var game_seat = message["data"]?["game_seat"]?.AsObject();
            if (game_seat != null)
            {
                foreach (var seat in game_seat)
                {
                    int index = Convert.ToInt32(seat.Value?["seat"].ToString());
                    if (playerIndex == index)
                    {
                        //дебажим значения "role", которых у нас нету
                        var role = seat.Value["role"]?.ToString();
                        bool existingrole = ActionDataHub.actionTypeDict.TryGetValue(role, out action);
                        if (!existingrole)
                        {

                            DataHub.WakeError($"new role is: {role}. It must be added to Dictionary. error throw from GetActionTypeByIndex method");

                        }
                    }
                }
            }
            return action;
        }

        public static decimal GetPotByIndex(JsonNode message, int index)
        {

            var game_seat = message["data"]?["game_seat"]?.AsObject();
            if (game_seat != null)
            {
                foreach (var seat in game_seat)
                {
                    int playerIndex = Convert.ToInt32(seat.Value?["seat"].ToString());
                    if (playerIndex == index)
                    {
                        return Convert.ToDecimal(seat.Value["chips_round"]?.ToString().Replace('.', ','));
                    }
                }
            }
            return 0; // ошибка сидушка с таким индексом не найдена

        }

        static public List<SeatDataSuprema> GetRemainPlayers(JsonNode message, HandData hand)
        {
            var players = new List<SeatDataSuprema>();
            var playersJson = message["data"]["game_result"]["seats"];
            for(int i = 0; i < playersJson.AsObject().Count; i++)
            {
                var player = new SeatDataSuprema();
                //var currentPlayer = playersJson.AsObject().ElementAt(i);
                var currentPlayer = playersJson.AsObject().ElementAt(i);
                player.Uid = Convert.ToInt32(currentPlayer.Key);
                player.Pot = Convert.ToDecimal(currentPlayer.Value?["chips"]?.ToString().Replace(".",","));
                player.Collected = Convert.ToDecimal(currentPlayer.Value?["prize"]?.AsArray().ElementAt(0)?.ToString().Replace(".", ","));
                player.Index = Convert.ToInt32(currentPlayer.Value?["seat"]?.ToString());
                player.Stack = Convert.ToDecimal(currentPlayer.Value?["coins"]?.ToString().Replace(".", ","));
                player.Name = hand.Players.Find(x => x.Uid == player.Uid).Name;

                players.Add(player);
            }
            return players;
        }
        static public int? GetDealerIndex(JsonNode message)
        {
            var index = message["data"]?["game_info"]?["dealer_seat"]?.ToString();
            if(index != null)
                return Convert.ToInt32(index);
            return null;
        }

        internal static GameType GetGameType(JsonNode message)
        {
            if (HandWriter.currentHand.GameType == GameType.Undefined)
            {
                var gameType = message["data"]?["game_info"]?["type"]?.ToString();
                if (gameType != null)
                {
                    switch (Convert.ToString(gameType))
                    {
                        case "1":
                            {
                                return GameType.HoldemNoLimit;
                            }
                        default:
                            return GameType.Undefined;
                    }
                }
                return GameType.Undefined;
            }
            else
            {
                return HandWriter.currentHand.GameType;
            }
        }
               
        internal static string? GetHandNumber(JsonNode message)
        {
            var number = message["data"]?["seqno"]?.ToString();
            return number;
        }

        internal static DateTime GetDateTime(JsonNode message)
        {

            var dateStr = message["data"]?["ts"]?.ToString();
            if (dateStr != null)
                return new DateTime(1970,1 ,1).AddMilliseconds(double.Parse(dateStr));
            return new DateTime();

        }


        internal static List<SeatDataSuprema> GetTableState(JsonNode message)
        {
            List<SeatDataSuprema> tableState = new List<SeatDataSuprema>();
            var game_seat = message["data"]?["game_seat"]?.AsObject();
            if (game_seat != null)
            {
                foreach (var seat in game_seat)
                {

                    SeatDataSuprema seatData = new SeatDataSuprema(MessageReader.GetRound(message));
                    var strIndex = seat.Value?["seat"]?.ToString();
                    var pot = Convert.ToDecimal(seat.Value?["chips_round"]?.ToString().Replace('.', ','));
                    var stack = Convert.ToDecimal(seat.Value?["coins"]?.ToString().Replace('.', ','));
                    var role = seat.Value?["role"]?.ToString();
                    string? name = null;
                    if (strIndex != null)
                    {
                        seatData.Index = Convert.ToInt32(strIndex);
                        name = HandWriter.currentHand.Players.FirstOrDefault(x => x.Index == Convert.ToInt32(strIndex))?.Name;
                    }
                    if (name != null)
                        seatData.Name = name;
                    else seatData.Name = "Undefined";                    
                    seatData.Stack = stack;
                    
                    if (role != null)
                    {
                        try { seatData.Role = ActionDataHub.actionTypeDict[role]; }
                        catch (KeyNotFoundException ex)
                        {
                            lock (DataHub.sessionLogFile)
                            {
                                DataHub.WakeError("Такого ключа нету в списке actionTypeDict:");
                                DataHub.WakeError(ex.Message);
                            }
                        }
                        finally
                        {


                        }
                    }
                    seatData.Pot = pot;

                    tableState.Add(seatData);
                }
            }

            return tableState;
        }
    }
}
