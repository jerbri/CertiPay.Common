namespace CertiPay.Common.Logging
{
    using Serilog;
    using Serilog.Events;
    using System;

    public class SerilogManager : ILog
    {
        public static Serilog.LoggerConfiguration Configuration { get; set; }

        static SerilogManager()
        {
            // Provide a default rolling file and console configuration for Serilog

            Configuration =
                new LoggerConfiguration()

                .MinimumLevel.Is(GetLevel(LogManager.LogLevel))

                .Enrich.WithMachineName()
                .Enrich.WithProperty("ApplicationName", LogManager.ApplicationName)
                .Enrich.WithProperty("Version", LogManager.Version)
                .Enrich.WithProperty("Environment", EnvUtil.Current)

                .WriteTo.ColoredConsole(restrictedToMinimumLevel: LogEventLevel.Debug)

                .WriteTo.RollingFile(
                    pathFormat: LogManager.LogFilePath,
                    outputTemplate: "{Timestamp} [{Level}] ({Version} on {MachineName}) {Message}{NewLine}{Exception}"
                    );
        }

        private readonly Serilog.ILogger _log;

        public SerilogManager(Serilog.ILogger log)
        {
            _log = log;
        }

        public void Log(LogLevel level, string messageTemplate, params object[] propertyValues)
        {
            _log.Write(GetLevel(level), messageTemplate, propertyValues);
        }

        public void Log<TException>(LogLevel level, string messageTemplate, TException exception, params object[] propertyValues) where TException : Exception
        {
            _log.Write(GetLevel(level), exception, messageTemplate, propertyValues);
        }

        public static LogEventLevel GetLevel(LogLevel level)
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