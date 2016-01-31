using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine
{
    /// <summary>
    /// A class used to get messages from the ScriptEngine
    /// </summary>
    /// <remarks>
    ///  Info => Generic status messages, including updates
    ///  Warnings => Un-parsable objects and Non-game breaking errors
    ///  Errors => Game breaking errors (will most likely cause BF2 to crash)
    /// </remarks>
    public static class Logger
    {
        /// <summary>
        /// Gets or Sets whether the Logger is enabled to log messages
        /// </summary>
        public static bool Enabled = true;

        /// <summary>
        /// A list of messages that the Logger has produced
        /// </summary>
        public static List<LogEntry> Messages = new List<LogEntry>();

        /// <summary>
        /// A list of warning messages that the Logger has produced
        /// </summary>
        public static List<LogEntry> Warnings = new List<LogEntry>();

        /// <summary>
        /// A list of error messages that the Logger has produced
        /// </summary>
        public static List<LogEntry> Errors = new List<LogEntry>();

        /// <summary>
        /// A list of messages that the Logger has produced
        /// </summary>
        public static event EventHandler<LogEntry> OnMessageLogged;

        private static Object _syncObj = new Object();

        /// <summary>
        /// Stores an INFO message to be logged
        /// </summary>
        /// <param name="Message">The message text</param>
        /// <param name="WorkingFile">The ConFile being processed for this message</param>
        /// <param name="LineNumber">The ConFile line number being processed for this message</param>
        public static void Info(string Message, ConFile WorkingFile = null, int LineNumber = 0)
        {
            if (Enabled)
            {
                LogEntry entry = new LogEntry()
                {
                    Type = LogEntryType.Info,
                    Message = Message,
                    File = WorkingFile,
                    Line = LineNumber
                };

                lock (_syncObj)
                {
                    Messages.Add(entry);

                    if (OnMessageLogged != null)
                        OnMessageLogged(null, entry);
                }
            }
        }

        /// <summary>
        /// Stores an WARNING message to be logged
        /// </summary>
        /// <param name="Message">The message text</param>
        /// <param name="WorkingFile">The ConFile being processed for this message</param>
        /// <param name="LineNumber">The ConFile line number being processed for this message</param>
        public static void Warning(string Message, ConFile WorkingFile = null, int LineNumber = 0)
        {
            if (Enabled)
            {
                LogEntry entry = new LogEntry()
                {
                    Type = LogEntryType.Warning,
                    Message = Message,
                    File = WorkingFile,
                    Line = LineNumber
                };

                lock (_syncObj)
                {
                    Messages.Add(entry);
                    Warnings.Add(entry);

                    if (OnMessageLogged != null)
                        OnMessageLogged(null, entry);
                }
            }
        }

        /// <summary>
        /// Stores an ERROR message to be logged
        /// </summary>
        /// <param name="error">The message text</param>
        /// <param name="file">The ConFile being processed for this message</param>
        /// <param name="line">The ConFile line number being processed for this message</param>
        public static void Error(string error, ConFile file = null, int line = 0, Exception exception = null)
        {
            if (Enabled)
            {
                LogEntry entry = new LogEntry()
                {
                    Type = LogEntryType.Error,
                    Message = error,
                    File = file,
                    Line = line,
                    ExceptionObj = exception
                };

                lock (_syncObj)
                {
                    Messages.Add(entry);
                    Errors.Add(entry);

                    if (OnMessageLogged != null)
                        OnMessageLogged(null, entry);
                }
            }
        }
    }
}
