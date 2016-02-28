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
        public event LogEventHandler OnLog;
        public event LogEventHandler OnLogFiltered; // filtered by LogLevel

        public LogType LogLevel;

        private readonly string filePath;   // if filePath == null, then the log won't be logged to a file
        private readonly bool logToConsole;
        private readonly bool showExecutionPoint;
        private readonly int writeToFileInterval;

        private readonly Thread fileThread;
        private readonly ConcurrentQueue<LogEntry> fileQueue = new ConcurrentQueue<LogEntry>();

        public Logger(string filePath, bool logToConsole = true, LogType logLevel = LogType.Info, bool showExecutionPoint = true, int writeToFileInterval = 200)
        {
            this.filePath = filePath;
            this.logToConsole = logToConsole;
            this.LogLevel = logLevel;
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

        public void Log(string message, LogType type = LogType.Info, 
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {

            String clazz = Path.GetFileNameWithoutExtension(sourceFilePath);
            
            var entry = new LogEntry(message, type, clazz, memberName, sourceLineNumber);

            OnLog?.Invoke(entry);
            if (type >= LogLevel)
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
            switch (entry.Type)
            {
                case LogType.Ann:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogType.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.Error:
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
                            Log("Couldn't dequeue log entry", LogType.Error);
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
    }
}