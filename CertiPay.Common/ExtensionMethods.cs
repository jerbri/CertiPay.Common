using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

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

        /// <summary>
        /// Returns the display name from the display attribute on the enumeration, if available.
        /// Otherwise returns the ToString() value.
        /// </summary>
        public static string DisplayName(this Enum val)
        {
            return val.Display(e => e.Name);
        }

        /// <summary>
        /// Returns the short name from the display attribute on the enumeration, if available.
        /// Otherwise returns the ToString() value.
        /// </summary>
        public static string ShortName(this Enum val)
        {
            return val.Display(e => e.ShortName);
        }

        /// <summary>
        /// Returns the description from the display attribute on the enumeration, if available.
        /// Otherwise returns the ToString() value.
        /// </summary>
        public static string Description(this Enum val)
        {
            return val.Display(e => e.Description);
        }

        private static String Display(this Enum val, Func<DisplayAttribute, String> selector)
        {
            FieldInfo fi = val.GetType().GetField(val.ToString());

            DisplayAttribute[] attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return selector.Invoke(attributes[0]);
            }

            return val.ToString();
        }
    }
}