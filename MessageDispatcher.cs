using System;
using HandDataCollector.Models;
using System.Text.Json.Nodes;
using LogWork;

namespace HandDataCollector
{

    // Класс, который распределяет сообщения из сплошного потока
    // Создает событие на запиись сообщений в лог
    static class MessageDispatcher
    {

        static internal MessageFactory messageFactory;
        static internal LogWriter logWriter = new LogWriter(option: LogWriter.option.EVENTLOG);
        public static event Action<bool>? EndOfSession;

        internal static void RunDebugTest(bool liveReading = true)
        {

            

            messageFactory = new MessageFactory("Suprema");
            //чтение сообщений из файла лога, записанным снифером
            if (liveReading == false)
            {
                while (!DataHub.isMessageStackEmpty)
                {
                    
                        JsonNode? jsonMessage = JsonNode.Parse(DataHub.PopFirstMessage());
                        DispatchMessage(jsonMessage);
                   
                }
                Console.WriteLine("Test data has done");
                Console.ReadKey();
            }
            //чтение сообщение в реальном врмени со снифера
            else
            {

            }
        }
       
        internal static void ProcessMessage(string message)
        {
            JsonNode? jsonMessage = null;
            if (message != String.Empty)
            {
                //JsonNode? jsonMessage = JsonNode.Parse(DataHub.PopFirstMessage());
                try
                {
                    jsonMessage = JsonNode.Parse(message);
                }
                catch(Exception ex)
                {                  
                    //едем дальше
                }
                finally
                {
                    if(jsonMessage != null)
                        DispatchMessage(jsonMessage);
                }


            }
        }

       
        static void DispatchMessage(JsonNode? message)
        {
            //DoesWeMissNewhand(message);
            var mesType = MessageReader.GetMessageType(message);
            switch (mesType)
            {
                case MessageType.Gamestart:
                    {
                        HandlerGameStartMessage.Process(message);                        
                        break;
                    }
                case MessageType.Moveturn:
                    {
                        HandlerMoveturnMessage.Process(message);
                        break;
                    }
                case MessageType.Gameover:
                    {
                        HandlerGameOverMessage.Process(message);
                        break;
                    }
                case MessageType.Notify:
                    {
                        
                        HandlerNotifyMessage.Process(message);
                        break;
                    }
                case MessageType.Prompt:
                    HandlerPromptMessage.Process(message);
                    break;
                case MessageType.InitInfo:
                    {
                        break;
                    }
                default:
                    break;
            }
        }

        static void DoesWeMissNewhand(JsonNode message)
        {
            DataHub.messageCounter++;
            if (DataHub.messageCounter > 5)
            {
                HandWriter.CheckNewhand(message);
                DataHub.messageCounter = 0;
            }
        }              

    }
}