using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace HandDataCollector
{
    static internal class FileReader
    {
        public static void ReadFileToMessageStack()
        {
            try
            {
                using (StreamReader sr = new StreamReader(DataHub.sessionLogFile))
                {
                    while (sr.Peek() >= 0)
                    {
                        var matchMessage = DataHub.regJsonMessage.Match(sr.ReadLine());
                        if (matchMessage.Success)
                        {
                            DataHub.PushMessage(matchMessage.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

