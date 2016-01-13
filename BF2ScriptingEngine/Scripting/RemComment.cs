using System;
using BF2ScriptingEngine.Scripting;

namespace BF2ScriptingEngine
{
    public class RemComment : ConFileEntry
    {
        public bool IsRemBlock;

        /// <summary>
        /// The position in the AiFile this rem comment is located
        /// </summary>
        public int Position;

        /// <summary>
        /// The comment text
        /// </summary>
        public string Value;

        public bool HasValue
        {
            get { return !String.IsNullOrWhiteSpace(Value); }
        }

        public RemComment(Token token)
        {
            base.Token = token;
        }

        public void AppendLine(string line)
        {
            if (String.IsNullOrWhiteSpace(Value))
                Value = line;
            else
                Value = String.Concat(Value, Environment.NewLine, line);
        }

        public override string ToFileFormat(Token token = null)
        {
            return Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
