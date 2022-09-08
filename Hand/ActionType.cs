using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.OrLog.Models.Hand
{
    public enum ActionType
    {
        Undefined,
        DidNothing,
        PostsSmallBlind,
        PostsBigBlind,
        PostsBigBlindAhead,
        PostsAnte,
        PostsDeadBlind,
        PostsStraddle,
        Folds,
        Checks,
        Calls,
        CallAllIn,
        Bets,
        BetAllIn,
        Raises,
        RaiseAllIn,
        AllIn,        
        SitOut,
        SitIn
    }
    public static class ActionDataHub {
        

        //get ActionType by "role" field in message
        public static readonly Dictionary<string, ActionType> actionTypeDict = new Dictionary<string, ActionType>
        {
            { "0" , ActionType.SitIn}, // DidNothing
            { "2" , ActionType.SitIn}, // начинает улцу?
            { "3" , ActionType.Checks}, // 
            { "4" , ActionType.Calls},  // колл на рейз. прифлоп (стредл)
            { "10" , ActionType.Checks}, // чек после колла на прифлопе
            { "12" , ActionType.Checks},
            { "13" , ActionType.Checks}, // крайний чек в руке. ривер
            { "20" , ActionType.RaiseAllIn},
            { "22" , ActionType.BetAllIn},
            { "23" , ActionType.BetAllIn}, // олл ин после того как все чекнули/ первая ставка на кругу
            { "30" , ActionType.Calls}, // колл бб на прифлопе (так же кол на рейз прифлоп)
            { "32" , ActionType.Calls}, // колл на рейз прифлопе
            { "33" , ActionType.Calls}, //на терне полсе бета предыдущего аппонента.
            { "34" , ActionType.Raises}, // рирейз (стредл?)
            { "40" , ActionType.Folds}, // фолд на прифлопе
            { "42" , ActionType.Folds}, // фолд на флопе
            { "43" , ActionType.Folds}, // фол на флопе
            { "50" , ActionType.Raises},
            { "52" , ActionType.Raises}, // ререйз на прифлопе
            { "53" , ActionType.Raises},
            { "54" , ActionType.Raises}, // стредл ?
            { "60", ActionType.PostsBigBlindAhead }, // досрочно сел за стол, поставил бб
            { "63" , ActionType.PostsBigBlind},
            { "70" , ActionType.Bets}, //  наверное бетсмотреть 3лог
            { "72" , ActionType.Bets},
            { "73" , ActionType.Bets},
            { "82",  ActionType.PostsSmallBlind},
            { "93",  ActionType.PostsBigBlind }, // BB ?
            { "100" , ActionType.SitOut},
            { "108" , ActionType.Undefined}, // наверное сидит в ауте

        };

    }



}
