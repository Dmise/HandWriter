using System.Reflection;
using HandDataCollector.Exceptions;
using HandDataCollector.Models;



namespace HandDataCollector
{
    internal enum MessageToLog
    {
        HandHeader,
        SeatsInit,
        Action,
        Dealt,
        Flop,
        Turn,
        River,
        ShowDown,
        Summary,
        Result

    }
    public class LogWriter
    {
        private object logWriterLock = new object();
        static private string logFileDir = string.Empty;
        string logFileName = "Log.txt";
        string pathToFile;
        public enum option
        {
            DEFAULT,
            EVENTLOG
        }
        
        public LogWriter(string logDir = "default", string logName = "default", option option = option.DEFAULT)
        {
            InitLogFile(logDir, logName);
            

            if (option == option.EVENTLOG)
            {
                //HandWriter.DataToLogCreated += WriteMessageByEvent; HERE
                //HandlerGameStartMessage.MessageProcessed += WriteLogByEventAsync;
                //HandlerMoveturnMessage.MessageProcessed += WriteLogByEventAsync;
                //HandlerGameOverMessage.MessageProcessed += WriteLogByEventAsync;
            }

        }

        public void WriteErrorMessage(string message)
        {
            WriteMessageAsync("\t\n[ERROR]");
            WriteMessageAsync(message+"\t\n");
        }

        public async Task WriteDataAsync(DataToLog data) 
        {
            await Task.Run(() =>
            {
                if (data.messagesList.Count > 0)
                {
                    WriteMessage(data.messagesList);
                }
                else if(data.message != null)
                {
                    WriteMessage(data.message);
                }
            });
            
        }

        public void WriteData(DataToLog data)
        {           
                if (data.messagesList.Count > 0)
                {
                    WriteMessage(data.messagesList);
                }
                else if (data.message != null)
                {
                    WriteMessage(data.message);
                }
        }

        public void Run()
        {
            //  читаем сообщения снизу стека, 
            while (true) // DataHub.handDataList.Count != -1
            {
                if (DataHub.isStackEmpty())
                {
                    Thread.Sleep(50);
                    continue;
                }
                MessageDispatcher.ProcessMessage(DataHub.PopFirstMessage());
            }
            //Console.ReadLine();
        }
        void SaveOldLog(string logFileName)
        {
            lock (logWriterLock) {
                if (File.Exists(Path.Combine(logFileDir, logFileName)))
                {

                    string iterationName = logFileName;
                    var fileName = logFileName.Split('.');
                    int counter = 1;
                    while (File.Exists(Path.Combine(logFileDir, iterationName)))
                    {
                        iterationName = fileName[0] + Convert.ToString(counter++) + '.' + fileName[1];
                    }
                    File.Move(logFileName, iterationName);
                }
            }

        }
        private void InitLogFile(string logDirectory = "default", string logFile = "default")
        {
            if (logFile != "default")
                logFileName = logFile;
            if (logDirectory == "default")
            {
                logFileDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                //logFileDir = Assembly.GetExecutingAssembly().Location;
            }
            else
            {
                logFileDir = logDirectory;
            }
            SaveOldLog(logFileName);
            pathToFile = Path.Combine(logFileDir, logFileName);
            //File.Create(pathToFile);
            
        }

        //public void async WriteMessageAsync    

        public async Task WriteMessageAsync(List<string> logMessage)
        {
            await Task.Run(() => WriteMessage(logMessage));
            

        }
        public async void WriteMessageAsync(string logMessage)            
        {
            await Task.Run(() => WriteMessage(logMessage));
        }


        public Task WriteMessage(List<string> messageList)
        {



            if (logFileDir == string.Empty)
            {
                throw new LogFileException("Директория лог-файла не определена. ");
            }
            try
            {
                lock (logWriterLock)
                {
                    using (StreamWriter sw = new StreamWriter(pathToFile, true))
                    {
                        foreach (var message in messageList)
                        {
                            sw.WriteLine(message);
                        }
                    }
                    return Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }

        }
        public Task WriteMessage(string logMessage)
        {

            if (logFileDir == string.Empty)
            {

                throw new LogFileException("Директория лог-файла не определена. ");
            }
            try
            {
                lock (logWriterLock)
                {
                    using (StreamWriter sw = new StreamWriter(pathToFile, true))
                    {
                        sw.WriteLine(logMessage);
                    }
                    return Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                
            }

        }

    }
}
