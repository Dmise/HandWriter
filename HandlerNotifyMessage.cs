using System.Text.Json.Nodes;
using Engine.OrLog.Models.Hand;
using HandDataCollector.Models;

namespace HandDataCollector
{
    internal class HandlerNotifyMessage
    {
        internal static void Process(JsonNode message)
        {
            if (HandWriter.currentHand.HeroName == null)
            {
                var name = message["apiData"]?["clubMember"]?["displayID"]?.ToString();
                if (name != null && HandWriter.currentHand.HeroName == null)
                    HandWriter.currentHand.HeroName = name;
            }
            if (HandWriter.currentHand.HeroIndex == -1)
            {

                var uid = message["apiData"]?["clubMember"]?["playerID"]?.ToString();
                if (uid != null)
                {
                    try
                    {
                        HandWriter.currentHand.HeroIndex = HandWriter.currentHand.Players.First(p => p.Uid == Convert.ToInt32(uid)).Index;
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {

                    }

                }

            }



            //if (currentHand.Players.Count != 0){
            //SeatData? user = null;
            //try
            //{
            //    user = currentHand.Players.Find(x => x.Uid == Convert.ToInt32(currentHand.UserVaraibles["playerID"]));
            //}
            //catch(KeyNotFoundException ex)
            //{
            //    DataHub.WakeError($"error throw from HandWriter.ProcessNotify method" +
            //                        $"\t\ncurrentHand.UserVaraibles[\"playerID\"] KEy not found. HandID # {currentHand.HandId}.");
            //}
            //finally
            //{

            //}
            //if (user != null && user.Index == -1)
            //    currentHand.HeroIndex = (int)user.Index ;
            //}

        }
    }
}
