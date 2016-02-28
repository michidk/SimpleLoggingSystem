using System;

namespace SimpleLoggingSystem
{
    public struct LogEntry
    {

        /// <summary>
        /// The content of this log entry.
        /// In most cases this is the message of this log entry.
        /// </summary>
        public string Content { get; private set; }
        /// <summary>
        /// The log level of this log entry.
        /// </summary>
        public LogLevel Level { get; private set; }

        /// <summary>
        /// The class which created the log entry.
        /// </summary>
        public string Class { get; private set; }
        /// <summary>
        /// The method which created the log entry.
        /// </summary>
        public string Method { get; private set; }
        /// <summary>
        /// The line in which the log entry was created.
        /// </summary>
        public int LineNumber { get; private set; }

        /// <summary>
        /// The time of creation of this log entry.
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Creates a new log entry.
        /// </summary>
        /// <param name="message">
        /// The log message of this log entry.
        /// </param>
        /// <param name="level">
        /// The log level of this log entry.
        /// </param>
        /// <param name="clazz">
        /// Filled by the compiler. Ignore this field.
        /// </param>
        /// <param name="method">
        /// Filled by the compiler. Ignore this field.
        /// </param>
        /// <param name="lineNumber">
        /// Filled by the compiler. Ignore this field.
        /// </param>
        public LogEntry(string message, LogLevel level = LogLevel.Info, string clazz = "", string method = "", int lineNumber = 0)
        {
            Content = message;
            Level = level;

            Class = clazz;
            Method = method;
            LineNumber = lineNumber;

            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Returns the string which is later logged to the console.
        /// This string doesnt't contain as much information as the string which is logged to the log-file.
        /// </summary>
        /// <param name="showExecutionPoint">
        /// Wether the class, the method and the line of execution should be included in this string or not.
        /// </param>
        /// <returns>
        /// A string which contains the most important informations about this log entry.
        /// </returns>
        public string ToConsoleString(bool showExecutionPoint = true)
        {
            String execPoint = $"{Class}.{Method}:{LineNumber} ";
            return $"{Timestamp.ToShortTimeString()} ({Level}) {(showExecutionPoint ? execPoint : "")}{Content}";
        }

        /// <summary>
        /// Returns the string which is later logged to the log file.
        /// This string contains detailed informations about the log entry.
        /// </summary>
        /// <returns>
        /// A string which contains detailed informations about this log entry.
        /// </returns>
        public override string ToString()
        {
            return $"{Timestamp.ToLongTimeString()} {Timestamp.ToShortDateString()} ({Level}): {Class}.{Method}:{LineNumber} {Content}";
        }
    }
}