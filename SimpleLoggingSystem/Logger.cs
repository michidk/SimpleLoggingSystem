using System.Collections.Generic;
using System.IO;

namespace SimpleLoggingSystem
{
    public delegate void LogEventHandler(LogEntry entry);

    public class Logger
    {
        public event LogEventHandler OnLog;
        public readonly List<LogEntry> log = new List<LogEntry>();

        private readonly string filePath;

        public Logger(string filePath)
        {
            this.filePath = filePath;

            if (filePath != null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            OnLog += LogToFile;
        }

        public void Log(string message, LogType type = LogType.Info)
        {
            var entry = new LogEntry(message, type);
            log.Add(entry);

            OnLog?.Invoke(entry);
        }

        private void LogToFile(LogEntry entry)
        {
            using (var writer = File.AppendText(filePath))
            {
                writer.WriteLineAsync(entry.ToString());
            }
        }
    }
}