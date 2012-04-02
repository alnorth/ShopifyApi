using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Shopify;
namespace SampleMvcApplication1
{

    public class ShopifyAuthorize : AuthorizeAttribute
    {
        /// <summary>
        /// Test to see if the current http context is authorized for access to Shopify API
        /// </summary>
        /// <param name="httpContext">current httpContext</param>
        /// <returns>true if the current http context is authorized for access to Shopify API, otherwise false</returns>
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var authState = httpContext.Session["shopify_auth_state"] as ShopifyAuthorizationState;
            if (authState == null || String.IsNullOrWhiteSpace(authState.AccessToken))
                return false;
            return true;
        }
    }
}