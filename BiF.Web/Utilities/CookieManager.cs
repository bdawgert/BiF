using System.Linq;
using System.Web;

namespace BiF.Web.Utilities
{
    public static class CookieManager
    {

#if DEBUG
        internal static bool UseSecure = false;
#else
        internal static bool UseSecure = true;
#endif

        internal static HttpCookie GetCookie(string key)
        {
            HttpCookie cookie = HttpContext.Current.Response.Cookies.AllKeys.Contains(key)
                ? HttpContext.Current.Response.Cookies[key]
                : HttpContext.Current.Request.Cookies[key];

            return cookie;

        }

        internal static void SetCookie(HttpCookie cookie) {
            cookie.Secure = UseSecure;
            HttpContext.Current.Response.Cookies.Set(cookie);
        }

    }
}