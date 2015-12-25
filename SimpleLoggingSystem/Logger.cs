using System;
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
        private const int writeToFileInterval = 200;

        public event LogEventHandler OnLog;
        public event LogEventHandler OnLogFiltered; // filtered by LogLevel

        public LogType LogLevel;

        private readonly string filePath;   // if filePath == null, then the log won't be logged to a file
        private readonly bool logToConsole;

        private readonly List<ModuleLog> modules = new List<ModuleLog>();
        private readonly Thread fileThread;
        private readonly Queue<LogEntry> fileQueue = new Queue<LogEntry>();
        
        public Logger(string filePath, bool logToConsole = true, LogType logLevel = LogType.Info)
        {
            this.filePath = filePath;
            this.logToConsole = logToConsole;
            this.LogLevel = logLevel;

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

        public void Log(string message, string module = null, LogType type = LogType.Info)
        {
            var entry = new LogEntry(message, module, type);

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

            Console.WriteLine(entry.ToConsoleString());

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
                        sb.AppendLine(fileQueue.Dequeue().ToString());
                    }
                    using (var writer = File.AppendText(filePath))
                    {
                        writer.WriteAsync(sb.ToString());
                    }
                }
                Thread.Sleep(writeToFileInterval);
            }
        }

        public ModuleLog CreateModule(string name)
        {
            var module = new ModuleLog(name, this);
            modules.Add(module);
            return module;
        }
    }
}