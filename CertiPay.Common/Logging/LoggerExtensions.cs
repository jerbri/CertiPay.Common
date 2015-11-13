namespace CertiPay.Common.Logging
{
    using System;

    /// <summary>
    /// Provide usability extensions for working with the ILog implementations
    /// </summary>
    /// <remarks>
    /// This was inspired by RavenDB's logging abstractions and extensions along with Serilog's interface
    /// </remarks>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Log a templated message at the VERBOSE/TRACE log level with properties
        /// </summary>
        /// <param name="messageTemplate">A string message that accepted templated values (either {0} {1} or {prop_1} {prop_2}) a la String.Format</param>
        /// <param name="propertyValues">The properties to replace in the message template</param>
        public static void Verbose(this ILog logger, string messageTemplate, params object[] propertyValues)
        {
            logger.Log(LogLevel.Verbose, messageTemplate, propertyValues);
        }

        /// <summary>
        /// Log a templated message at the DEBUG log level with properties
        /// </summary>
        /// <param name="messageTemplate">A string message that accepted templated values (either {0} {1} or {prop_1} {prop_2}) a la String.Format</param>
        /// <param name="propertyValues">The properties to replace in the message template</param>
        public static void Debug(this ILog logger, string messageTemplate, params object[] propertyValues)
        {
            logger.Log(LogLevel.Debug, messageTemplate, propertyValues);
        }

        /// <summary>
        /// Log a templated message at the DEBUG log level with properties
        /// </summary>
        /// <typeparam name="TException">The exception that occurred to log the stack trace for</typeparam>
        /// <param name="messageTemplate">A string message that accepted templated values (either {0} {1} or {prop_1} {prop_2}) a la String.Format</param>
        /// <param name="propertyValues">The properties to replace in the message template</param>
        public static void DebugException<TException>(this ILog logger, string messageTemplate, TException exception, params object[] propertyValues) where TException : Exception
        {
            logger.Log<TException>(LogLevel.Debug, messageTemplate, exception, propertyValues);
        }

        /// <summary>
        /// Log a templated message at the INFO log level with properties
        /// </summary>
        /// <param name="messageTemplate">A string message that accepted templated values (either {0} {1} or {prop_1} {prop_2}) a la String.Format</param>
        /// <param name="propertyValues">The properties to replace in the message template</param>
        public static void Info(this ILog logger, string messageTemplate, params object[] propertyValues)
        {
            logger.Log(LogLevel.Info, messageTemplate, propertyValues);
        }

        /// <summary>
        /// Log a templated message at the INFO log level with properties
        /// </summary>
        /// <typeparam name="TException">The exception that occurred to log the stack trace for</typeparam>
        /// <param name="messageTemplate">A string message that accepted templated values (either {0} {1} or {prop_1} {prop_2}) a la String.Format</param>
        /// <param name="propertyValues">The properties to replace in the message template</param>
        public static void InfoException<TException>(this ILog logger, string messageTemplate, TException exception, params object[] propertyValues) where TException : Exception
        {
            logger.Log<TException>(LogLevel.Info, messageTemplate, exception, propertyValues);
        }

        /// <summary>
        /// Log a templated message at the WARN log level with properties
        /// </summary>
        /// <param name="messageTemplate">A string message that accepted templated values (either {0} {1} or {prop_1} {prop_2}) a la String.Format</param>
        /// <param name="propertyValues">The properties to replace in the message template</param>
        public static void Warn(this ILog logger, string messageTemplate, params object[] propertyValues)
        {
            logger.Log(LogLevel.Warn, messageTemplate, propertyValues);
        }

        /// <summary>
        /// Log a templated message at the WARN log level with properties
        /// </summary>
        /// <typeparam name="TException">The exception that occurred to log the stack trace for</typeparam>
        /// <param name="messageTemplate">A string message that accepted templated values (either {0} {1} or {prop_1} {prop_2}) a la String.Format</param>
        /// <param name="propertyValues">The properties to replace in the message template</param>
        public static void WarnException<TException>(this ILog logger, string messageTemplate, TException exception, params object[] propertyValues) where TException : Exception
        {
            logger.Log<TException>(LogLevel.Warn, messageTemplate, exception, propertyValues);
        }

        /// <summary>
        /// Log a templated message at the ERROR log level with properties
        /// </summary>
        /// <param name="messageTemplate">A string message that accepted templated values (either {0} {1} or {prop_1} {prop_2}) a la String.Format</param>
        /// <param name="propertyValues">The properties to replace in the message template</param>
        public static void Error(this ILog logger, string messageTemplate, params object[] propertyValues)
        {
            logger.Log(LogLevel.Error, messageTemplate, propertyValues);
        }

        /// <summary>
        /// Log a templated message at the ERROR log level with properties
        /// </summary>
        /// <typeparam name="TException">The exception that occurred to log the stack trace for</typeparam>
        /// <param name="messageTemplate">A string message that accepted templated values (either {0} {1} or {prop_1} {prop_2}) a la String.Format</param>
        /// <param name="propertyValues">The properties to replace in the message template</param>
        public static void ErrorException<TException>(this ILog logger, string messageTemplate, TException exception, params object[] propertyValues) where TException : Exception
        {
            logger.Log<TException>(LogLevel.Error, messageTemplate, exception, propertyValues);
        }

        /// <summary>
        /// Log a templated message at the FATAL log level with properties
        /// </summary>
        /// <param name="messageTemplate">A string message that accepted templated values (either {0} {1} or {prop_1} {prop_2}) a la String.Format</param>
        /// <param name="propertyValues">The properties to replace in the message template</param>
        public static void Fatal(this ILog logger, string messageTemplate, params object[] propertyValues)
        {
            logger.Log(LogLevel.Fatal, messageTemplate, propertyValues);
        }

        /// <summary>
        /// Log a templated message at the FATAL log level with properties
        /// </summary>
        /// <typeparam name="TException">The exception that occurred to log the stack trace for</typeparam>
        /// <param name="messageTemplate">A string message that accepted templated values (either {0} {1} or {prop_1} {prop_2}) a la String.Format</param>
        /// <param name="propertyValues">The properties to replace in the message template</param>
        public static void FatalException<TException>(this ILog logger, string messageTemplate, TException exception, params object[] propertyValues) where TException : Exception
        {
            logger.Log<TException>(LogLevel.Fatal, messageTemplate, exception, propertyValues);
        }

        /// <summary>
        /// Adds additional context to the logger that will be used on all subsequent log entries.
        /// </summary>
        /// <param name="propertyName">The name to store the context with</param>
        /// <param name="value">The value to store</param>
        /// <param name="destructureObjects">If true, will destructure (serialize) the object for storage. Defaults to false.</param>
        public static ILog AddLogContext(this ILog logger, string propertyName, object value, Boolean destructureObjects = false)
        {
            logger = logger.WithContext(propertyName, value, destructureObjects);
            
            return logger;
        }
    }
}