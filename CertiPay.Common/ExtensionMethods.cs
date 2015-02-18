using System;

namespace CertiPay.Common
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Trims the string of any whitestace and leaves null if there is no content.
        /// </summary>
        public static String TrimToNull(this String s)
        {
            return String.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }
    }
}