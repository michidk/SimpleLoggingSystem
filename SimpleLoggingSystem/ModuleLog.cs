namespace SimpleLoggingSystem
{
    public class ModuleLog
    {
        public string Name { get; private set; }

        private Logger logger;

        internal ModuleLog(string name, Logger logger)
        {
            this.Name = name;
            this.logger = logger;
        }

        public void Log(string message, LogType type = LogType.Info)
        {
            logger.Log(message, Name, type);
        }
    }
}