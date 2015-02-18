namespace CertiPay.Common.Logging
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Provides extensions for logging events for metric analysis. Metric names should be in a format that is easy to parse.
    /// </summary>
    /// <remarks>
    /// Inspired by StatsD and MiniProfiler. You should read through information of them for an idea of how and why this is implemented.
    /// </remarks>
    public static class MetricLoggingExtensions
    {
        public const String Timed_Operation_Start_Template = "Beginning operation {TimedOperationId}: {TimedOperationDescription}";

        public const String Timed_Operation_Finish_Template = "Completed operation {TimedOperationId}: {TimedOperationDescription} in {TimedOperationElapsed} ({TimedOperationElapsedInMs} ms)";

        public const String Timed_Operation_Warning_Template = "Operation {TimedOperationId}: {TimedOperationDescription} exceeded the limit of {WarningLimit} by completing in {TimedOperationElapsed}  ({TimedOperationElapsedInMs} ms)";

        // TODO They'll pass in a logger in the extension method, but we'll use a more refined logging mechanism?

        private static readonly ILog metrics_log = new SerilogLogger(LogManager.logger.ForContext("IsMetric", true));

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
        public static IDisposable Timer(this ILog logger, String description, String identifier = "", TimeSpan? warnIfExceeds = null)
        {
            identifier = identifier.TrimToNull() ?? Guid.NewGuid().ToString();

            return new Timing(metrics_log, warnIfExceeds, identifier, description);
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
            private readonly Stopwatch _sw;

            public Timing(ILog logger, TimeSpan? warnIfExceeds, object identifier, string description)
            {
                _logger = logger;
                _warnIfExceeds = warnIfExceeds;
                _identifier = identifier;
                _description = description;

                _logger.Info(Timed_Operation_Start_Template, _identifier, _description);

                _sw = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                _sw.Stop();

                if (_warnIfExceeds.HasValue && _sw.Elapsed > _warnIfExceeds.Value)
                {
                    _logger.Warning(Timed_Operation_Warning_Template, _identifier, _description, _warnIfExceeds.Value, _sw.Elapsed, _sw.ElapsedMilliseconds);
                }
                else
                {
                    _logger.Info(Timed_Operation_Finish_Template, _identifier, _description, _sw.Elapsed, _sw.ElapsedMilliseconds);
                }
            }
        }
    }
}