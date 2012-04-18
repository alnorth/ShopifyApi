using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using SampleMvcApplication1.Models;

using Shopify;

namespace SampleMvcApplication1.Controllers
{
    public class AccountController : Controller
    {

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // strip the .myshopify.com in case they added it
                string shop = model.ShopName.Replace(".myshopify.com", String.Empty);
                ShopifyAuthClient client = new ShopifyAuthClient(shop, ConfigurationManager.AppSettings["Shopify.ConsumerKey"], ConfigurationManager.AppSettings["Shopify.ConsumerSecret"]);

                // prepare the URL that will be executed after authorization is requested
                Uri requestUrl = this.Url.RequestContext.HttpContext.Request.Url;
                Uri returnURL = new Uri(string.Format("{0}://{1}{2}",
                                                        requestUrl.Scheme,
                                                        requestUrl.Authority,
                                                        this.Url.Action("ShopifyAuthCallback", "Account")));

                //
                client.RequestUserAuthorization(new string[] { ConfigurationManager.AppSettings["Shopify.Scope"] }, returnURL);
                return null;
            }

            return View(model);
        }

        public ActionResult ShopifyAuthCallback(string code, string shop)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(shop))
                return RedirectToAction("Index", "Home");

            shop = shop.Replace(".myshopify.com", String.Empty);

            ShopifyAuthClient client = new ShopifyAuthClient(shop, ConfigurationManager.AppSettings["Shopify.ConsumerKey"], ConfigurationManager.AppSettings["Shopify.ConsumerSecret"]);
            ShopifyAuthorizationState authState = client.ProcessAuthorization();
            if (authState != null && authState.AccessToken != null)
            {
                this.Session["shopify_auth_state"] = authState;
            }

            return RedirectToAction("Index", "Home");
        }

    }
}
