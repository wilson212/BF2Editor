using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine
{
    public class LogEntry
    {
        public LogEntryType Type;

        public string Message = "";

        public ConFile File;

        public int Line;
    }

    public enum LogEntryType
    {
        Info, Warning, Error
    }
}
