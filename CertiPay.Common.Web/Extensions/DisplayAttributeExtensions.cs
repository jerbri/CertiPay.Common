using System.ComponentModel.DataAnnotations;

namespace CertiPay.Common.Web.Extensions
{
    public static class DisplayAttributeExtensions
    {
        internal static DisplayAttribute Copy(this DisplayAttribute attribute)
        {
            if (attribute == null) return null;

            return new DisplayAttribute
            {
                Name = attribute.Name,
                GroupName = attribute.GroupName,
                Description = attribute.Description,
                ResourceType = attribute.ResourceType,
                ShortName = attribute.ShortName,
                Prompt = attribute.Prompt
            };
        }

        /// <summary>
        /// Returns true if the attribute has a name set but does not have a resource
        /// </summary>
        internal static bool CanSupplyDisplayName(this DisplayAttribute attribute)
        {
            return attribute != null && attribute.ResourceType != null && !string.IsNullOrEmpty(attribute.Name);
        }
    }
}