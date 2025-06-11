using System;

namespace LabFramework.Core.Logging
{
    /// <summary>
    /// Log levels for the framework
    /// </summary>
    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
        Critical = 5,
        None = 6
    }
    
    /// <summary>
    /// Logging service interface
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// Log a message with the specified level
        /// </summary>
        /// <param name="level">Log level</param>
        /// <param name="message">Message to log</param>
        /// <param name="exception">Optional exception</param>
        void Log(LogLevel level, string message, Exception exception = null);
        
        /// <summary>
        /// Log a trace message
        /// </summary>
        /// <param name="message">Message to log</param>
        void LogTrace(string message);
        
        /// <summary>
        /// Log a debug message
        /// </summary>
        /// <param name="message">Message to log</param>
        void LogDebug(string message);
        
        /// <summary>
        /// Log an information message
        /// </summary>
        /// <param name="message">Message to log</param>
        void LogInformation(string message);
        
        /// <summary>
        /// Log a warning message
        /// </summary>
        /// <param name="message">Message to log</param>
        void LogWarning(string message);
        
        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="exception">Optional exception</param>
        void LogError(string message, Exception exception = null);
        
        /// <summary>
        /// Log a critical message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="exception">Optional exception</param>
        void LogCritical(string message, Exception exception = null);
    }
    
    /// <summary>
    /// Console-based logging service implementation
    /// </summary>
    public class ConsoleLoggingService : ILoggingService
    {
        private readonly LogLevel _minimumLevel;
        
        public ConsoleLoggingService(LogLevel minimumLevel = LogLevel.Information)
        {
            _minimumLevel = minimumLevel;
        }
        
        public void Log(LogLevel level, string message, Exception exception = null)
        {
            if (level < _minimumLevel)
                return;
                
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var levelString = level.ToString().ToUpper();
            
            Console.ForegroundColor = GetColorForLevel(level);
            Console.WriteLine($"[{timestamp}] [{levelString}] {message}");
            
            if (exception != null)
            {
                Console.WriteLine($"Exception: {exception}");
            }
            
            Console.ResetColor();
        }
        
        public void LogTrace(string message) => Log(LogLevel.Trace, message);
        public void LogDebug(string message) => Log(LogLevel.Debug, message);
        public void LogInformation(string message) => Log(LogLevel.Information, message);
        public void LogWarning(string message) => Log(LogLevel.Warning, message);
        public void LogError(string message, Exception exception = null) => Log(LogLevel.Error, message, exception);
        public void LogCritical(string message, Exception exception = null) => Log(LogLevel.Critical, message, exception);
        
        private static ConsoleColor GetColorForLevel(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => ConsoleColor.Gray,
                LogLevel.Debug => ConsoleColor.White,
                LogLevel.Information => ConsoleColor.Green,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Critical => ConsoleColor.DarkRed,
                _ => ConsoleColor.White
            };
        }
    }
}

