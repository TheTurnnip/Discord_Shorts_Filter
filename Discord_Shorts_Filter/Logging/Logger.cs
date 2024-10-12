
namespace Discord_Shorts_Filter.Logging
{
    public sealed class Logger
    {

        private static readonly string _currentDateTime = DateTime.Now.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss");

        public static void Critical(string message)
        {
            Console.WriteLine($"CRITICAL || {_currentDateTime} || Message: {message}");
        }

        public static void Error(string message)
        {
            Console.WriteLine($"ERROR || {_currentDateTime} || Message: {message}");
        }

        public static void Info(string message)
        {
            Console.WriteLine($"INFO || {_currentDateTime} || Message: {message}");

        }

        public static void Warn(string message)
        {
            Console.WriteLine($"WARN || {_currentDateTime} || Message: {message}");
        }

        public static void Debug(string message) 
        {
            Console.WriteLine($"DEBUG || {_currentDateTime} || Message: {message}");
        }

    }
}
