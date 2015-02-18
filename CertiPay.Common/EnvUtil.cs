using System;
using System.Configuration;

namespace CertiPay.Common
{
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

        public static Boolean IsLocal { get { return Environment.Local == Current; } }

        public static Boolean IsTest { get { return Environment.Test == Current; } }

        public static Boolean IsStaging { get { return Environment.Staging == Current; } }

        public static Boolean IsProd { get { return Environment.Production == Current; } }

        private static readonly Lazy<Environment> current = new Lazy<Environment>(() =>
        {
            Environment environment = Environment.Local;

            String envString = ConfigurationManager.AppSettings["Environment"].TrimToNull() ?? "Local";

            Enum.TryParse<Environment>(value: envString, ignoreCase: true, result: out environment);

            return environment;
        });
    }
}