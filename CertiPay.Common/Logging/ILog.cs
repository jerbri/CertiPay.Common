namespace CertiPay.Common.Logging
{
    using System;

    /// <summary>
    /// A generic interface for writing log entries to various destinations. The intent behind this interface
    /// is to avoid taking a direct dependency on a logger implementation or abstraction (i.e. commons.logging).
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Anything and everything you might want to know about a running block of code.
        /// </summary>
        void Verbose(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Internal system events that aren't necessarily observable from the outside.
        /// </summary>
        void Debug(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Internal system events that aren't necessarily observable from the outside.
        /// </summary>
        void Debug(Exception exception, string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// The lifeblood of operational intelligence - things happen.
        /// </summary>
        void Info(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// The lifeblood of operational intelligence - things happen.
        /// </summary>
        void Info(Exception exception, string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Service is degraded or endangered.
        /// </summary>
        void Warning(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Service is degraded or endangered.
        /// </summary>
        void Warning(Exception exception, string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Functionality is unavailable, invariants are broken or data is lost.
        /// </summary>
        void Error(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Functionality is unavailable, invariants are broken or data is lost.
        /// </summary>
        void Error(Exception exception, string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// If you have a pager, it goes off when one of these occurs.
        /// </summary>
        void Fatal(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// If you have a pager, it goes off when one of these occurs.
        /// </summary>
        void Fatal(Exception exception, string messageTemplate, params object[] propertyValues);
    }
}