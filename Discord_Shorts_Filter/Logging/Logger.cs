namespace Discord_Shorts_Filter.Logging;

public sealed class Logger
{
    private static Dictionary<string, Logger> Loggers { get; set; }

    private string LoggerName { get; set; }
    private LogLevel LogLevel { get; set; }
    
    private Logger(string loggerName, LogLevel logLevel = LogLevel.Info) 
    {
        LoggerName = loggerName;
        LogLevel = logLevel;
    }

    public static Logger GetLogger(string loggerName)
    {
        if (!Loggers.ContainsKey(loggerName))
        {
            Loggers.Add(loggerName, new Logger(loggerName));
        }
        return Loggers[loggerName];
    }

    public static Logger GetLogger(string loggerName, LogLevel logLevel)
    {
        if (!Loggers.ContainsKey(loggerName))
        {
            Loggers.Add(loggerName, new Logger(loggerName, logLevel));
        }
        return Loggers[loggerName];
    }

    public void Debug(string message)
    {
        if (LogLevel <= LogLevel.Debug)
        {
            string formattedMessage = $"DEBUG || {LoggerName} || {GetDateTime()}: {message}";
            Console.WriteLine(formattedMessage);
        }
    }

    public void Info(string message)
    {
        if (LogLevel <= LogLevel.Info)
        {
            string formattedMessage = $"INFO || {LoggerName} || {GetDateTime()}: {message}";
            Console.WriteLine(formattedMessage);
        }
    }

    public void Warn(string message)
    {
        if (LogLevel <= LogLevel.Warning)
        {
            string formattedMessage = $"WARNING || {LoggerName} || {GetDateTime()}: {message}";
            Console.WriteLine(formattedMessage);
        }
    }

    public void Error(string message)
    {
        if (LogLevel <= LogLevel.Error)
        {
            string formattedMessage = $"ERROR || {LoggerName} || {GetDateTime()}: {message}";
            Console.WriteLine(formattedMessage);
        }
    }

    public void Critical(string message)
    {
        if (LogLevel <= LogLevel.Critical)
        {
            string formattedMessage = $"CRITICAL || {LoggerName} || {GetDateTime()}: {message}";
            Console.WriteLine(formattedMessage);
        }
    }

    private string GetDateTime()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}