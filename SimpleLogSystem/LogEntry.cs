using System;

namespace SimpleLogSystem
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogType Type { get; set; }
        public string Content { get; set; }

        public LogEntry(string message, LogType type = LogType.Info)
        {
            Timestamp = DateTime.Now;
            Type = type;
            Content = message;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}): {2}", Timestamp.ToShortTimeString(), Type, Content);
        }
    }
}