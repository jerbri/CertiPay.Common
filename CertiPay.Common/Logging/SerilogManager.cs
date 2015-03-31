namespace CertiPay.Common.Logging
{
    using Serilog;
    using Serilog.Events;
    using System;

    public class SerilogManager : ILog
    {
        public static Serilog.LoggerConfiguration Configuration { get; set; }

        private readonly Lazy<Serilog.ILogger> _log = new Lazy<Serilog.ILogger>(() => Configuration.CreateLogger());

        static SerilogManager()
        {
            // Provide a default rolling file and console configuration for Serilog

            // Additional configuration or sinks can be added by updating the SeriLogManager.Configuration property

            Configuration =
                    new LoggerConfiguration()

                    .MinimumLevel.Is(GetLevel(LogManager.LogLevel))

                    .Enrich.WithMachineName()
                    .Enrich.WithProperty("ApplicationName", LogManager.ApplicationName)
                    .Enrich.WithProperty("Version", LogManager.Version)
                    .Enrich.WithProperty("Environment", EnvUtil.Current)

                    .WriteTo.ColoredConsole()

                    .WriteTo.RollingFile(
                        pathFormat: LogManager.LogFilePath,
                        outputTemplate: "{Timestamp} [{Level}] ({Version} on {MachineName}) {Message}{NewLine}{Exception}"
                        );
        }

        private readonly String _key;

        internal SerilogManager(String key)
        {
            _key = key;
        }

        public void Log(LogLevel level, string messageTemplate, params object[] propertyValues)
        {
            _log.Value.ForContext("name", _key).Write(GetLevel(level), messageTemplate, propertyValues);
        }

        public void Log<TException>(LogLevel level, string messageTemplate, TException exception, params object[] propertyValues) where TException : Exception
        {
            _log.Value.ForContext("name", _key).Write(GetLevel(level), exception, messageTemplate, propertyValues);
        }

        internal static LogEventLevel GetLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Fatal:
                    return LogEventLevel.Fatal;

                case LogLevel.Warn:
                    return LogEventLevel.Warning;

                case LogLevel.Info:
                    return LogEventLevel.Information;

                case LogLevel.Debug:
                    return LogEventLevel.Debug;

                case LogLevel.Verbose:
                default:
                    return LogEventLevel.Verbose;
            }
        }
    }
}