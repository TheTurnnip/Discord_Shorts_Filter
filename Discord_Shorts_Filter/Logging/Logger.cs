namespace Discord_Shorts_Filter.Logging
{
    /// <summary>
    /// Contains static methods to log to the console.
    /// </summary>
    public sealed class Logger
    {
        private static string GetCurrentDateTime()
        {
            return DateTime.Now.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss");
        }

        public static void Critical(string message)
        {
            Console.WriteLine($"CRITICAL || {GetCurrentDateTime()} || Message: {message}");
        }

        public static void Error(string message)
        {
            Console.WriteLine($"ERROR || {GetCurrentDateTime()} || Message: {message}");
        }

        public static void Info(string message)
        {
            Console.WriteLine($"INFO || {GetCurrentDateTime()} || Message: {message}");

        }

        public static void Warn(string message)
        {
            Console.WriteLine($"WARN || {GetCurrentDateTime()} || Message: {message}");
        }

        public static void Debug(string message) 
        {
            Console.WriteLine($"DEBUG || {GetCurrentDateTime()} || Message: {message}");
        }

    }
}
