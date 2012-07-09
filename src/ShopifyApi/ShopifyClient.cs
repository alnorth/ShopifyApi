using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Dynamic;

namespace Shopify {
    public class ShopifyClient : DynamicObject {

        private string _baseUrl { get; set; }
        public string AccessToken { get; set; }
        public string ShopName { get; set; }
        public ShopifyAuthorizationState AuthState { get; set; }

        public ShopifyClient(ShopifyAuthorizationState authState)
        {
            this.AuthState = authState;
            this.AccessToken = authState.AccessToken;
            this.ShopName = authState.ShopName;

            _initBaseURL();
        }

        public ShopifyClient(string shopeName, string accessToken)
	    {
            if (string.IsNullOrEmpty(shopeName))
                throw new ArgumentNullException("accessToken");

            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentNullException("accessToken");

            ShopName = shopeName;
            AccessToken = accessToken;

            _initBaseURL();
	    }

        private void _initBaseURL()
        {
            //https://some-store.myshopify.com/admin/some-resource
            _baseUrl = String.Format("https://{0}.myshopify.com/admin/", ShopName); //some-store.myshopify.com
        }

        /// <summary>
        /// A simple GET request to the Shopify API
        /// </summary>
        string Send(string url) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("X-Shopify-Access-Token", this.AccessToken);

            var response = (HttpWebResponse)request.GetResponse();
            string result = "";

            using (Stream stream = response.GetResponseStream()) {
                StreamReader sr = new StreamReader(stream);
                result = sr.ReadToEnd();
                sr.Close();
            }
            return result;
        }

        /// <summary>
        /// This allows you to work with the data at Shopify using a Property, which represents Products, Customers, etc
        /// </summary>
        public override bool TryGetMember(GetMemberBinder binder, out object result) {

            var name = binder.Name.ToLower();

            //we can do this because the Shopify stuff is all pluralized with "s" :)
            if (!name.EndsWith("s"))
                name += "s";

            result = new ShopifyObject(name, ShopName, AccessToken);
            return true;
        }

        /// <summary>
        /// This builds a query with the passed in named arguments - shopify.Products(collection_id:121212)
        /// </summary>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
            var name = binder.Name.ToLower() + ".json";
            var url = _baseUrl + name;

            //params?
            var info = binder.CallInfo;
            var looper = 0;
            if (info.ArgumentNames.Count > 0) {

                for (int i = 0; i < args.Length; i++) {
                    var argName = info.ArgumentNames[i].ToLower();
                    var val = args[i];
                    //the ID is a singular call
                    //with a special format
                    if (argName == "id") {
                        url = url.Replace(".json", "/" + val + ".json");
                    } else {
                        if (looper == 0)
                            url += "?";
                        else
                            url += "&";
                        url += string.Format("{0}={1}", argName, val);
                    }
                    looper++;
                }
            }
            var json = Send(url);
            result = JsonHelper.Decode(json);
            return true;
        }


    }
}
