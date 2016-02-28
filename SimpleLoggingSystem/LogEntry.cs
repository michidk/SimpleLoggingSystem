using System;

namespace SimpleLoggingSystem
{
    public struct LogEntry
    {
        public LogType Type { get; private set; }
        public string Content { get; private set; }

        public string Class { get; private set; }
        public string Method { get; private set; }
        public int LineNumber { get; private set; }

        public DateTime Timestamp { get; private set; }

        public LogEntry(string message, LogType type = LogType.Info, string clazz = "", string method = "", int lineNumber = 0)
        {
            Content = message;
            Type = type;

            Class = clazz;
            Method = method;
            LineNumber = lineNumber;

            Timestamp = DateTime.Now;
        }

        // pretty string
        public string ToConsoleString(bool showExecutionPoint = true)
        {
            String execPoint = $"{Class}.{Method}:{LineNumber} ";
            return $"{Timestamp.ToShortTimeString()} ({Type}) {(showExecutionPoint ? execPoint : "")}{Content}";
        }

        // etc, e.g. for log file
        public override string ToString()
        {
            return $"{Timestamp.ToLongTimeString()} {Timestamp.ToShortDateString()} ({Type}): {Class}.{Method}:{LineNumber} {Content}";
        }
    }
}