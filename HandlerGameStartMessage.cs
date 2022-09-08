using Engine.OrLog.Models.Hand;
using System.Text.Json.Nodes;
using HandDataCollector.Models;

namespace HandDataCollector
{
    //после 3х gamestart сообщений должно быть записано в handWriter.currentHand
    // сформирвоан лист List<SeatData> Players   +
    // HandId  +
    // DealerIndex + 
    // GameType - 
    // Date +
    // TableSize +
    // HeroIndex  
    // HeroName
    // TableName
    // 
    // SmallBlindValue 
    // BigBlindValue
    // SmallBlindIndex
    // SmallBlindIndex

    

    
    internal static  class HandlerGameStartMessage
    {
        internal static bool headerWasLogged_flag { get; private set; } = false;
        internal static bool playersListWasCreated_flag { get; private set; } = false;
        internal static bool BlindsWasFilled_flag { get; private set; } = false;
        internal static bool playersListWasLogged_flag { get; private set; } = false;

        internal delegate void sendData(DataToLog data);
        internal static event sendData? MessageProcessed;
        static DateTime dateDefault = new DateTime();
        enum MessageType
        {
            First,
            Second,
            Third,
            Undefined
        }



        internal static void Process(JsonNode message)
        {
            switch (WhichGameStart(message))
            {
                case MessageType.First:
                    HandWriter.CheckNewhand(message);
                    ReadFieldsFromFirstGameStart(message);

                    CreatePlayersList(message);
                    FillPlayersNames(message);
                    break;

                case MessageType.Third:
                    FillBlinds(message);
                    AddBlindsToAction(message);
                    //if (HeaderDataReady)
                    //LogHeaderAsync();
                    //LogPlayersAsync();
                    //LogBlindsAsync();
                    LogHeader();
                    LogPlayers();
                    LogBlinds();
                    break;
            }

            MessageType WhichGameStart(JsonNode message)
            {
                var gamer = message["data"]?["gamer"]?.GetHashCode();
                //var gameSeat = message["data"]?["game_seat"]?.GetHashCode();
                var gameStatus = message["data"]?["game_status"]?.GetHashCode();
                if (gamer != null)
                    return MessageType.First;
                else if (gameStatus == null)
                    return MessageType.Second;
                else 
                    return MessageType.Third;
                return MessageType.Undefined;

            }
            void ReadFieldsFromFirstGameStart(JsonNode message)
            {
                var dealerIndex = MessageReader.GetDealerIndex(message);
                if (dealerIndex != null)
                {
                    HandWriter.currentHand.DealerIndex = (int)dealerIndex;
                }

                HandWriter.currentHand.Date = MessageReader.GetDateTime(message);

                if (HandWriter.currentHand.TableSize == 0)
                {
                    HandWriter.currentHand.TableSize = MessageReader.GetTableSize(message);
                }
            }

            void FillBlinds(JsonNode message)
            {
                var indexSmall = MessageReader.GetIndexByRole(message, ActionType.PostsSmallBlind);
                if (indexSmall != null)
                {
                    var seatSmall = MessageReader.GetSeatData(message, (int)indexSmall);
                    HandWriter.currentHand.SmallBlindIndex = (int)seatSmall.Index;
                    HandWriter.currentHand.SmallBlindValue = seatSmall.Pot;
                }

                var indexBig = MessageReader.GetIndexByRole(message, ActionType.PostsBigBlind);
                if (indexBig != null)
                {
                    var seatBig = MessageReader.GetSeatData(message, (int)indexBig);
                    HandWriter.currentHand.BigBlindIndex = seatBig.Index;
                    HandWriter.currentHand.BigBlindValue = seatBig.Pot;
                }
            }

            void AddBlindsToAction(JsonNode message)
            {
                try
                {
                    HandWriter.currentHand.PreflopActions.Add(new ActionData
                    {
                        Role = ActionType.PostsSmallBlind,
                        PlayerIndex = HandWriter.currentHand.SmallBlindIndex,
                        PlayerName = HandWriter.currentHand.Players.First(p => p.Index == HandWriter.currentHand.SmallBlindIndex).Name,
                        Bet = HandWriter.currentHand.SmallBlindValue,
                        ToBet = HandWriter.currentHand.SmallBlindValue,
                    });
                }
                catch (Exception ex)
                {
                    MessageDispatcher.logWriter.WriteMessage("[Warning] in AddBlindsToAction method with SB adding ");
                }
                try 
                { 
                    HandWriter.currentHand.PreflopActions.Add(new ActionData
                    {
                        Role = ActionType.PostsBigBlind,
                        PlayerIndex = HandWriter.currentHand.BigBlindIndex,
                        PlayerName = HandWriter.currentHand.Players.FirstOrDefault(p => p.Index == HandWriter.currentHand.BigBlindIndex)?.Name,
                        Bet = HandWriter.currentHand.BigBlindIndex,
                        ToBet = HandWriter.currentHand.BigBlindIndex,
                    });
                }
                catch (ArgumentNullException ex)
                {
                    
                    MessageDispatcher.logWriter.WriteMessage("[Warning] in AddBlindsToAction method with BB adding");
                }

            }

            Task LogHeader()
            {
                var headerData = PrepareHeaderData();
                MessageDispatcher.logWriter.WriteData(headerData);
                
                return Task.CompletedTask;  

                DataToLog PrepareHeaderData()
                {
                    var data = new DataToLog();
                    data.message = MessageDispatcher.messageFactory.CreateHeader(HandWriter.currentHand);
                    return data;
                }
            }

            async Task LogHeaderAsync()
            {
                var headerData = PrepareHeaderData();
                await MessageDispatcher.logWriter.WriteDataAsync(headerData);
                
                
                DataToLog PrepareHeaderData()
                {
                    var data = new DataToLog();
                    data.message = MessageDispatcher.messageFactory.CreateHeader(HandWriter.currentHand);
                    return data;
                }
            }
            

            async Task LogBlindsAsync()
            {
                var data = new DataToLog();
                data.messagesList.Add(MessageDispatcher.messageFactory.CreateSBmessage(HandWriter.currentHand));
                data.messagesList.Add(MessageDispatcher.messageFactory.CreateBBmessage(HandWriter.currentHand));
                await MessageDispatcher.logWriter.WriteDataAsync(data);
                
            }
            Task LogBlinds()
            {
                var data = new DataToLog();
                data.messagesList.Add(MessageDispatcher.messageFactory.CreateSBmessage(HandWriter.currentHand));
                data.messagesList.Add(MessageDispatcher.messageFactory.CreateBBmessage(HandWriter.currentHand));
                MessageDispatcher.logWriter.WriteData(data);
                
                return Task.CompletedTask;

                
            }

        }

        

