using System;

namespace CertiPay.Common.Logging
{
    /// <summary>
    /// A generic interface for writing log entries to various destinations. The intent behind this interface
    /// is to avoid taking a direct dependency on a logger implementation or abstraction (i.e. commons.logging).
    /// </summary>
    /// <remarks>
    /// This interface is inspired after LibLog by DamianH and RavenDB's logging abstractions
    /// </remarks>
    public interface ILog
    {
        void Log(LogLevel level, string messageTemplate, params object[] propertyValues);

        void Log<TException>(LogLevel level, string messageTemplate, TException exception, params object[] propertyValues) where TException : Exception;
    }
}