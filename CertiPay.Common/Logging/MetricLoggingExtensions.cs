namespace CertiPay.Common.Logging
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Provides extensions for logging events for metric analysis. Metric names should be in a format that is easy to parse.
    /// </summary>
    /// <remarks>
    /// Inspired by StatsD and MiniProfiler, and most especially Serilog.Metrics extensions
    /// </remarks>
    public static class MetricLoggingExtensions
    {
        private const String Timer_Start = "Starting Operation {TimedOperationId}: {TimedOperationDescription}";

        private const String Timer_Finish = "Completed Operation {TimedOperationId}: {TimedOperationDescription} in {TimedOperationElapsed} ({TimedOperationElapsedInMs} ms)";

        private const String Timer_Warning = "Operation {TimedOperationId}: {TimedOperationDescription} exceeded the limit of {WarningLimit} by completing in {TimedOperationElapsed}  ({TimedOperationElapsedInMs} ms)";

        /// <summary>
        /// Track the execution time of a particular block of code for debugging purposes with the given description and identifier.
        /// A timer is a measure of the number of milliseconds elapsed between a start and end time, for example the
        /// time to complete rendering of a web page for a user. Valid timer values are in the range [0, 2^64^).
        ///
        /// The identifier allows tracking from the start of an event to the finish, like a request id.
        /// You can raise warning level events if a timeout is exceeded by providing a threshold timespan.
        /// </summary>
        /// <example>
        ///
        /// using(Log.Timer("Load the Companies", warnIfExceeds: TimeSpan.FromSeconds(1))
        /// {
        ///     LoadTheCompanies();
        /// }
        ///
        /// </example>
        public static IDisposable Timer(this ILog logger, String description, object context = null, TimeSpan? warnIfExceeds = null, LogLevel level = LogLevel.Info)
        {
            // If no identifer was provided for this transaction, just create a GUID to use

            context = context ?? Guid.NewGuid().ToString();

            return new Timing(logger, warnIfExceeds, context, description, level);
        }

        //public static void Gauge(this ILog logger, String name, String unit_of_measure, Func<long> value)
        //{
        //
        //}

        // TODO Counter Increment/Decrement/Reset?

        /// <summary>
        /// An implementation of the metrics gathering class that will track the amount of time the given action takes to complete.
        /// </summary>
        private sealed class Timing : IDisposable
        {
            private readonly ILog _logger;
            private readonly TimeSpan? _warnIfExceeds;
            private readonly object _identifier;
            private readonly string _description;
            private readonly LogLevel _level;
            private readonly Stopwatch _sw;

            public Timing(ILog logger, TimeSpan? warnIfExceeds, object identifier, string description, LogLevel level = LogLevel.Info)
            {
                _logger = logger;
                _warnIfExceeds = warnIfExceeds;
                _identifier = identifier;
                _description = description;
                _level = level;

                _logger.Log(_level, Timer_Start, _identifier, _description);

                _sw = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                _sw.Stop();

                if (_warnIfExceeds.HasValue && _sw.Elapsed > _warnIfExceeds.Value)
                {
                    _logger.Log(LogLevel.Warn, Timer_Warning, _identifier, _description, _warnIfExceeds.Value, _sw.Elapsed, _sw.ElapsedMilliseconds);
                }
                else
                {
                    _logger.Log(_level, Timer_Finish, _identifier, _description, _sw.Elapsed, _sw.ElapsedMilliseconds);
                }
            }
        }
    }
}