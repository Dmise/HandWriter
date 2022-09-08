using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.OrLog.Models.Hand;
using HandDataCollector;

namespace LogWork
{
    internal class MessageFactory
    {

        private string currencySign;
        private string roomName;
        private CultureInfo culture = new CultureInfo("en-US");


        static class Messages
        {
            internal static string doesntShowHand = "doesn't show hand";
            internal static string holeCard = "*** HOLE CARDS ***";
            internal static string flop = "*** FLOP ***";
            internal static string turn = "*** TURN ***";
            internal static string river = "*** RIVER ***";
            internal static string showDown = "*** SHOW DOWN ***";
            internal static string summary = "*** SUMMARY ***";
            internal static string seat = "Seat";

            internal static string uncalledBet = "Uncalled bet";
            internal static string postsSB = "posts small blind";
            internal static string postsBB = "posts big blind";
            internal static string dealtTo = "Dealt to";
            internal static string sb = "(small blind)";
            internal static string bb = "(big blind)";
            internal static string dealer = "(button)";

            internal static string foldedBeforeFlop = "folded before Flop";
            internal static string foldedOnFlop = "folded on the Flop";
            internal static string foldedOnTurn = "folded on the Turn";
            internal static string foldedOnRiver = "folded on the River";


            internal static class Action
            {
                internal static string allIn = "all in";
                internal static string folds = "folds";
                internal static string bets = "bets";
                internal static string raises = "raises";
            }
        }




        public enum CurrencySign
        {
            Dollar,
            Euro, // U+20AC - unicode, 0x20AC - UTF-16
            Rupee // U+20A8 - unicode, 0x20A8 - UTF-16
        }



        public MessageFactory(string room, string currency = "dollar", CultureInfo cultureInput = null)
        {
            if (cultureInput != null)
                culture = cultureInput;
            else
                SetupDefaultCulture();
            MessageFactorySetup(room, currency);
        }
        private void MessageFactorySetup(string room, string currency)
        {
            roomName = room;
            switch (currency)
            {

                case "dollar":
                    currencySign = "$";
                    break;
                case "euro":
                    currencySign = "\u20AC";
                    break;
                case "rupee":
                    currencySign = "\u20A8";
                    break;
                case "chips":
                    currencySign = "chips ";
                    break;
                default:
                    currencySign = "";
                    break;

            }
        }
        private void SetupDefaultCulture()
        {

            culture.NumberFormat.NumberDecimalSeparator = ".";
            culture.NumberFormat.CurrencyDecimalSeparator = ".";
            culture.NumberFormat.NumberDecimalDigits = 2;
            culture.NumberFormat.CurrencyDecimalDigits = 2;

        }

        internal string UncalledRaise(string? name, decimal value)
        {
            var betValue = value.ToString(culture);
            if (name == null)
                name = "Undefined name";
            return $"{Messages.uncalledBet} ({currencySign}{betValue}) returned to {name}";
        }
        internal string CreateHeader(HandData handData)
        {
            string sbValue = handData.SmallBlindValue.ToString(culture);
            string bbValue = handData.BigBlindValue.ToString(culture);
            string header = $"{roomName} Hand #{handData.HandId}: ({currencySign}{sbValue}/{currencySign}{bbValue}) {GameType(handData)} [{handData.Date.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture)} UTC]\t\n" +
                    $"Table {handData.TableName} {handData.TableSize}-max Seat #{handData.DealerIndex} is the button";
            return string.Format(culture, header);
        }
        internal string DoesntShow(string hero)
        {
            return $"{hero}: doesn't show hand";
        }
        
        //using in Show down
        internal string PlayerShowsCards(SeatData player, string pattern = "", List<CardData>? lightCards = null)
        {
            if (player.HandCards.Count == 2)
                return $"{player.Name}: shows [{player.HandCards[0]} {player.HandCards[1]}] ({pattern} {lightCards})";
            else if (player.HandCards.Count == 4)
                return $"{player.Name}: shows [{player.HandCards[0]} {player.HandCards[1]} {player.HandCards[2]} {player.HandCards[3]}] ({pattern} {lightCards})";
            return "Players Shows Cards - zaglushka";
        }
        internal string CollectedFromPot(string hero, string valueInput)
        {
            string value = valueInput.ToString(culture);
            
            

            return $"{hero} collected {currencySign}{value} from pot";

        }

