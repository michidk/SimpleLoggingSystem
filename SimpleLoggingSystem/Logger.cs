using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using System.Threading;

namespace SimpleLoggingSystem
{
    public delegate void LogEventHandler(LogEntry entry);

    public class Logger
    {
        /// <summary>
        /// This event is called, when a log entry was created, regardless of the LogFilter level.
        /// </summary>
        public event LogEventHandler OnLog;
        /// <summary>
        /// This event is called, when a log entry was created whose logLevel is higher than the LogFilter level.
        /// </summary>
        public event LogEventHandler OnLogFiltered;

        /// <summary>
        /// the current log level.
        /// If a log-entries logLevel is lower than the logFilter-level, which was set in the constructor, 
        /// OnLog will be called but not OnLogFiltered.
        /// This also means that the entry wont be shown in the console, but will be logged to the file. (If file-logging is enabled)
        /// </summary>
        public LogLevel LogFilter;

        private readonly string filePath;
        private readonly bool logToConsole;
        private readonly bool showExecutionPoint;
        private readonly int writeToFileInterval;

        private readonly Thread fileThread;
        private readonly ConcurrentQueue<LogEntry> fileQueue = new ConcurrentQueue<LogEntry>();

        /// <summary>
        /// Creates a new Logger.
        /// The main class of the logging system.
        /// </summary>
        /// <param name="filePath">
        /// The path of the log file. To disable logging to a file, set it to null.
        /// </param>
        /// <param name="logToConsole">
        /// Set it to false, to disable logging with the Console class.
        /// </param>
        /// <param name="logFilter">
        /// All log entries with a log level whose are less than logFilter, wont be shown in the console.
        /// </param>
        /// <param name="showExecutionPoint">
        /// If the class, method and line of execution should be shown in the console.
        /// </param>
        /// <param name="writeToFileInterval">
        /// The write interval in milliseconds.
        /// </param>
        public Logger(string filePath, bool logToConsole = true, LogLevel logFilter = LogLevel.Info, bool showExecutionPoint = true, int writeToFileInterval = 200)
        {
            this.filePath = filePath;
            this.logToConsole = logToConsole;
            this.LogFilter = logFilter;
            this.showExecutionPoint = showExecutionPoint;
            this.writeToFileInterval = writeToFileInterval;

            if (filePath != null)
            {
                var info = new FileInfo(filePath).Directory;
                info?.Create();
                OnLog += OnLogToFile;
                fileThread = new Thread(StartFileThread);
                fileThread.IsBackground = true;
                fileThread.Start();
            }

            if (logToConsole)
                OnLogFiltered += OnLogToConsole;
        }

        /// <summary>
        /// Creates a new log entry.
        /// </summary>
        /// <param name="message">
        /// The message of this log entry.
        /// </param>
        /// <param name="level">
        /// The log level describes the importance of an log entry.
        /// If its lower than the logFilter-level, which was set in the constructor, the log entry wont be shown in the console.
        /// </param>
        /// <param name="memberName">
        /// Filled by the compiler. Ignore this field.
        /// </param>
        /// <param name="sourceFilePath">
        /// Filled by the compiler. Ignore this field.
        /// </param>
        /// <param name="sourceLineNumber">
        /// Filled by the compiler. Ignore this field.
        /// </param>
        public void Log(string message, LogLevel level = LogLevel.Info, 
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {

            String clazz = Path.GetFileNameWithoutExtension(sourceFilePath);
            
            var entry = new LogEntry(message, level, clazz, memberName, sourceLineNumber);

            OnLog?.Invoke(entry);
            if (level >= LogFilter)
                OnLogFiltered?.Invoke(entry);
        }

        // Log To File isn't filtered, and the detailed string is used
        private async void OnLogToFile(LogEntry entry)
        {
            fileQueue.Enqueue(entry);
        }

        // Log To Console is filtered & colored, and toConsoleString is used (but you can implement your own)
        private void OnLogToConsole(LogEntry entry)
        {
            var color = Console.ForegroundColor;
            switch (entry.Level)
            {
                case SimpleLoggingSystem.LogLevel.Ann:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case SimpleLoggingSystem.LogLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.WriteLine(entry.ToConsoleString(showExecutionPoint));

            Console.ForegroundColor = color;
        }

        private void StartFileThread()
        {
            while (true)
            {
                if (fileQueue.Count > 0)
                {
                    StringBuilder sb = new StringBuilder(fileQueue.Count);
                    while (fileQueue.Count > 0)
                    {
                        LogEntry log;
                        var success = fileQueue.TryDequeue(out log);
                        if (success)
                        {
                            sb.AppendLine(log.ToString());
                        }
                        else
                        {
                            Log("Couldn't dequeue log entry", LogLevel.Error);
                        }
                    }
                    using (var writer = File.AppendText(filePath))
                    {
                        writer.Write(sb.ToString());
                    }
                }
                Thread.Sleep(writeToFileInterval);
            }
        }

        /* TODO use this
        private static void WriteToFile(string value, string path)
        {
            using (var writer = new StreamWriter(path, true))
            using (var syncWriter = TextWriter.Synchronized(writer))
                syncWriter.WriteLine(value);
        }
        */
    }
}