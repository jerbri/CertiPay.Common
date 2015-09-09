using System;
using System.Web.Mvc;
using RequireHttpsAttributeBase = System.Web.Mvc.RequireHttpsAttribute;

namespace CertiPay.Common.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RequireHttpsOverrideAttribute : RequireHttpsAttributeBase
    {
        // This class is borrowed from the AppHarbor FAQ's.
        // Right now, we mainly use it for the Request.IsLocal check to avoid requiring SSL locally
        // but we'll likely need it in the future for the load balancer header check

        private const String PROTOCOL_HEADER_FIELD = "X-Forwarded-Proto";

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.HttpContext.Request.IsSecureConnection)
            {
                return;
            }

            if (filterContext.HttpContext.Request.IsLocal)
            {
                return;
            }

            if (string.Equals(filterContext.HttpContext.Request.Headers[PROTOCOL_HEADER_FIELD], Uri.UriSchemeHttps, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            HandleNonHttpsRequest(filterContext);
        }
    }
}