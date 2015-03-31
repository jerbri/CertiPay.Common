namespace CertiPay.Common.Logging
{
    using CertiPay.Common;
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;
    using System;
    using System.Configuration;

    /// <summary>
    /// A class to handle the configuration and routing of log entries to various sinks, so that
    /// an application is agnostic to it's destination.
    /// </summary>
    public class LogManager
    {
        static LogManager()
        {
            // Note: These properties are statically configured here, but can be adjusted via their
            // respective set properties by an application for special use cases.

            ApplicationName = ConfigurationManager.AppSettings["Application"].TrimToNull() ?? "Unknown";

            LogFilePath = ConfigurationManager.AppSettings["LogFilePath"].TrimToNull() ?? String.Format(@"c:\Logs\{0}\{1}\{2}.log", EnvUtil.Current, ApplicationName, "{Date}");

            LogLevel = new LoggingLevelSwitch((LogEventLevel)Enum.Parse(typeof(LogEventLevel), value: ConfigurationManager.AppSettings["LogLevel"].TrimToNull() ?? "Information", ignoreCase: true));

            Version = Utilities.Version<LogManager>();
        }

        /// <summary>
        /// String describing the location of the log files, with {Date} in the place
        /// of the file date. E.g. "Logs\myapp-{Date}.log" will result in log files such
        /// as "Logs\myapp-2013-10-20.log", "Logs\myapp-2013-10-21.log" and so on..
        ///
        /// Pulls from AppSettings["LogFilePath"], defaults to c:\Logs\{Environment}\{Application}\-{Date}.log
        /// </summary>
        public static String LogFilePath { get; set; }

        /// <summary>
        /// The name of the application being logged for.
        ///
        /// Pulls from AppSettings["Application"], defaults to Unknown
        /// </summary>
        public static String ApplicationName { get; set; }

        /// <summary>
        /// The application version number, included in the logs for debugging purposes.
        ///
        /// Defaults to the version of CertiPay.Common package
        /// </summary>
        public static String Version { get; set; }

        /// <summary>
        /// Adjusts the logging level for the entire log system.
        ///
        /// Pulls from AppSettings["LogLevel"], dsefaults to Information.
        /// Possible values are Verbose, Debug, Information, Warning, Error, Fatal
        /// </summary>
        public static LoggingLevelSwitch LogLevel { get; private set; }

        public static ILog GetLogger<T>()
        {
            return new SerilogLogger(logger.ForContext<T>());
        }

        public static ILog GetLogger(Type type)
        {
            return new SerilogLogger(logger.ForContext(type));
        }

        public static ILog GetLogger(String key)
        {
            return new SerilogLogger(logger.ForContext("name", key));
        }

        internal static ILogger logger { get { return _logger.Value; } }

        private static Lazy<ILogger> _logger = new Lazy<ILogger>(() =>
        {
            // Using a lazy<t> here ensures that this is only done once, thread-safe, on first-use
            // After the logmanager is used, the properties such as path, version, appname, and environment
            // name are all set for the duration i think?

            Log.Logger =
                new LoggerConfiguration()

                .MinimumLevel.ControlledBy(LogManager.LogLevel)

                .Enrich.WithMachineName()

                .Enrich.WithProperty("ApplicationName", LogManager.ApplicationName)
                .Enrich.WithProperty("Version", LogManager.Version)
                .Enrich.WithProperty("Environment", EnvUtil.Current)

                .WriteTo.ColoredConsole(restrictedToMinimumLevel: LogEventLevel.Debug)

                .WriteTo.RollingFile(
                    pathFormat: LogFilePath,
                    outputTemplate: "{Timestamp} [{Level}] ({Version} on {MachineName}) {Message}{NewLine}{Exception}"
                    )

                //.WriteTo.Sink(new RollingFileSink
                //(
                //    pathFormat: LogFilePath,
                //    textFormatter: new JsonFormatter(), // use json so we an scoop these up via logstash?
                //    fileSizeLimitBytes: null, // default to 1gb
                //    retainedFileCountLimit: 31 // default to 31
                //))

                // .WriteTo ElasticSearch("settings", , restrictedToMinimumLevel: LogEventLevel.Debug)

                .CreateLogger();

            return Log.Logger;
        });
    }
}