        internal string ShowDown()
        {
            return Messages.showDown;
        }

        string GameType(HandData hand)
        {
            string gameTypeMessage = "GameType";
            switch (hand.GameType)
            {
                case Engine.OrLog.Models.Hand.GameType.HoldemNoLimit:
                    gameTypeMessage = "Hold'em No Limit";
                    break;
                case Engine.OrLog.Models.Hand.GameType.OmahaNoLimit:
                    gameTypeMessage = "Omaha No Limit";
                    break;
                case Engine.OrLog.Models.Hand.GameType.OmahaPotLimit:
                    gameTypeMessage = "Omaha Pot Limit";
                    break;
                default:
                    gameTypeMessage = "undefined game type";
                    break;
            }
            return gameTypeMessage;
        }
        internal string SmallBlind(HandData hand)
        {

            try
            {
                var playerName = hand.Players.First(p => p.Index == hand.SmallBlindIndex).Name;
                if (playerName != null)
                {
                    var sbValue = hand.SmallBlindValue.ToString(culture);
                    return $"{playerName}: posts small blind {currencySign}{sbValue}";
                }
                else
                    return "ERROR.null_reference | В текущей руке(HandData) не был найден игрок такой, что player.Index == hand.SmallBlindIndex";
            }
            catch (Exception ex)
            {
                return "ERROR.exception | В текущей руке(HandData) не был найден игрок такой, что player.Index == hand.SmallBlindIndex. ";
            }
        }

        internal string FlopMessage(HandData hand)
        {

            return $"{Messages.flop} [{hand.TableCards[0].ToString()} {hand.TableCards[1].ToString()} {hand.TableCards[2].ToString()}]";
        }
        internal string TurnMessage(HandData hand)
        {
            return $"{Messages.turn} [{hand.TableCards[0]} {hand.TableCards[1]} {hand.TableCards[2]}] [{hand.TableCards[3]}]";
        }

        internal string RiverMessage(HandData hand)
        {
            return $"{Messages.river} [{hand.TableCards[0]} {hand.TableCards[1]} {hand.TableCards[2]} {hand.TableCards[3]}] [{hand.TableCards[4]}]";
        }

        internal string BigBlind(HandData hand)
        {
            try
            {
                var playerName = hand.Players.First(p => p.Index == hand.BigBlindIndex).Name;
                if (playerName != null)
                {
                    var bbValue = hand.BigBlindValue.ToString(culture);
                    return $"{playerName}: posts big blind {currencySign}{bbValue}";
                }
                else
                    return "ERROR.null_reference | В текущей руке(HandData) не был найден игрок такой, что player.Index == hand.SmallBlindIndex";
            }
            catch (Exception ex)
            {
                return "ERROR.exception | В текущей руке(HandData) не был найден игрок такой, что player.Index == hand.SmallBlindIndex. ";
            }
        }

        internal List<string> HoleCards(HandData hand)
        {
            var list = new List<string>();

            try
            {
                var hero = hand.Players.First(x => x.Name == hand.HeroName);
                if (hero.HandCards.Count > 0)
                {
                    list.Add($"{Messages.holeCard}");
                    list.Add($"{Messages.dealtTo} {hand.HeroName} [{hero.HandCards[0]} {hero.HandCards[1]}]");

                }
            }
            catch
            {

            }

            return list;
        }
        internal List<string> SeatsList(HandData hand)
        {
            var list = new List<string>();
            foreach (var player in hand.Players)
            {
                var stackValue = player.Stack.ToString(culture);
                list.Add($"Seat {player.Index}: {player.Name} ({currencySign}{stackValue} in chips)");
            }
            return list;

        }

