using CertiPay.Common.Logging;
using System;
using System.Configuration;

namespace CertiPay.Common
{
    /// <summary>
    /// A mechanism for determining the current execution environment based on a configuration setting.
    /// Useful for executing different code paths if running locally our outside of production.
    ///
    /// This requires that ConfigurationManager.AppSettings["Environment"] is set to Local/Test/Staging/Production
    /// </summary>
    public static class EnvUtil
    {
        public enum Environment
        {
            Local,
            Test,
            Staging,
            Production
        }

        /// <summary>
        /// Returns the current executing environment. Defaults to Local if no value is set in the config
        /// </summary>
        public static Environment Current { get { return current.Value; } }

        /// <summary>
        /// Returns true if the current environment is marked as Environment.Local
        /// </summary>
        public static Boolean IsLocal { get { return Environment.Local == Current; } }

        /// <summary>
        /// Returns true if the current environment is marked as Environment.Test
        /// </summary>
        public static Boolean IsTest { get { return Environment.Test == Current; } }

        /// <summary>
        /// Returns true if the current environment is marked as Environment.Staging
        /// </summary>
        public static Boolean IsStaging { get { return Environment.Staging == Current; } }

        /// <summary>
        /// Returns true if the current environment is marked as Environment.Production
        /// </summary>
        public static Boolean IsProd { get { return Environment.Production == Current; } }

        private static readonly ILog Log = LogManager.GetLogger(typeof(EnvUtil).Name);

        private static readonly Lazy<Environment> current = new Lazy<Environment>(() =>
        {
            Environment environment = Environment.Local;

            String envString = ConfigurationManager.AppSettings["Environment"].TrimToNull() ?? "Local";

            if (!Enum.TryParse<Environment>(value: envString, ignoreCase: true, result: out environment))
                Log.Warn("Environment configuration is invalid. {0}", envString);

            return environment;
        });
    }
}