        private static async void LogPlayersAsync()
        {
            var playerList = PrepareSeatListData();
            await MessageDispatcher.logWriter.WriteDataAsync(playerList);

            DataToLog PrepareSeatListData()
            {
                var data = new DataToLog();
                data.messagesList = MessageDispatcher.messageFactory.SeatsList(HandWriter.currentHand);
                return data;
            }
        }

        private static Task LogPlayers()
        {
            var playerList = PrepareSeatListData();
            MessageDispatcher.logWriter.WriteData(playerList);
            return Task.CompletedTask;

            DataToLog PrepareSeatListData()
            {
                var data = new DataToLog();
                data.messagesList = MessageDispatcher.messageFactory.SeatsList(HandWriter.currentHand);
                return data;
            }
        }
        #region Create Players List

        //Fill from gamer object
        // Сформировываем лист игроков
        // должно быть прочитано в первом сообщении gameStrat
        static bool FillPlayersNames(JsonNode message)
        {
            //read gamer object
            var players = message["data"]?["gamer"]?.AsObject();
            if (players != null)
            {
                foreach (var player in players)
                {
                    int playerid = Convert.ToInt32(player.Value?["uid"]?.ToString());
                    string? name = player.Value?["displayID"]?.ToString();
                    var findedPlayer = HandWriter.currentHand.Players.Find(p => p.Uid == playerid);
                    if (findedPlayer != null)
                    {
                        findedPlayer.Name = name;
                    }                                                 
                }
                return true;
            }
            else
                return false;
        }


        //Fill SeatDatas from game_seat object 
        // заполняем уже сформированный лист игроков
        static void CreatePlayersList(JsonNode message)
        {
            
            var game_seats = message["data"]?["game_seat"]?.AsObject();
            if (game_seats != null)
            {
                foreach (var seat in game_seats)
                {
                    int uid = Convert.ToInt32(seat.Key?.ToString());
                    var stack = Convert.ToDecimal(seat.Value?["coins"]?.ToString().Replace('.', ','));
                    var index = Convert.ToInt32(seat.Value?["seat"]?.ToString());
                    HandWriter.currentHand.Players.Add(new SeatData { Index = index, Stack = stack, Uid = uid });
                }
                HandWriter.currentHand.Players.Sort(new IndexComparer());
            }
        }

        class IndexComparer : IComparer<SeatData>
        {
            public int Compare(SeatData? x, SeatData? y)
            {

                if (x != null && y != null)
                {
                    if (x.Index != null && y.Index != null)
                        return (int)x.Index - (int)y.Index;
                    else if (x.Index == null && y.Index != null)
                        return -1;
                    else if (x.Index != null && y.Index == null)
                        return 1;
                }
                else if (x == null && y != null)
                    return -1;
                else if (x != null && y == null)
                    return 1;
                return 0;



            }
        }
        #endregion

             

        private static bool HeaderDataReady
        {
            get
            {
                if(HandWriter.currentHand.Date != dateDefault &&
                   HandWriter.currentHand.DealerIndex != -1 &&
                   HandWriter.currentHand.TableSize != 0 &&
                   HandWriter.currentHand.SmallBlindIndex != -1 )
                {
                    return true;
                }
                return false;
            }
        }       
       
        #region for Internal

        internal static void DropFlags()
        {
            
            headerWasLogged_flag = false;
            playersListWasCreated_flag = false;
            BlindsWasFilled_flag  = false;
            playersListWasLogged_flag = false;
        }
        #endregion


    }
}
