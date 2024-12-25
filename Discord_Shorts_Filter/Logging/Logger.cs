namespace Discord_Shorts_Filter.Logging
{
    /// <summary>
    /// Contains static methods to log to the console.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Gets the datetime when the method is called.
        /// </summary>
        /// <returns>The current datetime.</returns>
        private static string GetCurrentDateTime()
        {
            return DateTime.Now.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss");
        }

        /// <summary>
        /// Logs a message with the Citical level
        /// </summary>
        /// <param name="message">The log message.</param>
        public static void Critical(string message)
        {
            Console.WriteLine($"CRITICAL || {GetCurrentDateTime()} || Message: {message}");
        }

        /// <summary>
        /// Logs a message with the Error level.
        /// </summary>
        /// <param name="message">The log message.</param>
        public static void Error(string message)
        {
            Console.WriteLine($"ERROR || {GetCurrentDateTime()} || Message: {message}");
        }

        /// <summary>
        /// Logs a message with the Info level.
        /// </summary>
        /// <param name="message">The log message.</param>
        public static void Info(string message)
        {
            Console.WriteLine($"INFO || {GetCurrentDateTime()} || Message: {message}");

        }

        /// <summary>
        /// Logs a message with the Warn level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Warn(string message)
        {
            Console.WriteLine($"WARN || {GetCurrentDateTime()} || Message: {message}");
        }

        /// <summary>
        /// Logs a message with the Debug level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Debug(string message) 
        {
            Console.WriteLine($"DEBUG || {GetCurrentDateTime()} || Message: {message}");
        }

    }
}
