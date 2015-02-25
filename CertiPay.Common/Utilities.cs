namespace CertiPay.Common
{
    using System;
    using System.Linq;
    using System.Reflection;

    public static class Utilities
    {
        private static Lazy<String> version = new Lazy<String>(() =>
            {
                AssemblyInformationalVersionAttribute attribute =
                    Assembly
                    .GetExecutingAssembly()
                    .GetCustomAttributes(false)
                    .OfType<AssemblyInformationalVersionAttribute>()
                    .FirstOrDefault();

                return attribute == null ? "Unknown" : attribute.InformationalVersion;
            });

        /// <summary>
        /// Returns the AssemblyVersion attribute of the executing library (not CertiPay.Common)
        /// </summary>
        public static String Version { get { return version.Value; } }
    }
}