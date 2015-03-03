namespace CertiPay.Common
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class Utilities
    {
        /// <summary>
        /// Returns the AssemblyInformationalVersion attribute of the executing library (not CertiPay.Common)
        /// </summary>
        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        public static String Version()
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