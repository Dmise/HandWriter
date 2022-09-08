using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandDataCollector.Models
{
    internal enum MessageType
    {
        Undefined,
        Gamestart,
        Moveturn,
        Prompt,
        Countdown,
        Gameover,
        Notify,
        InitInfo
    }

    internal static class StrValues
    { 
        static internal readonly string HeaderWasCreated = "header"; 

    }
    public class DataToLog : EventArgs
    {
        public string? message;
        public List<string> messagesList { get; set; } = new List<string>();
    }



    // ---Сомнительные элементы---
    struct RegExpressions
    {
        public string seqno = @"""seqno""[^\w]*(\d*)"; // "seqno"[^\w]*(\d*)
        public string eventReg = @"""event""[^\w]*(?<="")(\w*)(?="")"; // "event"[^\w]* (?<=")(\w*)(?=")

        public RegExpressions()
        {
        }
    }


}
