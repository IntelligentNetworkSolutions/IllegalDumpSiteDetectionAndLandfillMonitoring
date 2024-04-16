using Microsoft.AspNetCore.Html;
using Westwind.Globalization;

namespace MainApp.MVC.Helpers
{
    public static class DbResHtml
    {
        public static HtmlString T(string resId, string? resourceSet = null, string? lang = null)
        {
            return new HtmlString(DbRes.T(resId, resourceSet, lang));
        }

        //public static HtmlString TDefault(string resId, string defaultText, string? resourceSet = null, string? lang = null)
        //{
        //    var result = DbRes.T(resId, resourceSet, lang);
        //    if (resId == result)
        //        return new HtmlString(defaultText);
        //    else
        //        return new HtmlString(result);
        //}
    }
}
