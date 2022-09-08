using System.Text.Json.Nodes;
using Engine.OrLog.Models.Hand;
using HandDataCollector.Models;

namespace HandDataCollector
{
    static class HandlerMoveturnMessage
    {
        internal delegate void sendData (DataToLog data);
        internal static event sendData? MessageProcessed;
        static List<SeatDataSuprema> tableState = new List<SeatDataSuprema>();
        internal static ActionDataSuprema currentAction = new ActionDataSuprema();

        

        public static void Process(JsonNode message)
        {
            
            // Это первое дейсвие ?
            if(tableState.Count != 0 ||  currentAction.PlayerIndex != -1)
            {
                var actionsBeforeProcess = HandWriter.currentHand.ActionCount;
                ProcessPrevAction(message);
                // если в было добавлено новое действие.
                var actionsAfterProcess = HandWriter.currentHand.ActionCount;
                if (actionsBeforeProcess != actionsAfterProcess)
                {
                    LogLastAction(HandWriter.currentHand);
                }

               
            }
            
            CreateNewAction(message);
                   

        }

        static void ProcessPrevAction(JsonNode message)
        {
            if (currentAction.PlayersInPlay > 1)
            {

                // записывеаем дейсвия игрока, если нам известен индекс предыдущего актора
                if (currentAction.PlayerIndex != -1)
                {
                    AddPrevAction(message);

                }
                //// мы не знаем индекс предыдущего актора, поэтмоу считываем значение с состояния стола.
                else
                {
                    var curStreet = MessageReader.GetRound(message);
                    if (curStreet == tableState?[0].ActionRound)
                    {
                        AddPrevActionFromTableState(message);

                    }
                }

                // Если последним действие было повышеине ставки, записать это в текущую руку
                if (HandWriter.isLastActionBetOrRaise)
                {
                    var betValue = HandWriter.LastAction.Bet;
                    switch (HandWriter.LastActionRound)
                    {
                        case (Round.Preflop):
                            HandWriter.currentHand.preflopBiggestBet = betValue;
                            break;
                        case (Round.Flop):
                            HandWriter.currentHand.flopBiggestBet = betValue;
                            break;
                        case (Round.Turn):
                            HandWriter.currentHand.turnBiggestBet = betValue;
                            break;
                        case (Round.River):
                            HandWriter.currentHand.riverBiggestBet = betValue;
                            break;

                    }
                }

            }

        }



        static void LogLastAction(HandData hand)
        {
            var action = HandWriter.LastAction;
            var message = MessageDispatcher.messageFactory.Action(action);
            MessageDispatcher.logWriter.WriteMessage(message);

        }
       

        static void AddPrevAction(JsonNode message)
        {
            var pot = MessageReader.GetPotByIndex(message, currentAction.PlayerIndex);
            currentAction.Bet = pot - currentAction.ToBet;
            currentAction.ToBet = pot;
            currentAction.Role = MessageReader.GetActionTypeByIndex(message, currentAction.PlayerIndex);
            AddCurrentActorToActionList();
        }

        static void AddCurrentActorToActionList()
        {
            switch (currentAction.Round)
            {
                case Round.Preflop:
                    HandWriter.currentHand.PreflopActions.Add(currentAction);
                    break;
                case Round.Flop:
                    if (HandWriter.currentHand.IsFlop == false)
                    {
                        HandWriter.currentHand.IsFlop = true;
                        LogNewStreet();
                    }
                    HandWriter.currentHand.FlopActions.Add(currentAction);
                    break;
                case Round.Turn:
                    if (HandWriter.currentHand.IsTurn == false)
                    {
                        HandWriter.currentHand.IsTurn = true;
                        LogNewStreet();
                    }
                    HandWriter.currentHand.TurnActions.Add(currentAction);
                    break;
                case Round.River:
                    if (HandWriter.currentHand.IsRiver == false)
                    {
                        HandWriter.currentHand.IsRiver = true;
                        LogNewStreet();
                    }
                    HandWriter.currentHand.RiverActions.Add(currentAction);
                    break;

            }
            currentAction = new ActionDataSuprema();
        }

