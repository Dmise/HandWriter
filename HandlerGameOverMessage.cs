using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using HandDataCollector.Models;
using Engine.OrLog.Models.Hand;
using HandDataCollector.Exceptions;

namespace HandDataCollector
{
    internal static class HandlerGameOverMessage
    {
        internal delegate void sendData(DataToLog data);
        internal static event sendData? MessageProcessed;
        private static List<SeatDataSuprema> winners = new List<SeatDataSuprema>();
        internal static void Process(JsonNode message)
        {

            FillCurrentHandFields(message);
            DataHub.PushHand(HandWriter.currentHand);

            Log(message);
            //MessageProcessed?.Invoke(data);

            HandWriter.currentHand = new Engine.OrLog.Models.Hand.HandData();
            ResetHandlerGameOver();
            //HandlerGameStartMessage.DropFlags();



        }

        static void ResetHandlerGameOver()
        {
            winners = new List<SeatDataSuprema>();
        }
        static void FillCurrentHandFields(JsonNode message)
        {
            HandWriter.currentHand.TotalPot = MessageReader.GetTotalPot(message);
            HandWriter.currentHand.Rake = HandWriter.currentHand.TotalPot - MessageReader.GetPrizeValue(message);

            FillHeroName();
            FillRemainPlayersFields();



            //Functions 

            void FillHeroName()
            {
                if (HandWriter.currentHand.HeroIndex == -1 && HandWriter.heroName != null)
                {
                    try
                    {
                        int? index = HandWriter.currentHand.Players.First(x => x.Name == HandWriter.heroName).Index;
                        if (index != null)
                            HandWriter.currentHand.HeroIndex = (int)index;
                    }
                    catch { }
                }
            }

           
            void FillRemainPlayersFields()
            {

                var remainPlayers = MessageReader.GetReamainPlayersFromGameOver(message);
                foreach(var playerJson in remainPlayers)
                {
                    var uid = Convert.ToInt32(playerJson.Key);
                    var player = HandWriter.currentHand.Players.FirstOrDefault(x => x.Uid == uid);

                    //fill prize
                    var prize = Convert.ToDecimal(playerJson.Value?["prize"]?[0].ToString().Replace(".",","));                    
                    if(prize != 0)
                    {
                        if (player != null)
                            player.Collected = prize;
                    }
                    
                    //fill cards
                    var cardsArray = message["data"]?["game_result"]?["seats"]?.AsObject().ElementAt(0).Value?["cards"]?.AsArray();
                    var firstCardValue = cardsArray?.ElementAt(0)?.GetValue<int>();
                    if (firstCardValue != 0)
                    {
                        List<CardData> cards = new List<CardData>();
                        for (int i = 0; i < cardsArray.Count; i++)
                        {
                            cards.Add(new CardData(cardsArray.ElementAt(i).ToString()));
                        }
                        player.HandCards = cards;
                    }


                }
            }
        }

        private static void Log(JsonNode message)
        {
            DataToLog data = new DataToLog();
            PrepareDataForLog();
            MessageDispatcher.logWriter.WriteData(data);
            MessageDispatcher.logWriter.WriteMessage(""); //  "\r\n"

            void PrepareDataForLog()
            {
                SeatData player = new SeatData();

                var ramainPlayers = MessageReader.GetPlayersInPlay(message);
                if (ramainPlayers == 1)
                {
                    //Uncalled bet
                    try
                    {
                        CreateUncalledBetRow();
                    }
                    catch
                    {

                    }
                    finally
                    {

                    }

                    //Winner Show or does not show cards
                    CreateShowRow();

                    //Collected
                    CreateCollectedRow();

                    //Summary
                    Summary();

                }
                else
                {
                    //Show Down
                    ShowDown();
                    
                    Summary();
                }

                if (data.message == null && data.messagesList.Count == 0)
                    data.message = " GameOver message - > 1 players remains\r\n";

                //Functions
                void CreateUncalledBetRow()
                {
                    
                    decimal lastRaise = Convert.ToDecimal(MessageReader.LastRaise(message));
                    var uid = message["data"]?["game_result"]?["seats"]?.AsObject().ElementAt(0).Key;

                    var findedPlayer = HandWriter.currentHand.Players.FirstOrDefault(x => x.Uid == Convert.ToInt32(uid));
                    
                    if (findedPlayer != null)
                    {
                        player = findedPlayer;
                    }
                    else
                    {
                        throw new UserIsNotFound($"В списке игроков не был найден игрок с uid {uid}");
                    }

                    data.messagesList.Add(MessageDispatcher.messageFactory.UncalledRaise(player.Name, lastRaise));
                }

                void CreateShowRow()
                {
                    var cardsArray = message["data"]?["game_result"]?["seats"]?.AsObject().ElementAt(0).Value?["cards"]?.AsArray();
                    var firstCardValue = cardsArray?.ElementAt(0)?.GetValue<int>();
                    if (firstCardValue == 0)
                    {
                        data.messagesList.Add(MessageDispatcher.messageFactory.DoesntShow(player.Name));
                    }
                    else
                    {
                        
                        var pattern = MessageReader.GetPatternsFromGameOver(message);
                        var lightCards = MessageReader.GetLightCardsFromGameOver(message);
                        data.messagesList.Add(MessageDispatcher.messageFactory.PlayerShowsCards(player, pattern, lightCards));

                    }
                }
                void ShowDown()
                {
                    data.messagesList.Add(MessageDispatcher.messageFactory.ShowDown());
                    var remainsPlayer = MessageReader.GetRemainPlayers(message, HandWriter.currentHand); // No namess
                    remainsPlayer.Sort(delegate(SeatDataSuprema x, SeatDataSuprema y)
                    {
                        return x.Index.CompareTo(y.Index);                        
                    });

                    // shows
                    foreach(var player in remainsPlayer)
                    {
                        if(player.HandCards.Count == 0)
                        {
                            data.messagesList.Add(MessageDispatcher.messageFactory.DoesntShow(player.Name));
                        }
                        else
                        {
                            data.messagesList.Add(MessageDispatcher.messageFactory.PlayerShowsCards(player));
                        }
                    }
                    // collect
                    foreach (var player in remainsPlayer)
                    {
                        if (player.Collected != 0)
                            data.messagesList.Add(MessageDispatcher.messageFactory.CollectedFromPot(player.Name, player.Collected.ToString()));
                    }
                }

                void CreateCollectedRow()
                {
                    var prize = MessageReader.GetPrizeFromGameOverByUid(message, Convert.ToString(player.Uid));
                    data.messagesList.Add(MessageDispatcher.messageFactory.CollectedFromPot(player.Name, prize));
                }

                void Summary()
                {
                    data.messagesList.AddRange(MessageDispatcher.messageFactory.SummaryPotBoard(HandWriter.currentHand));

                    foreach (var player in HandWriter.currentHand.Players)
                    {
                        data.messagesList.Add(MessageDispatcher.messageFactory.SeatSummary(player, HandWriter.currentHand));
                    }

                }


            }
        }

    }
}

