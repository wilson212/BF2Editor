using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine
{
    public class LogEntry
    {
        public LogEntryType Type { get; set; }

        public string Message { get; set; } = "";

        public ConFile File { get; set; }

        public int Line { get; set; } = 0;

        public Exception ExceptionObj { get; set; }
    }

    public enum LogEntryType
    {
        Info, Warning, Error
    }
}
