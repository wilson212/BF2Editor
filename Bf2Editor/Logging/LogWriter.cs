using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using System.Timers;

namespace BF2Editor.Logging
{
    /// <summary>
    /// Provides an object wrapper for a file that is used to
    /// store LogMessage's into. Uses a Multi-Thread safe Queueing
    /// system, and provides full Asynchronous writing and flushing
    /// </summary>
    public class LogWriter : IDisposable
    {
        /// <summary>
        /// Our Queue of log messages to be written, Thread safe
        /// </summary>
        private ConcurrentQueue<LogMessage> LogQueue;

        /// <summary>
        /// Full path to the log file
        /// </summary>
        private FileInfo LogFile;

        /// <summary>
        /// Our Timer object for writing to the log file
        /// </summary>
        private Timer LogTimer;

        /// <summary>
        /// Our log flushing task
        /// </summary>
        private Task FlushTask;

        /// <summary>
        /// Inicates during the FlushLog() method that the app has requested
        /// the Log file be truncated before any new messages are appended
        /// </summary>
        private bool QueueTruncate = false;

        /// <summary>
        /// Indicates whether we are disposed
        /// </summary>
        private bool Disposed = false;

        /// <summary>
        /// Gets or sets whether we having logging enabled.
        /// If disabled, all invokes to Write() will be ignored
        /// </summary>
        public bool LoggingEnabled = true;

        /// <summary>
        /// Creates a new Log Writter instance
        /// </summary>
        /// <param name="FileLocation">The location of the logfile. If the file doesnt exist,
        /// It will be created.</param>
        /// <param name="Truncate">If set to true and the logfile is over XX size, it will be truncated to 0 length</param>
        /// <param name="TruncateLen">
        ///     If <paramref name="Truncate"/> is true, The size of the file must be at least this size, 
        ///     in bytes, to truncate it
        /// </param>
        public LogWriter(string FileLocation, bool Truncate = false, int TruncateLen = 2097152)
        {
            // Set internals
            LogFile = new FileInfo(FileLocation);
            LogQueue = new ConcurrentQueue<LogMessage>();

            // Test that we are able to open and write to the file
            using (FileStream stream = LogFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                // If the file is over 2MB, and we want to truncate big files
                if (Truncate && LogFile.Length > TruncateLen)
                {
                    stream.SetLength(0);
                    stream.Flush();
                }
            }

            // Create our task
            FlushTask = new Task(FlushLog);

            // Start a log timer, and auto write new logs every 3 seconds
            LogTimer = new Timer(3000);
            LogTimer.Elapsed += (s, e) =>
            {
                if (LogQueue.Count > 0 && FlushTask.Status != TaskStatus.Running)
                {
                    // Dispose old instance
                    if (FlushTask.Status != TaskStatus.Created)
                    {
                        // Create new
                        FlushTask.Dispose();
                        FlushTask = new Task(FlushLog);
                    }

                    // Start the task
                    FlushTask.Start();
                }
            };
            LogTimer.Start();
        }

        /// <summary>
        /// Adds a message to the queue, to be written to the log file
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        public void Write(string message)
        {
            // Dont write if we arent enabled
            if (!LoggingEnabled) return;

            // Push to the Queue
            LogQueue.Enqueue(new LogMessage(message));
        }

        /// <summary>
        /// Adds a message to the queue, to be written to the log file
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        public void Write(string message, params object[] items)
        {
            // Dont write if we arent enabled
            if (!LoggingEnabled) return;

            // Push to the Queue
            LogQueue.Enqueue(new LogMessage(String.Format(message, items)));
        }

        /// <summary>
        /// Queues the underlying log writer to empty the logfile before
        /// appending any more messages to it. Messages that are awaiting
        /// to be written to the log file are NOT removed from Queue 
        /// </summary>
        public async void ClearLog()
        {
            QueueTruncate = true;

            // Since we are clearing, we wait until the current
            // operation is finished
            if (FlushTask.Status == TaskStatus.Running)
                await FlushTask;
        }

        /// <summary>
        /// Flushes the Queue to the physical log file
        /// </summary>
        private async void FlushLog()
        {
            // Append messages
            using (FileStream fs = LogFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                // if the user requests a truncate, empty the file now before flushing
                if (QueueTruncate)
                {
                    fs.SetLength(0);
                    QueueTruncate = false;
                }
                else // move to end of the stream for appending
                    fs.Seek(0, SeekOrigin.End);

                // write each message on a line
                while (LogQueue.Count > 0)
                {
                    LogMessage entry;
                    if (LogQueue.TryDequeue(out entry))
                        await writer.WriteLineAsync(String.Format("[{0}]\t{1}", entry.LogTime, entry.Message));
                }
            }
        }

        public async void Dispose()
        {
            // Dont do this more then once
            if (Disposed) return;
            Disposed = true;

            // Dispose timer
            LogTimer.Stop();
            LogTimer.Dispose();

            // Flush everything
            if (LogQueue.Count > 0 && FlushTask.Status != TaskStatus.Running)
                FlushLog();
            else
                await FlushTask;

            // Dispose flusher
            FlushTask.Dispose();
        }

        /// <summary>
        /// Destructor. Make sure we flush!
        /// </summary>
        ~LogWriter()
        {
            if (!Disposed)
                Dispose();
        }
    }
}
