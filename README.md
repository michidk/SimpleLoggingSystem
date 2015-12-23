# SimpleLoggingSystem
a simple logging-system in csharp

## Usage
### General
First of all you have to create a new Logger instance:
```cs
  public static Logger Logger = new Logger("log.txt", true, logLevel);
```
You just need one in your whole project.
'log.txt' is the file, where all (whatever you log level is) your logs are written to (with time and date).
If you want to disable file logging, then just set it to 'null'.
The second parameter (boolean) defines whether the log should be written to the console or not.
The logLevel defines which actions should be logged to the console and if the event 'OnLogFiltered' is fired.

There are 5 'LogType's you can use:
```
  Development,
  Debug,
  Info,
  Warning,
  Error
```

To log something, you simply execute:
```cs
  logger.Log("my log message", "Logger Test", LogType.Debug);
```
The first parameter is your log message, the third one is the 'LogType' of this message.
The second parameter defines your module, which is displayed in front of your message. You can set it to 'null' if you dont want to use a module.

### Modules
Often you want to use modules, but you don't want to define the module name for a certain module multiple times.
To avoid this, you can use the 'ModuleLog' class.
```cs
  private ModuleLog log = Globals.Logger.CreateModule("Server");
```

### Events
If you want to implement your own file or console logging then you can simply listen to the two events 'OnLog' or 'OnLogFiltered'.
For example:
```cs
  public void Init() 
  {
    logger.OnLog += OnLogToConsole;
  }
  
  private void OnLogToConsole(LogEntry entry)
  {
      Console.WriteLine(entry.ToString());
  }
```
'OnLogFiltered' is filtered by 'logLevel'.
