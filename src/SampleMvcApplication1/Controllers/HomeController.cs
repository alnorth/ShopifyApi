using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shopify;

namespace SampleMvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        private dynamic _shopify;

        public ActionResult Index()
        {
            ViewBag.Message = "Shopify API Sample";

            return View();
        }

        /// <summary>
        /// This sample action needs to be authorized by Shopify because it makes API Calls
        /// </summary>
        /// <returns></returns>
        [ShopifyAuthorize]
        public ActionResult About()
        {
            dynamic shopResponse = _shopify.Shop();
            ViewBag.ShopName = shopResponse.shop.name;

            dynamic collectionResponse = _shopify.Custom_Collections();
            ViewBag.Collections = collectionResponse.custom_collections;
            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ShopifyAuthorizationState authState = Session["shopify_auth_state"] as ShopifyAuthorizationState;
            if (authState != null)
                _shopify = new ShopifyClient(authState);
        }
    }
}
