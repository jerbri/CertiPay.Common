using System;

namespace CertiPay.Common.Logging
{
    /// <summary>
    /// Provide usability extensions for working with the ILog implementations
    /// </summary>
    /// <remarks>
    /// This was inspired by RavenDB's logging abstractions and extensions along with Serilog's interface
    /// </remarks>
    public static class LoggerExtensions
    {
        public static void Verbose(this ILog logger, string messageTemplate, params object[] propertyValues)
        {
            logger.Log(LogLevel.Verbose, messageTemplate, propertyValues);
        }

        public static void Debug(this ILog logger, string messageTemplate, params object[] propertyValues)
        {
            logger.Log(LogLevel.Debug, messageTemplate, propertyValues);
        }

        public static void DebugException<TException>(this ILog logger, string messageTemplate, TException exception, params object[] propertyValues) where TException : Exception
        {
            logger.Log<TException>(LogLevel.Debug, messageTemplate, exception, propertyValues);
        }

        public static void Info(this ILog logger, string messageTemplate, params object[] propertyValues)
        {
            logger.Log(LogLevel.Info, messageTemplate, propertyValues);
        }

        public static void InfoException<TException>(this ILog logger, string messageTemplate, TException exception, params object[] propertyValues) where TException : Exception
        {
            logger.Log<TException>(LogLevel.Info, messageTemplate, exception, propertyValues);
        }

        public static void Warning(this ILog logger, string messageTemplate, params object[] propertyValues)
        {
            logger.Log(LogLevel.Warn, messageTemplate, propertyValues);
        }

        public static void WarnException<TException>(this ILog logger, string messageTemplate, TException exception, params object[] propertyValues) where TException : Exception
        {
            logger.Log<TException>(LogLevel.Debug, messageTemplate, exception, propertyValues);
        }

        public static void Error(this ILog logger, string messageTemplate, params object[] propertyValues)
        {
            logger.Log(LogLevel.Error, messageTemplate, propertyValues);
        }

        public static void ErrorException<TException>(this ILog logger, string messageTemplate, TException exception, params object[] propertyValues) where TException : Exception
        {
            logger.Log<TException>(LogLevel.Error, messageTemplate, exception, propertyValues);
        }

        public static void Fatal(this ILog logger, string messageTemplate, params object[] propertyValues)
        {
            logger.Log(LogLevel.Fatal, messageTemplate, propertyValues);
        }

        public static void FatalException<TException>(this ILog logger, string messageTemplate, TException exception, params object[] propertyValues) where TException : Exception
        {
            logger.Log<TException>(LogLevel.Fatal, messageTemplate, exception, propertyValues);
        }
    }
}