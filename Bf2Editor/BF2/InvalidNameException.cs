using System;

namespace BF2Editor
{
    class InvalidNameException : Exception
    {
        public InvalidNameException() : base() { }

        public InvalidNameException(string Message) : base(Message)  { }
    }
}