        internal List<string> SummaryPotBoard(HandData hand)
        {
            var list = new List<string>();
            list.Add(Messages.summary);
            list.Add($"Total pot {currencySign}{hand.TotalPot.ToString(culture)} | Rake {currencySign}{hand.Rake.ToString(culture)}");
            if (hand.IsFlop == false)
                list.Add($"Board []");
            else if (hand.IsTurn == false)
                list.Add($"Board [{hand.TableCards[0]} {hand.TableCards[1]} {hand.TableCards[2]}]");
            else if (hand.IsRiver == false)
                list.Add($"Board [{hand.TableCards[0]} {hand.TableCards[1]} {hand.TableCards[2]} {hand.TableCards[3]}]");
            else
                list.Add($"Board [{hand.TableCards[0]} {hand.TableCards[1]} {hand.TableCards[2]} {hand.TableCards[3]} {hand.TableCards[4]}]");
                   
            return list;
        }

        internal string SeatSummary(SeatData player, HandData hand)
        {
            
                string? playerRole = null;
            string summary = $"Seat {player.Index}: {player.Name}";
            
                if (hand.BigBlindIndex == player.Index)
                    playerRole = Messages.bb;
                else if (hand.SmallBlindIndex == player.Index)
                    playerRole = Messages.sb;
                else if (hand.DealerIndex == player.Index)
                    playerRole = Messages.dealer;
                if (playerRole != null)
                {
                    summary += $" {playerRole}"; 
                }
            if (player.Collected != 0)
            {
                summary += $" collected ({currencySign}{player.Collected.ToString(culture)})";
            }
            else
            {
                try
                {
                    var action = HandWriter.GetPlayerLastAction(player.Index, HandWriter.currentHand);
                    switch (action.Round)
                    {
                        case Round.Preflop:
                            summary += " folded before Flop";
                            if (action.ToBet == 0)
                                summary += " (didn't bet)";
                            break;
                        case Round.Flop:
                            summary += " folded on the Flop";
                            break;
                        case Round.Turn:
                            summary += " folded on the Turn";
                            break;
                        case Round.River:
                            summary += " folded on the River";
                            break;
                    }
                }
                catch 
                {

                }
                finally
                {

                }
                
            }

            return summary;
                         
        }
        
        
        
        internal string CreateSBmessage(HandData handData)
        {
            var nickSB = handData.Players.First(p => p.Index == handData.SmallBlindIndex).Name;
            var sbValue = handData.SmallBlindValue.ToString(culture);
            if (nickSB == null)
                nickSB = "[Warning] SmallBlind Nickname"; // WARNING
            return $"{nickSB}: {Messages.postsSB} {currencySign}{sbValue}";
        }
        internal string CreateBBmessage(HandData handData)
        {
            string? nickBB;
            string bbValue = "undefinded";
            try
            {
                nickBB = handData.Players.First(p => p.Index == handData.BigBlindIndex).Name;
                bbValue = handData.BigBlindValue.ToString(culture);
            }
            catch (Exception ex)
            {
                nickBB = null;
            }
            if (nickBB == null)
            {
                nickBB = "[Warning] BigBlind Nickname is null"; // WARNING

            }
            return $"{nickBB}: {Messages.postsBB} {currencySign}{bbValue}";
        }
        internal string Action(ActionData action)
        {
            string actionType = "acton type";
            var betValue = action.Bet.ToString(culture);
            switch (action.Role)
            {
                case ActionType.Calls:
                    actionType = "calls";
                    return $"{action.PlayerName}: {actionType} {currencySign}{betValue}";
                case ActionType.Bets:
                    actionType = Messages.Action.bets;
                    return $"{action.PlayerName}: {actionType} {currencySign}{betValue}";
                case ActionType.Raises:
                    actionType = "raises";
                    break;
                case ActionType.AllIn:
                    actionType = Messages.Action.allIn;
                    break;
                case ActionType.RaiseAllIn:
                    actionType = "raises all in";
                    break;
                case ActionType.BetAllIn:
                    actionType = "bets all in";
                    break;
                case ActionType.CallAllIn:
                    actionType = "call all in";
                    return $"{action.PlayerName}: {actionType} {currencySign}{betValue}";


                case ActionType.Checks:
                    return $"{action.PlayerName}: checks";
                case ActionType.Folds:
                    return $"{action.PlayerName}: folds";
                case ActionType.PostsStraddle:
                    actionType = "posts straddle";
                    break;
                case ActionType.PostsAnte:
                    actionType = "posts ante";
                    break;
            }
            return $"{action.PlayerName}: {actionType} {currencySign}{betValue} to {currencySign}{betValue}";
        }
    }
}
