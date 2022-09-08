
using System.Text.Json.Nodes;


namespace HandDataCollector
{
    internal static class HandlerPromptMessage
    {
        
       

        public static void Process(JsonNode message)
        {
            
            
            
            if (IsPromptWithGameSeat(message))
            {
                var cards = MessageReader.ReadHeroCardsFromPromptMessage(message);
                CheckName();
                CheckHeroIndex();
                AddHoledCardsToHero();

                try
                {
                    var logged = HandWriter.currentHand.UserVaraibles["DealtLogged"];
                    if(logged == "false")
                    {
                        HandWriter.currentHand.UserVaraibles["DealtLogged"] = "true";
                        Log();
                    }
                    
                }
                catch (Exception ex)
                {
                    
                }

            }

            

            void CheckName()
            {
                if(HandWriter.currentHand.HeroName == null)
                {
                    var uid = message["uid"]?.GetValue<int>();
                    if (uid != null)
                    {
                        var name = HandWriter.currentHand.Players.FirstOrDefault(p => p.Uid == uid)?.Name;
                        if (name != null)
                            HandWriter.currentHand.HeroName = name;
                    }
                }
            }
            void Log()
            {
                MessageDispatcher.logWriter.WriteMessage(MessageDispatcher.messageFactory.HoleCards(HandWriter.currentHand));
            }

            void CheckHeroIndex()
            {
                if(HandWriter.currentHand.HeroIndex == -1)
                {
                    var index = MessageReader.ReadHeroIndexFromPromptMessage(message);
                    if(index != -1)
                    {
                        HandWriter.currentHand.HeroIndex = index;
                    }
                }
            }

            void AddHoledCardsToHero()
            {
                var cards = MessageReader.ReadHeroCardsFromPromptMessage(message);
                if (cards != null)
                {
                    HandWriter.currentHand.Players.First(x => x.Name == HandWriter.currentHand.HeroName).HandCards = cards;

                }
            }
        }

        static bool IsPromptWithGameSeat(JsonNode message)
        {
            var seat = message["data"]?["game_seat"]?.GetHashCode();
            if (seat != null)
                return true;
            return false;
        }
    }
}
