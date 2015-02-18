using System;
using System.Configuration;

namespace CertiPay.Common
{
    public enum Environment
    {
        Local,
        Test,
        Staging,
        Production
    }

    public interface IEnvUtil
    {
        /// <summary>
        /// Returns the current executing environment. Defaults to Local if no value is set in the config
        /// </summary>
        Environment Current { get; }

        Boolean IsLocal { get; }

        Boolean IsTest { get; }

        Boolean IsStaging { get; }

        Boolean IsProd { get; }
    }

    public class EnvUtil : IEnvUtil
    {
        public virtual Environment Current { get { return current.Value; } }

        public Boolean IsLocal { get { return Environment.Local == current.Value; } }

        public Boolean IsTest { get { return Environment.Test == current.Value; } }

        public Boolean IsStaging { get { return Environment.Staging == current.Value; } }

        public Boolean IsProd { get { return Environment.Production == current.Value; } }

        private static readonly Lazy<Environment> current = new Lazy<Environment>(() =>
        {
            Environment environment = Environment.Local;

            String envString = ConfigurationManager.AppSettings["Environment"].TrimToNull() ?? "Local";

            Enum.TryParse<Environment>(value: envString, ignoreCase: true, result: out environment);

            return environment;
        });
    }
}