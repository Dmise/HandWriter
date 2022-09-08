using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandDataCollector.Exceptions
{
    internal class LogFileException : Exception
    {
        public LogFileException() { }
        internal LogFileException(string message) : base(message)
        {

        }
        public LogFileException(string message, Exception inner)
        : base(message, inner) { }

    }

    internal class UserIsNotFound : Exception
    {
        public UserIsNotFound() { }

        internal UserIsNotFound(string message) : base(message) { }

        public UserIsNotFound (string message, Exception inner) :
            base(message, inner) { }
    }
}
