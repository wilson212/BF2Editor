using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting;

namespace BF2ScriptingEngine
{
    public class ParseException : Exception, ISerializable
    {
        public Token Token { get; internal set; }

        public ParseException() : base()
        {
            // Add implementation.
        }

        public ParseException(string message, Token token = null) 
            : base(message)
        {
            // Add implementation.
            Token = token;
        }

        public ParseException(string message, Token token = null, Exception inner = null) 
            : base(message, inner)
        {
            // Add implementation.
            Token = token;
        }

        // This constructor is needed for serialization.
        protected ParseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Add implementation.
        }
    }
}
