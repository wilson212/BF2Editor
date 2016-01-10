using System;

namespace BF2Editor.Logging
{
    public class LogMessage
    {
        /// <summary>
        /// The string of this message
        /// </summary>
        public string Message;

        /// <summary>
        /// The local DateTime when this message was created
        /// </summary>
        public DateTime LogTime { get; protected set; }

        public LogMessage(string Message)
        {
            this.Message = Message;
            this.LogTime = DateTime.Now;
        }

        public LogMessage(string Message, params object[] Items)
        {
            this.Message = String.Format(Message, Items);
            this.LogTime = DateTime.Now;
        }
    }
}
