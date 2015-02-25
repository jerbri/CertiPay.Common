namespace CertiPay.Common
{
    using System;
    using System.Linq;
    using System.Reflection;

    public static class Utilities
    {
        /// <summary>
        /// Returns the AssemblyInformationalVersion attribute of the executing library (not CertiPay.Common)
        /// </summary>
        public static String Version
        {
            get
            {
                AssemblyInformationalVersionAttribute attribute =
                    Assembly
                    .GetCallingAssembly()
                    .GetCustomAttributes(false)
                    .OfType<AssemblyInformationalVersionAttribute>()
                    .FirstOrDefault();

                return attribute == null ? "Unknown" : attribute.InformationalVersion;
            }
        }
    }
}