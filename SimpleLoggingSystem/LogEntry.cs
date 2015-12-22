using System;

namespace SimpleLoggingSystem
{
    public struct LogEntry
    {
        public string Module { get; private set; }
        public LogType Type { get; private set; }
        public string Content { get; private set; }
        public DateTime Timestamp { get; private set; }

        public LogEntry(string message, string module = null, LogType type = LogType.Info)
        {
            Content = message;
            Module = module;
            Type = type;
            Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Timestamp.ToShortTimeString()} ({Type}): {(String.IsNullOrEmpty(Module) ? "" : "[" + Module + "] ")}{Content}";
        }

        public string ToDetailedString()
        {
            return $"{Timestamp.ToLongTimeString()} {Timestamp.ToShortDateString()} ({Type}): {(String.IsNullOrEmpty(Module) ? "" : "[" + Module + "] ")}{Content}";
        }
    }
}