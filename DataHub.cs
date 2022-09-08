using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.OrLog.Models.Hand;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;


namespace HandDataCollector
{

    internal static class DataHub
    {
        private static readonly object messageStackLock = new object();
        private static List<string> messagesStack = new List<string>();
        static public List<HandData> handDataList = new List<HandData> { };
        public static ActionDataSuprema prevActor = new ActionDataSuprema();
        public static ActionDataSuprema currentActor = new ActionDataSuprema();
        internal static event Action<string>? ErrorEvent;

        //internal static LogWriter logWriter = new LogWriter();
        public static Regex regJsonMessage = new Regex(@"(?={)(.*)");


        public static readonly string sessionLogFile = @"C:\Docs\Projects\HellYeah\Suprema\SourceCode\Test\data\log3.txt";
       
        public static int messageCounter = 0;
        public static bool isMessageStackEmpty
        {
            get
            {
                if (messagesStack.Count == 0)
                    return true;
                return false;
            }
        }

        public static void WakeError(string message)
        {
            ErrorEvent?.Invoke(message);
        }
        internal static bool isStackEmpty()
        {
            if (messagesStack.Count == 0)
                return true;
            return false;
        }



        //  ---Functions---
        
        static public void PushMessage(string str)
        {
            lock (messageStackLock) 
            {
                messagesStack.Add(str);
            }
        }
        

        static public string PopFirstMessage()
        {
            lock (messageStackLock)
            {
                if (messagesStack.Count > 0)
                {
                    var mes = messagesStack[0];
                    messagesStack.RemoveAt(0);
                    return mes;
                }
                else
                    return String.Empty;
            }
        }

        static public void PushHand(HandData hand)
        {
            handDataList.Add(hand); 
        }


    }
}
