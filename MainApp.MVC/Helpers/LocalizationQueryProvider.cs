﻿using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace MainApp.MVC.Helpers
{
    public class LocalizationQueryProvider : RequestCultureProvider
    {
        public static readonly string DefaultParamterName = "culture";

        public string QureyParamterName { get; set; } = DefaultParamterName;

        /// <inheritdoc />
        public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            var query = httpContext.Request.Query;
            var exists = query.TryGetValue(DefaultParamterName, out StringValues culture);

            if (!exists)
            {
                exists = query.TryGetValue("returnUrl", out StringValues requesturl);
                // hack because Identityserver4 does some magic here...
                // Need to set the culture manually
                if (exists)
                {
                    var request = requesturl.ToArray()[0];
                    Uri uri = new Uri("http://faketopreventexception" + request);
                    var query1 = QueryHelpers.ParseQuery(uri.Query);
                    var requestCulture = query1.FirstOrDefault(t => t.Key == DefaultParamterName).Value;

                    var cultureFromReturnUrl = requestCulture.ToString();
                    if (string.IsNullOrEmpty(cultureFromReturnUrl))
                        return NullProviderCultureResult;

                    culture = cultureFromReturnUrl;
                }
            }

            var providerResultCulture = ParseDefaultParamterValue(culture);

            // Use this cookie for following requests, so that for example the logout request will work
            if (!string.IsNullOrEmpty(culture.ToString()))
            {
                var cookie = httpContext.Request.Cookies[".AspNetCore.Culture"];
                var newCookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture));

                if (string.IsNullOrEmpty(cookie)) 
                    httpContext.Response.Cookies.Append(".AspNetCore.Culture", newCookieValue);
            }

            return Task.FromResult(providerResultCulture);
        }

        public static ProviderCultureResult? ParseDefaultParamterValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var cultureName = value;
            var uiCultureName = value;

            if (cultureName == null && uiCultureName == null)
                return null;

            if (cultureName != null && uiCultureName == null)
                uiCultureName = cultureName;

            if (cultureName == null && uiCultureName != null)
                cultureName = uiCultureName;

            return new ProviderCultureResult(cultureName, uiCultureName);
        }
    }
}
