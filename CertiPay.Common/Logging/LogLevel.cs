namespace CertiPay.Common.Logging
{
    /// <summary>
    /// The verbosity level that the log entries will be recorded at.
    /// </summary>
    /// <remarks>
    /// Inspired by Serilog's log levels and descriptions
    /// </remarks>
    public enum LogLevel
    {
        /// <summary>
        /// Anything and everything you might want to know about a running block of code.
        /// </summary>
        Verbose,

        /// <summary>
        /// Internal system events that aren't necessarily observable from the outside.
        /// </summary>
        Debug,

        /// <summary>
        /// The lifeblood of operational intelligence - things happen.
        /// </summary>
        Info,

        /// <summary>
        /// Service is degraded or endangered.
        /// </summary>
        Warn,

        /// <summary>
        /// Functionality is unavailable, invariants are broken or data is lost.
        /// </summary>
        Error,

        /// <summary>
        /// If you have a pager, it goes off when one of these occurs.
        /// </summary>
        Fatal
    }
}