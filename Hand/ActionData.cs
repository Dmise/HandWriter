using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandDataCollector;

namespace Engine.OrLog.Models.Hand
{
    public class ActionData
    {
        public ActionType Role { get; set; }
        public int PlayerIndex { get; set; } = -1; // сидушка за столом
        public string? PlayerName { get; set; } 
        public decimal Bet { get; set; } // на какую величину увеличиватся ставка 
        public decimal ToBet { get; set; } // до какой величины бет (что по факту лежит перед игроком)
    }

    public class ActionDataSuprema : ActionData
    {
        public Round Round { get; set; } = Round.Undefined;

        public int PlayersInPlay { get; set; } = 0;
        public ActionDataSuprema() { }
        public ActionDataSuprema(Round round)
        {
            Round = round;
        }
        
    }
}
