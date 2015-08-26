using System.Collections.Generic;
using System.Linq;

namespace CertiPay.Common.Web.Extensions
{
    public static class EnumerableExtensions
    {
        internal static IEnumerable<T> Replace<T>(this IEnumerable<T> collection, T source, T replacement) where T : class
        {
            var collectionWithout = collection;

            if (source != null)
            {
                collectionWithout = collectionWithout.Except(new[] { source });
            }

            return collectionWithout.Union(new[] { replacement });
        }
    }
}