        //static void AddBlindsAction(JsonNode message)
        //{
        //    var tableState = MessageReader.GetTableState(message);
        //    var smallblinds = tableState.Where(x => x.Role == ActionType.PostsSmallBlind);
        //    var bigblinds = tableState.Where(x => x.Role == ActionType.PostsBigBlind);
        //    foreach (var player in smallblinds)
        //    {
        //        ActionData ad = new ActionData();
        //        ad.Role = player.Role;
        //        ad.PlayerName = player.Name;
        //        ad.PlayerIndex = (int)player.Index;
        //        ad.Bet = player.Pot;
        //        ad.ToBet = player.Pot;
        //        HandWriter.currentHand.PreflopActions.Add(ad);
        //        HandWriter.currentHand.SmallBlindIndex = ad.PlayerIndex;
        //        HandWriter.currentHand.SmallBlindValue = (decimal)ad.ToBet;
        //    }
        //    foreach (var player in bigblinds)
        //    {
        //        ActionData ad = new ActionData();
        //        ad.Role = player.Role;
        //        ad.PlayerName = player.Name;
        //        ad.PlayerIndex = (int)player.Index;
        //        ad.Bet = player.Pot;
        //        ad.ToBet = player.Pot;
        //        HandWriter.currentHand.PreflopActions.Add(ad);
        //        HandWriter.currentHand.BigBlindValue = (decimal)ad.ToBet;
        //        if(bigblinds.Count() > 1)
        //        {
        //            var players = message["data"]["game_seat"].AsObject();
        //            foreach(var pl in players)
        //            {
        //                var role = pl.Value["role"].ToString();
        //                if(role == "93")  // yes Magic Number 
        //                {
        //                    HandWriter.currentHand.BigBlindIndex = Convert.ToInt32(pl.Value["seat"]?.ToString());
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            HandWriter.currentHand.BigBlindIndex = ad.PlayerIndex;
        //        }
        //    }
        //}
        static void AddPrevActionFromTableState(JsonNode message)
        {
            var activeTableState = MessageReader.GetTableState(message);
            if ((activeTableState != null) && (tableState.Count != 0))
            {
                foreach (var seatInPrevState in tableState)
                {

                    if (seatInPrevState.Role != ActionType.SitOut && seatInPrevState.Role != ActionType.Folds)
                    {
                        var activeMarkedSeat = activeTableState.First(x => x.Index == seatInPrevState.Index);
                        if (activeMarkedSeat != null)
                        {
                            if (seatInPrevState?.Role != activeMarkedSeat?.Role)
                            {
                                currentAction.Role = activeMarkedSeat.Role;
                                currentAction.PlayerIndex = (int)activeMarkedSeat.Index;
                                currentAction.PlayerName = activeMarkedSeat.Name;
                                var pot = activeMarkedSeat.Pot;
                                currentAction.ToBet = pot;
                                currentAction.Bet = pot - seatInPrevState.Pot;
                                currentAction.Round = activeMarkedSeat.ActionRound;
                                tableState = new List<SeatDataSuprema>();
                                AddCurrentActorToActionList();
                                break;
                            }
                        }
                    }
                }
            }
            else
            {                                   
                DataHub.WakeError("We miss previous or current tablestate");                           
                AddCurrentActorToActionList();
            }
        }
       
        static void CreateNewAction(JsonNode message)
        {
            currentAction = new ActionDataSuprema();
            var playersInPlay = MessageReader.GetPlayersInPlay(message);
            currentAction.PlayersInPlay = playersInPlay;

            
            
            if (playersInPlay > 1)
            {
                var curIndex = MessageReader.GetActiveSeatIndex(message);
                CheckCards();
                // индекс текущего игрока написан в сообщении?
                if (curIndex != -1)
                {
                    
                    currentAction.PlayerIndex = (int)curIndex;
                    
                    var name = HandWriter.currentHand.Players?.FirstOrDefault(x => x.Index == curIndex)?.Name;
                    if (name != null)
                        currentAction.PlayerName = name;
                    else currentAction.PlayerName = "Undefined";
                   var pot = MessageReader.GetPotByIndex(message, currentAction.PlayerIndex);
                    currentAction.ToBet = pot;
                    currentAction.Round = MessageReader.GetRound(message);

                }
                //// мы не знаем индекс предыдущего актора, поэтмоу считываем значение с состояния стола.
                else
                {
                    tableState = MessageReader.GetTableState(message);
                }
            }
            

            //local functions

            
            void CheckCards()
            {
                var cards = message["data"]?["game_info"]?["shared_cards"]?.AsArray();
                if (cards != null)
                {
                    if (cards.Count != 0 && cards.Count != HandWriter.currentHand.TableCards.Count)
                    {
                        HandWriter.currentHand.TableCards.Clear();                        
                        foreach(var card in cards)                            
                            HandWriter.currentHand.TableCards.Add(new CardData(card.ToString()));
                        
                    }
                }
            }
             
        }

        static void LogNewStreet()
        {
            switch (HandWriter.currentHand.TableCards.Count)
            {
                case 3:
                    var flopMessage = MessageDispatcher.messageFactory.FlopMessage(HandWriter.currentHand);
                    MessageDispatcher.logWriter.WriteMessage(flopMessage);
                    break;
                case 4:
                    var turnMessage = MessageDispatcher.messageFactory.TurnMessage(HandWriter.currentHand);
                    MessageDispatcher.logWriter.WriteMessage(turnMessage);
                    break;

                case 5:
                    var riverMessage = MessageDispatcher.messageFactory.RiverMessage(HandWriter.currentHand);
                    MessageDispatcher.logWriter.WriteMessage(riverMessage);

                    break;
            }
        }
    }

    
}
