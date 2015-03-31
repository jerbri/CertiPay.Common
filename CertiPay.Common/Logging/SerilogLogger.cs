namespace CertiPay.Common.Logging
{
    using System;

    internal class SerilogLogger : ILog
    {
        // This class is a thin wrapper around a Serilog ILogger so that we aren't depending directly on Serilog throughout the system

        private readonly Serilog.ILogger _log;

        public SerilogLogger(Serilog.ILogger log)
        {
            _log = log;
        }

        public void Log(LogLevel level, string messageTemplate, params object[] propertyValues)
        {
            switch (level)
            {
                case LogLevel.Verbose:
                    _log.Verbose(messageTemplate, propertyValues);
                    break;

                case LogLevel.Debug:
                    _log.Debug(messageTemplate, propertyValues);
                    break;

                case LogLevel.Info:
                    _log.Information(messageTemplate, propertyValues);
                    break;

                case LogLevel.Warn:
                    _log.Warning(messageTemplate, propertyValues);
                    break;

                case LogLevel.Error:
                    _log.Error(messageTemplate, propertyValues);
                    break;

                case LogLevel.Fatal:
                    _log.Fatal(messageTemplate, propertyValues);
                    break;
            }
        }

        public void Log<TException>(LogLevel level, string messageTemplate, TException exception, params object[] propertyValues) where TException : Exception
        {
            switch (level)
            {
                case LogLevel.Verbose:
                    _log.Verbose(exception, messageTemplate, propertyValues);
                    break;

                case LogLevel.Debug:
                    _log.Debug(exception, messageTemplate, propertyValues);
                    break;

                case LogLevel.Info:
                    _log.Information(exception, messageTemplate, propertyValues);
                    break;

                case LogLevel.Warn:
                    _log.Warning(exception, messageTemplate, propertyValues);
                    break;

                case LogLevel.Error:
                    _log.Error(exception, messageTemplate, propertyValues);
                    break;

                case LogLevel.Fatal:
                    _log.Fatal(exception, messageTemplate, propertyValues);
                    break;
            }
        }
    }
}