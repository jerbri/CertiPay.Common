using System;

namespace CertiPay.Common.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class MetadataConventionsAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public Type ResourceType { get; set; }
    }
}