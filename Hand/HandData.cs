using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Engine.OrLog.Models.Hand
{
    public class HandData
    {
        public string HandId { get; set; }
        public GameType GameType { get; set; } = GameType.Undefined; //holdem omaha
        public decimal SmallBlindValue { get; set; }
        public decimal BigBlindValue { get; set; }
        public DateTime Date { get; set; }
        public string TableName { get; set; } = "TableName";
        public int TableSize { get; set; }  //6max = 
        public int DealerIndex { get; set; } = -1;
        public List<SeatData> Players { get; set; }
        public int SmallBlindIndex { get; set; } = -1;
        public int BigBlindIndex { get; set; } = -1;
        public string HeroName { get; set; }
        public int HeroIndex { get; set; } = -1; // положение нашего игрока на столе
        public List<ActionData> PreflopActions { get; set; }
        public bool IsFlop { get; set; }
        public List<ActionData> FlopActions { get; set; }
        public bool IsTurn { get; set; }
        public List<ActionData> TurnActions { get; set; }
        public bool IsRiver { get; set; }
        public List<ActionData> RiverActions { get; set; }
        public List<CardData> TableCards { get; set; }

        public decimal preflopBiggestBet { get; set; }
        public decimal flopBiggestBet { get; set; }
        public decimal turnBiggestBet { get; set; }
        public decimal riverBiggestBet { get; set; }

        public decimal TotalPot { get; set; }
        public decimal Rake { get; set; }

        public Dictionary<string, string> UserVaraibles { get; set; }

        public int ActionCount
        {
            get
            {
                return PreflopActions.Count
                       + FlopActions.Count
                       + RiverActions.Count
                       + TurnActions.Count;
            }
        }



        public HandData()
        {
            Players = new List<SeatData>();
            PreflopActions = new List<ActionData>();
            FlopActions = new List<ActionData>();
            TurnActions = new List<ActionData>();
            RiverActions = new List<ActionData>();
            TableCards = new List<CardData>();

            UserVaraibles = new Dictionary<string, string>();
        }
    }

    public class HandDataSuprema : HandData
    {
        public List<SeatDataSuprema> Players { get; set; }

       
        public HandDataSuprema() { }
        
           
    }

} // namespace
 




