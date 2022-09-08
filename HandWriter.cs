using Engine.OrLog.Models.Hand;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using HandDataCollector.Models;


namespace HandDataCollector
{

    // Класс, который собирает HandData из получаемых сообщений
    // Создает события на запись логов

    public static class HandWriter
    {
        internal static string heroName;
        internal static bool initialised = false;
        internal static HandData currentHand = new HandData();

        internal static bool isLastActionBetOrRaise
        {
            get
            {
                if(LastAction.Role == ActionType.BetAllIn || LastAction.Role == ActionType.Bets 
                    || LastAction.Role == ActionType.Raises || LastAction.Role == ActionType.RaiseAllIn)

                {
                    return true;
                }
                return false;
                
            }
        }
        

        internal static int MovesCount
        {
            get
            {
                return currentHand.PreflopActions.Count +
                       currentHand.FlopActions.Count +
                       currentHand.TurnActions.Count +
                       currentHand.RiverActions.Count;
            }

        }
        internal static ActionData PeekLastAction
        {
            get 
            { 
                return new ActionData(); 
            }
        }

        public static void PushMessageOnStack(string message)
        {
            var matchMessage = DataHub.regJsonMessage.Match(message);
            if (matchMessage.Success)
            {
                DataHub.PushMessage(matchMessage.Value);
            }
        }
        
        /// <summary>
        /// Add players to current hand from "gamer" object
        /// </summary>
        /// <param name="message"></param>
        


        internal static void UpdateTableCards(JsonNode message)
        {
            string? jsonCards = message?["data"]?["game_info"]?["shared_cards"]?
                        .ToString();
            if (jsonCards != null)
            {
                Regex reg = new Regex(@"\d+");
                var matches = Regex.Matches(jsonCards, @"\d+");
                if (matches.Count() > currentHand.TableCards.Count)
                {
                    for (int i = 0; i < matches.Count; i++)
                    {
                        if (currentHand.TableCards.ElementAtOrDefault(i) == null)
                        {
                            currentHand.TableCards.Add(new CardData(matches[i].ToString()));
                        }
                        currentHand.TableCards[i] = new CardData(matches[i].ToString());

                    }
                }
            }
        }
        internal static bool IsItFirstActionOnStreet(JsonNode message, Round gamestat)
        {
            switch (gamestat)
            {
                case Round.Preflop:
                    if (HandWriter.currentHand.PreflopActions.Count == 0 && DataHub.currentActor.Round == Round.Undefined)
                        return true;
                    break;
                case Round.Flop:
                    if (HandWriter.currentHand.FlopActions.Count == 0 && DataHub.currentActor.Round == Round.Preflop)
                        return true;
                    break;
                case Round.Turn:
                    if (HandWriter.currentHand.TurnActions.Count == 0 && DataHub.currentActor.Round == Round.Flop)
                        return true;
                    break;
                case Round.River:
                    if (HandWriter.currentHand.RiverActions.Count == 0 && DataHub.currentActor.Round == Round.Turn)
                        return true;
                    break;
            }
            return false;

        }
        

        internal static void CheckNewhand(JsonNode message)
        {
            var seqnoValue =  message["data"]?["seqno"]?.ToString(); // hand number value
            if (seqnoValue != null)
            {
                string handId = Convert.ToString(seqnoValue);
                if (currentHand.HandId == null)
                {
                    currentHand.HandId = handId;
                    InitCurrentHandVariables();
                }
                else
                {
                    if (currentHand.HandId != handId)
                    {
                        currentHand = new HandData();
                        currentHand.HandId = handId;
                        InitCurrentHandVariables();
                    }
                }
            }
        }

        internal static ActionDataSuprema GetPlayerLastAction(int playerIndex, HandData hand)
        {
            

            
               var action = GetLastPlayerAction(hand.RiverActions, playerIndex);
            if(action != null)
                return action;
            action = GetLastPlayerAction(hand.TurnActions, playerIndex);
            if(action != null)
                return action;
            action = GetLastPlayerAction(hand.FlopActions, playerIndex);
            if (action != null)
                return action;
            action = GetLastPlayerAction(hand.PreflopActions, playerIndex);
            if(action != null)
                return action;
            throw new Exception("Players actions was not found");


            //Functions

            ActionDataSuprema? GetLastPlayerAction(List<ActionData> list, int index)
            {

                //ActionDataSuprema? action = null;
                
                for (var i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].PlayerIndex == index)
                    {
                        ActionDataSuprema lastAction = (ActionDataSuprema)list[i];
                        return lastAction;
                    }
                }
                return null;

            }

        }

        internal static Round LastActionRound
        {
            get
            {
                if (currentHand.RiverActions.Count != 0)
                {
                    return Round.River;
                }
                else if (currentHand.TurnActions.Count != 0)
                {
                    return Round.Turn;
                }
                else if (currentHand.FlopActions.Count != 0)
                {
                    return Round.Flop;

                }
                return Round.Preflop;
            }
        }
        internal static ActionData LastAction
        {
            get
            {
                if (currentHand.RiverActions.Count != 0)
                {
                    return currentHand.RiverActions.Last();
                }
                else if (currentHand.TurnActions.Count != 0)
                {
                    return currentHand.TurnActions.Last();
                }
                else if (currentHand.FlopActions.Count != 0)
                {
                    return currentHand.FlopActions.Last();

                }
                return currentHand.PreflopActions.Last();
            }
        }

        static void InitCurrentHandVariables()
        {
            currentHand.UserVaraibles["DealtLogged"] = "false";
        }
        

    }
}
