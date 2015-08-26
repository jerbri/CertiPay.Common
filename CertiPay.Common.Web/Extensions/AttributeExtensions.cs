using System;
using System.Linq;
using System.Reflection;

namespace CertiPay.Common.Web.Extensions
{
    public static class AttributeExtensions
    {
        internal static TAttribute GetAttributeOnTypeOrAssembly<TAttribute>(this Type type) where TAttribute : Attribute
        {
            // Check the type first, then check the type's assembly for the given attribute

            return type.First<TAttribute>() ?? type.Assembly.First<TAttribute>();
        }

        private static TAttribute First<TAttribute>(this ICustomAttributeProvider attributeProvider) where TAttribute : Attribute
        {
            return attributeProvider.GetCustomAttributes(true).OfType<TAttribute>().FirstOrDefault();
        }
    }
}