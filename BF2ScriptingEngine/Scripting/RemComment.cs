using System;

namespace BF2ScriptingEngine
{
    public class RemComment
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

        public void AppendLine(string line)
        {
            if (String.IsNullOrWhiteSpace(Value))
                Value = line;
            else
                Value = String.Concat(Value, Environment.NewLine, line);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
