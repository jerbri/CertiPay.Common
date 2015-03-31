namespace CertiPay.Common
{
    using System;
    using System.Linq;
    using System.Reflection;

    public static class Utilities
    {
        /// <summary>
        /// Returns the assembly version of the assembly formatted as Major.Minor (build Build#)
        /// </summary>
        public static String AssemblyVersion<T>()
        {
            var version =
                typeof(T)
                .Assembly
                .GetName()
                .Version;

            return String.Format("{0}.{1} (build {2})", version.Major, version.Minor, version.Build);
        }

        /// <summary>
        /// Returns the assembly informational version of the type, or assembly version if not available
        /// </summary>
        public static String Version<T>()
        {
            var attribute =
                typeof(T)
                .Assembly
                .GetCustomAttributes(false)
                .OfType<AssemblyInformationalVersionAttribute>()
                .FirstOrDefault();

            return attribute == null ? AssemblyVersion<T>() : attribute.InformationalVersion;
        }
    }
}